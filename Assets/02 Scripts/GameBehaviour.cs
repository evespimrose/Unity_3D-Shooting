using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public float Health;
    public float AttackRange;
    public float AttackCoolTimer;
    public int Damage;
    public float MoveSpeed;
}

public class GameBehaviour : MonoBehaviour
{
    public float GetDistance(Vector3 a, Vector3 b)
    {
        return float.MaxValue;
    }
}
