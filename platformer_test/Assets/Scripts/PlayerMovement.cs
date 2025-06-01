using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jump;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float coyoteCounter;
    [SerializeField] private int extraJumps;
    [SerializeField] private float wallJumpX;
    [SerializeField] private float wallJumpY;
    private int jumpCounter;
    private Animator anim;
    private BoxCollider2D BoxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        //Components
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        BoxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        //Get axis
        horizontalInput = Input.GetAxis("Horizontal");
        

        //Flip player sprite L/R
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }


        //Set animator
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", IsGroundet());

        //jump
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        //Jump hight Adj
        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2);
        }

        if (OnWall())
        {
            body.gravityScale = 0;
            body.linearVelocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (IsGroundet())
            {
                coyoteCounter = coyoteTime;
                jumpCounter = extraJumps;
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }
        }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && !OnWall() && jumpCounter <= 0)
        {
            return;
        }

        if (OnWall())
        {
            WallJump();
        }
        else
        {
            if (IsGroundet())
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jump);
            }
            else
            {
                if(coyoteCounter > 0)
                {
                    body.linearVelocity = new Vector2(body.linearVelocity.x, jump);
                }
                else
                {
                    if(jumpCounter > 0)
                    {
                        body.linearVelocity = new Vector2(body.linearVelocity.x, jump);
                        jumpCounter--;
                    }
                }
            }

            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        wallJumpCooldown = 0;
    }

    private bool IsGroundet()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(BoxCollider.bounds.center, BoxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(BoxCollider.bounds.center, BoxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && IsGroundet() && !OnWall();
    }

    private void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
