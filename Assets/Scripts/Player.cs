using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float hAxis;
    float vAxis;
    public float speed;
    bool walkDown;

    Vector3 moveVec;

    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
 
        transform.position += moveVec * speed * Time.deltaTime * (walkDown ? 0.3f : 1f);



        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", walkDown);
    }
}
