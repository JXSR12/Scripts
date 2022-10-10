using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/* Controls the Enemy AI */

public class Enemy : MonoBehaviour, IDamageable
{

	public float lookRadius;  // Detection range for player
	public int damagePerHit; //Attack damage per hit
	private int health; //Enemy Current HP
	public int maxHealth; //Enemy MaxHP
	public float hitDelaySeconds;
	public bool selfRespawn;
	public int selfRespawnDelaySeconds;
	public int minXpGive;
	public int maxXpGive;
	public string enemyName;
	
	public TextMeshProUGUI nameText;
	public CanvasGroup canvasGroup;
	public Canvas canvas;
	public Transform lookAt;

	Transform target;
	NavMeshAgent agent;

	Animator animator;
	
	int LayerOutlined;
	int LayerNormal;

	public bool isCampObject;
	public EnemyCamp enemyCamp;

	private float nextHitTime = -1;
	private float initCorpseY;

	private Vector3 initialPos;

	private bool corpsePullDown = false;
	private bool isDead = false;

	void SetLayerRecursively(GameObject obj, int newLayer)
	{
		obj.layer = newLayer;
	
		foreach(Transform child in obj.transform)
		{
			SetLayerRecursively(child.gameObject, newLayer);
		}
	}

	public void SetCamp(EnemyCamp camp)
	{
		this.enemyCamp = camp;
		this.isCampObject = true;
		this.selfRespawn = false;
	}
	
	void Start()
	{
		target = GameObject.Find("Character").transform;
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponentInChildren<Animator>();
		initialPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
		canvasGroup.alpha = 0;
		nameText.text = enemyName;
		LayerOutlined = LayerMask.NameToLayer("Outlined");
		LayerNormal = LayerMask.NameToLayer("NoCameraCollision");
		ResetHealth();
	}

	private IEnumerator RespawnDelay()
	{
		yield return new WaitForSeconds(selfRespawnDelaySeconds);
		ResetHealth();
		ResetLocation();
		agent.isStopped = false;
		isDead = false;
		
		Debug.Log("Enemy Respawned");
	}

	private IEnumerator DestroyDelay()
	{
		yield return new WaitForSeconds(6);
		if(isCampObject)
		{
			enemyCamp.ConsumeObjectDestroyed();
		}
		Destroy(gameObject);
		Debug.Log("Enemy Destroyed");
	}

	public void ResetLocation()
    {
        gameObject.transform.position = initialPos;
		animator.SetBool("dead", false);
    }

	public void Die()
	{
		if (!isDead)
		{
			UpdateHealth(0);
			isDead = true;
			animator.SetBool("dead", true);
			animator.SetBool("attack", false);
			animator.SetBool("chase", false);
			agent.ResetPath();
			agent.isStopped = true;
			AfterKill();
			canvasGroup.alpha = 0;
			SetLayerRecursively(gameObject.transform.Find("Model").gameObject, LayerNormal);

			if(selfRespawn){
				StartCoroutine(RespawnDelay());
			}else{
				corpsePullDown = true;
				initCorpseY = gameObject.transform.position.y;
				Destroy(GetComponent<Rigidbody>());
				target.gameObject.GetComponent<Player>().RemoveEnemy(this);
				Destroy(GetComponent<CapsuleCollider>());
				Destroy(GetComponent<NavMeshAgent>());
				StartCoroutine(DestroyDelay());
			}
		}
	}

	public void AfterKill()
	{
		target.gameObject.GetComponent<Player>().AddXp(Random.Range(minXpGive, maxXpGive));
	}

	public void ReceiveDamage(int dmg)
	{
		if (health > dmg)
		{
			UpdateHealth(health - dmg);
			Debug.Log(enemyName + " has received " + dmg + " damage from player! Health: " + health);
		}
		else
		{
			Die();
		}
	}

	public void UpdateHealth(int newHp)
	{
		health = newHp;
		gameObject.GetComponentInChildren<EnemyHealthBar>().SetHealth(health);
	}

	public void ResetHealth()
	{
		UpdateHealth(maxHealth);
		gameObject.GetComponentInChildren<EnemyHealthBar>().ResetHealth(maxHealth);
	}

	void Hit(float timeSkip) //After frameSkip amount of frame, it will do the action
    {
		if(Time.time >= nextHitTime && nextHitTime != -1)
        {
			target.gameObject.GetComponent<Player>().ReceiveDamage(damagePerHit);
			nextHitTime = Time.time + timeSkip;
        }
        else if (nextHitTime == -1)
        {
			//First hit (extra dmg)
			target.gameObject.GetComponent<Player>().ReceiveDamage(damagePerHit * 2);
			nextHitTime = Time.time + timeSkip;
		}
	}

	void UpdateCombat()
	{
		float distance = Vector3.Distance(target.position, transform.position);
		
		if (distance <= lookRadius && !isDead)
		{
			canvasGroup.alpha = 1;
			agent.SetDestination(target.position);
			animator.SetBool("chase", true);
			SetLayerRecursively(gameObject.transform.Find("Model").gameObject, LayerOutlined);
			
			if (distance <= agent.stoppingDistance && !isDead)
			{
				FaceTarget();
				animator.SetBool("attack", true);
				animator.SetBool("chase", false);
				Hit(hitDelaySeconds);
			}
            else
            {
				animator.SetBool("attack", false);
				animator.SetBool("chase", true);
				nextHitTime = -1;
			}
        }
        else
		{
			SetLayerRecursively(gameObject.transform.Find("Model").gameObject, LayerNormal);
			canvasGroup.alpha = 0;
			if(!isCampObject){
				agent.ResetPath();
				animator.SetBool("chase", false);
			}else{
				if(transform.position.x < enemyCamp.GetPos().x - enemyCamp.regionRadius || transform.position.x > enemyCamp.GetPos().x + enemyCamp.regionRadius || transform.position.z < enemyCamp.GetPos().z - enemyCamp.regionRadius || transform.position.z > enemyCamp.GetPos().z + enemyCamp.regionRadius){
					float returnX = Random.Range(enemyCamp.GetPos().x - enemyCamp.regionRadius, enemyCamp.GetPos().x + enemyCamp.regionRadius);
					float returnZ = Random.Range(enemyCamp.GetPos().z - enemyCamp.regionRadius, enemyCamp.GetPos().z + enemyCamp.regionRadius);
					agent.SetDestination(new Vector3(returnX, enemyCamp.GetPos().y, returnZ));
					animator.SetBool("chase", true);
				}else{
					agent.ResetPath();
					animator.SetBool("chase", false);
				}
			}
			
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(!isDead){
			UpdateCombat();
			canvas.transform.LookAt(2 * transform.position - lookAt.position);
		}
		if (corpsePullDown)
		{
			PullDownCorpse();
		}
	}

	void PullDownCorpse()
	{
		initCorpseY-=0.02f;
		gameObject.transform.position = new Vector3(gameObject.transform.position.x, initCorpseY, gameObject.transform.position.z);
	}
	
	void FaceTarget()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}
}