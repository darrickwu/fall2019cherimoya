using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlliedAI : MonoBehaviour
{
    private bool actionTime;
    private GameObject tgt;
    private AllyStats yourStats;
    private AllyStats enemyStats;
    public GameManagerr manager;
    public GameObject logicManage;
    private playermovement playerMove;
    public GameObject svd;
    private PlayParticle ps;
    // Start is called before the first frame update
    void Start()
    {
        actionTime = false;
        yourStats = GetComponent<AllyStats>();
        playerMove = logicManage.GetComponent<playermovement>();
        ps = svd.GetComponent<PlayParticle>();
    }

    // Update is called once per frame
    void Update()
    {
        //right click to shoot at target
        if (actionTime) {
            if (Input.GetButtonDown("Fire2"))
            {
                
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    //layer 10 is an enemy and raycast from this player to the enemy also succeeds
                    if (hit.collider.gameObject.layer == 10)
                    {
                        tgt = hit.collider.gameObject;
                        RaycastHit hit2;

                        //another raycast from this position to the enemy to check line of sight
                        if (Physics.Raycast(transform.position, (tgt.transform.position - transform.position), out hit2, Mathf.Infinity))
                        {
                            //attack code
                            if (hit2.collider.gameObject.layer == 10)
                            {
                                transform.LookAt(tgt.transform);
                                //call particle scripts
                                ps.playParticle();
                                //wait after shooting
                                
                                enemyStats = tgt.GetComponent<AllyStats>();
                                float hitChance = yourStats.accuracy - enemyStats.evasion;
                                bool RNGSuccess = Random.Range(0.0f, 101.0f) <= hitChance;
                                Debug.Log("hit chance is " + hitChance);
                                if (RNGSuccess)
                                {
                                    Debug.Log("you hit the enemy for " + yourStats.weaponDamage + " damage.");
                                    enemyStats.health -= yourStats.weaponDamage;
                                    Debug.Log("enemey health is " + enemyStats.health);
                                }
                                //prevent second shot
                                actionTime = false;
                                StartCoroutine(ExampleCoroutine());
                            }
                        }
                            //rotate the player to shoot at the enemy
                            //calculate hit chance
                            //shoot
                            
                        
                    }

                }
            }
            if (Input.GetButtonDown("Jump"))
            {
                manager.set_turn(false);
                actionTime = false;
                playerMove.playerActionDone = true;
            }
        }
    }

    public void alliedAction() {
        actionTime = true;
    }



    IEnumerator ExampleCoroutine()
    {
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(2);
        //end turn
        manager.set_turn(false);
        //prevent second move (left click)
        playerMove.playerActionDone = true;
       
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
