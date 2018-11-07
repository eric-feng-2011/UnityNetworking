using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LevelManager : NetworkBehaviour {

    public GameObject collectPrefab;
    public GameObject ground;

    public float timer = 0;
    public float timeSpawn = 0.0f;

    private LevelManager singleton;

	// Use this for initialization
	//void Awake () {
 //       if (singleton != null) {
 //           DestroyImmediate(gameObject);
 //           return;
 //       }
 //       singleton = this;
 //       //DontDestroyOnLoad(gameObject);

 //       transform.position = Vector3.zero;
	//}
	
	// Update is called once per frame
	[Command]
    public void CmdSpawnCollectable () {
        float x = UnityEngine.Random.Range(- ((ground.transform.localScale.x - 1) * 10) + 1,
                                           (ground.transform.localScale.x - 1) * 10) - 1;
        float z = UnityEngine.Random.Range(-((ground.transform.localScale.z - 1) * 10) - 1,
                                           (ground.transform.localScale.z - 1) * 10) - 1;

        collectPrefab.transform.position = new Vector3(x, 0.5f, z);

        var collect = (GameObject)Instantiate(collectPrefab);
        collect.transform.parent = GameObject.FindGameObjectWithTag("PickUp")
            .GetComponent<Transform>();
        NetworkServer.Spawn(collect);
    }
}
