using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GameManagerr manager;
    public Transform seeker, target;
   // public Vector3 pt;
    Grid grid;
    public GameObject UNITS;
    private int currentUnit = 0;
    private bool findingPath;
    private bool moveDone;
    private bool actionTime;
    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Start()
    {
        findingPath = false;
        moveDone = true;
        actionTime = false;
    }

    private void Update()
    {
        if ((Input.GetButtonDown("next") && !findingPath && moveDone && !actionTime) || (manager.switchAction))
        {
            //print("CHANGING PATH");

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
                seeker = UNITS.transform.GetChild(currentUnit);

            }
            manager.switchAction = false;
            manager.switchDestroy(); 
        }


        if (Input.GetButtonDown("prev") && !findingPath && moveDone && !actionTime)
        {
            //print("CHANGING PATH");

            if (currentUnit == 0)
            {
                currentUnit = UNITS.transform.childCount - 1;
            }
            else
            {
                currentUnit--;
            }

            seeker = UNITS.transform.GetChild(currentUnit);
        }


    }

    public void setDone(bool choice)
    {
        moveDone = choice;
    }

    public void setAction(bool choice)
    {
        actionTime = choice;
    }

    //some actual a* stuff
    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        findingPath = true;
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            //pick minimum fcost node out the openset
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost 
                    == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            //we chose a node to put in closed and its no longer open
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            //we made it
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return; 
            }

            //if new path to neighhbor is shorter or neighbor is not in open
            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if ( newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    //gcost is cost to get to neighbor
                    //hcost is dist to tgt node
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }

        }
    }

    //from parent back to start node using parents of nodes
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();        
        grid.path = path;
        findingPath = false;
    }

    //costs 14 to go diagonal 10 to gup up down left right
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
