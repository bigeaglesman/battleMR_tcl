using UnityEngine;
using UnityEngine.AI;

public class Unit2 : MonoBehaviour
{
	public int id;
	public float range;
	public float maxHP;
	public float attackPower;
	public float health;

	public Card cards;
	public NavMeshAgent navagent;
	public Transform target;

	public enum UnitState { Idle, Attacking }
	public UnitState currentState = UnitState.Idle;

	public bool isEnemy = true;
	private UnitAnimator animator;
	public GameObject UnitManager;

	private void Start()
	{
		navagent = GetComponent<NavMeshAgent>();
		animator = GetComponent<UnitAnimator>();
	}

	private void Update()
	{
		if (currentState == UnitState.Attacking && target != null)
		{
			float distance = Vector3.Distance(transform.position, target.position);
			if (distance <= range)
			{
				animator.PlayAttack();
				Attack();
			}
			else
			{
				navagent.SetDestination(target.position);
				animator.PlayRun();
			}
		}
	}

	public void AttackTarget(Transform newTarget)
	{
		target = newTarget;
	}

	public void Attack()
	{
		target.GetComponent<Health>().TakeDamage(attackPower);

		if (id == 0 && cards.warCard[4] > 0)
		{
			float reflectDmg = attackPower * 0.15f * cards.warCard[4];
			target.GetComponent<Health>().TakeDamage(reflectDmg);
		}
	}

	public void Stop()
	{
		navagent.ResetPath();
		animator.StopAttack();
		currentState = UnitState.Idle;
	}

	public void TakeDamage(float damage)
	{
		health -= damage;
		if (health < 0) Die();
	}

	private void Die()
	{
		animator.PlayDie();
		UnitManager.GetComponent<UnitManager3>().DestroyUnit(this, isEnemy, id);
	}
}
