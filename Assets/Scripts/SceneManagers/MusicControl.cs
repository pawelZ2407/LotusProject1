using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class MusicControl : MonoBehaviour
{

    public List<AudioSource> Themes;

    public Transform Player;
    public GameObject[] FireBalls;

    public List<float> Distances;
    public float smallest;
    public float NoCombat;
    public float VolumeDegrade;

    // Start is called before the first frame update
    void Start()
    {
        VolumeDegrade = 10;

        Themes.Add(transform.Find("Menu").transform.GetComponent<AudioSource>());
        Themes.Add(transform.Find("Combat").transform.GetComponent<AudioSource>());
        Themes.Add(transform.Find("Wilderness").transform.GetComponent<AudioSource>());

        Themes[0].Stop();
        Themes[1].Stop();
        Themes[2].Stop();
        smallest = 100;
    }

    // Update is called once per frame
    void Update()
    {
        SetCombatMusic();
        SetMenuMusic();
        SetWildernessMusic();
    }

    void SetCombatMusic()
    {
        try
        {

            if (Player == null) { Player = GameObject.Find("Player(Clone)").transform; }

            NoCombat -= Time.deltaTime;
            FireBalls = GameObject.FindGameObjectsWithTag("Projectile");
            for (int i = 0; i < FireBalls.Length; i++)
            {
                Distances.Add(Vector2.Distance(Player.position, FireBalls[i].transform.position));
            }

            if (Distances.Count > 0) { smallest = Distances.Min(); } else { smallest = 100; }

            if (smallest <= 10)
            {
                if (!Themes[1].isPlaying)
                {
                    StopAllMusic();
                    Themes[1].Play();
                    Themes[1].volume = 0.3f;
                }
                NoCombat = 15;
            }
            Distances.Clear();

            if (Themes[1].isPlaying && NoCombat <= 0)
            {
                VolumeDegrade -= Time.deltaTime;
                Themes[1].volume = Themes[1].volume * (VolumeDegrade / 10);
                if (Themes[1].volume <= 0.001f)
                {
                    Themes[1].volume = 0;
                    VolumeDegrade = 10;
                    Themes[1].Stop();
                }
            }
        }
        catch
        {
        }
    }

    void SetMenuMusic()
    {
        if(SceneManager.GetActiveScene().name == "Menu")
        {

            Themes[0].volume = 0.5f;

            if (!Themes[0].isPlaying)
            {
                Themes[0].Play();
            }
        }
        else
        {
            Themes[0].Stop();
        }
    }

    void StopAllMusic()
    {
        for (int i = 0; i < Themes.Count; i++)
        {
            Themes[i].Stop();
        }
    }

    void SetWildernessMusic()
    {
        int songsPlaying = 0;
        for (int i = 0; i < Themes.Count; i++)
        {
            if (Themes[i].isPlaying)
            {
                songsPlaying += 1;
            }
        }

        if (songsPlaying == 0)
        {
            if (!Themes[2].isPlaying)
            {
                Themes[2].Play();
            }
        }
    }

}
