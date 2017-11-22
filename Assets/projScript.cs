using UnityEngine;
using System.Collections;

public class projScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag.Contains("player") || (coll.gameObject.tag.Contains("platform"))) {
            Destroy(gameObject);
        }
    }
}
