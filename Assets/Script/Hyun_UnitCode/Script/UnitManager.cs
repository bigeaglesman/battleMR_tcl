using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unit;

public class UnitManager : MonoBehaviour
{
	//MR
	private float CheckInterval = 0.0f; // 0.2초 간격으로 탐지
	private float lastCheckTime = 0;

	private Vector3 mouseWorldPosition;
	public int currentResources = 100;
	public Transform spawnPoint;
	public GameObject[] unitPrefabs;
	private int unitnum = 1;//유닛 번호.
	private int selectedid = -1;
	public int spawnCount = 10; // 한 번에 소환할 유닛 수
	public float unitSpacing = 2.0f; // 유닛 간 간격
	public Card cards;
	private List<Unit> allyUnits = new List<Unit>();
	private List<Unit> enemyUnits = new List<Unit>();
	private List<List<Unit>> units = new List<List<Unit>>();
	private Dictionary<int, List<Unit>> allyUnitsByType = new Dictionary<int, List<Unit>>();// 이전에 소환된 유닛 종류를 추적
	private Dictionary<int, List<Unit>> enemyUnitsByType = new Dictionary<int, List<Unit>>();// 이전에 소환된 유닛 종류를 추적
	public List<Unit> selectedUnits = new List<Unit>();
	private int testint = 0;
	private int allyUnitIndex = 0; // Ally Unit Index
	private int enemyUnitIndex = 0; // EnemyUnitIndex
	private const int unitsPerFrame = 5; // number of Units Updated in frame
	private int updateteam=0;

	private void Start()
	{
		units.Add(allyUnits);
		units.Add(enemyUnits);
	}

	void Update()
	{
		if (Mouse.current.leftButton.isPressed)
		{
			SelectUnitsById(); //유닛 선택.
		}
		if (Mouse.current.rightButton.isPressed)
		{
			MoveUnits(); // 유닛 이동(강제move)
		}
		if (Keyboard.current.digit1Key.isPressed) SpawnUnit(0, true);
		if(Keyboard.current.digit2Key.isPressed) SpawnUnit(0, false);
		if (Time.time - lastCheckTime >= CheckInterval)
		{
			if (testint == 0)
			{
				/*SpawnUnit(unitnum, true);
				SpawnUnit(unitnum, false);
				testint++;*/
			}
			if (updateteam == 0)
			{
				UpdateUnitActions(updateteam, ref allyUnitIndex);
			}
			else
				UpdateUnitActions(updateteam, ref enemyUnitIndex);
			//if(XRInputDeviceButtonReader)
			/*	SpawnUnit(unitnum, true);
			//if (Joystick.current.IsPressed())
				SpawnUnit(unitnum, false);*/
			//임시로 누르면 0나오게 해둠.
			/*if (Input.GetKeyDown("1"))
				unitnum = 0;
			if (Input.GetKeyDown("2"))
				unitnum = 1;*/
			lastCheckTime = Time.time;
			if (updateteam == 0)
				updateteam = 1;
			else
				updateteam = 0;
		}
		if (Keyboard.current.pKey.isPressed) // 패트롤
		{
			Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
			Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (selectedid != -1)
					foreach (Unit unit in allyUnitsByType[selectedid])
					{
						Vector3 pos = hit.point;
						unit.Patrol(pos);
						unit.gameObject.name = "1";
					}
				else
				{
					Vector3 pos = hit.point;
					allyUnits[0].Patrol(pos);
					allyUnits[0].gameObject.name = "1";
				}
			}
		}
}
	public void MoveUnits()
	{
		Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();//마우스 위치 확인
		Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
		RaycastHit hit;//위치 보려고 레이캐스트 사용.
		if (selectedid != -1)
		{
			foreach (Unit units in allyUnitsByType[selectedid])
			{
				if (Physics.Raycast(ray, out hit))
				{

					foreach (Unit unit in allyUnitsByType[selectedid])
					{
						Vector3 pos = hit.point;
						unit.MoveTo(pos);//클릭 지점으로 이동.
						unit.gameObject.name = "1";
					}

				}
			}
		}
		else
		{
			if (Physics.Raycast(ray, out hit))
			{
				Vector3 pos = hit.point;
				allyUnits[0].MoveTo(pos);
				allyUnits[0].gameObject.name = "1";
			}
		}
	}
	public void AttackUnits(int i)
	{
		foreach (Unit units in allyUnitsByType[selectedid])
			units.AttackWIthTag(i);
	}

	// spawnUnit


	public void SpawnUnit(int unitIndex, bool isAlly)
	{
		if (unitIndex < 0 || unitIndex >= unitPrefabs.Length) return;
		//unitindex로 나누니까 unitindex로 부대저장해주고 나중에 공격타겟 잡을 때 unit index 로 매칭시켜줄 예정.
		Vector3 spawnBasePosition = GetMouseWorldPosition() ?? spawnPoint.position;
		GameObject unitPrefab = unitPrefabs[unitIndex];
		if (unitPrefab == null)
		{
			UnityEngine.Debug.Log("Invalid unit prefab.");
			return;
		}
		Unit unit = unitPrefab.GetComponent<Unit>();

		if (unit == null || unit.cost > currentResources)
		{
			UnityEngine.Debug.Log("Not enough resources to summon this unit."); // UI에 띄워주면 좋을것으로 보임.-필요x
			return;
		}


		Dictionary<int, List<Unit>> unitGroup = isAlly ? allyUnitsByType : enemyUnitsByType;//아군과 적군의 각각 소환된 유닛을 관리.
		if (unitGroup.ContainsKey(unitIndex))
		{
			UnityEngine.Debug.Log($"Unit type {unitIndex} has already been spawned."); //이미소환됨.
			return;
		}else
			unitGroup[unitIndex] = new List<Unit>();
		//???
		int temp = isAlly? 1 : 0;
		for (int i = 0; i < spawnCount; i++)
		{
			Vector3 spawnPosition = spawnBasePosition+new Vector3(i * unitSpacing, 0, 0); // 가로로 일렬 배치
			GameObject newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);

			Unit newUnitComponent = newUnit.GetComponent<Unit>();
			newUnitComponent.UnitManager = gameObject;
			newUnitComponent.isEnemy = !isAlly;
			newUnitComponent.id = unitIndex;
			//newUnitComponent.Stop();
			if (isAlly)
			{
				allyUnits.Add(newUnitComponent);
				unit.cards = cards;
				newUnit.layer = 6;
				newUnit.GetComponent<Sensor>().enemyLayer = 3;
				switch (unitIndex)
				{
					case 0:  // 🟢 전사 카드 적용
						unit.defense += cards.warCard[2] * (unit.defense * 0.2f); // 카드 3: 방어력 20% 증가
						unit.maxHP *= 1 + (cards.warCard[3] * 0.05f); // 카드 4: 최대 체력 증가 (5%씩)
						unit.reflectDamage += cards.warCard[4] * 0.15f; // 카드 5: 받는 피해 반사 15% 증가
						unit.shieldBonus *= 1 + (cards.warCard[6] * 0.3f); // 카드 7: 실드량 30% 증가
						unit.defense += (cards.warCard[8] > 0) ? unit.defense * 0.5f : 0; // 카드 8: 15초간 방어력 50% 증가
						break;
					case 1:
						// 🟢 궁수 카드 적용
						unit.attackPower *= 1 + (cards.archCard[0] * 0.1f); // 카드 1: 공격력 10% 증가
						unit.criticalRate += cards.archCard[2] * 0.15f; // 카드 3: 치명타 확률 15% 증가
						unit.criticalDamage *= 1 + (cards.archCard[3] * 0.3f); // 카드 4: 치명타 피해 30% 증가
						unit.attackSpeed *= 1 + (cards.archCard[5] * 0.1f); // 카드 6: 공격 속도 10% 증가
						unit.range *= 1 + (cards.archCard[7] * 0.1f); // 카드 8: 사거리 10% 증가
						break;
					case 2:
						// 🟢 힐러 카드 적용
						unit.maxHP *= 1 + (cards.healCard[3] * 0.2f); // 카드 4: 힐러 체력 20% 증가
						unit.healAmount *= 1 + (cards.healCard[4] * 0.05f); // 카드 5: 힐량 증가 (5%씩)
						break;
					case 3:
						// 🟢 마법사 카드 적용
						unit.attackPower *= 1 + (cards.magCard[1] * 0.05f); // 카드 2: 스킬 피해 증가-> 일단 공격력으로 간주.
						unit.range *= 1 + (cards.magCard[4] * 0.3f); // 카드 5: 스킬 범위 증가
						break;
					default:
						break;
				}
				unit.health = unit.maxHP;
			}
			else
			{
				enemyUnits.Add(newUnitComponent);
				newUnit.layer = 3;
				newUnit.GetComponent<Sensor>().enemyLayer = 6;
			}
			//소환 표시
			unitGroup[unitIndex].Add(newUnitComponent);
			UnityEngine.Debug.Log($"Spawned {unit.unitName} (ID: {newUnitComponent.id}). Remaining resources: {currentResources}");
		}

	}

	// UndoSpawn
	public void UndoSpawn(int unitID)
	{
		Unit targetUnit = allyUnits.Find(unit => unit.id == unitID);
		if (targetUnit != null)
		{
			RefundUnit(targetUnit, allyUnits);
			return;
		}

		targetUnit = enemyUnits.Find(unit => unit.id == unitID);
		if (targetUnit != null)
		{
			RefundUnit(targetUnit, enemyUnits);
		}
	}

	// Update Unit Behavior (Round-Robin)
	private void UpdateUnitActions(int k, ref int unitIndex)
	{
		//if (units[k].Count == 0) return;
		for (int i = 0; i < unitsPerFrame; i++)
		{
			if (units[k].Count == 0) break;
			unitIndex %= units[k].Count; // loop index
			Unit unit = units[k][unitIndex];
			if (units[1-k].Count > 0) //상대방 유닛이 있을때만 동작.
			if (unit.currentState == Unit.UnitState.Attacking || unit.currentState == Unit.UnitState.AttackingwithMoving || unit.currentState == Unit.UnitState.Patrol || unit.currentState == Unit.UnitState.AttackingPatrol)//
			{
				if (unit.target == null)
					unit.target = units[1 - k][0].transform;
				Transform closestEnemy = FindClosestEnemy(unit.transform.position, unit.target, unit.GetComponent<Unit>().isEnemy ? allyUnits : enemyUnits);
				if (closestEnemy != null)
				{
					
					if (unit.currentState == Unit.UnitState.Patrol)
					{
						if (Vector3.Distance(unit.transform.position, closestEnemy.position) < Math.Max(unit.target.GetComponent<Unit>().range,unit.range) + 1)
						{
							unit.currentState = Unit.UnitState.AttackingPatrol;
							unit.AttackTarget(closestEnemy);
						}
					}
					else if (unit.currentState == Unit.UnitState.AttackingPatrol)
					{
						if (Vector3.Distance(unit.transform.position, closestEnemy.position) > Math.Max(unit.target.GetComponent<Unit>().range, unit.range) + 4) // 적 사거리도 포함.
						{
							unit.currentState = Unit.UnitState.Patrol;
							unit.ResumePatrol();
						}
						else
							unit.AttackTarget(closestEnemy);
					}
					else
						unit.AttackTarget(closestEnemy);
				}
				else if (unit.currentState == Unit.UnitState.Attacking)
					unit.Stop();//가장 가까운 적이 멀면 Stop
				/*else
				{
					Vector3 randomPoint = GetRandomPoint(unit.transform.position); //이부분 승리모션으로 변경할지 고민.
					unit.MoveTo(randomPoint);
				}*/
			}
			if (unit.currentState == Unit.UnitState.AttackingUnit)
			{
				int key = unit.targetUnit;
				//Todo.원거리 유닛만 공격.-> 원거리 근거리로 나눌거니까 원거리 0 근거리 1 정도로 해서 원거리는 0, 근거리는 1 같은식으로 지정하고, 어태킹 타입이든 유닛이든 지정해서 명령할때 값변경시켜서 0대신 그변수넣으면될듯.
				{
					if (!allyUnitsByType.ContainsKey(key))
						unit.GetComponent<Unit>().currentState = Unit.UnitState.AttackingwithMoving;
					else if (allyUnitsByType[key].Count > 0)
					{
						Transform closestEnemy = FindClosestEnemy(unit.transform.position, unit.target, unit.GetComponent<Unit>().isEnemy ? enemyUnitsByType[key] : allyUnitsByType[key]);
						unit.AttackTarget(closestEnemy);
					}
					else
						unit.GetComponent<Unit>().currentState = Unit.UnitState.AttackingwithMoving;
				}

			}

			unitIndex++; // 다음 팀으로 이동
		}

		//UnityEngine.Debug.Log(units.Count);
	}

	private void RefundUnit(Unit targetUnit, List<Unit> unitList)
	{
		//리펀드-> 리트릿 정도로 수정하고 기능수정하면 될듯? 부대 인덱스 날려주고 소환기록 0으로.-Todo
		currentResources += targetUnit.cost;
		unitList.Remove(targetUnit);
		Destroy(targetUnit.gameObject);
		UnityEngine.Debug.Log($"Refunded resources: {targetUnit.cost}. Current resources: {currentResources}");
	}
	public void DestroyUnit(Unit targetUnit, bool isEnemy, int unitIndex)
	{
		List<Unit> unitList = new List<Unit>();
		List<Unit> unitList2 = new List<Unit>();
		if (isEnemy == false)
		{
			unitList = allyUnits;
			unitList2 = allyUnitsByType[unitIndex];
		}
		else
		{
			unitList = enemyUnits;
			unitList2 = enemyUnitsByType[unitIndex];
		}

		unitList.Remove(targetUnit);
		unitList2.Remove(targetUnit);
		UnityEngine.Debug.Log(unitList2.Count);
		Destroy(targetUnit.gameObject);
	}

	private Transform FindClosestEnemy(Vector3 position, Transform target, List<Unit> potentialTargets)
	{
		float closestDistanceSquared = (target.position - position).sqrMagnitude; // init Max Distance
		Transform closestEnemy = target;

		foreach (Unit enemy in potentialTargets)
		{
			float distanceSquared = (enemy.transform.position - position).sqrMagnitude; // 제곱 거리 계산
			if (distanceSquared < closestDistanceSquared)
			{
				closestDistanceSquared = distanceSquared;
				closestEnemy = enemy.transform;
			}
		}

		return closestEnemy;
	}

	private Vector3 GetRandomPoint(Vector3 origin)
	{
		Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 10f;
		randomDirection += origin;
		UnityEngine.AI.NavMeshHit hit;
		if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
		{
			return hit.position;
		}
		return origin;
	}

	internal void startEnemies()
	{
		throw new NotImplementedException();
	}

	private void SelectUnitsById() //유닛선택. 
	{
		Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
		Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			Unit clickedUnit = hit.collider.GetComponent<Unit>();
			if (clickedUnit != null)
			{
				if (clickedUnit.isEnemy == false)
				{
					int targetId = clickedUnit.id;
					//성능을 위해 그냥 id만 저장. 아래는 유닛들을 저장하는 코드.
					selectedid = targetId; //명령시에 selectid로 해당 id를 가진 모든 유닛에 명령.
										   //Todo-선택된 유닛들 아래에 초록원. 원래 선택되어있다면 해당원은 없애야함. -> 차라리 selected라는 변수를 줘서 true라면 해당 모형을 키는건 어떨까?
				}
				/*selectedUnits.Clear();
				foreach (Unit unit in allyUnitsByType[targetId])
				{
					//해당 id의 모든 유닛 선택.
					selectedUnits.Add(unit);
				}*/
			}
		}
	}


	private Vector3? GetMouseWorldPosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
		{
			return hit.point;
		}
		return null; // 못 찾으면 null 반환
	}

	/*수정중인 함수-아마도 awake에 들어가면 될듯.
	///DB unitStats = dbManager.GetUnitStats(unitIndex);
if(unitStats != null) {
    newUnitComponent.attackPower = unitStats.attackPower;
    newUnitComponent.attackSpeed = unitStats.attackSpeed;
    newUnitComponent.health = unitStats.health;
    newUnitComponent.defense = unitStats.defense;
    // 나머지 스탯도 마찬가지로 할당
}*/
}
