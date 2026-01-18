using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager3 : MonoBehaviour
{
	private int allyUnitIndex = 0;
	public GameObject[] unitPrefabs;
	private Dictionary<int, List<Unit2>> allyUnitsByType = new Dictionary<int, List<Unit2>>();
	private Dictionary<int, List<Unit2>> enemyUnitsByType = new Dictionary<int, List<Unit2>>();
	private List<Unit2> allyUnits = new List<Unit2>();
	private List<Unit2> enemyUnits = new List<Unit2>();
	private int unitsPerFrame = 5;
	private List<List<Unit2>> units = new List<List<Unit2>>();

	public Card cards;
	public int[] attackPower = { 5, 10, 0, 10 };
	public float[] range = { 0.2f, 0.2f, 0.2f, 0.2f };
	public int[] hp = { 200, 100, 100, 100 };

	private void Start()
	{
		units.Add(allyUnits);
		units.Add(enemyUnits);
	}

	void Update()
	{
		sudoUpdateUnitActions(0, ref allyUnitIndex);
	}

	private void sudoUpdateUnitActions(int k, ref int unitIndex)
	{
		for (int i = 0; i < unitsPerFrame; i++)
		{
			if (units[k].Count == 0) break;

			unitIndex %= units[k].Count;
			Unit2 unit = units[k][unitIndex];

			if (unit.currentState == Unit2.UnitState.Attacking)
			{
				Transform closestEnemy = FindClosestEnemy(
					unit.transform.position,
					unit.isEnemy
				);

				if (closestEnemy != null)
				{
					unit.AttackTarget(closestEnemy);
				}
				else
				{
					unit.Stop();
				}
			}
			unitIndex++;
		}
	}

	public void SpawnUnit(GameObject unitPrefab, bool isAlly, int unitIndex, Vector3 location, Quaternion spawnRotation, Transform BattleField)
	{
		Dictionary<int, List<Unit2>> unitGroup = isAlly ? allyUnitsByType : enemyUnitsByType;
		if (!unitGroup.ContainsKey(unitIndex))
		{
			unitGroup[unitIndex] = new List<Unit2>();
		}

		GameObject newUnit = Instantiate(unitPrefab, location, spawnRotation, BattleField);
		Unit2 newUnitComponent = newUnit.GetComponent<Unit2>();
		newUnitComponent.UnitManager = gameObject;
		newUnitComponent.isEnemy = !isAlly;
		newUnitComponent.id = unitIndex;
		newUnitComponent.attackPower = attackPower[unitIndex];
		newUnitComponent.health = hp[unitIndex];
		newUnitComponent.range = range[unitIndex];
		newUnit.GetComponent<NavMeshAgent>().stoppingDistance = range[unitIndex];

		if (isAlly)
		{
			ApplyCardEffects(newUnitComponent, unitIndex);
			allyUnits.Add(newUnitComponent);
		}
		unitGroup[unitIndex].Add(newUnitComponent);
	}

	private void ApplyCardEffects(Unit2 unit, int unitIndex)
	{
		switch (unitIndex)
		{
			case 0:
				unit.attackPower *= 1 + (cards.warCard[0] * 0.1f);
				unit.maxHP *= 1 + (cards.warCard[3] * 0.05f);
				break;
			case 1:
				unit.attackPower *= 1 + (cards.archCard[0] * 0.15f);
				unit.range *= 1 + (cards.archCard[7] * 0.2f);
				break;
			case 3:
				unit.attackPower *= 1 + (cards.magCard[1] * 0.1f);
				break;
		}
		unit.health = unit.maxHP;
	}

	private Transform FindClosestEnemy(Vector3 position, bool isCurrentUnitEnemy)
	{
		List<Unit2> targetUnits = isCurrentUnitEnemy ? allyUnits : enemyUnits;
		Transform closestEnemy = null;
		float closestDistanceSquared = Mathf.Infinity;

		foreach (Unit2 unit in targetUnits)
		{
			if (unit == null || unit.transform == null) continue;

			float distanceSquared = (unit.transform.position - position).sqrMagnitude;
			if (distanceSquared < closestDistanceSquared)
			{
				closestDistanceSquared = distanceSquared;
				closestEnemy = unit.transform;
			}
		}
		return closestEnemy;
	}

	public void DestroyUnit(Unit2 targetUnit, bool isEnemy, int unitIndex)
	{
		List<Unit2> unitList = isEnemy ? enemyUnits : allyUnits;
		List<Unit2> unitList2 = isEnemy ? enemyUnitsByType[unitIndex] : allyUnitsByType[unitIndex];

		unitList.Remove(targetUnit);
		unitList2.Remove(targetUnit);
		Destroy(targetUnit.gameObject);
	}

	public void StartBattle()
	{
		foreach (Unit2 unit in allyUnits)
		{
			unit.currentState = Unit2.UnitState.Attacking;
		}
	}
}
