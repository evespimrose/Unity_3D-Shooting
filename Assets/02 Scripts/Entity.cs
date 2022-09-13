using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public abstract class Entity : GameBehaviour
{
    [Header("스탯")] public Stat Stat;
    public Vector3 Position => transform.position;
    protected SpriteRenderer m_SpriteRenderer;
    protected Entity m_Target;
    protected bool m_Death;
    public bool Death => m_Death;
    protected virtual void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public abstract void TakeHit(int damage);

    public abstract void Init(int dataIndex);

}

public class Monster : Entity
{

    // 몬스터의 상태 정의: 열거형
    public enum States
    {
        Spawn,
        Idle,
        Run,
        Attack,
        Death,
    }

    // FSM 
    private StateMachine<States> FSM;

    protected override void Awake()
    {
        base.Awake();

        FSM = new StateMachine<States>(this);
        FSM = StateMachine<States>.Initialize(this);
        FSM.ChangeState(States.Idle);
    }

    public override void TakeHit(int damage)
    {
        
        if (m_Death) return;

        Stat.Health -= damage;

        //OnTakeHit(damage);

        if (Stat.Health <= 0)
        {
            // OnDeath(damage);
            FSM.ChangeState(States.Death, StateTransition.Overwrite);
        }
    }

    public override void Init(int monsterIndex)
    {
        // 외형 설정
        // 데이터 설정 (스탯...)

        m_Death = false;

        FSM.ChangeState(States.Spawn);
    }

    /**************************************** Spawn ************************************/
    protected virtual IEnumerator Spawn_Enter()
    {
        //var duration = PlayAnimationAndGetDuration("Spawn");

        yield return new WaitForSeconds(0); // duration

        //BattleManager.Instance.AddMonster(this);

        FSM.ChangeState(States.Idle);
    }

    /**************************************** Idle ************************************/
    protected virtual void Idle_Enter()
    {
        //PlayAnimation("Idle");
    }

    protected virtual void Idle_Update()
    {
        var distance = GetDistance(Position, m_Target.Position);

        if (distance <= Stat.AttackRange && Stat.AttackCoolTimer <= 0)
        {
            FSM.ChangeState(States.Attack);
        }

        if (distance > Stat.AttackRange)
        {
            FSM.ChangeState(States.Run);
        }
    }

    protected virtual void Idle_Exit()
    {
        Debug.Log(gameObject.name + ": Idle_Exit");
    }

    /**************************************** Run ************************************/
    protected virtual void Run_Enter()
    {
        //PlayAnimation("Run");
    }

    protected virtual void Run_Update()
    {
        if (GetDistance(Position, m_Target.Position) <= Stat.AttackRange)
        {
            FSM.ChangeState(States.Attack);
        }

        Vector2.MoveTowards(Position, m_Target.Position, Time.deltaTime * Stat.MoveSpeed);
    }

    protected virtual void Run_Exit()
    {
        Debug.Log(gameObject.name + ": Run_Exit");
    }


    /**************************************** Attack ************************************/
    protected virtual IEnumerator Attack_Enter()
    {
        //var duration = PlayAnimationAndGetDuration("Attack");

        var halfWait = new WaitForSeconds(1 / 2f);     // duration

        yield return halfWait;

        m_Target.TakeHit(Stat.Damage);

        yield return halfWait;

        FSM.ChangeState(States.Idle);
    }



    /**************************************** Death ************************************/
    protected virtual IEnumerator Death_Enter()
    {
        m_Death = true;

        //var duration = PlayAnimationAndGetDuration("Death");

        yield return new WaitForSeconds(1);     //duration

        //BattleManager.Instance.RemoveMonster(this);

        //SafeSetActive(false);
    }
}