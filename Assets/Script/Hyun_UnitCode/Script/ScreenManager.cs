
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
	public static ScreenManager Instance;

	public GameObject rangePrefab;

	[HideInInspector] public GameObject currentRangeIndicator;
	[HideInInspector] public int activatedSkill = -1;
	[HideInInspector] public bool isActivated;
	int flag;

	void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		/*rayInteractor = GetComponent<XRRayInteractor>();
		if (rayInteractor == null)
		{
			flag = 0;
		}*/
	}

	void Update()
	{
		if (flag == 0)
		{
			//Debug.Log("NO XR"); -XR없어서 문제발생 방지.
		}
		else
		{
			UpdateSkillRange();
			HandleSkillInput();
		}
	}

	void UpdateSkillRange()
	{
		if (!isActivated || activatedSkill == -1)
		{
			if (currentRangeIndicator != null)
			{
				Destroy(currentRangeIndicator);
			}
			return;
		}

		if (currentRangeIndicator == null)
		{
			currentRangeIndicator = Instantiate(rangePrefab);
			float range = SkillManager.Instance.skillRadius[activatedSkill];
			currentRangeIndicator.transform.localScale = new Vector3(range, 0.1f, range);
		}

		/*if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
		{
			currentRangeIndicator.transform.position = hit.point + Vector3.up * 0.1f;
		}*/
	}

	void HandleSkillInput()
	{
		/*if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
		{
			if (SkillManager.Instance.IsCasting)
			{
				// 스킬 사용
				if (Input.GetButtonDown("XRI_Right_TriggerButton"))
				{
					SkillManager.Instance.UseSkill(hit.point);
				}
			}
			else
			{
				// 스킬 선택
				if (Input.GetButtonDown("XRI_Right_GripButton"))
				{
					int selectedSkill = 0; // UI에서 스킬 ID 가져오기
					SkillManager.Instance.ActivateSkill(selectedSkill);
				}
			}
		}*/
	}

}