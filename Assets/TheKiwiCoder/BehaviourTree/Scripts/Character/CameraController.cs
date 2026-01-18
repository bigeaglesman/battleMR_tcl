using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // 카메라 이동 속도
    public float panBorderThickness = 10f; // 화면 경계에서 이동 감지 거리
    public Vector2 panLimit = new Vector2(200f, 200f); // ✅ 이동 제한 범위 확장 (기존 50 → 200)

    public float scrollSpeed = 1500f; // 줌 속도
    public float zoomSensitivity = 3f; // 줌 감도
    public float minY = 5f; // 최소 줌
    public float maxY = 100f; // 최대 줌

    void Update()
    {
        Vector3 pos = transform.position;

        // ✅ WASD 또는 화면 모서리 이동
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        // ✅ 마우스 휠 줌인/줌아웃 감도 증가
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float zoomAmount = Mathf.Sign(scroll) * zoomSensitivity * scrollSpeed * Time.deltaTime;
            pos.y -= zoomAmount;
        }

        // ✅ 카메라 이동 범위 제한 (이동 가능 범위 확장)
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

        transform.position = pos;
    }
}
