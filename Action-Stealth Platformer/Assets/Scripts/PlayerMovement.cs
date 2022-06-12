using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : LedgeDetection
{
    Rigidbody2D rb2d;
    Animator anim;
    
    public float moveAcceleration;
    public float maxMoveSpeed;
    public float deceleration;

    public float groundRayCastLength;
    public float jumpForce = 12f;
    public float airLinearDrag = 2.5f;
    public float fallMultiplier = 8f;
    public float lowJumpFallMultiplier = 5f;

    public float jumpRememberTime = .25f;
    public float groundedRememberTime = .25f;


    [Header("Corner Correction (don't change y or z)")]
    public float topRayCastLength;
    public Vector3 edgeRaycastOffset;
    public Vector3 innerRaycastOffset;

    float jumpPressedRemember = 0f;
    float groundedRemember = 0f;

    bool isFacingRight = true;

    [Header("Ledge Climb")]


    public float ledgeClimbAnimLength;

    //Implemented new solution for start position
    //public float ledgeClimbStartXOffset;
    //public float ledgeClimbStartYOffset;

    public float ledgeClimbEndXOffset;
    public float ledgeClimbEndYOffset;

    public Vector2 startAdjustment;

    bool canLedgeClimb = false;
    CapsuleCollider2D col;

    enum AnimStates { Idle, Run, Jump, Fall, Landing, LedgeClimb }
    enum AnimParamaters { isGrounded, horInput, isJumping, isFalling, isLedgeClimbing }

    private Vector3 groundRaycastOffset;
    [HideInInspector]public bool isGrounded => Physics2D.Raycast(transform.position + groundRaycastOffset, Vector2.down, groundRayCastLength, terrainLayer) || Physics2D.Raycast(transform.position - groundRaycastOffset, Vector2.down, groundRayCastLength, terrainLayer);

    float horizontalInput;
    float verticalInput;
    bool isChaningDirection => (rb2d.velocity.x > 0f && horizontalInput < 0f || rb2d.velocity.x < 0 && horizontalInput > 0);
    bool canJump => jumpPressedRemember > 0 && groundedRemember > 0;
    bool canCornerCorrect => Physics2D.Raycast(transform.position + edgeRaycastOffset, Vector2.up, topRayCastLength, terrainLayer) && !Physics2D.Raycast(transform.position + innerRaycastOffset, Vector2.up, topRayCastLength, terrainLayer) || Physics2D.Raycast(transform.position - edgeRaycastOffset, Vector2.up, topRayCastLength, terrainLayer) && !Physics2D.Raycast(transform.position - innerRaycastOffset, Vector2.up, topRayCastLength, terrainLayer);
    bool isFalling => rb2d.velocity.y < 0;

    bool canMove = true;
    bool canFlip = true;



    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    // Start is called before the first frame update 
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        anim.SetBool($"{AnimParamaters.isGrounded}", isGrounded);
        anim.SetFloat($"{AnimParamaters.horInput}", Mathf.Abs(horizontalInput));
        horizontalInput = Input.GetAxisRaw("Horizontal");


        //flip when input is negative
        if (horizontalInput < 0 && isFacingRight)
            Flip();
        //flip when input is positive
        else if(horizontalInput > 0 && !isFacingRight)
            Flip();
        groundedRemember -= Time.deltaTime;
        jumpPressedRemember -= Time.deltaTime;
        if (isGrounded)
        {
            anim.SetBool($"{AnimParamaters.isJumping}", false);
            anim.SetBool($"{AnimParamaters.isFalling}", false);

            groundedRemember = groundedRememberTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressedRemember = jumpRememberTime;
        }
       
        if (isFalling)
        {
            anim.SetBool($"{AnimParamaters.isFalling}", true);
            anim.SetBool($"{AnimParamaters.isJumping}", false);
        }
        LedgeClimb();

    }

    void FixedUpdate()
    {
        if(isFalling)
            CheckSurroundings();
        //Debug.Log($"Can ledge climb: {canLedgeClimb}");
        //Debug.Log($"Is touching wall: {isTouchingWall}");
        //Debug.Log($"Is ledge detected: {isLedgeDetected}");

        if (canMove)
            MoveCharacter();
       
        if (isGrounded)
        {
            //This makes me jump higher when moving
            ApplyGroundLinearDrag();
        }
        else
        {
            ApplyAirLinearDrag();
            FallMultiplier();
        }
        if (canJump)
        {
            //Debug.Log($"{canJump}");
            jumpPressedRemember = 0;
            groundedRemember = 0;
            Jump();
           
        }
        if (canCornerCorrect && !isGrounded)
            CornerCorrect(rb2d.velocity.y);
}
    void LedgeClimb()
    {
        if (isLedgeDetected && !canLedgeClimb && isFalling)
        {
            canLedgeClimb = true;
            canMove = false;
            canFlip = false;

            currentPos = transform.position;

            //Implemented new solution for start position
            if (isFacingRight)
            {
                //ledgePosStart = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbStartXOffset, Mathf.Floor(ledgePosBot.y) + ledgeClimbStartYOffset);
                ledgePosEnd = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbEndXOffset, Mathf.Floor(ledgePosBot.y) + ledgeClimbEndYOffset);
            }
            else
            {
                //ledgePosStart = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbStartXOffset, Mathf.Floor(ledgePosBot.y) + ledgeClimbStartYOffset);
                ledgePosEnd = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbEndXOffset, Mathf.Floor(ledgePosBot.y) + ledgeClimbEndYOffset);
            }

            anim.SetBool($"{AnimParamaters.isLedgeClimbing}", canLedgeClimb);
            StartCoroutine(FinishLedgeClimb());
        }
        
         if(canLedgeClimb)
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

    void MoveCharacter()
    {
        rb2d.AddForce(new Vector2(horizontalInput, 0f) * moveAcceleration);

        if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxMoveSpeed, rb2d.velocity.y);
    }

    void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(horizontalInput) < .4f || isChaningDirection)
        {
            rb2d.drag = deceleration;
        }
        else
            rb2d.drag = 0f;
    }

    void ApplyAirLinearDrag()
    {
        rb2d.drag = airLinearDrag;
    }

    void Jump()
    {
        ApplyAirLinearDrag();
        anim.SetBool($"{AnimParamaters.isJumping}", true);
        anim.SetBool($"{AnimParamaters.isFalling}", false);

        rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    

    private void OnDrawGizmos()

    {
        //Ground Check
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + groundRaycastOffset, transform.position + groundRaycastOffset + Vector3.down * groundRayCastLength);
        Gizmos.DrawLine(transform.position - groundRaycastOffset, transform.position - groundRaycastOffset + Vector3.down * groundRayCastLength);

        //Corner Check
        Gizmos.DrawLine(transform.position + edgeRaycastOffset, transform.position + edgeRaycastOffset + Vector3.up * topRayCastLength);
        Gizmos.DrawLine(transform.position - edgeRaycastOffset, transform.position - edgeRaycastOffset + Vector3.up * topRayCastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset, transform.position + innerRaycastOffset + Vector3.up * topRayCastLength);
        Gizmos.DrawLine(transform.position - innerRaycastOffset, transform.position - innerRaycastOffset + Vector3.up * topRayCastLength);

        //Corner Distance Check
        Gizmos.DrawLine(transform.position - innerRaycastOffset + Vector3.up * topRayCastLength, transform.position - innerRaycastOffset + Vector3.up * topRayCastLength + Vector3.left * topRayCastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset + Vector3.up * topRayCastLength, transform.position + innerRaycastOffset + Vector3.up * topRayCastLength + Vector3.right * topRayCastLength);

        //Ledge Grab
        Gizmos.color = Color.red;
        //Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, terrainLayer);
        //Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, terrainLayer);

        Gizmos.color = Color.green;

        //Implemented new solution for start position
        //Gizmos.DrawLine(ledgePosStart, ledgePosEnd);
        Gizmos.DrawLine(currentPos, ledgePosEnd);

    }

    void FallMultiplier()
    {
        if (isFalling)
        {
            rb2d.gravityScale = fallMultiplier;
        }
        else if (rb2d.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb2d.gravityScale = lowJumpFallMultiplier;
        }
        else
            rb2d.gravityScale = 1f;
    }

    void Flip()
    {
        if (canFlip && !canLedgeClimb)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }   
    }

    void CornerCorrect(float yVelocity)
    {
        //Push to right
        RaycastHit2D hit = Physics2D.Raycast(transform.position - innerRaycastOffset + Vector3.up * topRayCastLength, Vector3.left, topRayCastLength, terrainLayer);

        if (hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRayCastLength, transform.position - edgeRaycastOffset + Vector3.up * topRayCastLength);
            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, transform.position.z);
            rb2d.velocity = new Vector2(rb2d.velocity.x, yVelocity);
        }

        //Push to left
        hit = Physics2D.Raycast(transform.position + innerRaycastOffset + Vector3.up * topRayCastLength, Vector3.right, topRayCastLength, terrainLayer);
        if (hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRayCastLength, transform.position + edgeRaycastOffset + Vector3.up * topRayCastLength);
            transform.position = new Vector3(transform.position.x - newPos, transform.position.y, transform.position.z);
            rb2d.velocity = new Vector2(rb2d.velocity.x, yVelocity);
        }
    }
}
