using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponsChanger : MonoBehaviour {

    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackSpeed = 0.2f;
    [SerializeField] private float targetAngle = 100;
    private bool inAttackMode = false;
    private bool wasAttacked = false;
    public void defaultPosition(GameObject currentWeapon) 
        {
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
        currentWeapon.transform.position = player.transform.position;
        currentWeapon.transform.parent = playerAttack.transform;
        if (currentWeapon.CompareTag("Swipe")) 
            {
            Vector2 defaultPosLeft = new Vector2(-playerAttack.MeleeWeapon[playerAttack.ChoosedWeapon].transform.localPosition.x, playerAttack.MeleeWeapon[playerAttack.ChoosedWeapon].transform.localPosition.y);
            Vector2 defaultPosRight = new Vector2(playerAttack.MeleeWeapon[playerAttack.ChoosedWeapon].transform.localPosition.x, playerAttack.MeleeWeapon[playerAttack.ChoosedWeapon].transform.localPosition.y);
            //Vector2 defaultPosUp = new Vector2(playerAttack.MeleeWeapon[playerAttack.ChoosedWeapon].transform.localPosition.x, playerAttack.MeleeWeapon[playerAttack.ChoosedWeapon].transform.localPosition.y);
            //Vector2 defaultPosDown = new Vector2(playerAttack.MeleeWeapon[playerAttack.ChoosedWeapon].transform.localPosition.x, playerAttack.MeleeWeapon[playerAttack.ChoosedWeapon].transform.localPosition.y);
            if ( animator.GetBool("MovingRight") == true && inAttackMode == false)
                {
                currentWeapon.transform.localPosition = defaultPosRight;

                Vector3 defaultRotation = new Vector3(0, 0, -30);
                currentWeapon.transform.localEulerAngles = defaultRotation;
            }
            if ( animator.GetBool("MovingLeft") == true && inAttackMode == false) 
                {
                currentWeapon.transform.localPosition = defaultPosLeft;
                Vector3 rotationLeft = new Vector3(0, 0, 30);
                currentWeapon.transform.localEulerAngles = rotationLeft;
            }
        }   
    }
    public IEnumerator StartAttack(GameObject currentWeapon)
        {
        inAttackMode = true;
        wasAttacked = true;
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
        currentWeapon.transform.parent = playerAttack.transform;
        if (animator.GetBool("MovingRight") == true) 
        {
            currentWeapon.transform.localEulerAngles = new Vector3(0, 0, targetAngle);
        }
        else if(animator.GetBool("MovingLeft") == true) {
            currentWeapon.transform.localEulerAngles = new Vector3(0, 0, -targetAngle);
        }
      /*  else if(animator.GetBool("MovingUp") == true) {
            currentWeapon.transform.localEulerAngles = new Vector3(0, 0, -targetAngle);
        }
        else if(animator.GetBool("MovingDown") == true) {
            currentWeapon.transform.localEulerAngles = new Vector3(0, 0, -targetAngle);
        }*/
        yield return new WaitForSeconds(attackSpeed / 2);
        inAttackMode = false;
        yield return new WaitForSeconds(attackSpeed);
        wasAttacked = false;
    }
}

