using UnityEngine;
using System.Collections;
using Assets;

public class paanScript : MonoBehaviour {
    public Rigidbody2D rb;

    SpriteRenderer sr;
    Animator anim;

    public AnimatorOverrideController blueController;
    public AnimatorOverrideController purpleController;
    public AnimatorOverrideController yellowController;
    public RuntimeAnimatorController baseController;

    public GameObject paanClone;
    public GameObject mainCamera;
    GameObject clone;

    string direction;
    string color;

    int flipCount = 1;
    int cloneCount = 1;

    float jumpFromHeight;
    float speedMult = 1.0f;
    float gravityFlip = -1.0f;

    bool reachedMaxJump = false;
    bool isGrounded = true;
    bool isFalling = false;
    bool canJumpAgain = true;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim.runtimeAnimatorController = baseController;
        color = "yellow";
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (color == "yellow")
        {
            yellowAbilities();
        }
        else if (color == "blue")
        {
            blueAbilities();
        }
        else if (color == "purple")
        {
            purpleAbilities();
        }

        if (Input.GetKeyDown("q"))
        {
            transform.position = new Vector2(-3.3f, -3.37f);
            sr.flipY = false;
            rb.gravityScale = 6.0f;
            rb.velocity = new Vector2(0f, 0f);
            isGrounded = true;
            isFalling = false;
            mainCamera.GetComponent<Transform>().position = new Vector3(0f, 0f, -1.0f);
        }

        if (Input.GetKeyDown("x"))
        {
            switchColors();
        }

        if (Input.GetKeyUp("left shift"))
        {
            anim.SetBool("running", false);
            speedMult = 1.0f;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey("right"))
        {
            walkRight();
        }
        else if (Input.GetKey("left"))
        {
            walkLeft();
        }
        else
        {
            anim.SetBool("walking", false);
            anim.SetBool("running", false);
        }
    }

    // COLLISIONS *********************
    //landing on platforms
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (sr.flipY == false)
        {
            if (coll.transform.position.y < rb.transform.position.y)
            {
                isGrounded = true;
                isFalling = false;
            } else if (coll.transform.position.y > rb.transform.position.y)
            {
                reachedMaxJump = true;
            }
        } else if (sr.flipY)
        {
            if (coll.transform.position.y > rb.transform.position.y)
            {
                isGrounded = true;
                isFalling = false;
            }
        }

        if (isGrounded && !isFalling)
        {
            flipCount = 1;
        }

        float collPosY;
        var paanPos = rb.transform.position;

        if (coll.gameObject.tag.Contains("platform")) {
            gameObject.transform.SetParent(coll.gameObject.transform);
            if (coll.transform.childCount == 0) {
                collPosY = coll.transform.position.y;
            } else {
                collPosY = coll.transform.GetChild(0).transform.position.y;
            }

            if (paanPos.y > collPosY) {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                reachedMaxJump = false;
            }
        } else if (coll.gameObject.tag.Contains("spike"))
        {
            transform.position = new Vector2(-3.3f, -3.37f);
            sr.flipY = false;
            rb.gravityScale = 6.0f;
            rb.velocity = new Vector2(0f, 0f);
            isGrounded = true;
            isFalling = false;
            mainCamera.GetComponent<Transform>().position = new Vector3(0f, 0f, -1.0f);
        }
    }
    // END COLLISIONS *********************

    //yellow abilities
    private void yellowAbilities()
    {
        if (sr.flipY == true)
        {
            if (rb.velocity.y >= 0.1f)
            {
                isFalling = true;
            }
            else
            {
                isFalling = false;
            }
        }
        else if (sr.flipY == false)
        {
            if (rb.velocity.y <= -0.1f && gameObject.transform.parent == null)
            {
                isFalling = true;
            }
            else
            {
                isFalling = false;
            }
        }

        if (Input.GetKeyDown("space") && isGrounded && canJumpAgain)
        {
            gameObject.transform.parent = null;
            jumpFromHeight = rb.transform.position.y;
            if (sr.flipY == true)
            {
                rb.velocity += new Vector2(rb.velocity.x, -12.0f);
            } else
            {
                rb.velocity += new Vector2(rb.velocity.x, 12.0f);
            }
            isGrounded = false;
            canJumpAgain = false;
        }
        else if (Input.GetKey("space") && !reachedMaxJump && !isFalling && !isGrounded && !canJumpAgain)
        {
            if (sr.flipY == true)
            {
                reachedMaxJump = (jumpFromHeight - rb.position.y) >= 1.6f ? true : false;
                rb.velocity += new Vector2(0, -1.6f);
            } else {
                reachedMaxJump = (rb.position.y - jumpFromHeight) >= 1.6f ? true : false;
                rb.velocity += new Vector2(0, 1.6f);
            }
        }
        else if (Input.GetKeyUp("space"))
        {
            isFalling = true;
            reachedMaxJump = false;
            canJumpAgain = true;
        }
    }
    
    //blue abilities
    private void blueAbilities()
    {
        if (Input.GetKeyDown("space") && sr.flipY == false && flipCount == 1)
        {
            sr.flipY = true;
            rb.velocity = new Vector2(0f, (rb.velocity.y * 0.45f));
            rb.gravityScale = rb.gravityScale * gravityFlip;
            flipCount = 0;
        }
        else if (Input.GetKeyDown("space") && sr.flipY == true && flipCount == 1) {
            sr.flipY = false;
            rb.velocity = new Vector2(0f, (rb.velocity.y * 0.45f));
            rb.gravityScale = rb.gravityScale * gravityFlip;
            flipCount = 0;
        }
    }

    //purple abilities
    private void purpleAbilities()
    {
        if (Input.GetKeyDown("space") && cloneCount == 1)
        {
            clone = Instantiate(paanClone, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), gameObject.transform.rotation) as GameObject;
            clone.gameObject.GetComponent<Rigidbody2D>().gravityScale = rb.gravityScale;
            clone.GetComponent<SpriteRenderer>().flipX = sr.flipX;
            clone.GetComponent<SpriteRenderer>().flipY = sr.flipY;
            cloneCount -= 1;
            StartCoroutine(teleportToCloneWithDelay(clone));
        } else if (Input.GetKeyDown("space") && cloneCount == 0)
        {
            StopCoroutine(teleportToCloneWithDelay(clone));
            teleportToClone(clone);
        }
    }

    //switch colors
    private void switchColors()
    {
        if (color == "yellow")
        {
            anim.runtimeAnimatorController = blueController;
            color = "blue";
        }
        else if (color == "blue")
        {
            anim.runtimeAnimatorController = purpleController;
            color = "purple";
        }
        else if (color == "purple")
        {
            anim.runtimeAnimatorController = yellowController;
            color = "yellow";
        }
    }

    //**********************************************************************************************************************
    //basic movement stuff
    private void walkRight()
    {
        direction = "right";
        if (Input.GetKey("left shift"))
        {
            anim.SetBool("running", true);
            anim.SetBool("walking", false);
            speedMult = 2.4f;
        }
        else
        {
            anim.SetBool("walking", true);
            anim.SetBool("running", false);
        }
        sr.flipX = false;
        transform.Translate(0.08f * speedMult, 0, 0);
    }

    private void walkLeft()
    {
        direction = "left";
        if (Input.GetKey("left shift"))
        {
            anim.SetBool("running", true);
            anim.SetBool("walking", false);
            speedMult = 2.4f;
        }
        else
        {
            anim.SetBool("walking", true);
            anim.SetBool("running", false);
        }
        sr.flipX = true;
        transform.Translate(-0.08f * speedMult, 0, 0);
    }

    private IEnumerator teleportToCloneWithDelay(GameObject clone)
    {
        yield return new WaitForSeconds(4.0f);

        if (clone != null)
        {
            rb.transform.position = clone.transform.position;
            rb.gravityScale = clone.GetComponent<Rigidbody2D>().gravityScale;

            sr.transform.rotation = clone.transform.rotation;
            sr.flipY = clone.GetComponent<SpriteRenderer>().flipY;
            sr.flipX = clone.GetComponent<SpriteRenderer>().flipX;

            Destroy(clone);
            cloneCount += 1;
        }
    }

    private void teleportToClone(GameObject clone)
    {
        rb.transform.position = clone.transform.position;
        rb.gravityScale = clone.GetComponent<Rigidbody2D>().gravityScale;

        sr.transform.rotation = clone.transform.rotation;
        sr.flipY = clone.GetComponent<SpriteRenderer>().flipY;
        sr.flipX = clone.GetComponent<SpriteRenderer>().flipX;

        Destroy(clone);
        cloneCount += 1;
    }
}
