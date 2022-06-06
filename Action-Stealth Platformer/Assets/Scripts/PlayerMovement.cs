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
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position + groundRaycastOffset, transform.position + groundRaycastOffset + Vector3.down * groundRayCastLength);
        Gizmos.DrawLine(transform.position - groundRaycastOffset, transform.position - groundRaycastOffset + Vector3.down * groundRayCastLength);
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

}
