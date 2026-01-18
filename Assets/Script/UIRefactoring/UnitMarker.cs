using System.Collections.Generic;
using UnityEngine;

public class UnitMarker : MonoBehaviour
{
    //public Transform unit;
    //public Transform battlefield;

    //public GameObject battlefieldPrefab;

    //public RectTransform markerUI;
    //public RectTransform miniMapRect;

    

    //public Vector3 battlefieldMin;
    //public Vector3 battlefieldMax;


    //void Start()
    //{
    //    
    //}

    //void Update()
    //{
    //    if (unit == null || markerUI == null || miniMapRect == null) return;

    //    Vector3 pos = unit.position;

    //    float normalizedX = Mathf.InverseLerp(battlefieldMin.x, battlefieldMax.x, pos.x);
    //    float normalizedZ = Mathf.InverseLerp(battlefieldMin.z, battlefieldMax.z, pos.z);

    //    float mapWidth = miniMapRect.rect.width;
    //    float mapHeight = miniMapRect.rect.height;

    //    float localX = (normalizedX - 0.5f) * mapWidth;
    //    float localY = (normalizedZ - 0.5f) * mapHeight;

    //    markerUI.localPosition = new Vector3(localX, localY, 0);
    //}

    public RectTransform miniMapRect;
    public GameObject markerPrefab;
    public List<Transform> unitList;// GameManager에서 받아와야함 , 아군, 적군 나눌 소요 존재
    public Vector3 battlefieldMin, battlefieldMax;

    public GameObject battlefieldPrefab;

    private List<RectTransform> markers = new();    

    private Collider battlefieldCollider;
    private Bounds bounds;

    void Start()
    {

        battlefieldCollider = battlefieldPrefab.GetComponent<Collider>();
        bounds = battlefieldCollider.bounds;
        battlefieldMin = bounds.min;
        battlefieldMax = bounds.max;
        foreach (var unit in unitList)
        {
            var marker = Instantiate(markerPrefab, miniMapRect);
            markers.Add(marker.GetComponent<RectTransform>());
        }
    }

    void Update()
    {
        for (int i = 0; i < unitList.Count; i++)
        {
            Vector3 pos = unitList[i].position;

            float normalizedX = Mathf.InverseLerp(battlefieldMin.x, battlefieldMax.x, pos.x);
            float normalizedZ = Mathf.InverseLerp(battlefieldMin.z, battlefieldMax.z, pos.z);

            float width = miniMapRect.rect.width;
            float height = miniMapRect.rect.height;

            float localX = (normalizedX - 0.5f) * width;
            float localY = (normalizedZ - 0.5f) * height;

            markers[i].localPosition = new Vector3(localX, localY, 0);
        }
    }
}
