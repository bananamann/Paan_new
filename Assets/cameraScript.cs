using UnityEngine;
using System.Collections;

public class cameraScript : MonoBehaviour {
    public GameObject player;
    public GameObject background;

    public float smoothSpeed = 0.35f;
    public Vector3 offset = new Vector3(0.0f, 1.0f, 0.0f);

    public bool followX = true;
    public bool followY = false;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("player");
	}

    void Update() {
        //if (player.transform.position.x < 0f) {
        //    followX = false;
        //} else {
        //    followX = true;
        //}

        //if (player.transform.position.y < 0) {
        //    followY = false;
        //} else {
        //    followY = true;
        //}
    }

	void FixedUpdate () {
        //float xPos = transform.position.x;
        //float yPos = transform.position.y;

        //if (followX) {
        //    xPos = player.transform.position.x;
        //} else {
        //    xPos = transform.position.x;
        //}

        //if (followY) {
        //    yPos = player.transform.position.y;
        //} else {
        //    yPos = transform.position.y;
        //}

        Vector3 desiredPosition = (new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z)) + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        //transform.position = new Vector3(xPos, yPos, transform.position.z);
    }
}
