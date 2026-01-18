using System;

[Serializable]
public class DB
{
	public int unitID;         // 유닛을 구분하는 ID
	public string unitName;
	public float attackPower;     // 공격력
	public float attackSpeed;     // 공격속도
	public float health;          // 체력
	public float defense;         // 방어력
	public float criticalChance;  // 치명타 확률 (0~1)
	public float criticalDamage;  // 치명타 피해 (배율)
	public float moveSpeed;       // 이동 속도
	public float attackRange;     // 사정거리
}//스텟 찍을때는 이DB
[Serializable]
public class Card
{
	public int[] magCard= new int[9];
	public int[] healCard = new int[9];
	public int[] warCard = new int[9];
	public int[] archCard = new int[9];
}//카드 번호별 갯수