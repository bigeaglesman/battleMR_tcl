using Fusion;
using NUnit.Framework;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;

public class AllyUnitManager : NetworkBehaviour
{
    private int allyUnitIndex = 0;

    private List<GameObject> allyUnitList = new List<GameObject>();

    [SerializeField] private NetworkRunner runner;

    public float[] range = { 0.1f, 0.002f, 0.002f, 0.002f };

    public GameObject roundManagerPrefab;

    public RoundManager roundManager;

    private void Start()
    {
    }

    public void SpawnAllyUnit(GameObject unitPrefab, Vector3 spawnLocation,
            Quaternion spawnRotation, Transform BattleField)
    {
        if (runner == null || runner.LocalPlayer == null)
        {
            runner = FindObjectOfType<NetworkRunner>();
            Debug.Log("runner set in allymanager");
        }
        if (unitPrefab == null)
        {
            UnityEngine.Debug.Log("Invalid unit prefab.");
            return;
        }
        Debug.Log($"runner: {runner}");
        Debug.Log($"unitPrefab: {unitPrefab}");
        Debug.Log($"LocalPlayer11: {runner?.LocalPlayer}");
        NetworkObject newNetworkObj = runner.Spawn(unitPrefab, spawnLocation,
                spawnRotation, runner.LocalPlayer);
        GameObject newAllyUnit = newNetworkObj.gameObject;
        NavMeshAgent navAgent = newAllyUnit.GetComponent<NavMeshAgent>();
        navAgent.enabled = true;
        navAgent.stoppingDistance = range[0];
        Health health = newAllyUnit.GetComponent<Health>();
        health.unitManager = this;
        allyUnitList.Add(newAllyUnit.gameObject);
        Debug.Log("UnitSpawned called in manager");
    }

    //public void DestroyAllyUnit()

    public void StartBattle()
    {
        Debug.Log("BattleStart");
        if (HasStateAuthority && runner.IsSharedModeMasterClient)
        {
            if (FindObjectOfType<RoundManager>() == null)
            {
                roundManager = runner.Spawn(roundManagerPrefab, Vector3.zero, 
                    Quaternion.identity, Object.InputAuthority).GetComponent<RoundManager>();
                Debug.Log("RoundManager Spawned");
            }
        }
        foreach (GameObject allyUnit in allyUnitList)
        {
            Debug.Log("unit start");
            allyUnit.GetComponent<BehaviourTreeRunner>().StartTree();
        }
    }

    public void NotifyUnitDeath(GameObject unit)
    {
        if (allyUnitList.Contains(unit))
        {
            allyUnitList.Remove(unit);
        }

        if (allyUnitList.Count == 0)
        {
            Debug.Log("All ally units are dead");
            if (runner.IsSharedModeMasterClient)
                roundManager.RPC_GameOver(0);
            else
                roundManager.RPC_GameOver(1);
        }
    }
}