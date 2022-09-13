using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

//ToDo : 상태머신 연결을 위해 Init, TakeHit 구현 필요
public class Player : MonoBehaviour     //Entity
{
    public enum States
    {
        Init,
        Countdown,
        Play,
        Win,
        Lose
    }

    float hAxis;
    float vAxis;
    public float speed;
    bool wDown;
    bool jDown;
    bool isJump;
    bool isDodge;
    public float jPower;

    public float health = 100;
    public float damage = 20;

    private float startHealth;

    private StateMachine<States, StateDriverUnity> fsm;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rb;

    Animator animator;

    void Awake()
    {
        isJump = false;
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        startHealth = health;
    }

    void Update()
    {
        GetInput();

        Move();

        Turn();

        Jump();

        Dodge();
    }

    void Init_Enter()
    {
        Debug.Log("Waiting for start button to be pressed");
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButton("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge)
            moveVec = dodgeVec;

        transform.position += moveVec * speed * Time.deltaTime * (wDown ? 0.3f : 1f);

        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            rb.AddForce(Vector3.up * jPower, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
        }
    }
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2;
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }
}
