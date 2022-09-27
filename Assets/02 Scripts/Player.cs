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
    };
    public enum Weapons
    {
        Hammer,
        HandGun,
        SubMachineGun
    };

    public GameObject[] weapons;
    public bool[] hasWeapon;

    float hAxis;
    float vAxis;
    public float speed;

    bool wDown;
    bool jDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    public float jPower;

    public float health = 100;
    public float damage = 20;

    private float startHealth;

    private StateMachine<States, StateDriverUnity> fsm;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rb;

    Animator animator;

    GameObject NearObject;
    GameObject EquipWeapon;
    int equipWeaponIndex = -1;

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

        Interaction();

        Swap();
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
        iDown = Input.GetButton("Interaction");
        sDown1 = Input.GetButton("Swap1");
        sDown2 = Input.GetButton("Swap2");
        sDown3 = Input.GetButton("Swap3");

    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge)
            moveVec = dodgeVec;

        if (isDodge)
            moveVec = Vector3.zero;

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

    void Swap()
    {
        if (sDown1 && (!hasWeapon[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapon[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapon[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if (sDown1 || sDown2 || sDown3)
        {
            if(EquipWeapon)
                EquipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            EquipWeapon = weapons[weaponIndex];

            EquipWeapon.SetActive(true);

            animator.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if(iDown && NearObject != null)
        {
            if(NearObject.tag == "Weapon")
            {
                Item item = NearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;

                Destroy(NearObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            NearObject = other.gameObject;

        Debug.Log(NearObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            NearObject = null;
    }
}
