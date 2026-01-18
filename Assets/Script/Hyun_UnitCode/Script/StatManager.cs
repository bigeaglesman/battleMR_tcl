using UnityEngine;

public class StatManager : MonoBehaviour
{
	public static StatManager Instance { get; private set; }

	private DBManager dbManager;

	private void Awake()
	{
		// 싱글톤 설정
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		dbManager = GetComponent<DBManager>();

		// 유닛 능력치 불러오기 (파일이 없다면 기본값 설정)
		dbManager.LoadStats();
	}

	// 특정 유닛의 능력치 가져오기
	public DB GetUnitStats(int unitID)
	{
		return dbManager.GetUnitStats(unitID);
	}

}
