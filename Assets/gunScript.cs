using UnityEngine;
using System.Collections;

public class gunScript : MonoBehaviour {

    public GameObject projectile;

	// Use this for initialization
	void Start () {
        InvokeRepeating("FireGun", 0f, 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void FireGun()
    {
        var proj = Instantiate(projectile, new Vector2(gameObject.transform.position.x - 0.3f, gameObject.transform.position.y - 0.2f), Quaternion.identity) as GameObject;
        proj.GetComponent<Rigidbody2D>().velocity = new Vector2(-3f, 0);
    }
}
