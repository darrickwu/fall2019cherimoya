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
    // Start is called before the first frame update
    void Start()
    {
        actionTime = false;
        yourStats = GetComponent<AllyStats>();
        playerMove = logicManage.GetComponent<playermovement>();
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
                    if (hit.collider.gameObject.layer == 10)
                    {
                        //rotate the player to shoot at the enemy
                        //calculate hit chance
                        //shoot
                        tgt = hit.collider.gameObject;
                        enemyStats = tgt.GetComponent<AllyStats>();
                        float hitChance = yourStats.accuracy - enemyStats.evasion;
                        bool RNGSuccess = Random.Range(0.0f, 101.0f) <= hitChance;
                        if (RNGSuccess) {
                            Debug.Log("you hit the enemy for " + yourStats.weaponDamage + " damage.");
                            enemyStats.health -= yourStats.weaponDamage;
                            Debug.Log("enemey health is " + enemyStats.health);
                        }
                        manager.set_turn(false);
                        actionTime = false;
                        playerMove.playerActionDone = true;
                    }

                }
            }
        }
    }

    public void alliedAction() {
        actionTime = true;
    }
}
