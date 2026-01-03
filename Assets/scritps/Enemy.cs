using UnityEngine;

public class Enemy : Entity
{
    protected override void Update()
    {
        handleCollision();//Handlo collision
        //Handle movement
        handleMovement();
        //Handle animations
        handleAnimations();
        //Handle Flip
        handleFlip();
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
}
