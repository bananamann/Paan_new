using UnityEngine;
using System.Collections;

public class movingPlatScript : MonoBehaviour {

    Rigidbody2D rb;

    Vector2 origPosition;

    public float dist;
    public float speed;

    public string direction;

    bool moveBack = false;

	// Use this for initialization
	void Start () {
        rb = gameObject.GetComponent<Rigidbody2D>();
        origPosition = rb.transform.position;
	}
	
	void FixedUpdate () {
        if (direction == "up")
        {
            if ((rb.transform.position.y - origPosition.y) >= dist)
            {
                moveBack = true;
            }
            else if ((rb.transform.position.y - origPosition.y) <= 0f)
            {
                moveBack = false;
            }

            if (!moveBack)
            {
                rb.transform.position = new Vector2(rb.transform.position.x, (rb.transform.position.y + speed));
            }
            else if (moveBack)
            {
                rb.transform.position = new Vector2(rb.transform.position.x, (rb.transform.position.y - speed));
            }
        }
        else if (direction == "down")
        {
            if ((origPosition.y - rb.transform.position.y) >= dist)
            {
                moveBack = true;
            }
            else if ((origPosition.y - rb.transform.position.y) <= 0f)
            {
                moveBack = false;
            }

            if (!moveBack)
            {
                rb.transform.position = new Vector2(rb.transform.position.x, (rb.transform.position.y - speed));
            }
            else if (moveBack)
            {
                rb.transform.position = new Vector2(rb.transform.position.x, (rb.transform.position.y + speed));
            }
        }
        else if (direction == "left")
        {
            if ((origPosition.x - rb.transform.position.x) >= dist)
            {
                moveBack = true;
            }
            else if ((origPosition.x - rb.transform.position.x) <= 0f)
            {
                moveBack = false;
            }

            if (!moveBack)
            {
                rb.transform.position = new Vector2((rb.transform.position.x - speed), rb.transform.position.y);
            }
            else if (moveBack)
            {
                rb.transform.position = new Vector2((rb.transform.position.x + speed), rb.transform.position.y);
            }
        }
        else if (direction == "right")
        {
            if ((rb.transform.position.x - origPosition.x) >= dist)
            {
                moveBack = true;
            }
            else if ((rb.transform.position.x - origPosition.x) <= 0f)
            {
                moveBack = false;
            }

            if (!moveBack)
            {
                rb.transform.position = new Vector2((rb.transform.position.x + speed), rb.transform.position.y);
            }
            else if (moveBack)
            {
                rb.transform.position = new Vector2((rb.transform.position.x - speed), rb.transform.position.y);
            }
        }


























        //if (vertDist > 0f)
        //{
        //    if ((rb.transform.position.y - origPosition.y) >= vertDist)
        //    {
        //        moveBack = true;
        //    } else if ((rb.transform.position.y - origPosition.y) <= 0f)
        //    {
        //        moveBack = false;
        //    }

        //    if (!moveBack)
        //    {
        //        rb.transform.position = new Vector2(rb.transform.position.x, (rb.transform.position.y + speed));
        //    } else if (moveBack)
        //    {
        //        rb.transform.position = new Vector2(rb.transform.position.x, (rb.transform.position.y - speed));
        //    }
        //}
        //else if (horDist > 0f)
        //{
        //    if ((rb.transform.position.x - origPosition.x) >= horDist)
        //    {
        //        moveBack = true;
        //    }
        //    else if ((rb.transform.position.x - origPosition.x) <= 0f)
        //    {
        //        moveBack = false;
        //    }

        //    if (!moveBack)
        //    {
        //        rb.transform.position = new Vector2((rb.transform.position.x + speed), (rb.transform.position.y));
        //    }
        //    else if (moveBack)
        //    {
        //        rb.transform.position = new Vector2((rb.transform.position.x - speed), rb.transform.position.y);
        //    }
        //}
        //else if (horDist < 0f)
        //{
        //    if ((origPosition.x - rb.transform.position.x) >= horDist)
        //    {
        //        moveBack = true;
        //    }
        //    else if ((origPosition.x - rb.transform.position.x) <= 0f)
        //    {
        //        moveBack = false;
        //    }

        //    if (!moveBack)
        //    {
        //        rb.transform.position = new Vector2((rb.transform.position.x - speed), (rb.transform.position.y));
        //    }
        //    else if (moveBack)
        //    {
        //        rb.transform.position = new Vector2((rb.transform.position.x + speed), rb.transform.position.y);
        //    }
        //}
    }
}
