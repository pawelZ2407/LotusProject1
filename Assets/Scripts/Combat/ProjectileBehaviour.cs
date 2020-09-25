using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{

    public float speed = 10.0f;
    float elevation;
    float heading;

    public float TimeLeft;

    Vector2 angleToVector;

    // Start is called before the first frame update
    void Start()
    {
        TimeLeft = 3;
        elevation = Mathf.Deg2Rad * transform.eulerAngles.z;
        heading = Mathf.Deg2Rad * transform.eulerAngles.y;
//        angleToVector = new Vector2(Mathf.Cos(heading) * Mathf.Sin(elevation) * -1, Mathf.Cos(elevation) * Mathf.Cos(heading));
        angleToVector = new Vector2(Mathf.Cos(elevation) * Mathf.Cos(heading), Mathf.Cos(heading) * Mathf.Sin(elevation));
    }

    // Update is called once per frame
    void Update()
    {
        TimeLeft -= Time.deltaTime;
        transform.GetComponent<Rigidbody2D>().velocity = angleToVector * speed;
        if (TimeLeft <= 0) { Destroy(gameObject); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (transform.name.Contains("Player"))
        {
            if(collision.transform.tag != "HostPlayer") { Destroy(gameObject); }
        }
        if (transform.name.Contains("Enemy"))
        {
            if (collision.transform.tag == "HostPlayer") { collision.transform.SendMessage("Damage", 10); Destroy(gameObject); }
            if (collision.transform.tag == "OtherPlayers") { Destroy(gameObject); }
        }
    }
}
