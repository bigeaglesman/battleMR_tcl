using System.Collections;
using TMPro;
using UnityEngine;

public class UnitCollideSpawn : MonoBehaviour
{
	public GameObject unitPrefab;
	public Transform battleField;
	public float spawnCooldown = 1f;
	private float lastSpawnTime = 0f;
	public float checkRadius = 1f; // 검사 반경
    public LayerMask objectLayer;
    public UnitManager2 manager;

    public AllyUnitManager allyUnitManager;

	public int unitCount;
	public int unitIndex;
	public TMP_Text unitCountText;
    public Vector3 returnLocalPosition; // 복귀할 로컬 위치
    public Quaternion returnLocalRotation; // 복귀할 로컬 회전
    public float idleTimeBeforeReturn = 2f; // 대기 시간

    private float timer = 0f;
    private Vector3 lastLocalPosition;
    private Quaternion lastLocalRotation;

	private void Start()
	{
		unitCountText.text = string.Format("x{0}", unitCount);
        lastLocalPosition = transform.localPosition;
        lastLocalRotation = transform.localRotation;
        returnLocalPosition = lastLocalPosition; // 복귀 위치 설정
        returnLocalRotation = lastLocalRotation; // 복귀 회전 설정
	}
    void Update()
    {
        // 움직임 감지 (로컬 위치 기준)
        if (transform.localPosition != lastLocalPosition)
        {
            timer = 0f; // 움직이면 타이머 리셋
            lastLocalPosition = transform.localPosition;
            lastLocalRotation = transform.localRotation;
        }
        else
        {
            timer += Time.deltaTime; // 움직이지 않으면 타이머 증가
        }

        // 일정 시간이 지나면 복귀 (로컬 위치 기준)
        if (timer >= idleTimeBeforeReturn)
        {
            transform.localPosition = returnLocalPosition; // 위치 복귀
            transform.localRotation = returnLocalRotation; // 회전 복귀
            timer = 0f; // 복귀 후 타이머 초기화
        }
    }
	private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Field")) // 특정 오브젝트와 충돌 시
        {
			Vector3 hitPoint = other.ClosestPoint(transform.position);
            if (Time.time >= lastSpawnTime + spawnCooldown && 
				!Physics.CheckSphere(hitPoint, checkRadius, objectLayer) &&
				unitCount > 0) // 1초 이상 지났다면
            {
				Debug.Log("COLLIDE DETECTED");
				unitCount--;
				unitCountText.text = string.Format("x{0}", unitCount);
				float yRotation = transform.eulerAngles.y;
				Quaternion spawnRotation = Quaternion.Euler(0, yRotation, 0);
				//Instantiate(unitPrefab, hitPoint, spawnRotation); // 유닛 생성
				//manager.SpawnUnit(unitPrefab, true, unitIndex, hitPoint, spawnRotation, battleField);
                allyUnitManager.SpawnAllyUnit(unitPrefab, hitPoint, spawnRotation, battleField);
                lastSpawnTime = Time.time; // 마지막 생성 시간 갱신
            }
		}
    }
}
