using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class Sensor : MonoBehaviour
{
	//public GameObject detectedEnemies = new GameObject(); // 감지된 적군 목록
	public LayerMask enemyLayer; // 적군 레이어

	private void OnTriggerEnter(Collider other)
	{
		// 적군 레이어에 속한 오브젝트만 추가
		if (GetComponent<Unit>().currentState == Unit.UnitState.Idle&&other.gameObject.layer == enemyLayer)
		{
			if (GetComponent<Unit>().isEnemy != other.gameObject.GetComponent<Unit>().isEnemy)
			{
				GetComponent<Unit>().AttackTarget(other.transform);
				GetComponent<Unit>().currentState = Unit.UnitState.Attacking;
				Debug.Log("Enemy detected Mode changed");
			}
		}


	}
}
