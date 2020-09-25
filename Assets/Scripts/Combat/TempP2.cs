using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempP2 : MonoBehaviour
{

    float health;

    // Start is called before the first frame update
    void Start()
    {
        health = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (health<=0) { Destroy(gameObject); }   
    }

    public void Damage(int dmg)
    {
        health -= dmg;
    }
}
