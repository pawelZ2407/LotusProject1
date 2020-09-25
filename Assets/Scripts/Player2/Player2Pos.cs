using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using System.Text;
using System;

public class Player2Pos : MonoBehaviour
{

    public GameObject EnemyBullet;
    public GameObject Shield;

    public Vector2 DesiredPos;
    public Vector2 BulletSpawn;

    public Quaternion rotation;

    public Animator anim;
    public bool Sprinting;

    public List<AudioSource> FX = new List<AudioSource>();

    void Start()
    {
        anim = transform.GetComponent<Animator>();
        Sprinting = false;
        for (int i = 0; i < transform.Find("MyFX").childCount; i++)
        {
            FX.Add(transform.Find("MyFX").GetChild(i).GetComponent<AudioSource>());
        }
    }
    public void AnimateMe(string MyAnimation)
    {
        if (MyAnimation == "Up")
        {
            anim.SetBool("MovingUp", true);
            anim.SetBool("MovingLeft", false);
            anim.SetBool("MovingDown", false);
            anim.SetBool("MovingRight", false);
        }
        else if (MyAnimation == "Left")
        {
            anim.SetBool("MovingLeft", true);
            anim.SetBool("MovingUp", false);
            anim.SetBool("MovingDown", false);
            anim.SetBool("MovingRight", false);
        }
        else if (MyAnimation == "Down")
        {
            anim.SetBool("MovingDown", true);
            anim.SetBool("MovingLeft", false);
            anim.SetBool("MovingUp", false);
            anim.SetBool("MovingRight", false);
        }
        else if (MyAnimation == "Right")
        {
            anim.SetBool("MovingRight", true);
            anim.SetBool("MovingLeft", false);
            anim.SetBool("MovingDown", false);
            anim.SetBool("MovingUp", false);
        }
        else if (MyAnimation == "Idle")
        {
            anim.SetBool("MovingRight", false);
            anim.SetBool("MovingLeft", false);
            anim.SetBool("MovingDown", false);
            anim.SetBool("MovingUp", false);
        }

        if (MyAnimation != "Idle") { PlayFX(false); } else { PlayFX(true); }

    }
    public void AnalyzePackets(string msg)
    {
        try
        {
            if (msg.Contains("Disconnect"))
            {
                Destroy(gameObject);
            }
            if (msg.Contains("pos"))
            {
                string info = msg.Replace(',', '.');                
                string[] Pos = info.Replace("pos", "").Split('a');
                
                DesiredPos.x = float.Parse(Pos[0]);
                DesiredPos.y = float.Parse(Pos[1]);
                
                transform.position = DesiredPos;
                
                Array.Clear(Pos, 0, Pos.Length);
                msg = string.Empty;
                info = string.Empty;
            }
            if (msg.Contains("abi"))
            {
                int Ability = int.Parse(msg[3].ToString());
                if (Ability == 0)
                {
                    SpawnBullet(msg);
                }
                else if (Ability == 1)
                {
                    GameObject barrier = (GameObject)Instantiate(Shield, transform);
                }
            }
            if (msg.Contains("anim"))
            {
                AnimateMe(msg.Replace("anim", ""));
            }
            if (msg.Contains("Sound"))
            {                
                if (msg.Replace("Sound", "") == "Sprinting")
                {
                    Sprinting = true;
                }
                else
                {
                    Sprinting = false;
                }
                }
        }
        catch
        {
        }
    }

    void PlayFX(bool Still)
    {
        if (Still)
        {
            for (int i = 0; i < FX.Count; i++)
            {
                FX[i].Stop();
            }
        }
        else
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
    }

    void SpawnBullet(string msg)
    {
        string Info = msg.Replace(',', '.');
        string temp = Info.Split('r')[1];

        rotation = Quaternion.Euler(0, 0, float.Parse(temp));

        GameObject projectile = (GameObject)Instantiate(EnemyBullet, transform.position, rotation);

        msg = string.Empty;
        Info = string.Empty;
    }
}
