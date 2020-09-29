using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    [SerializeField] private GameObject[] meleeWeapon = new GameObject[3];
    public GameObject[] MeleeWeapon
    {
        get { return meleeWeapon; }
        set { meleeWeapon = value; }
    }
    [SerializeField] private GameObject weaponChanger;
    private int choosedWeapon = 0;
    private GameObject _currentWeapon;
    public int ChoosedWeapon {
        get { return choosedWeapon; }
        set { choosedWeapon = value; }
    }
    private void Start() 
    {
        _currentWeapon = Instantiate(meleeWeapon[0]);
    }
    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Destroy(_currentWeapon);
            choosedWeapon = 0;
            _currentWeapon = Instantiate(meleeWeapon[choosedWeapon]);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Destroy(_currentWeapon);
            choosedWeapon = 1;
            _currentWeapon = Instantiate(meleeWeapon[choosedWeapon]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Destroy(_currentWeapon);
            choosedWeapon = 2;
            _currentWeapon = Instantiate(meleeWeapon[choosedWeapon]);
        }
        WeaponsChanger meleeWeapons = weaponChanger.GetComponent<WeaponsChanger>();
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(meleeWeapons.StartAttack(_currentWeapon));
        }
        else {
            meleeWeapons.defaultPosition(_currentWeapon);
        }
    }
}
