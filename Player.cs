using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    enum PlayerState{
        IDLE,
        WALKING,
        SPRINTING,
        SPACEMOVING, //Rolling
        ATTACKING,
        ATTACKING_COMBO
    }

    //PLAYER PROPERTIES
    public int health;

    [SerializeField]
    public int maxHealth;

    public int level = 1;
    public int xp = 0;
    public int maxXp = 100;
    public int attackDamage;
    private int weaponDamage = 0;

    public float attackDelaySeconds;
    public float secondsBeforeHit;
    
    private int levelPoints = 0;
    private int agilityLevel = 0;
    private int strengthLevel = 0;
    private int powerLevel = 0;

    //

    // Start is called before the first frame update
    private Animator animator;

    public bool isSpacemoveDisabled = false;
    private bool isHealingHealth = false;
    private int healingHpPerFrame = 0;

    [SerializeField]
    private float spacemoveDurationSeconds;

    private float currentTime = 0.0f;
    private float cooldownEndTime = 0.0f;

    private float endSpacemoveTime = -1;

    [SerializeField]
    private float spacemoveCooldownTime; 
    //--

    //Movement Related
    private CharacterController controller;

    private List<Enemy> targetInRange = new List<Enemy>();
    private Enemy target;
    private CapsuleCollider attackScanner;

    [SerializeField]
    private float normalSpeed;

    [SerializeField]
    private float runningSpeed;

    private float currentVelocity;
    private float gravityPull = 9.8f;

    private bool isSpaceMove = false;

    [SerializeField]
    private float maxRotateSpeed = 0.1f;

    private float distToGround;

    private float lastMoveDirectionX;
    private float lastMoveDirectionZ;

    [SerializeField]
    private Transform camera;

    private float startSpacemoveTriggerTime = -1;
    private float endSpacemoveTriggerTime = -1;

    private float horizontal;
    private float vertical;
    private Vector3 direction = new Vector3();
    private Vector3 moveDirection = new Vector3();

    private float targetAngle;
    private float angle;

    private float nextHitTime = -1;
    private float actualHitTime = -1;

    private bool isUsingWeapon = false;
    private int textureId = 0;

    //--
    public void Die()
    {
        transform.position = new Vector3(0, 0, 0);
        GameObject.Find("DeathOverlay").GetComponent<DeathOverlay>().OnDeathShow();
        GameObject.Find("GameManager").GetComponent<CursorManager>().UnlockCursor();
    }

    public void ReceiveDamage(int dmg)
    {
        if(health > dmg)
        {
            UpdateHealth(health - dmg);
        }else{
            Die();
        }
    }

    public void UpdateHealth(int newHp)
    {
        health = newHp;
        GameObject.Find("HealthBar").GetComponent<HealthBar>().SetHealth(health, maxHealth);
    }
    
    public void UpdateXp(int newXp)
    {
        xp = newXp;
        GameObject.Find("XpBar").GetComponent<XpBar>().SetXp(xp, maxXp);
    }

    public void ResetHealth()
    {
        GameObject.Find("HealthBar").GetComponent<HealthBar>().ResetHealth(maxHealth);
        UpdateHealth(maxHealth);
    }

    void ChangeMaxHp(int newMax)
    {
        maxHealth = newMax;
        GameObject.Find("HealthBar").GetComponent<HealthBar>().ResetHealth(maxHealth);
        GameObject.Find("HealthBar").GetComponent<HealthBar>().SetHealth(health, maxHealth);
    }
    
    public void ResetXp(int newMax)
    {
        maxXp = newMax;
        GameObject.Find("XpBar").GetComponent<XpBar>().ResetXp(maxXp);
        UpdateXp(0);
    }

    public void SetTextureName(string name)
    {
        switch(name)
        {
            case "grass":
                textureId = 0;
            break;
            case "sand":
                textureId = 1;
            break;
            case "stone":
                textureId = 2;
            break;
            default:
                textureId = 0;
            break;
        }
    }

    void UpdateTarget()
    {
        target = targetInRange.Count == 0 ? null : targetInRange[0];
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Enemy>() != null && !targetInRange.Contains(other.gameObject.GetComponent<Enemy>()))
        {
            Enemy met = other.gameObject.GetComponent<Enemy>();
            Debug.Log("Added " + met.enemyName + " to targets in range!");
            targetInRange.Add(other.gameObject.GetComponent<Enemy>());
        }
        
    }
 
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<Enemy>() != null && targetInRange.Contains(other.gameObject.GetComponent<Enemy>()))
        {
            Enemy exit = other.gameObject.GetComponent<Enemy>();
            Debug.Log("Removed " + exit.enemyName + " from targets in range!");
            targetInRange.Remove(other.gameObject.GetComponent<Enemy>());
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if(targetInRange.Contains(enemy)){
            targetInRange.Remove(enemy);
        }
    }

    public void SetUseWeapon(bool value, int wepDamage)
    {
        isUsingWeapon = value;
        animator.SetBool("weapon", value);
        weaponDamage = wepDamage;
        GameObject.Find("DamageAmount").GetComponent<EffectiveDamageUI>().UpdateDamageAmount(attackDamage, weaponDamage);
    }
    
    int CalculateAttackDamage()
    {
        int total = attackDamage + weaponDamage;

        return total;
    }

    void Attack()
    {
        int effectiveDmg = CalculateAttackDamage();
        if(targetInRange.Count != 0)
        { 
            if(Time.time >= nextHitTime)
            {
                Debug.Log("Player attacking, applying "+ effectiveDmg + " damage to target named " + targetInRange[0].enemyName);
                targetInRange[0].ReceiveDamage(effectiveDmg);
                nextHitTime = Time.time + attackDelaySeconds;
            }
        }
        else
        {
            // Debug.Log("Player Attack: No Target! - Targets in range count : " + targetInRange.Count);
        }
    }

    void InitiateHit()
    {
        actualHitTime = Time.time + secondsBeforeHit;
    }

    void Spacemove()
    {
        float curTime = Time.time;
        startSpacemoveTriggerTime = curTime + (spacemoveDurationSeconds/6); //Around first 1/6 of the animation, it was just starting, so we wont apply boost for that phase of animation.
        //
        if(curTime > endSpacemoveTime){
            endSpacemoveTime = Time.time + spacemoveDurationSeconds;
            animator.SetBool("spacemove", true);
        }  
    }

    void StartSpacemove()
    {
        float curTime = Time.time;
        if(curTime > endSpacemoveTriggerTime){
            endSpacemoveTriggerTime = curTime + (spacemoveDurationSeconds/6.0f)*5.0f; //The amount of time the boost is applied is only 5/6 of animation duration because first 1/6 is delayed.
            isSpaceMove = true;
        }  
    }

    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;
        controller = GetComponent<CharacterController>();
        //
        attackScanner = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();

        ResetHealth();
        UpdateHealth(health);
        ResetXp(maxXp);
        UpdateXp(xp);
        GameObject.Find("XpBar").GetComponent<XpBar>().SetLevel(level);
        GameObject.Find("DamageAmount").GetComponent<EffectiveDamageUI>().UpdateDamageAmount(attackDamage, weaponDamage);
    }

    void UpdateGravityForce()
    {
        if(Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f) == false){
            gravityPull+= 9.8f;
        }else{
            gravityPull = 9.8f;
        }
    }

    void UpdateMovement(PlayerState state)
    {
        switch(state)
        {
            case PlayerState.WALKING:
                moveDirection.x *= normalSpeed + 2*agilityLevel;
                moveDirection.z *= normalSpeed + 2*agilityLevel;
            break;
            case PlayerState.SPRINTING:
                moveDirection.x *= runningSpeed + ((runningSpeed/normalSpeed)* (2*agilityLevel));
                moveDirection.z *= runningSpeed + ((runningSpeed/normalSpeed)* (2*agilityLevel));
            break;
            case PlayerState.ATTACKING:
                InitiateHit();
                moveDirection.x = 0;
                moveDirection.z = 0;
            break;
            case PlayerState.ATTACKING_COMBO:
                InitiateHit();
                moveDirection.x = 0;
                moveDirection.z = 0;
            break;
            case PlayerState.IDLE:
                moveDirection.x = 0;
                moveDirection.z = 0;
            break;
        }
        if(isSpaceMove)
        {
            if(IsPlayerMoving()){
                moveDirection.x *= 1.6f;
                moveDirection.z *= 1.6f;
            }else{
                moveDirection.x = lastMoveDirectionX * (normalSpeed + 2*agilityLevel);
                moveDirection.z = lastMoveDirectionZ * (normalSpeed + 2*agilityLevel);
            }
        }
    }

    void UpdateAnimation(PlayerState state)
    {
        animator.SetInteger("texture", textureId);
        switch(state)
        {
            case PlayerState.WALKING:
                animator.SetBool("run", true);
                animator.SetBool("sprint", false);
                animator.SetFloat("DirectionX", direction.x);
                animator.SetFloat("DirectionY", Mathf.Abs(direction.z));
                animator.SetBool("combo", false);
                animator.SetBool("attack", false);
                break;
            case PlayerState.SPRINTING:
                animator.SetBool("run", true);
                animator.SetBool("sprint", true);
                animator.SetBool("combo", false);
                animator.SetBool("attack", false);
                break;
            case PlayerState.ATTACKING:
                animator.SetBool("run", false);
                animator.SetBool("sprint", false);
                animator.SetBool("attack", true);
                animator.SetBool("combo", false);
                break;
            case PlayerState.ATTACKING_COMBO:
                animator.SetBool("run", false);
                animator.SetBool("sprint", false);
                animator.SetBool("attack", true);
                animator.SetBool("combo", true);
            break;
            case PlayerState.IDLE:
                animator.SetBool("run", false);
                animator.SetBool("sprint", false);
                animator.SetBool("combo", false);
                animator.SetBool("attack", false);
                break;
        }
    }

    void UpdateDirection()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0, vertical).normalized;
    }

    void UpdateMoveDirection()
    {
        moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        moveDirection = moveDirection.normalized;
        if (direction.magnitude >= 0.1f)
        {
            lastMoveDirectionX = moveDirection.x;
            lastMoveDirectionZ = moveDirection.z;
        }
    }

    void UpdateTargetAngle()
    {
        targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
    }

    void UpdateAngle()
    {
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, maxRotateSpeed);
    }

    void UpdateCameraRotation()
    {
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    bool IsPlayerMoving()
    {
        return direction.magnitude >= 0.1f;
    }

    void CheckForSpacemoveFrames()
    {
        currentTime = Time.time;
        if(Time.time >= startSpacemoveTriggerTime && startSpacemoveTriggerTime != -1)
        {
            startSpacemoveTriggerTime = -1;
            StartSpacemove();
        }

        if(Time.time >= endSpacemoveTriggerTime && endSpacemoveTriggerTime != -1)
        {
            endSpacemoveTriggerTime = -1;
            isSpaceMove = false;
        }

        
        if(Time.time >= endSpacemoveTime && endSpacemoveTime != -1)
        {
            endSpacemoveTime = -1;
            animator.SetBool("spacemove", false);

            cooldownEndTime = currentTime + spacemoveCooldownTime;
            isSpacemoveDisabled = true;
        }

        if(Time.time >= cooldownEndTime && cooldownEndTime != -1)
        {
            cooldownEndTime = -1;
            isSpacemoveDisabled = false;
        }
    }

    void CheckInputs()
    {
        if(IsPlayerMoving())
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                UpdateAnimation(PlayerState.SPRINTING);
                UpdateMovement(PlayerState.SPRINTING);
            }else{
                UpdateAnimation(PlayerState.WALKING);
                UpdateMovement(PlayerState.WALKING);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(isSpacemoveDisabled == false)
                {
                    Spacemove();
                }
                else
                {
                    GameObject.Find("MessageWarning").GetComponent<MessageWarning>().ShowMessage(2, "You can roll again in " + (cooldownEndTime - Time.time).ToString("0.00") + " seconds!");
                }
            }
        }else{
            UpdateAnimation(PlayerState.IDLE);
            UpdateMovement(PlayerState.IDLE);
        }
        if (Input.GetMouseButtonDown(0))
        {
            UpdateAnimation(PlayerState.ATTACKING);
            UpdateMovement(PlayerState.ATTACKING);
            if (Input.GetMouseButton(1))
            {
                UpdateAnimation(PlayerState.ATTACKING_COMBO);
            }
        }
    }

    void ExecuteMovement()
    {
        moveDirection.y += (gravityPull * - 1);
        controller.Move(moveDirection * Time.deltaTime);
    }

    public void AddXp(int amount)
    {
        if ((xp + amount) < maxXp)
        {
            UpdateXp(xp + amount);
        }
        else
        {
            LevelUp();
        }
        
    }

    public void SetHealingMode(bool isHealing, int hpPerSecond)
    {
        isHealingHealth = isHealing;
        healingHpPerFrame = hpPerSecond;
    }
    
    public void LevelUp()
    {
        level++;
        levelPoints += (level/10) + 1;
        GameObject.Find("XpBar").GetComponent<XpBar>().SetLevel(level);
        GameObject.Find("LevelUpOption").GetComponent<LevelUpOption>().LevelUpShow();
        ResetXp(maxXp + (int)(Mathf.Pow(level, 2) * 100));
        ResetHealth();
        GameObject.Find("LevelUpSFX").GetComponent<AudioSource>().Play(0);
    }

    public bool LevelPointSpend(int choice)
    {
        if (levelPoints >= 1)
        {
            switch (choice)
            {
                case 1: //AGILITY
                    agilityLevel++;
                    levelPoints--;
                    break;
                case 2: //STRENGTH
                    strengthLevel++;
                    ChangeMaxHp(maxHealth + (strengthLevel*200));
                    levelPoints--;
                    break;
                case 3: //POWER
                    powerLevel++;
                    attackDamage += 10;
                    GameObject.Find("DamageAmount").GetComponent<EffectiveDamageUI>().UpdateDamageAmount(attackDamage, weaponDamage);
                    levelPoints--;
                    break;
            }
            GameObject.Find("SkillSpendSFX").GetComponent<AudioSource>().Play(0);
            return levelPoints != 0;
        }

        return false;

    }

    void UpdateHealing()
    {
        if(isHealingHealth)
        {
            if(health + healingHpPerFrame <= maxHealth){
                UpdateHealth(health + healingHpPerFrame);
            }else{
                UpdateHealth(maxHealth);
            }
        }
    }

    public int GetLevelPoints()
    {
        return levelPoints;
    }
    public int GetAgilityLevel()
    {
        return agilityLevel;
    }
    
    public int GetStrengthLevel()
    {
        return strengthLevel;
    }
    
    public int GetPowerLevel()
    {
        return powerLevel;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        UpdateGravityForce();
        UpdateDirection();
        UpdateMoveDirection();
        if(direction.magnitude >= 0.1f){
            UpdateTargetAngle();
        }
        UpdateAngle();
        UpdateCameraRotation();
        CheckInputs();
        CheckForSpacemoveFrames();
        UpdateTarget();
        ExecuteMovement();
        UpdateHealing();
        if(Time.time >= actualHitTime && actualHitTime != -1){
            Attack();
            actualHitTime = -1;
        }
    }
}
