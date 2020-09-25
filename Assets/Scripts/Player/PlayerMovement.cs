using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float WalkSpeed = 5;
    public float RunSpeed = 7.5f;

    public float Stamina;
    Transform StaminaBar;
    public float regenCount;

    float PacketCooldown;
    public float CooldownBaseValue = 0.02f;

    public Transform movePoint;
    public List<AudioSource> FX = new List<AudioSource>();

    public LayerMask whatStopsMovement;

    NetworkManager NetManager;

    public bool Sprinting;

    public AxisControl AxisManager;

    public Vector2 OldPos;

    // Start is called before the first frame update
    void Start()
    {
        NetManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        AxisManager = GameObject.Find("AxisManager").GetComponent<AxisControl>();
        StaminaBar = GameObject.Find("StaminaBar").transform.Find("Bar").transform;
        movePoint = transform.Find("PlayerMovePoint");
        for (int i = 0; i < transform.Find("PlayerFX").childCount; i++)
        {
            FX.Add(transform.Find("PlayerFX").GetChild(i).GetComponent<AudioSource>());
        }
        PacketCooldown = 0;
        Stamina = 10;
        moveSpeed = WalkSpeed;
        movePoint.parent = null;
        Sprinting = false;
        OldPos.x = transform.position.x;
        OldPos.y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        SendPosition();
        regenCount -= Time.deltaTime;
        SprintMechanic();
        //BasicMovement();
        MovementPlayer();

        if(AxisManager.HorizontalAxis != 0 || AxisManager.VerticalAxis != 0)
        {
            PlaySoundFX();
        }
        else
        {
            for (int i = 0; i < FX.Count; i++)
            {
                FX[i].Stop();
            }
        }
    }

    public void EnableSprint()
    {
        Sprinting = true;
        NetManager.SendPacket("SprintingSound");
    }

    private void MovementPlayer()
    {
        if (AxisManager.VerticalAxis > 0)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + moveSpeed * Time.deltaTime);
        }
        if (AxisManager.VerticalAxis < 0)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - moveSpeed * Time.deltaTime);
        }
        if (AxisManager.HorizontalAxis > 0)
        {
            transform.position = new Vector2(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);
        }
        if (AxisManager.HorizontalAxis < 0)
        {
            transform.position = new Vector2(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y);
        }
    }

    void BasicMovement()
    {
        if (Vector2.Distance(transform.position, movePoint.position) <= 0.1f)
        {
            if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(AxisManager.HorizontalAxis, AxisManager.VerticalAxis, 0f), 0.1f, whatStopsMovement))
            {
                movePoint.position += new Vector3(AxisManager.HorizontalAxis, AxisManager.VerticalAxis, 0f);
            }
        }
    }
    void SprintMechanic()
    {
        StaminaBar.localScale = new Vector3(Stamina / 10, 1);

        if (Sprinting == true && Stamina > 0)
        {
            moveSpeed = RunSpeed;
            Stamina -= Time.deltaTime * 1.5f;
            regenCount = 2f;
        }
        else if (Sprinting == true && Stamina < 0.1f)
        {
            Sprinting = false;
            moveSpeed = WalkSpeed;
            NetManager.SendPacket("WalkingSound");
        }
        if (!Sprinting)
        {
            if (regenCount < 0 && Stamina < 10)
            {
                Stamina += Time.deltaTime * 0.75f;
            }
        }
    }
    void SendPosition()
    {
        if (NetManager.Mode)
        {
            PacketCooldown -= Time.deltaTime;
            if (OldPos.x != transform.position.x || OldPos.y != transform.position.y)
            {
                if (PacketCooldown <= 0)
                {
                    try
                    {
                        NetManager.SendPacket("pos" + Math.Round((double)transform.position.x, 1).ToString() + "a" + Math.Round((double)transform.position.y, 1).ToString());
                        PacketCooldown = CooldownBaseValue;
                    }
                    catch
                    {
                    }
                }
                OldPos.x = transform.position.x;
                OldPos.y = transform.position.y;
            }                       
        }
    }
    void PlaySoundFX()
    {
        if (!Sprinting)
        {
            if (!FX[0].isPlaying)
            {
                for (int i = 0; i < FX.Count; i++)
                {
                    FX[i].Stop();
                }
                FX[0].Play();
            }
        }
        else
        {
            if (!FX[1].isPlaying)
            {
                for (int i = 0; i < FX.Count; i++)
                {
                    FX[i].Stop();
                }
                FX[1].Play();
            }
        }
    }
    public void SendDisconnect()
    {
        NetManager.Disconnect();
    }
}
