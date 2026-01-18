using System;
using System.Collections.Generic;
using Fusion;
using TheKiwiCoder;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager2 : NetworkBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private int i;
	private int allyUnitIndex = 0;
	//�߰��� �ڵ�
	public GameObject[] unitPrefabs;
	private Dictionary<int, List<Unit>> allyUnitsByType = new Dictionary<int, List<Unit>>();// 이전에 소환된 유닛 종류를 추적
	private Dictionary<int, List<Unit>> enemyUnitsByType = new Dictionary<int, List<Unit>>();// 이전에 소환된 유닛 종류를 추적
	private List<Unit> allyUnits = new List<Unit>();
	private List<Unit> enemyUnits = new List<Unit>();
	private int selectedid = -1;
	private int unitsPerFrame = 5;
	private List<List<Unit>> units = new List<List<Unit>>();
	//private변수들
	public Card cards;
	public int[] attackPower = { 5, 10, 0, 10 };
	public int heal = 5;
	public float[] range = { 0.2f, 0.2f, 0.2f, 0.2f };
	public int[] hp = { 200, 100, 100, 100 };

	[SerializeField] private NetworkRunner runner;
	private void Start()
	{
		runner = FindObjectOfType<NetworkRunner>();
		units.Add(allyUnits); //allyUnits만 가지고 하면되니까 이코드 필요
		units.Add(enemyUnits);
	}

	// Update is called once per frame
	void Update()
	{
		sudoUpdateUnitActions(0, ref allyUnitIndex); //유닛 행동 업데이트(타겟 갱신)
	}

	private void sudoUpdateUnitActions(int k, ref int unitIndex)
	{
		//if (units[k].Count == 0) return;
		for (int i = 0; i < unitsPerFrame; i++)
		{
			if (units[k].Count == 0)
			{
				// Debug.Log("break;");
				break;
			}

				unitIndex %= units[k].Count; // loop index
			Unit unit = units[k][unitIndex];
			//if (units[1 - k].Count > 0) //상대방 유닛이 있을때만 동작.
			if (unit.currentState == Unit.UnitState.Attacking || unit.currentState == Unit.UnitState.AttackingwithMoving || unit.currentState == Unit.UnitState.Patrol || unit.currentState == Unit.UnitState.AttackingPatrol)//
			{
				bool ist;
				if (unit.target == null)
					ist = false;
				Transform closestEnemy;
				if (unit.id == 2)//ishealer?
				{ closestEnemy = FindClosestAlly(unit.transform.position, unit); }//target is closest ally!
				else
				{
					closestEnemy = FindClosestEnemy(unit.transform.position, unit.target, true);
					if (closestEnemy == null)
					{
						Debug.Log("error");
					}
				}
				if (closestEnemy != null)
				{

					if (unit.currentState == Unit.UnitState.Patrol)
					{
						if (Vector3.Distance(unit.transform.position, closestEnemy.position) < Math.Max(unit.target.GetComponent<Unit>().range, unit.range) + 1)
						{
							unit.currentState = Unit.UnitState.AttackingPatrol;
							unit.AttackTarget(closestEnemy);
						}
					}
					else if (unit.currentState == Unit.UnitState.AttackingPatrol)
					{
						if (Vector3.Distance(unit.transform.position, closestEnemy.position) > Math.Max(unit.target.GetComponent<Unit>().range, unit.range) + 4) // �� ��Ÿ��� ����.
						{
							unit.currentState = Unit.UnitState.Patrol;
							unit.ResumePatrol();
						}
						else
						{
							unit.AttackTarget(closestEnemy);
							Debug.Log("error2");
						}
					}
					else
					{
						unit.AttackTarget(closestEnemy);
						Debug.Log("target postion = " + closestEnemy.position);
					}
				}
				else if (unit.currentState == Unit.UnitState.Attacking)
					unit.Stop();//가장 가까운 적이 멀면 Stop
				/*else
				{
					Vector3 randomPoint = GetRandomPoint(unit.transform.position); //이부분 승리모션으로 변경할지 고민.
					unit.MoveTo(randomPoint);
				}*/
			}
			/*if (unit.currentState == Unit.UnitState.AttackingUnit)
			{
				int key = unit.targetUnit;
				//Todo.원거리 유닛만 공격.-> 원거리 근거리로 나눌거니까 원거리 0 근거리 1 정도로 해서 원거리는 0, 근거리는 1 같은식으로 지정하고, 어태킹 타입이든 유닛이든 지정해서 명령할때 값변경시켜서 0대신 그변수넣으면될듯.
				{
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

				unitIndex++;
			}*/

			//UnityEngine.Debug.Log(units.Count);
			unitIndex++; // 이부분 빠지면 제대로 안돌아감!
		}
	}

	public void SpawnUnit(GameObject unitPrefab, bool isAlly, int unitIndex, Vector3 location, Quaternion spawnRotation, Transform BattleField)
	{

		Vector3 spawnBasePosition = location;
		if (unitPrefab == null)
		{
			UnityEngine.Debug.Log("Invalid unit prefab.");
			return;
		}
		
		//Unit unit = unitPrefab.GetComponent<Unit>();
		//unitindex로 나누니까 unitindex로 부대저장해주고 나중에 공격타겟 잡을 때 unit index 로 매칭시켜줄 예정.
		/*if (unit == null || unit.cost > currentResources)
		{
			UnityEngine.Debug.Log("Not enough resources to summon this unit.");
			return;
		}*/

		Dictionary<int, List<Unit>> unitGroup = isAlly ? allyUnitsByType : enemyUnitsByType; //isAlly(2번째 변수 가 참이면 아군, 거짓이면 적군으로 추가.
		if (!unitGroup.ContainsKey(unitIndex))
		{
			unitGroup[unitIndex] = new List<Unit>();
		}
		//for (int i = 0; i < spawnCount; i++)여러마리 한번에 소환->필요x
		{
			//Vector3 spawnPosition = spawnBasePosition + new Vector3(i * unitSpacing, 0, 0); 위치 지정 코드였는데 없앰.
			float yRotation = transform.eulerAngles.y;
			NetworkObject newUnit = runner.Spawn(unitPrefab, spawnBasePosition, spawnRotation, runner.LocalPlayer);
			Debug.Log("Unit Spawned");
			Unit newUnitComponent = newUnit.GetComponent<Unit>();
			newUnitComponent.UnitManager = gameObject;
			newUnitComponent.isEnemy = !isAlly; //이걸로 아군 구분함.-위에 보면 true가져와서 무조건 아군소환되게 고정.
			newUnitComponent.id = unitIndex; //원래 이거 사용해서 원하는 프리팹 사용하는건데 하나만 있으니까 0으로 해놨음. UnitColliderSpawn에 public 변수로 id 추가해서 Spawn에 3번째 변수 id로 주면 될 것 같긴함.(Attacking Unit에 필요.)
			newUnitComponent.attackPower = attackPower[unitIndex];
			newUnitComponent.health = hp[unitIndex];
			newUnitComponent.healAmount = heal;
			newUnitComponent.range = range[unitIndex];
			newUnit.GetComponent<NavMeshAgent>().stoppingDistance = range[unitIndex];
			Debug.Log("set stoppingd distance");

			if (isAlly)//센서 컨트롤 하는것이라 빼버림.
			{
				ApplyCardEffects(newUnitComponent, unitIndex);
				allyUnits.Add(newUnitComponent);
				//newUnit.layer = 6;
				//newUnit.GetComponent<Sensor>().enemyLayer = 3;
			}/*
			else
			{
				enemyUnits.Add(newUnitComponent);
				newUnit.layer = 3;
				newUnit.GetComponent<Sensor>().enemyLayer = 6;
			}*/
			/*newUnitComponent.enabled = false; //Disable Unit Script
			newUnit.GetComponent<NavMeshAgent>().enabled = false;*/
			unitGroup[unitIndex].Add(newUnitComponent);
			UnityEngine.Debug.Log("Not enough resources to summon this unit.");
		}
	}
	private void ApplyCardEffects(Unit unit, int unitIndex)
	{
		switch (unitIndex)
		{
			case 0:
				unit.defense += cards.warCard[2] * (unit.defense * 0.2f);
				unit.maxHP *= 1 + (cards.warCard[3] * 0.05f);
				unit.reflectDamage += cards.warCard[4] * 0.15f;
				unit.shieldBonus *= 1 + (cards.warCard[6] * 0.3f);
				unit.defense += (cards.warCard[8] > 0) ? unit.defense * 0.5f : 0;
				break;
			case 1:
				unit.attackPower *= 1 + (cards.archCard[0] * 0.1f);
				unit.criticalRate += cards.archCard[2] * 0.15f;
				unit.criticalDamage *= 1 + (cards.archCard[3] * 0.3f);
				unit.attackSpeed *= 1 + (cards.archCard[5] * 0.1f);
				unit.range *= 1 + (cards.archCard[7] * 0.1f);
				break;
			case 2:
				unit.maxHP *= 1 + (cards.healCard[3] * 0.2f);
				unit.healAmount *= 1 + (cards.healCard[4] * 0.05f);
				unit.healer = cards.healCard; //heal Card into Unit
				break;
			case 3:
				unit.attackPower *= 1 + (cards.magCard[1] * 0.05f);
				unit.range *= 1 + (cards.magCard[4] * 0.3f);
				break;
		}
		unit.health = unit.maxHP;
	}
	private Transform FindClosestEnemy(Vector3 position, Transform target, bool isTarget)
	{
		// Debug.Log("선택시작");
		float closestDistanceSquared;

			closestDistanceSquared = 10000f;//( // init Max Distance
			Transform closestEnemy = target;

		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			float distanceSquared = (enemy.transform.position - position).sqrMagnitude; // ���� �Ÿ� ���
			if (distanceSquared < closestDistanceSquared)
			{
				Debug.Log("타겟 선택");
				closestDistanceSquared = distanceSquared;
				closestEnemy = enemy.transform;
			}
		}

		return closestEnemy;
	}
	private Transform FindClosestAlly(Vector3 position, Unit u)
	{
		// Debug.Log("선택시작");
		float closestDistanceSquared;
		Transform closestAlly = allyUnits[0].transform;
		closestDistanceSquared = 10000f;//( // init Max Distance
		if (u.target != null)
		{
			closestDistanceSquared = (u.target.transform.position - position).sqrMagnitude;
			closestAlly = u.target;
		}

		foreach (Unit ally in allyUnits)
		{
			float distanceSquared = (ally.transform.position - position).sqrMagnitude; // ���� �Ÿ� ���
			if (distanceSquared < closestDistanceSquared)
			{
				// Debug.Log("타겟 선택");
				closestDistanceSquared = distanceSquared;
				closestAlly = ally.transform;
			}
		}

		return closestAlly;
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
    public BehaviourTreeRunner runner2;
    public void StartBattle()
	{
		runner2.StartTree();
		foreach(Unit unit in allyUnits)
		{
			Debug.Log("Start Battle");
			unit.currentState = Unit.UnitState.AttackingwithMoving;
		}
	}
}
