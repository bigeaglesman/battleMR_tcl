using System.Collections;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayRun()
    {
        animator.SetBool("isRun", true);
    }

    public void StopRun()
    {
        animator.SetBool("isRun", false);
    }

    public void PlayAttack()
    {
        animator.SetBool("isAttack", true);
        StartCoroutine(PlayOneTime(0.2f, "isAttack"));
    }

    public void StopAttack()
    {
        animator.SetBool("isAttack", false);
    }

    public void PlayTakeDamage()
    {
        animator.SetBool("TakeDamage", true);
    }
    public void StopTakeDamage()
    {
        animator.SetBool("TakeDamage", false);
    }

    public void PlaySkill()
    {
        animator.SetBool("isUsingSkill", true);
        StartCoroutine(PlayOneTime(0.2f, "isUsingSkill"));
    }

    public void PlaySkill(float time)
    {
        animator.SetBool("isUsingSkill", true);
        StartCoroutine(PlayOneTime(time, "isUsingSkill"));
    }

    public void StopSkill()
    {
        animator.SetBool("isUsingSkill", false);
    }

    public void PlayDie()
    {
        animator.SetBool("isDead", true);
    }

    IEnumerator PlayOneTime(float time, string param)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool(param, false);
    }
}
