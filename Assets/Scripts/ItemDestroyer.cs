using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDestroyer : MonoBehaviour
{

    public float LifeTime;

    // Start is called before the first frame update
    void Start()
    {
        LifeTime = 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime < 0) { Destroy(gameObject); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "PickUp")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
