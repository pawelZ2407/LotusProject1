using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class PlayerAnimationControl : MonoBehaviour
{

    public Animator anim;
    public NetworkManager NetManager;
    public AxisControl AxisManager;

    public string WhatAnim;
    public string OldAnim;

    public float PacketCooldown;

    // Start is called before the first frame update
    void Start()
    {
        AxisManager = GameObject.Find("AxisManager").GetComponent<AxisControl>();
        anim = transform.GetComponent<Animator>();
        NetManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AxisManager.VerticalAxis == 1)
        {
            anim.SetBool("MovingUp", true);
            WhatAnim = ("Up");
            anim.SetBool("MovingLeft", false);
            anim.SetBool("MovingDown", false);
            anim.SetBool("MovingRight", false);
        }
        else if (AxisManager.HorizontalAxis == -1)
        {
            anim.SetBool("MovingLeft", true);
            WhatAnim = ("Left");
            anim.SetBool("MovingUp", false);
            anim.SetBool("MovingDown", false);
            anim.SetBool("MovingRight", false);
        }
        else if (AxisManager.VerticalAxis == -1)
        {
            anim.SetBool("MovingDown", true);
            WhatAnim = ("Down");
            anim.SetBool("MovingLeft", false);
            anim.SetBool("MovingUp", false);
            anim.SetBool("MovingRight", false);
        }
        else if (AxisManager.HorizontalAxis == 1)
        {
            anim.SetBool("MovingRight", true);
            WhatAnim = ("Right");
            anim.SetBool("MovingLeft", false);
            anim.SetBool("MovingDown", false);
            anim.SetBool("MovingUp", false);
        }
        else
        {
            anim.SetBool("MovingRight", false);
            anim.SetBool("MovingLeft", false);
            anim.SetBool("MovingDown", false);
            anim.SetBool("MovingUp", false);
            WhatAnim = "Idle";
        }
        //.......

        PacketCooldown -= Time.deltaTime;
        if (PacketCooldown <= 0)
        {
            if (OldAnim != WhatAnim)
            {                
                try
                {
                    SendAnimation(WhatAnim);
                    OldAnim = WhatAnim;
                    PacketCooldown = 0.15f;
                }
                catch
                {

                }
            }
        }

    }
    public void SendAnimation(string Direction)
    {
        if (NetManager.Mode)
        {
            NetManager.SendPacket("anim" + Direction);
        }
    }
}
