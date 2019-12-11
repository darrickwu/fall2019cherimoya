using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticle : MonoBehaviour
{

    private float time;
    public AudioSource gunshot;
    private float timer;
    private bool playFlag = false;
    public GameManagerr manager;
    // Start is called before the first frame update

    public ParticleSystem ps;

    void Start()
    {
        time = 0.15f;
        timer = time;
    }

    void Update()
    {   
        if (playFlag)
        {
            //after done shooting play the sound
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
        if (ps.isPlaying) ps.Stop();           
        if (!ps.isPlaying) ps.Play();
        //ps.Play();
        playFlag = true;
    }




}
