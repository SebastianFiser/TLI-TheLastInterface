using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_VFX : MonoBehaviour
{
    private Material OrigMat;
    private SpriteRenderer sr;

    [SerializeField] private Material onDamageFeedbackMat;
    [SerializeField] private float DamageFeedDuration = 0.15f;
    private Coroutine onDmgVfxCo;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        OrigMat = sr.material;
    }

    public void PlayOnDamageVFX()
    {
        if(onDmgVfxCo != null)
            StopCoroutine(onDmgVfxCo);

        onDmgVfxCo = StartCoroutine(OnDmgVfxCo());
    }

    private IEnumerator OnDmgVfxCo()
    {
        sr.material = onDamageFeedbackMat;

        yield return new WaitForSeconds(DamageFeedDuration);

        sr.material = OrigMat;
    }
}
