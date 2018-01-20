﻿using UnityEngine;
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

    Collision2D currentPlat;

    Vector2 startPos;
    Vector2 wallJumpStartPos;
    Vector3 camStartPos;

    string direction;
    string color;

    int flipCount = 1;
    int cloneCount = 1;

    float jumpFromHeight;
    float speedMult = 0.8f;
    float gravityFlip = 1.0f;

    bool reachedMaxJump = false;
    bool reachedMaxSpeed = false;
    bool isGrounded = true;
    bool isFalling = false;
    bool canJumpAgain = true;
    bool canWallJump = false;

	// Use this for initialization
	void Start () {
        startPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        camStartPos = mainCamera.transform.position;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim.runtimeAnimatorController = baseController;
        color = "yellow";
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (canWallJump)
        {
            if (sr.flipX)
            {
                canWallJump = wallJumpStartPos.x - rb.position.x >= 0.3f ? false : true;
            } else if (!sr.flipX)
            {
                canWallJump = rb.position.x - wallJumpStartPos.x >= 0.3f ? false : true;
            }
        }

        if (isFalling)
        {
            isGrounded = false;
        }

        if (color == "yellow")
        {
            yellowAbilities();
        }
        else if (color == "blue")
        {
            blueAbilities();
        }

        if (Input.GetButtonDown("Reset"))
        {
            ResetGame();
        }

        if (Input.GetButtonDown("Switch"))
        {
            switchColors();
        }

        if (Input.GetKeyUp("left shift"))
        {
            anim.SetBool("running", false);
            speedMult = 0.8f;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey("right") || (Input.GetAxis("Horizontal") == 1))
        {
            walkRight();
        }
        else if (Input.GetKey("left") || (Input.GetAxis("Horizontal") == -1))
        {
            walkLeft();
        }
        else
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x * 0.85f, rb.velocity.y);
            }
            anim.SetBool("walking", false);
            anim.SetBool("running", false);
            reachedMaxSpeed = false;
        }
    }

    // COLLISIONS *********************
    //landing on platforms
    void OnCollisionEnter2D(Collision2D coll)
    {
        currentPlat = coll;
        if (sr.flipY == false)
        {
            if ((coll.transform.position.y < rb.transform.position.y) && coll.gameObject.tag.Contains("platform"))
            {
                isGrounded = true;
                isFalling = false;
            } else if ((coll.transform.position.y > rb.transform.position.y) && coll.gameObject.tag.Contains("platform"))
            {
                reachedMaxJump = true;
            }
        } else if (sr.flipY)
        {
            if ((coll.transform.position.y > rb.transform.position.y) && coll.gameObject.tag.Contains("platform"))
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

            if (paanPos.y > collPosY && !sr.flipY) {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                reachedMaxJump = false;
            } else if (paanPos.y < collPosY && sr.flipY)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                reachedMaxJump = false;
            }
        } else if (coll.gameObject.tag.Contains("wall"))
        {
            wallJumpStartPos = rb.position;
            canWallJump = true;
        }
        else if (coll.gameObject.tag.Contains("obstacle"))
        {
            //ResetGameAfterDelay();
            ResetGame();
        }
    }
    // END COLLISIONS *********************

    //yellow abilities
    private void yellowAbilities()
    {
        isFalling = (gravityFlip * rb.velocity.y) <= -0.1f ? true : false;

        if (Input.GetButtonDown("Jump") && isGrounded && canJumpAgain && !isFalling)
        {
            gameObject.transform.parent = null;
            jumpFromHeight = rb.transform.position.y;
            rb.velocity = new Vector2(rb.velocity.x, (6.0f * gravityFlip));
            isGrounded = false;
            canJumpAgain = false;
        } else if (Input.GetButtonDown("Jump") && canWallJump)
        {
            var yJump = sr.flipY == true ? -20.0f : 20.0f;
            rb.velocity = new Vector2((8.0f * gravityFlip), yJump);
            sr.flipX = !sr.flipX;
            canWallJump = false;
        }
        else if (Input.GetButton("Jump") && !reachedMaxJump && !isFalling && !isGrounded && !canJumpAgain)
        {
            if (sr.flipY == true)
            {
                reachedMaxJump = (jumpFromHeight - rb.position.y) >= 1.0f ? true : false;
            } else {
                reachedMaxJump = (rb.position.y - jumpFromHeight) >= 1.0f ? true : false;
            }
            rb.velocity += new Vector2(0f, (2.2f * gravityFlip));
        }
        else if (Input.GetButtonUp("Jump"))
        {
            reachedMaxJump = false;
            canJumpAgain = true;
        }
    }
    
    //blue abilities
    private void blueAbilities()
    {
        if (Input.GetButtonDown("Jump") && flipCount == 1)
        {
            gravityFlip = gravityFlip * -1.0f;
            sr.flipY = !sr.flipY;
            rb.velocity = new Vector2(0f, (rb.velocity.y * 0.4f));
            rb.gravityScale = rb.gravityScale * -1.0f;
            flipCount = 0;
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
            anim.runtimeAnimatorController = yellowController;
            color = "yellow";
        }
    }

    //**********************************************************************************************************************
    //basic movement stuff
    private void walkRight()
    {
        if (Input.GetKey("left shift"))
        {
            anim.SetBool("running", true);
            anim.SetBool("walking", false);
            speedMult = 1.3f;
        }
        else
        {
            anim.SetBool("walking", true);
            anim.SetBool("running", false);
        }
        sr.flipX = false;
        
        if (rb.velocity.x <= (10f * speedMult))
        {
            rb.velocity += new Vector2(speedMult, 0f);
        } else
        {
            rb.velocity = new Vector2((10f * speedMult), rb.velocity.y);
        }

        //transform.Translate(0.08f * speedMult, 0, 0);
    }

    private void walkLeft()
    {
        if (Input.GetKey("left shift"))
        {
            anim.SetBool("running", true);
            anim.SetBool("walking", false);
            speedMult = 1.3f;
        }
        else
        {
            anim.SetBool("walking", true);
            anim.SetBool("running", false);
        }
        sr.flipX = true;
        
        if (rb.velocity.x >= (-10f * speedMult))
        {
            rb.velocity -= new Vector2(speedMult, 0f);
        } else
        {
            rb.velocity = new Vector2((-10f * speedMult), rb.velocity.y);
        }
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

    private IEnumerator ResetGameAfterDelay()
    {
        yield return new WaitForSeconds(2.0f);

        ResetGame();
    }

    private void ResetGame()
    {
        transform.position = startPos;
        mainCamera.gameObject.transform.position = camStartPos;
        sr.flipY = false;
        rb.gravityScale = 6.0f;
        rb.velocity = new Vector2(0f, 0f);
        isGrounded = true;
        isFalling = false;
    }
}
