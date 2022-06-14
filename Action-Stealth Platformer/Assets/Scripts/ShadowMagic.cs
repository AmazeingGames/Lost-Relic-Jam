using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMagic : MonoBehaviour
{
    public GameObject player;
    public GameObject shadowHandRef;
    private LedgeDetection ledgeDetection;
    PlayerMovement playerMovement;
    
    GameObject shadowhandObject;
    CircleCollider2D circColliderShadowhand;
    Rigidbody2D rb2dShadowhand;

    Vector2 playerPosition;
    bool isGrounded;

    public float upwardforceShadowhand;
    // Start is called before the first frame update

    /* Shadow hand grapplhook:
     * While player is grounded and holding fire:
     *      Summons grapplehook moving at a constant upward velocity
     *      Disables player actions such as movement, flipping, climping, etc.
     *      Once a ledge is detected lerps the player to that point and initiates ledge climb
     * If player lets go of fire before a ledge is found:
     *      Allows player to perform actions listed above
     *      Lerps grapplehook to player
     *      Once grapple reaches playe becomes disabled
     */
    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();

        if (shadowhandObject == null && shadowHandRef != null)
        {
            shadowhandObject = Instantiate(shadowHandRef);
            shadowhandObject.SetActive(false);
        }

        circColliderShadowhand = shadowhandObject.GetComponent<CircleCollider2D>();
        rb2dShadowhand = shadowhandObject.GetComponent<Rigidbody2D>(); 

        Debug.Log("ShadowhandObject null: " + (shadowhandObject == null));

        ledgeDetection = shadowhandObject.GetComponent<LedgeDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform.position;

        if (Input.GetMouseButtonDown(0) && playerMovement.isTouchingWall)
        {

            Debug.Log("Summon shadowhand");
            SummonShadowHand(playerPosition);
        }
        if (shadowhandObject.activeSelf == true && Input.GetMouseButtonUp(0) && !ledgeDetection.isLedgeDetected)
        {
            Debug.Log("Started shadowhand recall");
            StartCoroutine(RecallTo(.5f, true));
        }
    }
    void FixedUpdate()
    {
        isGrounded = playerMovement.isGrounded;
        if (ledgeDetection.isLedgeDetected)
        {
            rb2dShadowhand.velocity = Vector2.zero;
            StartCoroutine(RecallTo(.5f, false));
        }
        else
        {
            rb2dShadowhand.velocity = new Vector2(rb2dShadowhand.velocity.x, upwardforceShadowhand);
        }

        Debug.Log("ledge dectect" + (ledgeDetection.isLedgeDetected));
    }

    GameObject SummonShadowHand(Vector2 position)
    {
        if (shadowhandObject.activeSelf == false)
        {
            shadowhandObject.SetActive(true);
            shadowhandObject.transform.position = position;
            return shadowhandObject;
        }
        return null;
    }

    IEnumerator RecallTo(float duration, bool goingToPlayer)
    {
        var startPosition = shadowhandObject.transform.position;
        var percentComplete = .0f;

        while (percentComplete < 1.0f)
        {
            percentComplete += Time.deltaTime / duration;
    
            if (goingToPlayer)
                shadowhandObject.transform.position = Vector2.Lerp(startPosition, playerPosition, percentComplete);
            else if (playerMovement.canLedgeClimb == false && goingToPlayer == false)
            {
                player.transform.position = Vector2.Lerp(playerPosition, startPosition, percentComplete);
            }
            
            yield return null;

            Debug.Log("Stop");
        }

        playerMovement.canMove = true;
        ledgeDetection.isLedgeDetected = false;
        shadowhandObject.SetActive(false);
    }
}
