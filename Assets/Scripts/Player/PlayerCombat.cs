using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour
{
    public GameObject bullet;
    public GameObject shield;

    NetworkManager NetManager;
    InventorySystem InvSystem;

    public float FireballCooldown;
    public Transform FireCooldownBar;

    public float ShieldCooldown;
    public Transform ShieldCooldownBar;

    public float Health;
    public Transform HealthBar;

    public int SelectedAbility;

    public Vector2 mousePos;
    public ParticleSystem lightningAttack;
    private float lightningCountdown;
    public float lightningTimer;

    // Use this for initialization
    void Start()
    {
        Health = 100;
        FireballCooldown = 2f;
        ShieldCooldown = 10f;
        SelectedAbility = -1;

        NetManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        InvSystem = transform.GetComponent<InventorySystem>();
        FireCooldownBar = GameObject.Find("Cooldowns").transform.Find("FireBall").transform.Find("Bar").transform;
        ShieldCooldownBar = GameObject.Find("Cooldowns").transform.Find("Shield").transform.Find("Bar").transform;
    }

    void Update()
    {
        HealthBar.localScale = new Vector3(Health/100, 1f);        
        if (Health<= 0) { Destroy(gameObject); }

        ManageTouchs();
        ManageCooldowns();
        LightningAttack();
    }

    public void Damage(float dmg)
    {
        Health -= dmg;
    }

    void ShootingMechanic(Vector2 myPos, Vector2 target)
    {
        Vector2 direction = target - myPos;
        direction.y = -direction.y;
        direction.Normalize();
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        if (SelectedAbility == 0 && FireballCooldown >= 2)
        {
            GameObject projectile = (GameObject)Instantiate(bullet, myPos, rotation);
            if (NetManager.Mode)
            {
                NetManager.SendPacket("abi"+ SelectedAbility.ToString() + "r" + Math.Round(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, 2).ToString());
            }
            FireballCooldown = 0f;
        }
    }

    void ManageTouchs()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).position.x < Screen.width / 4 && Input.GetTouch(i).position.y < Screen.height / 2 || Input.GetTouch(i).position.x > 7 * (Screen.width / 8))
            {
            }
            else
            {
                if (!InvSystem.ShowingInv)
                {
                    Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.GetTouch(i).position.x, Screen.height - Input.GetTouch(i).position.y));
                    Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
                    ShootingMechanic(myPos, target);
                }
            }
        }
    }

    void ManageCooldowns()
    {
        FireballCooldown += Time.deltaTime;
        if (FireballCooldown >= 2) { FireballCooldown = 2; }
        FireCooldownBar.localScale = new Vector3(FireballCooldown/2, FireballCooldown/2);

        ShieldCooldown += Time.deltaTime;
        if (ShieldCooldown >= 10) { ShieldCooldown = 10; }
        ShieldCooldownBar.localScale = new Vector3(ShieldCooldown/10, ShieldCooldown/10);
    }

    public void SelectFireball()
    {
        if(SelectedAbility == 0)
        {
            SelectedAbility = -1;
        }
        else
        {
            SelectedAbility = 0;
        }
    }

    public void SelectShield()
    {
        GameObject barrier = (GameObject)Instantiate(shield, transform);
        if (NetManager.Mode)
        {
            NetManager.SendPacket("abi" + "1");
        }
        ShieldCooldown = 0;
    }

    private void LightningAttack()
    {

        lightningCountdown -= Time.deltaTime;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ParticleSystem newParticle = Instantiate(lightningAttack, transform.position, transform.rotation);
            newParticle.Play();
        }
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).position.x < Screen.width / 4 && Input.GetTouch(i).position.y < Screen.height / 2 || Input.GetTouch(i).position.x > 7 * (Screen.width / 8))
            {
            }
            else if(lightningCountdown <= 0)
            {
                Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.GetTouch(i).position.x, Screen.height - Input.GetTouch(i).position.y));
                Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
                Vector2 direction = target - myPos;
                direction.y = -direction.y;
                direction.Normalize();
                Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                ParticleSystem newParticle = Instantiate(lightningAttack, transform.position, rotation);
                newParticle.Play();
                lightningCountdown = lightningTimer;
            }
            
        }
        
    }
}