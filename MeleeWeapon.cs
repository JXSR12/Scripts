using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public int weaponDamage;
    public Player holder = null;
    public string weaponName;
    public int weaponLevelRestriction;
    public float weaponAdditionalRadius;

    private bool pickable = false;
    private Player activeContact = null;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(
            90,
            transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z);
    }

    // Update is called once per frame

    void Spin()
    {
        if (transform.rotation.z < 360)
        {
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,
                transform.rotation.eulerAngles.z + 0.5f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,
                0);
        }
    }

    void Handheld()
    {
        GameObject.Find("EquipSFX").GetComponent<AudioSource>().Play(0);
        GameObject rightWeaponSlot = GameObject.Find("RIGHT_WEAPON_COMBAT_SLOT");
        gameObject.transform.parent = rightWeaponSlot.transform;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 5, gameObject.transform.position.z);    
    }

    public void Drop()
    {
        Vector3 lastPos = new Vector3(gameObject.transform.parent.transform.position.x, gameObject.transform.parent.transform.position.y - 1, gameObject.transform.parent.transform.position.z);
        gameObject.transform.parent = null;
        gameObject.transform.position = lastPos;
        holder = null;
        pickable = false;
        activeContact = null;
        
        gameObject.transform.localRotation = Quaternion.Euler(
            90,
            gameObject.transform.localRotation.eulerAngles.y,
            gameObject.transform.localRotation.eulerAngles.z);
    }

    public void Take()
    {
        if (activeContact != null)
        {
            MeleeWeapon otherWeapon = GameObject.Find("RIGHT_WEAPON_COMBAT_SLOT").transform.GetComponentInChildren<MeleeWeapon>();
            if (otherWeapon != null)
            {
                otherWeapon.Drop();
            }
            
            holder = activeContact;
            holder.SetUseWeapon(true, weaponDamage);
            Handheld();
            GameObject.Find("ItemPickup").GetComponent<ItemPickup>().SetItemName("Nothing in range");
            GameObject.Find("ItemPickup").GetComponent<ItemPickup>().LeaveItemHide();
        }
    }
    
    void AdjustToHand()
    {
        gameObject.transform.position = gameObject.transform.parent.position;
        gameObject.transform.rotation = gameObject.transform.parent.rotation;
    }
    
    void Update()
    {
        if (holder == null)
        {
            Spin();
            if (pickable)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if(CheckLevel())
                    {
                        Take();
                        AdjustToHand();
                    }else
                    {
                        GameObject.Find("MessageWarning").GetComponent<MessageWarning>().ShowMessage(2, "You need to reach level " + weaponLevelRestriction + " to use this!");
                    }
                    
                }
            }
        }
        
    }

    bool CheckLevel()
    {
        return activeContact.level >= weaponLevelRestriction;
    }

    void OnTriggerEnter(Collider other)
    {
        if (holder == null && other.gameObject.GetComponent<Player>() != null)
        {
            GameObject.Find("ItemPickup").GetComponent<ItemPickup>().SetItemName(weaponName);
            GameObject.Find("ItemPickup").GetComponent<ItemPickup>().EnterItemShow();
            pickable = true;
            activeContact = other.gameObject.GetComponent<Player>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (holder == null && other.gameObject.GetComponent<Player>() != null)
        {
            GameObject.Find("ItemPickup").GetComponent<ItemPickup>().SetItemName("Nothing in range");
            GameObject.Find("ItemPickup").GetComponent<ItemPickup>().LeaveItemHide();
            pickable = false;
            activeContact = null;
        }
    }
}
