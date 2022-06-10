using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeClimb : MonoBehaviour
{
    enum AnimStates { Idle, Run, Jump, Fall, Landing, LedgeClimb }
    enum AnimParamaters { isGrounded, horInput, isJumping, isFalling, isLedgeClimbing }

    Vector2 ledgePosBot;
    Vector2 ledgePosEnd;
    Vector2 currentPos;

    public float ledgeClimbEndXOffset;
    public float ledgeClimbEndYOffset;

    public Vector2 startAdjustment;

    bool isTouchingWall;
    bool isTouchingLedge;

    bool canLedgeClimb = false;
    bool isLedgeDetected;

    public Transform ledgeCheck;
    public Transform wallCheck;

    public LayerMask terrainLayer;
    bool isFacingRight = true;

    public float wallCheckDistance;

    public float ledgeClimbAnimLength;

    Rigidbody2D rb2d;
    Animator anim;
    CircleCollider2D col;

    bool isFalling => rb2d.velocity.y < 0;
    bool canMove = true;
    bool canFlip = true;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ClimbLedge();
    }

    void FixedUpdate()
    {
        CheckSurroundings();
    }

    void ClimbLedge()
    {
        if (isLedgeDetected && !canLedgeClimb && isFalling)
        {
            canLedgeClimb = true;
            canMove = false;
            canFlip = false;

            currentPos = transform.position;

            if (isFacingRight)
            {
                ledgePosEnd = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbEndXOffset, Mathf.Floor(ledgePosBot.y) + ledgeClimbEndYOffset);


            }
            else
            {
                ledgePosEnd = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbEndXOffset, Mathf.Floor(ledgePosBot.y) + ledgeClimbEndYOffset);
            }

            anim.SetBool($"{AnimParamaters.isLedgeClimbing}", canLedgeClimb);
            StartCoroutine(FinishLedgeClimb());
        }

        if (canLedgeClimb)
            transform.position = currentPos + startAdjustment;

    }

    IEnumerator FinishLedgeClimb()
    {
        yield return new WaitForSeconds(ledgeClimbAnimLength);
        Debug.Log("Finish ledge climb");
        canLedgeClimb = false;
        transform.position = ledgePosEnd;
        canMove = true;
        canFlip = true;
        isLedgeDetected = false;
        anim.SetBool($"{AnimParamaters.isLedgeClimbing}", canLedgeClimb);
    }

    void CheckSurroundings()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, terrainLayer);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, terrainLayer);

        if (isTouchingWall && !isTouchingLedge && !isLedgeDetected && isFalling)
        {
            Debug.Log("We are touching a ledge");
            isLedgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }

    private void OnDrawGizmos()
    {
        //Ledge Grab
        Gizmos.color = Color.red;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(currentPos, ledgePosEnd);

    }
}
