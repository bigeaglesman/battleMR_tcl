using UnityEngine;
using System.Collections;

public class ArcherBowController : MonoBehaviour
{
    public Transform bowString; // 시위 오브젝트
    public Transform rightHand; // 오른손 트랜스폼
    public bool followRightHand = false;
    public Transform stringRestPos; // 원래 자리
    public Animator animator;
    private bool isLoadFinished = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void OnLoadFinished()
    {
        followRightHand = true; // 이제 시위를 오른손에 따라가게 함
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


        if(stateInfo.IsName("Load") && !isLoadFinished)
        {
            Debug.Log("Load");
            isLoadFinished = true;
            OnLoadFinished();
        }

        if (stateInfo.IsName("Release") && isLoadFinished)
        {
            Debug.Log("Release");
            isLoadFinished = false;
            OnReleaseFinished();
        }


        if (followRightHand)
        {
            bowString.position = rightHand.position;
        }

        if (animator.GetBool("isAttack"))
        {

        }
    }

    public void OnReleaseFinished()
    {
        followRightHand = false;
        //bowString.position = stringRestPos.position; // 바로 복귀하거나 Lerp로 자연스럽게
        StartCoroutine(ResetBowString());
    }



    IEnumerator ResetBowString()
    {
        Debug.Log("Reset");
        float t = 0f;
        Vector3 startPos = bowString.position;
        Vector3 endPos = stringRestPos.position;

        while (t < 1f)
        {
            bowString.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime * 5f; // 속도 조절
            yield return null;
        }

        bowString.position = endPos;
    }

}
