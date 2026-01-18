using System;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;
using Fusion;
using System.Runtime.CompilerServices;

public class UIManager : MonoBehaviour
{
	public GameObject battleFieldPrefab;
	private GameObject battleField;
	public Transform spawnPositions;
	public GameObject spawnTemplate;
	private int fieldFlag = 0;
	private int spawnBlockFlag = 0;
	private GameObject spawnBlock;

	public UnitCollideSpawn unit1Spawner;
	public UnitCollideSpawn unit2Spawner;
	public UnitCollideSpawn unit3Spawner;
	public UnitCollideSpawn unit4Spawner;

	[SerializeField] private NetworkRunner runner;
	[SerializeField] private GameObject capsule;

	public GameObject unitGrab1;
	public GameObject unitGrab2;
	public GameObject unitGrab3;
	public GameObject unitGrab4;
	public void SpawnNetCapsule()
	{
		runner = FindObjectOfType<NetworkRunner>();
		runner.Spawn(
            capsule,
            position: transform.position,
            rotation: Quaternion.identity,
            inputAuthority: runner.LocalPlayer  // 입력 권한 (보통 자기자신)
        );
	}

	public void SpawnFieldOnDesk()
	{
		MRUKRoom room = MRUK.Instance.GetCurrentRoom();
		// Vector3 DeskPosition = 
		// battleField = Ins
		List<MRUKAnchor> anchors = room.GetRoomAnchors();
		if (fieldFlag == 1)
			DeleteField();
		foreach(MRUKAnchor anchor in anchors)
		{
			if (anchor.AnchorLabels[0] == "TABLE" && fieldFlag == 0)
			{
				Vector3 anchorSize = anchor.GetAnchorSize();
				Vector3 zLenHalf = new Vector3(0,anchorSize.z/2,0);

				Debug.Log("label: "+anchor.AnchorLabels[0]);
				battleField = Instantiate(battleFieldPrefab, anchor.GetAnchorCenter() + zLenHalf, Quaternion.identity);
				// unit1Spawner.battleField = battleField.transform.Find("Field").transform;
				fieldFlag = 1;
			}
		}
	}
	public void SpawnFieldOnBed()
	{
		MRUKRoom room = MRUK.Instance.GetCurrentRoom();
		// Vector3 DeskPosition = 
		// battleField = Ins
		List<MRUKAnchor> anchors = room.GetRoomAnchors();
		if (fieldFlag == 1)
			DeleteField();
		foreach(MRUKAnchor anchor in anchors)
		{
			if (anchor.AnchorLabels[0] == "BED" && fieldFlag == 0)
			{
				Vector3 anchorSize = anchor.GetAnchorSize();
				Vector3 zLenHalf = new Vector3(0,anchorSize.z/2,0);
				Debug.Log("label: "+anchor.AnchorLabels[0]);
				battleField = Instantiate(battleFieldPrefab, anchor.GetAnchorCenter() + zLenHalf, Quaternion.identity);
				// unit1Spawner.battleField = battleField.transform.Find("Field").transform;
				fieldFlag = 1;
			}
		}
	}

	public void SpawnFieldCenter()
	{
		if (fieldFlag == 0)
		{
			MRUKRoom room = MRUK.Instance.GetCurrentRoom();
			MRUKAnchor floorAnchor = room.FloorAnchor;
			MRUKAnchor ceilingAnchor = room.CeilingAnchor;
			Vector3 spawnPosition = floorAnchor.GetAnchorCenter() +
				new Vector3(0,(ceilingAnchor.GetAnchorCenter()-floorAnchor.GetAnchorCenter()).y*1/3,0);
			battleField = Instantiate(battleFieldPrefab, spawnPosition, Quaternion.identity);
			SyncHeadHand syncHeadHand = GameObject.FindWithTag("SyncLocal").GetComponent<SyncHeadHand>();
			syncHeadHand.fieldTransform = GameObject.FindWithTag("Field").GetComponent<Transform>();
			// unit1Spawner.battleField = battleField.transform.Find("Field").transform;
			fieldFlag = 1;
		}

	}

	public void SetGrabposition()
	{
		Debug.Log("Set Grab");
		//unitGrab1.transform.location = battleField
		unitGrab1.transform.position = GameObject.FindWithTag("spawn1").transform.position;
		unitGrab1.transform.rotation *= Quaternion.Euler(0, -90f, 0);
		unitGrab2.transform.position = GameObject.FindWithTag("spawn2").transform.position;
		unitGrab2.transform.rotation *= Quaternion.Euler(0, -90f, 0);
		unitGrab3.transform.position = GameObject.FindWithTag("spawn3").transform.position;
		unitGrab3.transform.rotation *= Quaternion.Euler(0, -90f, 0);		
		//unitGrab4.transform.position = GameObject.FindWithTag("spawn4").transform.position;
		//unitGrab4.transform.rotation *= Quaternion.Euler(0, -90f, 0);
		unitGrab1.SetActive(true);
		unitGrab2.SetActive(true);
		unitGrab3.SetActive(true);
		//unitGrab4.SetActive(true);
		unitGrab1.GetComponent<UnitCollideSpawn>().battleField = battleField.transform;
		unitGrab2.GetComponent<UnitCollideSpawn>().battleField = battleField.transform;
		unitGrab3.GetComponent<UnitCollideSpawn>().battleField = battleField.transform;
		//unitGrab4.GetComponent<UnitCollideSpawn>().battleField = battleField.transform;

	}

	public void OnOffFieldMove()
	{
		if (fieldFlag == 1)
		{
			GameObject pin = battleField.GetComponent<FollowPin>().targetA;
			pin.SetActive(!pin.activeSelf);
		}
	}

	public void DeleteField()
	{
		if (fieldFlag == 1)
		{
			Destroy(battleField);
			unit1Spawner.battleField = null;
			fieldFlag = 0;
		}	
	}
	private void BattleStart()
	{
		//Todo
	}

	

	//
	/*private void sudoSelect(int id)
	{
		selectedid = id;
	}*/
	




	public void IncreaseFieldScale()
	{
		if (fieldFlag == 1)
			battleField.transform.localScale *= 1.2f;
	}

	public void DecreaseFieldScale()
	{
		if (fieldFlag == 1)
			battleField.transform.localScale *= 0.8f;

	}

	public void TransparentField()
	{
		if (fieldFlag == 1);
		battleField.GetComponent<characterSpawner>().TransparentField();
	}
	private Transform FindClosestEnemy(Vector3 position, Transform target, List<Unit> potentialTargets)
	{
		float closestDistanceSquared = (target.position - position).sqrMagnitude; // init Max Distance
		Transform closestEnemy = target;

		foreach (Unit enemy in potentialTargets)
		{
			float distanceSquared = (enemy.transform.position - position).sqrMagnitude; // ���� �Ÿ� ���
			if (distanceSquared < closestDistanceSquared)
			{
				closestDistanceSquared = distanceSquared;
				closestEnemy = enemy.transform;
			}
		}

		return closestEnemy;
	}
}
