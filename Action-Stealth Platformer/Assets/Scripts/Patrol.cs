using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    Rigidbody2D rb2d;
    BoxCollider2D wallCollider;
    public Transform groundCheck;
    public LayerMask terrainLayer;

    public float walkSpeed;

    bool shouldPatrol;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
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
            _Patrol();
        }
    }

    void _Patrol()
    {
        //Might put in fixed update
        if (!Physics2D.OverlapCircle(groundCheck.position, .1f, terrainLayer) || wallCollider.IsTouchingLayers(terrainLayer))
            Flip();
        transform.Translate(Vector2.right * walkSpeed * Time.deltaTime);
    }

    void Flip()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        walkSpeed *= -1;
        Debug.Log("Flipped");
    }
}
