using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

public class RandomPosition : ActionNode
{
    public Vector2 min = Vector2.one * -10;
    public Vector2 max = Vector2.one * 10;

    protected override void OnStart()
    {
        //Debug.Log("[RandomPosition] START");
    }

    protected override void OnStop()
    {
        //Debug.Log("[RandomPosition] STOP");
    }

    protected override State OnUpdate()
    {
        float randomX = Random.Range(min.x, max.x);
        float randomZ = Random.Range(min.y, max.y);

        Vector3 randomPosition = new Vector3(randomX, 0, randomZ); // y == 0

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            randomPosition = hit.position;
            //Debug.Log($"[RandomPosition] 유효한 NavMesh 위치 적용: {randomPosition}");
        }
        else
        {
            //Debug.LogError($"[RandomPosition] 유효한 NavMesh 위치를 찾을 수 없음: {randomPosition}");
        }

        //Blackboard에 적용
        blackboard.moveToPosition = randomPosition;
        //blackboard.moveToPosition.x = randomX;
        //blackboard.moveToPosition.z = randomZ;

        //blackboard.moveToPosition.x = Random.Range(min.x, max.x);
        //blackboard.moveToPosition.z = Random.Range(min.y, max.y);

        //Debug.Log($"[RandomPosition]: X={randomX:F2}, Z={randomZ:F2}");
        return State.Success;
    }
}
