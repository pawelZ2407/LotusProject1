using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player movement")]
    public float moveSpeed;
    public float WalkSpeed = 5;
    public float RunSpeed = 7.5f;

    [Header("Player sprinting")]
    public bool Sprinting;
    public Image sprintingButtonBG;

    [Header("Player stats")]
    public float Stamina;
    Transform StaminaBar;
    public float regenCount;
    public float CooldownBaseValue = 0.02f;

    [Header("Player audio")]
    public List<AudioSource> FX = new List<AudioSource>();

    //Oulsen 25-09-2020
    [Header("Joystick settings")]
    public Joystick joystickControls;
    public PlayerAnimationControl playerAnimationController;
    public float joyStickDeadZone = 0.6f;

    //Debug joystick stuff
    [Header("Joystick debugger")]
    public bool debugJoystick;
    public GameObject joystickDebugger;
    public TextMeshProUGUI debugJoystickVertical;
    public TextMeshProUGUI debugJoystickHorizontal;

    [Header("Network")]
    public Vector2 OldPos;

    //Private variables
    private float PacketCooldown;
    NetworkManager NetManager;
    // Start is called before the first frame update
    void Start()
    {
        NetManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        StaminaBar = GameObject.Find("StaminaBar").transform.Find("Bar").transform;
        for (int i = 0; i < transform.Find("PlayerFX").childCount; i++)
        {
            FX.Add(transform.Find("PlayerFX").GetChild(i).GetComponent<AudioSource>());
        }
        PacketCooldown = 0;
        Stamina = 10;
        moveSpeed = WalkSpeed;
        Sprinting = false;
        OldPos.x = transform.position.x;
        OldPos.y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        SendPosition();
        SprintMechanic();
        PlayerControls();

        if(joystickControls.Horizontal != 0 || joystickControls.Vertical != 0)
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
        if (Sprinting)
        {
            Sprinting = false;
            moveSpeed = WalkSpeed;
            sprintingButtonBG.color = Color.white;
        }
        else if (!Sprinting)
        {
            Sprinting = true;
            NetManager.SendPacket("SprintingSound");
            sprintingButtonBG.color = Color.yellow;
        }
    }

    private void PlayerControls()
    {
        //Note Oulsen: Use the joyStickDeadZone float in the inspector to set when the joystick will respond (value between 0.1 and 1). 
        //Example: 0.5f will enable movement when you move the joystick halfway and more towards the edge.
        //Note Oulsen: There is probably a better way to code this with less code, but it's a working system now.

        //MoveUp
        if (joystickControls.Vertical > joyStickDeadZone && joystickControls.Horizontal < joyStickDeadZone && joystickControls.Horizontal > -joyStickDeadZone)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y +  moveSpeed * Time.deltaTime);
            playerAnimationController.SwitchAnimation("Up");
        }
        //MoveDown
        if (joystickControls.Vertical < -joyStickDeadZone && joystickControls.Horizontal < joyStickDeadZone && joystickControls.Horizontal > -joyStickDeadZone)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y -  moveSpeed * Time.deltaTime);
            playerAnimationController.SwitchAnimation("Down");
        }
        //MoveRight
        if (joystickControls.Horizontal > joyStickDeadZone && joystickControls.Vertical < joyStickDeadZone && joystickControls.Vertical > -joyStickDeadZone)
        {
            transform.position = new Vector2(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);
            playerAnimationController.SwitchAnimation("Right");
        }
        //MoveLeft
        if (joystickControls.Horizontal < -joyStickDeadZone && joystickControls.Vertical < joyStickDeadZone && joystickControls.Vertical > -joyStickDeadZone)
        {
            transform.position = new Vector2(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y);
            playerAnimationController.SwitchAnimation("Left");
        }

        //MoveDiagonally (UpRight)
        if (joystickControls.Vertical > joyStickDeadZone && joystickControls.Horizontal > joyStickDeadZone)
        {
            float movespeedCorrected = moveSpeed / 1.5f;
            transform.position = new Vector2(transform.position.x + movespeedCorrected * Time.deltaTime, transform.position.y + movespeedCorrected * Time.deltaTime);

            //Needs a new animation
            playerAnimationController.SwitchAnimation("Up");
        }
        //MoveDiagonally (UpLeft)
        if (joystickControls.Vertical > joyStickDeadZone && joystickControls.Horizontal < -joyStickDeadZone)
        {
            float movespeedCorrected = moveSpeed / 1.5f;
            transform.position = new Vector2(transform.position.x - movespeedCorrected * Time.deltaTime, transform.position.y + movespeedCorrected * Time.deltaTime);

            //Needs a new animation
            playerAnimationController.SwitchAnimation("Up");
        }
        //MoveDiagonally (DownRight)
        if (joystickControls.Vertical < -joyStickDeadZone && joystickControls.Horizontal > joyStickDeadZone)
        {
            float movespeedCorrected = moveSpeed / 1.5f;
            transform.position = new Vector2(transform.position.x + movespeedCorrected * Time.deltaTime, transform.position.y - movespeedCorrected * Time.deltaTime);

            //Needs a new animation
            playerAnimationController.SwitchAnimation("Down");
        }
        //MoveDiagonally (DownLeft)
        if (joystickControls.Vertical < -joyStickDeadZone && joystickControls.Horizontal < -joyStickDeadZone)
        {
            float movespeedCorrected = moveSpeed / 1.5f;
            transform.position = new Vector2(transform.position.x - movespeedCorrected * Time.deltaTime, transform.position.y - movespeedCorrected * Time.deltaTime);

            //Needs a new animation
            playerAnimationController.SwitchAnimation("Down");
        }

        //Idle (no input)
        if (joystickControls.Vertical < joyStickDeadZone && joystickControls.Vertical > -joyStickDeadZone && joystickControls.Horizontal < joyStickDeadZone && joystickControls.Horizontal > -joyStickDeadZone)
        {
            playerAnimationController.SwitchAnimation("Idle");
        }

        //Debug Joystick values
        if (debugJoystick)
        {
            joystickDebugger.SetActive(true);
            debugJoystickHorizontal.text = "Horizontal value: " + joystickControls.Horizontal.ToString("#.####");
            debugJoystickVertical.text = "Vertical value: " + joystickControls.Vertical.ToString("#.####");
        }
        else if (!debugJoystick)
        {
            joystickDebugger.SetActive(false);
        }
    }

    void SprintMechanic()
    {
        regenCount -= Time.deltaTime;
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
