using Fusion;
using TheKiwiCoder;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Meta.XR.MRUtilityKit.FindSpawnPositions;

public class UnitState : NetworkBehaviour
{

	public enum State
	{
		Idle, 
		Run,
		Attack,
		Die
	}

    private Animator animator;

    [Networked] public State unitState { get; set; }
    private State unitStateBuf;

    public NavMeshAgent navagent;
    [SerializeField] private GameObject battleField;
    [Networked] Vector3 localposition { get; set; }
    [Networked] Quaternion localrotation { get; set; }
    public override void Spawned()
    {
        navagent = GetComponent<NavMeshAgent>();
        battleField = GameObject.FindWithTag("FieldParent");
        transform.SetParent(battleField.transform);
        animator = GetComponent<Animator>();


        if (HasInputAuthority)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                navagent.Warp(hit.position);
            }
            localposition = transform.position - battleField.transform.position;
            localrotation = transform.rotation;
            gameObject.GetComponent<BehaviourTreeRunner>().enabled = true;
            unitState = State.Idle;
            unitStateBuf = State.Idle;
            gameObject.tag = "Ally";
        }
        else
        {
            navagent.enabled = false;
            Vector3 flippedLocalPos = new Vector3(-localposition.x, localposition.y, -localposition.z);
            transform.position = battleField.transform.position + flippedLocalPos;
            transform.rotation = localrotation * Quaternion.Euler(0f, 180f, 0f);
            gameObject.tag = "Enemy";
        }

    }

	public override void FixedUpdateNetwork()
	{
        //foreach (var changedProperty in _changeDetector.DetectChanges(this))
        //{
        //    if (changedProperty == nameof(unitState))
        //    {
        //        Debug.Log("unit state change detected");
        //        changeAnimatorBool(unitStateBuf, unitState);
        //        unitStateBuf = unitState;
        //    }
        //}

        if (Object.HasInputAuthority)
        {
            Debug.Log("Network Update with authority");
            localposition = transform.position - battleField.transform.position;
            localrotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void Update()
    {
        if (!Object.HasInputAuthority)
        {
            Debug.Log("Network Update without authority");
            // transform.position = battleField.transform.TransformPoint(localposition);
            Vector3 flippedLocalPos = new Vector3(-localposition.x, localposition.y, -localposition.z);
            transform.position = battleField.transform.position + flippedLocalPos;
            transform.rotation = localrotation;
            Debug.Log("set enemy position");
        }
        if (unitStateBuf != unitState)
        {
            changeAnimatorBool(unitStateBuf, unitState);
            unitStateBuf = unitState;
        }
    }

    private void changeAnimatorBool(State prevState, State currentState)
	{
        Debug.Log("Change State" + prevState + " to " + currentState);
        switch(prevState)
        {
            case State.Idle:
                break;
            case State.Run:
                animator.SetBool("isRun", false);
                break;
            case State.Attack:
                animator.SetBool("isAttack", false);
                break;
        }
        switch(currentState)
        {
            case State.Idle:
                break;
            case State.Run:
                animator.SetBool("isRun", true);
                break;
            case State.Attack:
                animator.SetBool("isAttack", true);
                break;
        }
	}


}
