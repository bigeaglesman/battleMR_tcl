using UnityEngine;

public class SpawnUnit : MonoBehaviour
{
public GameObject unitPrefab; // 생성할 유닛 프리팹
    public int rows = 5; // 행 (세로)
    public int columns = 6; // 열 (가로)
    public float spacingX = 2.0f; // X축 간격
    public float spacingZ = 2.0f; // Z축 간격

    void Start()
    {
        SpawnUnits();
    }

    void SpawnUnits()
    {
        Vector3 startPosition = transform.position; // 시작 위치 기준
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 offset = new Vector3((columns - 1) * spacingX / 2, 0, (rows - 1) * spacingZ / 2);
				Vector3 spawnPosition = startPosition + new Vector3(col * spacingX, 0, row * spacingZ) - offset;
                Instantiate(unitPrefab, spawnPosition, Quaternion.identity, this.transform);
            }
        }
    }
}
