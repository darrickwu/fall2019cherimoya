using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticle : MonoBehaviour
{

    public float time;
    public AudioSource gunshot;
    private float timer;
    private bool playFlag = false;
    public GameManagerr manager;
    // Start is called before the first frame update

    public ParticleSystem ps;

    void Start()
    {
        timer = time;
    }

    void Update()
    {   
        if (playFlag)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                // Debug.Log("STOPPING");
                ps.Stop();
                timer = time;
                gunshot.Play();
                

                playFlag = false;
            }
        }
        
    }

    public void playParticle()
    {
        timer = time;
        //Debug.Log("PLAYING");
        ps.Play();
        playFlag = true;
    }




}
