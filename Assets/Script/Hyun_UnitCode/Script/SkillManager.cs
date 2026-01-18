using UnityEngine;
using UnityEngine.AI;

public class SkillManager : MonoBehaviour
{
	public static SkillManager Instance;

	public GameObject[] skillPrefabs;
	public float[] skillDamages;
	public float[] skillRadius;

	[HideInInspector] public bool IsCasting;

	void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public void ActivateSkill(int skillId)
	{
		if (skillId < 0 || skillId >= skillPrefabs.Length) return;

		IsCasting = true;
		ScreenManager.Instance.activatedSkill = skillId;
		ScreenManager.Instance.isActivated = true;
	}

	public void UseSkill(Vector3 position)
	{
		// 스킬 이펙트 생성
		Instantiate(
			skillPrefabs[ScreenManager.Instance.activatedSkill],
			position,
			Quaternion.identity
		);

		// 범위 내 적 유닛 검출
		Collider[] hits = Physics.OverlapSphere(
			position,
			skillRadius[ScreenManager.Instance.activatedSkill]
		);

		// 적 유닛에게 데미지 적용
		foreach (Collider hit in hits)
		{
			Unit unit = hit.GetComponent<Unit>();
			if (unit != null && unit.isEnemy)
			{
				unit.TakeDamage(null,skillDamages[ScreenManager.Instance.activatedSkill]);
			}
		}

		CancelSkill();
	}

	public void CancelSkill()
	{
		IsCasting = false;
		ScreenManager.Instance.activatedSkill = -1;
		ScreenManager.Instance.isActivated = false;
	}
}