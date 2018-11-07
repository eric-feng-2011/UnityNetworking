using UnityEngine;

// Include the namespace required to use Unity UI
using UnityEngine.UI;
using UnityEngine.Networking;

using System.Collections;

public class PlayerController : NetworkBehaviour {
	
	// Create public variables for player speed, and for the Text UI game objects
	public float speed;
	
    public GameObject collectPrefab;

    private Text countText;
    private Text winText;

    // Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
    private Rigidbody rb;

    //[SyncVar(hook = "SetCountText")]
    private int count;

    // Identity the local player via coloring
    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        gameObject.name = "Local Player";
        //foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
        //{
        //    Debug.Log(obj.name);
        //}
        countText = GameObject.Find("Count Text").GetComponent<Text>();
        winText = GameObject.Find("Win Text").GetComponent<Text>();



        rb = GetComponent<Rigidbody>();

        count = 0;
        SetCountText(count);
        winText.text = "";
    }

	// Each physics step..
	void FixedUpdate ()
	{

        if (!isLocalPlayer) {
            return;
        }

		// Set some local float variables equal to the value of our Horizontal and Vertical Inputs
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

        rb.AddForce(new Vector3(moveHorizontal, 0, moveVertical) * speed);

        if (transform.position.y < 0.0f) {
            transform.position = new Vector3(0, 0.5f, 0);
        }

        if (count >= 12) {
            CmdWinLose();
        }
	}



    // When this game object intersects a collider with 'is trigger' checked, 
    // store a reference to that collider in a variable named 'other'..
    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
        {
            return;
        }
        // ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
        if (other.gameObject.CompareTag("Pick Up"))
        {
            // Add one to the score variable 'count'
            RpcPickUp();
            //Debug.Log(gameObject.name + count);
            Destroy(other.gameObject);
            collectPrefab.transform.position = new Vector3(UnityEngine.Random.Range(-9, 9), 0.5f
                                                       , UnityEngine.Random.Range(-9, 9));

            var collect = (GameObject)Instantiate(collectPrefab);
            collect.transform.parent = GameObject.FindGameObjectWithTag("PickUp")
                .GetComponent<Transform>();
            NetworkServer.Spawn(collect);
        }
    }

    // Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
    void SetCountText(int score)
    {
        // Update the text field of our 'countText' variable
        // Debug.Log(score);
        //if (!isLocalPlayer) return;
        countText.text = "Count: " + score.ToString();
    }


    [ClientRpc]
    void RpcPickUp() {
        if (isLocalPlayer)
        {
            count += 1;
            SetCountText(count);
            //Debug.Log(gameObject.name + count);
        }
    }

    [ClientRpc]
    void RpcWinLose()
    {
        if (isLocalPlayer)
        {
            winText.text = "You Win!";
        }
        else
        {
            winText = GameObject.Find("Win Text").GetComponent<Text>();
            winText.text = "You Lose!";
        }
    }

    [Command]
    void CmdWinLose() {
        RpcWinLose();
    }
}