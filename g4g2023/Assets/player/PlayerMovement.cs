using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
 //input data
    Vector2 moveInput; 
    [SerializeField] bool jump;
    bool isAlive = true;

    //movement speed 
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float runModifier = 2f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float dashDuration = 0.5f;

    // add force
    Rigidbody2D rb2d;
    // abilities Checker
    [SerializeField] int jumpsAvailableCount = 1;
    public bool isDashing;
    public bool canDash = false;
    public bool canDoubleJump = false;
    public bool isRunning;
    //Animation
    Animator myAnimator;
    
    void Start(){
        rb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        AbilityCheck();
    }

    public void AbilityCheck()
    {
        int currentlevel = SceneManager.GetActiveScene().buildIndex;
        if(currentlevel == 1)
        {
            canDash = false;
            canDoubleJump = false;
        }
        else if(currentlevel == 2)
        {
            canDoubleJump = true;
        }
        else if(currentlevel > 2)
        {
            canDash = true;
            canDoubleJump = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!isAlive) { return; }

        Walk();
        moveInput.x = Input.GetAxis("Horizontal");

        FlipSprite();

        //jump when pressing spacebar
        Jump();

    }

    private void FlipSprite()
    {
        //flip sprites based on movement direction
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && !jump)
        {
            jump = true;
            Debug.Log("JUMPPPPPP");
            //rb2d.AddForce(Vector2.up * jumpForce *Time.deltaTime, ForceMode2D.Impulse);
            rb2d.velocity += new Vector2(0f, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag =="Ground")
            {
                jump = false;
            }
    }

    public void ResetJumpCount()
    {
        if(canDoubleJump)
        {
            jumpsAvailableCount = 2;
        }
        else
        {
            jumpsAvailableCount = 1;
        }
    }
    void Walk()
    {
        walkSpeed = baseSpeed;
        Vector2 playerVelocity = new Vector2 (moveInput.x*walkSpeed,rb2d.velocity.y);
        rb2d.velocity=playerVelocity;
        myAnimator.SetBool("IsRunning",false);
        bool PlayerHasHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) >Mathf.Epsilon;
        myAnimator.SetBool("IsWalking",PlayerHasHorizontalSpeed);
        
    }


    void OnRun(InputValue value)
    {
        baseSpeed = walkSpeed;
        isRunning=true;
        walkSpeed = runModifier;
        myAnimator.SetBool("IsWalking",false);
        myAnimator.SetBool("IsRunning",true);
    }


    void OnDash(InputValue value)
    {
        if(!isDashing && canDash)
        {
            StartCoroutine(Dash());
        }

    }
    IEnumerator Dash()
    {
        float baseSpeed = walkSpeed;
        isDashing=true;
        //myAnimator.SetBool("IsDashing",isDashing);
        //audiosource.PlayOneShot(DASHSFX)
        walkSpeed *= dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        walkSpeed = baseSpeed;
        isDashing = false;
        //myAnimator.SetBool("IsDashing",isDashing);
    }


}
