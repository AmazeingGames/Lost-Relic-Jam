using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LedgeDetection : MonoBehaviour
{ 
    public GameObject ledgeCheck;
    public GameObject wallCheck;
    public float wallCheckDistance = .3f;
    public LayerMask terrainLayer;
    

    public bool isTouchingWall { get; protected set; }
    public bool isLedgeDetected;

    protected bool isTouchingLedge;
    public Vector2 ledgePosBot { get; protected set; }
    public Vector2 ledgePosStart;
    public Vector2 ledgePosEnd;
    public Vector2 currentPos;


    void FixedUpdate()
    {
        CheckSurroundings();
    }

    protected bool CheckSurroundings()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.transform.position, transform.right, wallCheckDistance, terrainLayer);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.transform.position, transform.right, wallCheckDistance, terrainLayer);
        return IsTouchingLedge();
    }

    protected virtual bool IsTouchingLedge()
    {
        if (isTouchingWall && !isTouchingLedge && !isLedgeDetected)
        {
            Debug.Log("We are touching a ledge");
            isLedgeDetected = true;
            ledgePosBot = wallCheck.transform.position;
            return true;
        }
        return false;
    }
}
