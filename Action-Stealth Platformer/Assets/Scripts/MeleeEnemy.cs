using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    enum AnimParameters { isWalking, isAttacking }

    Animator anim;
    protected Rigidbody2D rb2d;
    protected BoxCollider2D wallCollider;
    public Transform groundCheck;
    public LayerMask terrainLayer;

    public float walkSpeed;

    protected bool shouldPatrol;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallCollider = GetComponent<BoxCollider2D>();
        shouldPatrol = true;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        if (shouldPatrol)
        {
            Patrol();
        }
    }

    protected void Patrol()
    {
        if (!Physics2D.OverlapCircle(groundCheck.position, .1f, terrainLayer) || wallCollider.IsTouchingLayers(terrainLayer))
            Flip();
        transform.Translate(Vector2.right * walkSpeed * Time.deltaTime);
        anim.SetBool($"{AnimParameters.isWalking}", true);
    }

    protected void Flip()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        walkSpeed *= -1;
    }
}
