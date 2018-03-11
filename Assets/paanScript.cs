using UnityEngine;
using System.Collections;
using Assets;
using System;

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
    float speedMult = 1.0f;
    float gravityFlip = 1.0f;

    bool reachedMaxJump = false;
    bool reachedMaxSpeed = false;
    bool isGrounded = true;
    bool isFalling = false;
    bool canJumpAgain = true;
    bool canWallJump = false;
    bool canWallJumpAgain = false;
    bool justWallJumped = false;

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

        if (Input.GetButtonUp("Jump"))
        {
            reachedMaxJump = false;
            canWallJumpAgain = true;
            canJumpAgain = true;
            if (!isFalling)
            {
                rb.velocity = new Vector2(rb.velocity.x, (rb.velocity.y * 0.4f));
            }
        }

        if (Input.GetButtonDown("Reset"))
        {
            ResetGame();
        }

        if (Input.GetButtonDown("Switch"))
        {
            switchColors();
        }

        if (isGrounded && !isFalling && !Input.GetButton("Jump"))
        {
            flipCount = 1;
        }
    }

    void FixedUpdate()
    {
        isFalling = (gravityFlip * rb.velocity.y) <= -0.1f ? true : false;
        isGrounded = rb.velocity.y == 0f ? true : false;

        if (Input.GetButton("Jump"))
        {
            UseAbility();
        }

        if (Input.GetAxis("Horizontal") == -1 || Input.GetAxis("Horizontal") == 1)
        {
            var xDir = Input.GetAxis("Horizontal");
            var isWalking = Input.GetButton("Walking");
            var maxSpeed = isWalking ? 3.0f : 10.0f;

            PaanMove(xDir, isWalking, maxSpeed);
        }
        else
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x * 0.45f, rb.velocity.y);
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

    private void YellowAbility()
    {
        if (isGrounded && canJumpAgain)
        {
            gameObject.transform.parent = null;
            jumpFromHeight = rb.transform.position.y;
            rb.velocity = new Vector2(rb.velocity.x, (14.0f * gravityFlip));
            canJumpAgain = false;
        }
        else if (Input.GetButtonDown("Jump") && canWallJump && canWallJumpAgain)
        {
            var yJump = 24.0f * gravityFlip;
            var xJump = 8.0f * gravityFlip;

            rb.velocity = new Vector2(xJump, yJump);
            sr.flipX = !sr.flipX;
            canWallJump = false;
            canWallJumpAgain = false;
            justWallJumped = true;
        }
        else if (!reachedMaxJump && !isFalling && !isGrounded && !canJumpAgain)
        {
            if (sr.flipY == true)
            {
                reachedMaxJump = (jumpFromHeight - rb.position.y) >= 1.6f ? true : false;
            }
            else
            {
                reachedMaxJump = (rb.position.y - jumpFromHeight) >= 1.6f ? true : false;
            }
            rb.velocity += new Vector2(0f, (2.0f * gravityFlip));
        }
    }

    private void BlueAbility()
    {
        if (flipCount == 1)
        {
            gravityFlip = gravityFlip * -1.0f;
            sr.flipY = !sr.flipY;
            rb.velocity = new Vector2(0f, (rb.velocity.y * 0.4f));
            rb.gravityScale = rb.gravityScale * -1.0f;
            Vector3 scale = transform.localScale;
            scale = -scale;
            flipCount = 0;
        }
    }

    //**********************************************************************************************************************
    //basic movement stuff
    private void PaanMove(float xDir, bool isWalking, float maxSpeed)
    {
        sr.flipX = xDir == -1.0f ? true : false;

        if (rb.velocity.x * xDir > maxSpeed)
        {
            rb.velocity = new Vector2(maxSpeed * xDir, rb.velocity.y);
        }


        anim.SetBool("running", !isWalking);
        anim.SetBool("walking", isWalking);

        speedMult = isWalking ? 0.5f : 1.0f;

        //rb.velocity = new Vector2(9.0f * speedMult * xDir, rb.velocity.y);
        if (rb.velocity.x * xDir <= maxSpeed)
        {
            rb.velocity += new Vector2(1.0f * speedMult * xDir, 0f);
        }
    }

    //switch colors
    private void switchColors()
    {
        anim.runtimeAnimatorController = color == "yellow" ? blueController : yellowController;
        color = color == "yellow" ? "blue" : "yellow";
    }

    private void UseAbility()
    {
        if (color == "yellow")
        {
            YellowAbility();
        }
        else
        {
            BlueAbility();
        }
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
        gravityFlip = 1.0f;
        rb.velocity = new Vector2(0f, 0f);
        color = "yellow";
        anim.runtimeAnimatorController = baseController;
        canJumpAgain = true;
        reachedMaxJump = false;
        flipCount = 1;
    }
}
