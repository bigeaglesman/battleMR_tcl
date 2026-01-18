using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ButtonSpawner : MonoBehaviour
{
	public GameObject buttonPrefab; // 생성할 버튼 프리팹
	public Transform panel; // 버튼이 추가될 부모 오브젝트
	public UnitManager um;

	public void SpawnButtons()
	{
		// 기존 버튼이 있다면 삭제
		foreach (Transform child in panel)
		{
			Destroy(child.gameObject);
		}

		// 새로운 버튼 3개 생성
		for (int i = 1; i <= 3; i++)
		{
			GameObject newButton = Instantiate(buttonPrefab, panel);
			newButton.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
			int number = i; // 람다 캡처 문제 방지
			Debug.Log(number);
			newButton.GetComponent<Button>().onClick.AddListener(() => um.AttackUnits(number));
		}
	}

}
