using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb2d;
    Animator anim;
    
    public float moveAcceleration;
    public float maxMoveSpeed;
    public float deceleration;
    public LayerMask groundLayer;
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

    enum AnimStates { Idle, Run, Jump, Fall, Landing }
    enum AnimParamaters { isGrounded, horInput, isJumping, isFalling }

    private Vector3 groundRaycastOffset;
    bool isGrounded => Physics2D.Raycast(transform.position + groundRaycastOffset, Vector2.down, groundRayCastLength, groundLayer) || Physics2D.Raycast(transform.position - groundRaycastOffset, Vector2.down, groundRayCastLength, groundLayer);

    float horizontalInput;
    float verticalInput;
    bool isChaningDirection => (rb2d.velocity.x > 0f && horizontalInput < 0f || rb2d.velocity.x < 0 && horizontalInput > 0);
    bool canJump => jumpPressedRemember > 0 && groundedRemember > 0;
    bool canCornerCorrect => Physics2D.Raycast(transform.position + edgeRaycastOffset, Vector2.up, topRayCastLength, groundLayer) && !Physics2D.Raycast(transform.position + innerRaycastOffset, Vector2.up, topRayCastLength, groundLayer) || Physics2D.Raycast(transform.position - edgeRaycastOffset, Vector2.up, topRayCastLength, groundLayer) && !Physics2D.Raycast(transform.position - innerRaycastOffset, Vector2.up, topRayCastLength, groundLayer);


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
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool($"{AnimParamaters.isGrounded}", isGrounded);
        anim.SetFloat($"{AnimParamaters.horInput}", Mathf.Abs(horizontalInput));
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput < 0 && isFacingRight)
            Flip();
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
       
        if (rb2d.velocity.y < 0)
        {
            anim.SetBool($"{AnimParamaters.isFalling}", true);
            anim.SetBool($"{AnimParamaters.isJumping}", false);
        }
             
    }

    void FixedUpdate()
    {
        MoveCharacter();
       
        if(isGrounded)
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
            Debug.Log($"{canJump}");
            jumpPressedRemember = 0;
            groundedRemember = 0;
            Jump();
        }
        if (canCornerCorrect)
            CornerCorrect(rb2d.velocity.y);
    }


    void MoveCharacter()
    {
        rb2d.AddForce(new Vector2(horizontalInput, 0f) * moveAcceleration);

        if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
        {
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxMoveSpeed, rb2d.velocity.y);
        }
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
    }

    void FallMultiplier()
    {
        if (rb2d.velocity.y < 0)
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
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void CornerCorrect(float yVelocity)
    {
        //Push to right
        RaycastHit2D hit = Physics2D.Raycast(transform.position - innerRaycastOffset + Vector3.up * topRayCastLength, Vector3.left, topRayCastLength, groundLayer);

        if (hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRayCastLength, transform.position - edgeRaycastOffset + Vector3.up * topRayCastLength);
            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, transform.position.z);
            rb2d.velocity = new Vector2(rb2d.velocity.x, yVelocity);
        }

        //Push to left
        hit = Physics2D.Raycast(transform.position + innerRaycastOffset + Vector3.up * topRayCastLength, Vector3.right, topRayCastLength, groundLayer);
        if (hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRayCastLength, transform.position + edgeRaycastOffset + Vector3.up * topRayCastLength);
            transform.position = new Vector3(transform.position.x - newPos, transform.position.y, transform.position.z);
            rb2d.velocity = new Vector2(rb2d.velocity.x, yVelocity);
        }
    }
}
