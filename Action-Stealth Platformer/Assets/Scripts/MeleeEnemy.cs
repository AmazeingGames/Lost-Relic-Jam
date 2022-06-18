using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    enum AnimParameters { isWalking, isAttacking }

    [Header("Patrol")]
    Animator anim;
    protected Rigidbody2D rb2d;
    protected BoxCollider2D wallCollider;
    public Transform groundCheck;
    public LayerMask terrainLayer;

    public float walkSpeed;
    bool shouldPatrol;
    bool isFacingRight = true;

    [Header("Attack")]
    public LayerMask playerLayer;

    public float chaseSpeed;

    public Transform startPosition;
    public float rayCastLength;
    public float minAttackDistance;
    public float attackCoolDown;

    RaycastHit2D hit;
    GameObject target;
    float playerEnemyDistance;
    bool isPlayerInRange;
    bool isCooling;
    bool isInAttackMode;
    float timer;
    Vector3 endPosition;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallCollider = GetComponent<BoxCollider2D>();
        shouldPatrol = true;

        timer = attackCoolDown;
    }

    // Update is called once per frame
    void Update()
    {
        endPosition.x = startPosition.position.x + rayCastLength;

        if (isFacingRight)
            hit = Physics2D.Raycast(startPosition.position, Vector2.right, rayCastLength, playerLayer);
        else
            hit = Physics2D.Raycast(startPosition.position, Vector2.left, rayCastLength, playerLayer);

        if (hit.collider != null)
        {
            if (hit.distance > minAttackDistance)
            {
                Chase();
            }
            else
            {
                Attack();
                Debug.Log("Attack");
            }
            Debug.Log("detectedPlayer");

        }
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
        isFacingRight = !isFacingRight;
    }

    void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, hit.point, chaseSpeed * Time.deltaTime);
        Debug.Log("Chasing");
    }

    void Attack()
    {

    }

    void OnDrawGizmos()
    {
    }
}
