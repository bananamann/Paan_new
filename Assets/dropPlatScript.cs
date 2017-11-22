using UnityEngine;
using System.Collections;

public class dropPlatScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "player" && coll.gameObject.transform.position.y > gameObject.transform.position.y)
        {
            gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        }
    }
}
