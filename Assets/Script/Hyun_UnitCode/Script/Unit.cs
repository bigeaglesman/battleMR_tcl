using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;
using Fusion;

public class Unit : NetworkBehaviour
{
	public int id;
	public string unitName;
	public int cost;
	public float range;
	//추가해서 코드에 반영해야되는 변수들
	//전사
	public float defense;
	public float maxHP;
	public float reflectDamage;
	public float shieldBonus;
	private float shield; //쉴드와 쉴드보너스는 별개...
	//궁수
	public float criticalRate, criticalDamage, attackSpeed;
	private float attackStack; //연속공격 횟수
	private Transform lastAttackedTarget; //연속 공격 판정을 위한 이전타겟
	//힐러
	public float healAmount;
	//법사-일단 변수x
	//카드 오브젝트
	public Card cards;
	//쉴드 계산용 임시변수
	private float temp;


	public float attackPower;
	[Networked] public float health {get; set;}

	public float distancePatrol = 0.5f;//패트롤도착 판정거리.
	private float CheckInterval = 0.2f; // 0.2초 간격으로 탐지
	private float lastCheckTime = 0;
	private float squaredThreshold=0.5f; // 목적지 도착 판정
	public GameObject UnitManager;
	public NavMeshAgent navagent;
	private UnitAnimator animator;
	public Transform target;
	public bool attacking = false;
	public Vector3[] destination = new Vector3[2];
	private int patrolIndex = 0; 
	public bool isEnemy = true;
	public bool moving = false;
	public enum UnitState { Idle, Moving, Attacking, AttackingwithMoving, AttackingUnit,AttackingPatrol, Patrol }//정지(홀드), 움직임, 공격(홀드공격), 돌격, 부대별 공격, 패트롤.
	[Networked] public UnitState currentState {get; set;} // spawned 에서 state idle 해주기기
	public int targetUnit;
	private bool buffed;
	private float buffCount;
	private int[] healbuff = new int[3];
	public int[] healer = new int[9];

	public Renderer[] renderers;

	[Networked] Vector3 localposition {get; set;}
	[Networked] Quaternion localrotation {get; set;}
	[SerializeField] private GameObject battleField;
public override void Spawned()
{
    navagent = GetComponent<NavMeshAgent>();
    renderers = GetComponentsInChildren<Renderer>();
    battleField = GameObject.FindWithTag("FieldParent");
    transform.SetParent(battleField.transform);

    if (HasInputAuthority)
    {
        navagent.enabled = false;
        navagent.Warp(transform.position); // 정확한 초기 위치 적용
        navagent.enabled = true;
        navagent.stoppingDistance = range;
		localposition = battleField.transform. position - transform.position;
		localrotation = transform.rotation;
		gameObject.tag = "Ally";
    }
    else
    {
        navagent.enabled = false;
        transform.position = battleField.transform.position;
		gameObject.tag = "Enemy";
    }

    currentState = UnitState.Idle;
}

// public override void FixedUpdateNetwork()
// {
		// if (!HasInputAuthority)
		// {
		// 	// transform.position = battleField.transform.TransformPoint(localposition);
		// 	transform.position = battleField.transform.position + localposition;
		// 	Debug.Log("set enemy position");
		// }
		// else
		// {
		// 	localposition = battleField.transform.position - transform.position;
		// }
// }

    public void SetVisible(bool isVisible)
    {
        if (renderers == null)
            renderers = GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            renderer.enabled = isVisible;
        }
    }

	public void battleStart()
	{
		if (!HasStateAuthority)
			SetVisible(true);
	}

	private void Start()
	{
		squaredThreshold = distancePatrol * distancePatrol;
		// navagent = gameObject.GetComponent<NavMeshAgent>();
		//navagent.stoppingDistance = range;
		animator = GetComponent<UnitAnimator>();
		currentState = UnitState.Idle;
	}

	private void Update()
	{
		if (!HasInputAuthority)
		{
			// transform.position = battleField.transform.TransformPoint(localposition);
			transform.position = battleField.transform.position + localposition;
			transform.rotation = localrotation;
			Debug.Log("set enemy position");
		}
		else
		{
			localposition = battleField.transform.position - transform.position;
			localrotation = transform.rotation;
		}
		Debug.Log("local pos "+localposition);
		if (Time.time - lastCheckTime >= CheckInterval)
		{
			if(buffed)
			{
				if (buffCount - Time.time > -3)
					if (healbuff[3] > 0)
						this.TakeHeal2(maxHP * 0.05f * Time.deltaTime);
			}
			//이동공격-어택땅/유닛공격/패트롤중 공격 처리.
			if ((currentState == UnitState.AttackingwithMoving || currentState == UnitState.AttackingUnit) && target == null) // 1. target destroyed before arrive
				navagent.ResetPath();
			else if ((currentState == UnitState.AttackingwithMoving || currentState == UnitState.AttackingUnit) && target != null) // 2. move
			{
				if (currentState == UnitState.AttackingwithMoving)
					if (Vector3.Distance(transform.position, target.position) <= range + 0.005f)
					{
						Attack();
						animator.StopRun();
					}
					else
					{
						navagent.SetDestination(target.transform.position);
						animator.PlayRun();
					}

				// float distance = Vector3.Distance(transform.position, target.position);
				// if (distance <= range)
				// {
				// 	navagent.ResetPath();
				// 	animator1.SetBool("isAttacking", true);
				// 	animator1.SetBool("isMoving", false);
				// 	//target.GetComponent<Unit>()?.TakeDamage(this,attackPower); //타겟이 데미지를 계산하는방식.
				// 	target.GetComponent<Health>().currentHealth-=attackPower; //새로운 방식.
				// }
				// else
				// {
				// 	if (navagent.destination != target.position)
				// 	{
				// 	}
				// 	animator1.SetBool("isAttacking", false);
				// 	animator1.SetBool("isMoving", true);
				// }
			}
			else if (currentState == UnitState.Patrol)
			{
				float squaredDistance = (transform.position - destination[patrolIndex]).sqrMagnitude;


				if (squaredDistance < squaredThreshold)
				{
					if (patrolIndex == 1)
						patrolIndex = 0;
					else
						patrolIndex = 1;
					navagent.SetDestination(destination[patrolIndex]);
				}
			}
			else if (currentState == UnitState.Attacking && target != null)
			{
				float distance = Vector3.Distance(transform.position, target.position);
				if (distance <= range + 0.005f)
				{
					animator.PlayAttack();
					float finalAP = attackPower;
					if (buffed && healbuff[1] > 0)
						finalAP *= (1.0f + healbuff[1] * 0.1f);//atk buff
					{

					}
					if ((id == 1))//궁수 카드 구현
					{
						if (cards.archCard[6] > 0 && target.GetComponent<Unit>().health / target.GetComponent<Unit>().maxHP < 0.5f)
						{
							finalAP *= 1 + (cards.archCard[6] * 0.25f);
						}

						// 카드 9: 같은 적 연속 공격 시 피해 증가 (최대 3회)
						if (cards.archCard[8] > 0 && lastAttackedTarget == target)
						{
							attackStack = Mathf.Min(attackStack + 1, 3);
							finalAP *= 1 + (attackStack * 0.15f * cards.archCard[8]);
						}
						else
						{
							attackStack = 0;
						}
						lastAttackedTarget = target;
					}
					navagent.ResetPath();
					//animator1.SetBool("isAttacking", true);
					//animator.PlayAttack();
					if (id == 2)//힐러 id 2였음.
					{
						target.GetComponent<Unit>()?.TakeHeal(healAmount, this); // 힐 구현.-아군 Unit스크립트 있으니 ㄱㅊ.
						if (healer[5] > 0)
							TakeHeal2(maxHP * 0.05f);
					}
					else//그외에는 공격 수행. 추가작업 x
					{

						//target.GetComponent<Unit>()?.TakeDamage(this, finalAP);
						target.GetComponent<Health>().TakeDamage(finalAP); //새로운 방식.
					}
				}
				else
				{
					Stop();
					currentState = UnitState.AttackingwithMoving;//if attacking distance not enough, switch state
				}
				/*else
				{
					/*Stop();
					Debug.Log(distance);
					Debug.Log("Enemy not exist");
				}*/
			}
			lastCheckTime = Time.time;
		}
	}

	public void MoveTo(Vector3 destination)
	{
		navagent.SetDestination(destination);
		//animator1.SetBool("isMoving", true);
		animator.PlayRun();
		currentState = UnitState.Moving;
	}

	public void AttackTarget(Transform newTarget)
	{
		target = newTarget;
	}

	public void Attack()
	{
		currentState = UnitState.Attacking;
	}

	public void Patrol(Vector3 pos)
	{
		currentState = UnitState.Patrol;
		//animator1.SetBool("isAttacking", false);
		//animator1.SetBool("isMoving", true);
		animator.StopAttack();
		animator.PlayRun();

		target = null;
		navagent.SetDestination(pos);
		destination[0] = transform.position;
		destination[1] = pos;
		patrolIndex = 1;
	}
	public void ResumePatrol()
	{
		navagent.SetDestination(destination[patrolIndex]);
	}

	public void AttackWithMove()
	{
		currentState = UnitState.AttackingwithMoving;
	}

	public void AttackWIthTag(int target)
	{
		currentState = UnitState.AttackingUnit;
		targetUnit = target;
	}

	public void AttackWIthLPatrol()
	{
		currentState = UnitState.AttackingPatrol;
	}

	public void Stop()
	{
		navagent.ResetPath();
		//animator1.SetBool("isMoving", false);
		//animator1.SetBool("isAttacking", false);
		animator.PlayRun();
		animator.StopAttack();
		currentState = UnitState.Idle;
	}


	public void TakeDamage(Unit u, float damage)
	{
		float reducedDamage = damage;
		if (id == 0)
		{
			// 🛡 카드 1: 체력 20% 잃을 때마다 방어력 증가
			if (cards.warCard[0] > 0)
			{
				int hpLossRatio = (int)(maxHP - health) * 5 / (int)maxHP;
				float defenseBonus = cards.warCard[0] * hpLossRatio;
				reducedDamage *= 1 - Mathf.Clamp(defenseBonus, 0, 0.5f);
			}

			// 🛡 카드 2: HP 50% 미만일 때 실드 획득
			if (cards.warCard[1] > 0 && health / maxHP < 0.5f)
			{
				shield += maxHP * 0.2f * cards.warCard[1];
			}

			// 🛡 카드 5: 받는 피해 15% 반사
			if (u != null)
				if (cards.warCard[4] > 0)
				{
					//u.TakeDamage(null, damage * 0.15f * cards.warCard[4]); //상대도 Unit()이면 이방식 사용.
					u.gameObject.GetComponent<Health>().TakeDamage(damage * 0.15f * cards.warCard[4]); //새로운 방식.
				}
		}
		if (buffed && healbuff[0] > 0)
			reducedDamage -= healbuff[0] * defense;//defense buff
		if (shield > 0)
		{
			temp = shield;
			shield -= reducedDamage;
			reducedDamage -= temp;
		}
		if (damage < 0)
			return;
		health -= reducedDamage;
		if (health < 0)
			Die();
	}


	public void TakeHeal(float damage, Unit u)
	{
		if (buffed && u.healer[8]>0)//healer card9(9-1=8)
			if(health / maxHP < 0.2)
				damage *= (1.0f+0.5f*u.healer[8]);
		if (maxHP < health + damage)
			health = maxHP;
		else
			health = health + damage;
		for(int i = 0; i < 3; i++)
		{
			if (u.healer[i] > 0)
				healbuff[i] = u.healer[i];
		}
		buffed = true;
		buffCount = Time.time;
	}

	public void TakeHeal2(float damage)
	{
		if (maxHP < health + damage)
			health = maxHP;
		else
			health = health + damage;
	}


	private void Die()
	{
		//죽는 애니메이터+딜레이 필요함.
		animator.PlayDie();
		UnitManager.GetComponent<UnitManager2>().DestroyUnit(this,isEnemy, id);
	}
	//유닛 아이디로.

}
