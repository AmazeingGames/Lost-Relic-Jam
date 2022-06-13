using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMagic : MonoBehaviour
{
    public GameObject player;
    public GameObject shadowHandRef;

    ShadowHand shadowHandScript;
    PlayerMovement playerMovement;
    
    GameObject shadowHandUse;
    CircleCollider2D circCollider;
    Rigidbody2D rb2d;

    Vector2 playerPosition;
    bool isGrounded;

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
        circCollider = GetComponent<CircleCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        playerMovement = player.GetComponent<PlayerMovement>();
        shadowHandScript = shadowHandRef.GetComponent<ShadowHand>();

        if (shadowHandUse == null && shadowHandRef != null)
        {
            shadowHandUse = Instantiate(shadowHandRef);
            shadowHandUse.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform.position;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Summon shadowhand");
            SummonShadowHand(playerPosition);
        }
        if (shadowHandUse.activeSelf == true && Input.GetMouseButtonUp(0))
        {
            Debug.Log("Started shadowhand recall");
            StartCoroutine(RecallShadowHand(.5f));
        }

    }

    void FixedUpdate()
    {
        isGrounded = playerMovement.isGrounded;
    }

    GameObject SummonShadowHand(Vector2 position)
    {
        if (shadowHandUse.activeSelf == false)
        {
            shadowHandUse.SetActive(true);
            shadowHandUse.transform.position = position;
            return shadowHandUse;
        }
        return null;
    }

    IEnumerator RecallShadowHand(float duration)
    {
        var startPosition = shadowHandUse.transform.position;
        var percentComplete = .0f;

        while (percentComplete < 1.0f)
        {
            percentComplete += Time.deltaTime / duration;

            shadowHandUse.transform.position = Vector2.Lerp(startPosition, playerPosition, percentComplete);

            yield return null;
        }
        shadowHandUse.SetActive(false);
    }
}
