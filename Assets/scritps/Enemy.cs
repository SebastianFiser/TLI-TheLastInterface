using UnityEngine;

public class Enemy : Entity
{
    private bool playerDetected;

    protected override void Update()
    {
        handleCollision();//Handlo collision
        //Handle movement
        handleMovement();
        //Handle animations
        handleAnimations();
        //Handle Flip
        handleFlip();
        //Handle HandleAttack
        HandleAttack();
    }

    protected override void HandleAttack()
    {
        //if player detected anim to set trigger attack
        if(playerDetected)
            anim.SetTrigger("attack");
    }

    protected override void handleMovement()
    {
        if (canMove == true)
        {
            //change velocity for move
            rb.linearVelocity = new Vector2(facingDir * moveSpeed, rb.linearVelocity.y);
        }
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    protected override void handleCollision()
    {
        base.handleCollision();
        playerDetected = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
    }
}
