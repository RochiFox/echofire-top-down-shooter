using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Idle info")] public float idleTime;

    public EnemyStateMachine StateMachine { get; private set; }

    protected virtual void Awake()
    {
        StateMachine = new EnemyStateMachine();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }
}