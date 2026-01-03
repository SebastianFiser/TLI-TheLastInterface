using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;

    [Header("Attack Details")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;


    [Header("Movement details")]
    [SerializeField]protected float moveSpeed = 3.5f;
    [SerializeField]private float jumpForce = 8;
    protected int facingDir = 1;
    private float xInput;
    private bool facingRight = true;
    protected bool canMove = true;
    private bool canJump = true;

    [Header("colision details")]
    [SerializeField] private float groundCheckDistance;
    private bool isGrounded;
    [SerializeField] private LayerMask whatIsGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        handleCollision();//Handlo collision
        //Handl input
        handleInput();
        //Handle movement
        handleMovement();
        //Handle animations
        handleAnimations();
        //Handle Flip
        handleFlip();
    }

    public void damageTargets()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsTarget);

        foreach (Collider2D enemy in enemyColliders)
        {
            Entity entityTarget = enemy.GetComponent<Entity>();
            entityTarget.takeDamage();
        }
    }

    private void takeDamage()
    {

    }

    public void EnableJumpAndMovement(bool enable)
    {
        canMove = enable;
        canJump = enable;
    }

    private void handleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

         if (Input.GetKeyDown(KeyCode.Space))
        {
            tryToJump();//jump
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            HandleAttack();
        }

    }

    protected virtual void HandleAttack()
    {
        if(isGrounded)
            anim.SetTrigger("attack");
    }



    private void tryToJump()
    {
        if (isGrounded && canJump)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    protected virtual void handleMovement()
    {
        if (canMove == true)
        {
            //change velocity for move
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
        }
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

    }

    protected virtual void handleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    protected void handleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);

    }

    protected void handleFlip()
    {
        if (rb.linearVelocity.x > 0 && facingRight == false)
            Flip();
        else if (rb.linearVelocity.x < 0 && facingRight == true)
            Flip();
    }

    protected void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir *= -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

    }
}
