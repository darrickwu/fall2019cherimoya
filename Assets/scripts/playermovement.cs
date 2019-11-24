using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : MonoBehaviour
{
    public List<Node> waypoints;
    public GameObject aStaar;
    public Grid grid;
    private Node waypointCurrent;
    private int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        grid = aStaar.GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            setDestination();
        }
        if (waypointCurrent != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypointCurrent.worldPosition, .1f);
            //get to the next waypoint if it is in bounds
            if (transform.position == waypointCurrent.worldPosition) {
                index++;
                if (index < waypoints.Count)
                {
                    waypointCurrent = waypoints[index];
                }
                else {
                    index = 0;
                    waypointCurrent = null;
                }
                
            }
        }

        //transform.Translate(Vector3.forward * Time.deltaTime);

    }

    public void setDestination()
    {
        Debug.Log("set Destination");
        waypoints = grid.path;
        if (waypoints != null)
            waypointCurrent = waypoints[0];
        
    }
}
