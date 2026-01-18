using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DBManager : MonoBehaviour
{
	private string filePath;
	public List<DB> unitStatsList = new List<DB>();
	public Card card;
	private string cardPath;

	private void Awake()
	{
		filePath = Path.Combine(Application.persistentDataPath, "characterStats.json");

		if (!File.Exists(filePath))
			InitializeDefaultStats();

		LoadStats();
	}
	//로드기능 사용시 필요한 카드로드기능.
	public Card LoadCardsFromJson(string fileName)
	{
		string path = Path.Combine(Application.streamingAssetsPath, cardPath);

		if (File.Exists(path))
		{
			string jsonText = File.ReadAllText(path);
			card = JsonUtility.FromJson<Card>(jsonText);
			Debug.Log("카드 정보 로드 완료!");
		}
		else
		{
			Debug.LogError("JSON 파일을 찾을 수 없습니다: " + path);
		}
		return card;
	}

	private void InitializeDefaultStats()
	{
		unitStatsList = new List<DB>
		{
			//Todo 케릭터 능력치 초기화.
		};

		SaveStats();
		Debug.Log("기본 유닛 데이터 초기화 완료.");
	}

	public void SaveStats()
	{
		string json = JsonConvert.SerializeObject(unitStatsList, Formatting.Indented);
		File.WriteAllText(filePath, json);
		Debug.Log($"모든 유닛 능력치 저장 완료: {filePath}");
	}

	public void LoadStats()
	{
		if (File.Exists(filePath))
		{
			string json = File.ReadAllText(filePath);
			unitStatsList = JsonConvert.DeserializeObject<List<DB>>(json);
			Debug.Log("모든 유닛 능력치 불러오기 완료!");
		}
		else
		{
			Debug.LogWarning("저장된 유닛 능력치 파일이 없습니다.");
		}
	}
	//값 수정할 때 사용가능.-업글에 사용하면 될것 같음.
	public void UpdateUnitStat(int unitID, string statName, float newValue)
	{
		DB unit = GetUnitStats(unitID);
		if (unit == null)
		{
			Debug.LogWarning($"ID {unitID} 유닛을 찾을 수 없습니다.");
			return;
		}

		switch (statName.ToLower())
		{
			case "attackpower": unit.attackPower = newValue; break;
			case "attackspeed": unit.attackSpeed = newValue; break;
			case "health": unit.health = newValue; break;
			case "defense": unit.defense = newValue; break;
			case "criticalchance": unit.criticalChance = newValue; break;
			case "criticaldamage": unit.criticalDamage = newValue; break;
			case "movespeed": unit.moveSpeed = newValue; break;
			case "attackrange": unit.attackRange = newValue; break;
			default:
				Debug.LogWarning($"잘못된 능력치 이름: {statName}");
				return;
		}

		SaveStats();
		Debug.Log($"ID {unitID}의 {statName}가 {newValue}로 업데이트되었습니다.");
	}

	public DB GetUnitStats(int unitID)
	{
		return unitStatsList.Find(unit => unit.unitID == unitID);
	}
	//카드 선택했을 때 카드 변경하기 위해 Save Card와 불러오기 기능을 위해 Load Card 필요.
}
