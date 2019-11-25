using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : MonoBehaviour
{
    public GameManager manager;
    public List<Node> waypoints;
    public GameObject aStaar;
    public Grid grid;
    public Pathfinding pathfinder;
    private Node waypointCurrent;
    private int index = 0;
    private bool moveDone = true;
    private bool performedAction = false;

    // Start is called before the first frame update
    void Start()
    {
        grid = aStaar.GetComponent<Grid>();
        pathfinder = aStaar.GetComponent<Pathfinding>();
    }

    // Update is called once per frame
    void Update()
    {
        //move to the clicked location
        if (Input.GetButtonDown("Fire1") && moveDone && GameManager.get_turn())
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
               
                setDestination(hit.point);
            }

            performedAction = true;
        }

        if(performedAction && moveDone)
        {
            manager.set_turn(false);
            performedAction = false;
        }

        if (waypointCurrent != null)
        {
            pathfinder.seeker.transform.position = Vector3.MoveTowards(pathfinder.seeker.transform.position, waypointCurrent.worldPosition, .1f);
            //get to the next waypoint if it is in bounds
            if (pathfinder.seeker.transform.position == waypointCurrent.worldPosition) {
                index++;
                if (index < waypoints.Count)
                {
                    waypointCurrent = waypoints[index];
                }
                else {
                    moveDone = true;
                }


            }
        }

        //transform.Translate(Vector3.forward * Time.deltaTime);

    }

    public void setDestination(Vector3 point)
    {
        Debug.Log("set Destination");
        moveDone = false;
        index = 0;
        waypointCurrent = null;

        pathfinder.FindPath(pathfinder.seeker.position, point);
        waypoints = grid.path;
        if (waypoints != null)
        {
            waypointCurrent = waypoints[0];
        }
    }
}
