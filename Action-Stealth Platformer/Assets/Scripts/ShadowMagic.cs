using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMagic : MonoBehaviour
{
    public GameObject player;
    public GameObject shadowHandRef;
    private LedgeDetection ledgeDetectionScript;
    PlayerMovement playerMovement;
    
    public GameObject shadowhandObject { get; private set; }
    CircleCollider2D circColliderShadowhand;
    Rigidbody2D rb2dShadowhand;

    Vector2 playerPosition;

    bool isHandRecalling;
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
    void Awake()
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

        ledgeDetectionScript = shadowhandObject.GetComponent<LedgeDetection>();
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
        if (shadowhandObject.activeSelf == true && Input.GetMouseButtonUp(0) && !ledgeDetectionScript.isLedgeDetected)
        {
            Debug.Log("Started shadowhand recall");
            StartCoroutine(RecallToPlayer(.5f, true));
        }
    }
    void FixedUpdate()
    {
        isGrounded = playerMovement.isGrounded;
        if (ledgeDetectionScript.isLedgeDetected)
        {
            rb2dShadowhand.velocity = Vector2.zero;
            StartCoroutine(RecallToPlayer(.5f, false));
        }
        else
        {
            rb2dShadowhand.velocity = new Vector2(rb2dShadowhand.velocity.x, upwardforceShadowhand);
        }
        ledgeDetectionScript.currentPos = shadowhandObject.transform.position;
    }

    GameObject SummonShadowHand(Vector2 position)
    {
        if (shadowhandObject.activeSelf == false)
        {
            playerMovement.canMove = false;
            playerMovement.canFlip = false;

            shadowhandObject.SetActive(true);
            shadowhandObject.transform.position = position;
            return shadowhandObject;
        }
        return null;
    }

    IEnumerator RecallToPlayer(float duration, bool goingToPlayer)
    {
        var startPosition = shadowhandObject.transform.position;
        var percentComplete = .0f;

        while (percentComplete < 1.0f)
        {
            percentComplete += Time.deltaTime / duration;
    
            if (goingToPlayer)
            {
                ledgeDetectionScript.enabled = false;

                playerMovement.canMove = true;
                playerMovement.canFlip = true;
                isHandRecalling = true;

                shadowhandObject.transform.position = Vector2.Lerp(startPosition, playerPosition, percentComplete);
            }
            //else if(!goingToPlayer && ledgeDetectionScript.isLedgeDetected)
                //player.transform.position = Vector2.Lerp(playerPosition, startPosition, percentComplete);
            
            yield return null;
        }

        playerMovement.canMove = true;
        playerMovement.canFlip = true;
        ledgeDetectionScript.isLedgeDetected = false;
        shadowhandObject.SetActive(false);
        isHandRecalling = false;
        ledgeDetectionScript.enabled = true;
    }
}
