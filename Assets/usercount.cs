using UnityEngine;
using Fusion;
using System.Collections;
using System.Linq;

public class usercount : MonoBehaviour
{
	[SerializeField] private NetworkRunner runner;

    private void Start()
    {
		runner = FindObjectOfType<NetworkRunner>();
        StartCoroutine(LogPlayerCountCoroutine());
    }

    private IEnumerator LogPlayerCountCoroutine()
    {
        while (true)
        {
            if (runner != null && runner.IsRunning)
            {
                int playerCount = runner.ActivePlayers.Count();
                Debug.Log($"[Fusion] 현재 접속 중인 플레이어 수: {playerCount}");
            }
            else if (runner == null)
            {
                Debug.LogWarning("[Fusion] NetworkRunner is null");
            }
			else
            {
                Debug.LogWarning("[Fusion] NetworkRunner is not running");
            }

            yield return new WaitForSeconds(1f); // 1초마다 반복
        }
    }
}
