using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{

    [Header("Pathfinding")]
    public Transform target;
    public float detectionDistance;
    public float pathUpdate = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWayPointDistance = 3f;
    public float jumbHeightRequirement = .8f;
    public float jumpHeight = .3f;
    public float jumpCheckOffset = .1f;

    [Header("Behaviour")]
    public bool shouldFollow = true;
    public bool canJump = true;
    public bool canChangeDirection = true;

    //Path path;
    int currentWaypoint = 0;
    bool isGrounded;
    //Seeker seeker;
    Rigidbody2D rb2d;



    // Start is called before the first frame update
    void Start()
    {
        //seeker = GetComponent<Seeker>();
        rb2d = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (TargetDetected() && shouldFollow)
            PathFollow();
    }

    void UpdatePath()
    {
        //if (shouldFollow && TargetDetected() && seeker.IsDone())
            //seeker.StartPath(rb2d.position, target.position, OnPathComplete());
    }

    void PathFollow()
    {
        //if (path == null)
            //return;

        isGrounded = Physics2D.Raycast(transform.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);

        //Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb2d.position).normalized();
        //Vector2 force = direction * speed * Time.deltaTime;

        if (canJump && isGrounded)
        {
            //if (direction.y > jumbHeightRequirement)
                //rb2d.AddForce(Vector2.up * speed * jumpHeight);
        }

        //rb2d.AddForce(force);

        //float distance = Vector2.Distance(rb2d.position, path.vectorPath[currentWaypoint]);
        //if (distance < nextWaypointDistance)
            currentWaypoint++;

        if (canChangeDirection)
        {
            if (rb2d.velocity.x > .05f)
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (rb2d.velocity.x < -.05f)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private bool TargetDetected()
    {
        return Vector2.Distance(transform.position, target.transform.position) < detectionDistance;
    }

    /*
    private void OnPathComplete(Path p)
    {
        if (!path.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    */

}


