using UnityEngine;

public class PlayerAnimaionEvents : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    public void damageTargets() => entity.damageTargets();

    private void DisableJumpAndMovement() => entity.EnableJumpAndMovement(false);

    private void EnableJumpAndMovement() => entity.EnableJumpAndMovement(true);
}
