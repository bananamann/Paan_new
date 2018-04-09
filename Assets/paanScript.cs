using UnityEngine;
using System.Collections;
using Assets;
using System;

public class paanScript : MonoBehaviour {
    public Rigidbody2D rb;

    BoxCollider2D bc;
    CircleCollider2D cc;

    SpriteRenderer sr;
    Animator anim;

    public AnimatorOverrideController blueController;
    public AnimatorOverrideController yellowController;
    public RuntimeAnimatorController baseController;

    public GameObject paanClone;
    public GameObject mainCamera;
    GameObject clone;

    Vector2 startPos;
    Vector2 wallJumpStartPos;
    Vector3 camStartPos;

    string color;

    int flipCount = 1;
    int jumpCount = 2;

    float jumpFromHeight;
    float gravityFlip = 1.0f;
    float maxFallSpeed = -20.0f;
    float xDir;

    bool reachedMaxJump = false;
    bool isGrounded = true;
    bool isFalling = false;
    bool canJumpAgain = true;

    // Use this for initialization
    void Start () {
        startPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        camStartPos = mainCamera.transform.position;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        cc = GetComponent<CircleCollider2D>();
        anim.runtimeAnimatorController = baseController;
        color = "yellow";
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonUp("Jump"))
        {
            reachedMaxJump = false;
            canJumpAgain = true;
            if (!isFalling)
            {
                rb.velocity = new Vector2(rb.velocity.x, (rb.velocity.y * 0.4f));
            }
        }

        if (Input.GetButtonDown("Reset"))
        {
            StartCoroutine(ResetGameAfterDelay());
        }

        if (Input.GetButtonDown("Flip"))
        {
            flipGrav();
        }
    }

    void FixedUpdate()
    {
        isFalling = (gravityFlip * rb.velocity.y) <= -0.1f ? true : false;
        isGrounded = rb.velocity.y == 0f ? true : false;

        if ((rb.velocity.y * gravityFlip) < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, (maxFallSpeed * gravityFlip));
        }

        if (Input.GetButton("Jump"))
        {
            Jump();
        }

        if (Input.GetAxis("Horizontal") == -1 || Input.GetAxis("Horizontal") == 1)
        {
            xDir = Input.GetAxis("Horizontal");
            var isWalking = Input.GetButton("Walking");
            var maxSpeed = isWalking ? 3.0f : 10.0f;

            sr.flipX = xDir == -1 ? true : false;
            PaanMove(xDir, isWalking, maxSpeed);
        }
        else
        {
            rb.velocity = new Vector2((rb.velocity.x * 0.45f), rb.velocity.y);

            anim.SetBool("walking", false);
            anim.SetBool("running", false);
        }
    }

    // COLLISIONS *********************
    void OnCollisionEnter2D(Collision2D coll)
    {
        var paanPos = rb.transform.position;
        var collPos = coll.transform.position;

        var contPos = new Vector2(coll.contacts[0].point.x, collPos.y);

        var collHalfWidth = (coll.collider.bounds.size.x / 2.0f);

        var hitSide = (Vector2.Distance(contPos, collPos) >= collHalfWidth) ? true : false;

        if (coll.gameObject.tag.Contains("platform") && !hitSide) {
            gameObject.transform.SetParent(coll.gameObject.transform);

            if ((paanPos.y * gravityFlip) > (collPos.y * gravityFlip))
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                flipCount = 1;
                jumpCount = 2;
                reachedMaxJump = false;
            }
        }
        else if (coll.gameObject.tag.Contains("obstacle"))
        {
            //ResetGameAfterDelay();
            ResetGame();
        }
    }
    // END COLLISIONS *********************

    private void Jump()
    {
        gameObject.transform.parent = null;

        if (jumpCount > 0 && canJumpAgain)
        {
            jumpFromHeight = rb.transform.position.y;
            jumpCount -= 1;
            rb.velocity = new Vector2(rb.velocity.x, (14.0f * gravityFlip));
        }

        canJumpAgain = false;

        if (!reachedMaxJump && !isFalling && !isGrounded)
        {
            if (sr.flipY)
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

    private void flipGrav()
    {
        if (flipCount == 1)
        {
            anim.runtimeAnimatorController = color == "yellow" ? blueController : yellowController;
            color = color == "yellow" ? "blue" : "yellow";
            gravityFlip = gravityFlip * -1.0f;
            sr.flipY = !sr.flipY;
            bc.offset = bc.offset * -1.0f;
            cc.offset = cc.offset * -1.0f;
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
        if (rb.velocity.x * xDir > maxSpeed)
        {
            rb.velocity = new Vector2(maxSpeed * xDir, rb.velocity.y);
        }

        anim.SetBool("running", !isWalking);
        anim.SetBool("walking", isWalking);

        var speedMult = isWalking ? 0.5f : 1.0f;

        if (isGrounded)
        {
            rb.velocity += new Vector2(1.6f * speedMult * xDir, 0f);
        }
        else if (!isGrounded)
        {
            rb.velocity += new Vector2(1.0f * speedMult * xDir, 0f);
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
        cc.offset = new Vector2(0f, -0.3f);
        bc.offset = new Vector2(0.078f, -0.066f);
        gravityFlip = 1.0f;
        rb.velocity = new Vector2(0f, 0f);
        color = "yellow";
        anim.runtimeAnimatorController = baseController;
        reachedMaxJump = false;
        flipCount = 1;
    }
}
