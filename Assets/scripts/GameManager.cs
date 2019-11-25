using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static bool playerTurn = true;
    public bool wait = false;
    public int time = 0;
    public GameObject playerCanvas;
    public GameObject enemyCanvas;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        playerCanvas.SetActive(get_turn());
        enemyCanvas.SetActive(!get_turn());
        //Perform AI Action and set player turn again
        if (!get_turn())
        {
            time += 1;
            if(time == 300)
            {
                set_turn(true);
                time = 0;
            }
        }
    }

    public static bool get_turn()
    {
        return playerTurn;
    }

    public void set_turn(bool currentTurn)
    {
        playerTurn = currentTurn;
    }

    //Unused at the moment. Intended to simulate enemy AI
    //performing action.
    public void AI()
    {
        if (wait)
        {
            playerCanvas.SetActive(false);
            enemyCanvas.SetActive(true);
            wait = false;
            set_turn(true);
            wait = true;
        }
    }
}
