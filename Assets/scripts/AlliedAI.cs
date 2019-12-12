using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlliedAI : MonoBehaviour
{
    private bool actionTime;
    private GameObject tgt;
    private UnitStats yourStats;
    private UnitStats enemyStats;
    public GameManagerr manager;
    public GameObject logicManage;
    private playermovement playerMove;
    public GameObject svd;
    private PlayParticle ps;
    private GameObject unit;
    private GameObject UNITS;
    private int currentUnit = 0;
    private bool moveDone;
    public GameObject aStaar;
    private Pathfinding pathfinder;
    // Start is called before the first frame update
    void Start()
    {
        UNITS = this.transform.gameObject;
        actionTime = false;
        pathfinder = aStaar.GetComponent<Pathfinding>();
        yourStats = transform.GetChild(0).gameObject.GetComponent<UnitStats>();
        playerMove = logicManage.GetComponent<playermovement>();
        ps = svd.GetComponent<PlayParticle>();
        unit = UNITS.transform.GetChild(0).gameObject;
        moveDone = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetButtonDown("next") && !actionTime && moveDone && manager.get_turn()) || (manager.switchAlly))
        {
            print("CHANGING ACTION");
            if (currentUnit == UNITS.transform.childCount - 1)
            {
                currentUnit = 0;
            }
            else
            {
                currentUnit++;
            }

            if (UNITS.transform.childCount > 0)
            {
                unit = UNITS.transform.GetChild(currentUnit).gameObject;
            }
            yourStats = unit.GetComponent<UnitStats>();
            svd = unit.transform.GetChild(2).gameObject;
            ps = svd.GetComponent<PlayParticle>();
            manager.switchAlly = false;
            manager.switchDestroy();
            return;
        }


        if (Input.GetButtonDown("prev") && !actionTime && moveDone && manager.get_turn())
        {
            print("CHANGING ACTION");

            if (currentUnit == 0)
            {
                currentUnit = UNITS.transform.childCount - 1;
            }
            else
            {
                currentUnit--;
            }

            unit = UNITS.transform.GetChild(currentUnit).gameObject;
            yourStats = unit.GetComponent<UnitStats>();
            svd = unit.transform.GetChild(2).gameObject;
            ps = svd.GetComponent<PlayParticle>();
            return;
        }


        //right click to shoot at target

        if (actionTime)
        {
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
                        if (Physics.Raycast(unit.transform.position, (tgt.transform.position - unit.transform.position), out hit2, Mathf.Infinity))
                        {
                            //attack code
                            if (hit2.collider.gameObject.layer == 10)
                            {
                                unit.transform.LookAt(tgt.transform);
                                //call particle scripts
                                //Debug.Log(ps);
                                ps.playParticle();
                                //wait after shooting

                                enemyStats = tgt.GetComponent<UnitStats>();
                                float hitChance = yourStats.accuracy - enemyStats.evasion;
                                bool RNGSuccess = Random.Range(0.0f, 101.0f) <= hitChance;
                                //Debug.Log("hit chance is " + hitChance);
                                if (RNGSuccess)
                                {
                                    // Debug.Log("you hit the enemy for " + yourStats.weaponDamage + " damage.");
                                    enemyStats.takeDamage(yourStats.weaponDamage);
                                    //Debug.Log("enemey health is " + enemyStats.health);
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
                pathfinder.setAction(false);
                manager.set_turn(false);
                actionTime = false;
                playerMove.playerActionDone = true;
            }
        }
    }

    public void alliedAction()
    {
        actionTime = true;
        moveDone = true;
    }

    public void setMove()
    {
        moveDone = false;
    }
    IEnumerator ExampleCoroutine()
    {
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(2);
        //end turn
        manager.set_turn(false);
        pathfinder.setAction(false);
        //prevent second move (left click)
        playerMove.playerActionDone = true;

        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
