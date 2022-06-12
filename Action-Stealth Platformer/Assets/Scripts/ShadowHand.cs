using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShadowHand : MonoBehaviour
{
    Rigidbody2D rb2d;
    CircleCollider2D col;

    public Transform ledgeCheck;
    public Transform wallCheck;
    public float wallCheckDistance;

    public float moveSpeed;

    bool isTouchingWall;
    bool isTouchingLedge;
    bool isLedgeDetected;

    Vector2 ledgePosBot;
    Vector2 ledgePosStart;
    Vector2 ledgePosEnd;
    Vector2 currentPos;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = new Vector2(0, moveSpeed);
    }

    void FixedUpdate()
    {
        
    }

    void CheckSurroundings()
    {
        //isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, terrainLayer);
        //isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, terrainLayer);

        if (isTouchingWall && !isTouchingLedge && !isLedgeDetected)
        {
            Debug.Log("We are touching a ledge");
            isLedgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }
}
