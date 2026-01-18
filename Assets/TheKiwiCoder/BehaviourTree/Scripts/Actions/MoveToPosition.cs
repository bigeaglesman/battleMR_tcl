using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveToPosition : ActionNode
{
    public float speed = 5;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    protected override void OnStart()
    {
        //Debug.Log($"[MoveToPosition] START - Target Position: {blackboard.moveToPosition}, Speed: {speed}, Acceleration: {acceleration}, StoppingDistance: {stoppingDistance}");
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.speed = speed;
        context.agent.destination = blackboard.moveToPosition;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;
    }

    protected override void OnStop()
    {
        //Debug.Log("[MoveToPosition] MOVING STOP");
    }

    protected override State OnUpdate()
    {
        if (context.agent.pathPending)
        {
            //Debug.Log("경로 계산 중");
            return State.Running;
        }

        float remainingDistance = context.agent.remainingDistance;
        //Debug.Log($"📏 [MoveToPosition] 현재 거리: {remainingDistance:F2}m (목표 허용 오차: {tolerance}m)");

        if (context.agent.remainingDistance < tolerance)
        {
            //Debug.Log("목표 도착 이동 성공");
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            //Debug.Log("[MoveToPosition] 경로 오류 - 이동 실패");

            return State.Failure;
        }

        return State.Running;
    }
}
