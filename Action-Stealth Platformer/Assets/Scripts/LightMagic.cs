using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMagic : MonoBehaviour
{
    // Start is called before the first frame update
    // LightPlatform

    /*
     *when player press key E, if not grounded, create light platform beneanth player. 
     *Light platform disappears in 30 secs.
     *Light platform no collusion
    */
    public GameObject lightPlatform;
    public GameObject player;
    private PlayerMovement playerMovement;

    bool isGrounded;
    public float time;
    //for platform position below player
    public float yoffset = -0.25F;
    //list created for remove and add lightplatform function
    List<GameObject> lightPlatforms = new List<GameObject>();
    public int numberOfPlatforms = 4;
    GameObject lightPlatformObject;


    public Vector2 playerPosition;
    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            lightPlatformObject=(Instantiate(lightPlatform, playerPosition, Quaternion.identity));
            lightPlatformObject.SetActive(false);
            lightPlatforms.Add(lightPlatformObject);     
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition =player.transform.position;
        playerPosition.y += yoffset;

        if (Input.GetButtonDown("lightPlatform") && !isGrounded)
           SpawnLightPlatform();
    
        Debug.Log(isGrounded);
    }
    //Not once per frame but at a standard fixed rate
    private void FixedUpdate()
    {
        isGrounded = playerMovement.isGrounded;

    }

    //adding platform for CreatePlatform
    GameObject SpawnLightPlatform(Vector3 location)
    {
        foreach (GameObject lightPlatform in lightPlatforms)
        {
            if (lightPlatform.activeSelf == false)
            {
                lightPlatform.SetActive(true);
                lightPlatform.transform.position = location;
                return lightPlatform;
            }
        }
        return null;
    }
    IEnumerator RemoveLightPlatform(GameObject lightPlatform)
    {
        Debug.Log("Platform removed starts");
        
        yield return new WaitForSeconds(time);
        lightPlatform.SetActive(false);

        Debug.Log("Platform removed stops");
    }

    void SpawnLightPlatform()
    {
        GameObject lightPlatform = SpawnLightPlatform(playerPosition);
        if (lightPlatform != null)
        {
            StartCoroutine(RemoveLightPlatform(lightPlatform));
        }
    }
}
