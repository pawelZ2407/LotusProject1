using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMechanics : MonoBehaviour
{

    public float TimeLeft;

    // Start is called before the first frame update
    void Start()
    {
        TimeLeft = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        TimeLeft -= Time.deltaTime;
        if(TimeLeft < 0) { Destroy(gameObject); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag != "HostPlayer" || collision.transform.tag != "OtherPlayer")
        {
            Destroy(collision.gameObject);
        }
    }
}
