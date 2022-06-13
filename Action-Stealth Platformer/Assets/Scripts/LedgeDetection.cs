using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LedgeDetection : MonoBehaviour
{ 
    public Transform ledgeCheck;
    public Transform wallCheck;
    public float wallCheckDistance = .3f;
    public LayerMask terrainLayer;

    protected bool isTouchingWall;
    protected bool isTouchingLedge;
    protected bool isLedgeDetected;

    protected Vector2 ledgePosBot;
    protected Vector2 ledgePosStart;
    protected Vector2 ledgePosEnd;
    protected Vector2 currentPos;


    void FixedUpdate()
    {
        CheckSurroundings();
    }

    bool CheckSurroundings()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, terrainLayer);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, terrainLayer);

        if (isTouchingWall && !isTouchingLedge && !isLedgeDetected)
        {
            Debug.Log("We are touching a ledge");
            isLedgeDetected = true;
            ledgePosBot = wallCheck.position;
            return true;
        }
        return false;
    }
}
