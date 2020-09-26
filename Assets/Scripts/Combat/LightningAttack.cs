using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAttack : MonoBehaviour
{
    private ParticleSystem particles;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if (particles.isStopped)
        {
            Destroy(this.gameObject);
        }
    }
}
