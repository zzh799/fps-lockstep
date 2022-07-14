using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class LocalController : MonoBehaviour
{
    public Animator Animator;
    public Transform Entity;

    public float Speed = 1;
    // Start is called before the first frame update

    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
    }

    private RaycastHit mouseCollision;

    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int Shoot = Animator.StringToHash("shoot");
    private static readonly int ShootEnd = Animator.StringToHash("shootEnd");
    private bool isShooting;
    private static readonly int IsShooting = Animator.StringToHash("isShooting");
    private static readonly int Roll = Animator.StringToHash("roll");

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        direction = direction.normalized;
        transform.position += direction * Speed * Time.deltaTime;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(mouseRay.origin, mouseRay.direction.normalized * 50);
        if (Physics.Raycast(mouseRay, out mouseCollision))
        {
            Vector3 mouseCollisionPoint = mouseCollision.point;
            mouseCollisionPoint.y = Entity.position.y;
            Entity.transform.LookAt(mouseCollisionPoint);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Animator.SetTrigger(Roll);
        }
        
        
        if (Input.GetMouseButton(0))
        {
            isShooting = true;
            Animator.SetBool(IsShooting, true);
        }
        else
        {
            if (isShooting)
            {
                Animator.SetBool(IsShooting, false);
                isShooting = false;
            }
        }

        if (Animator != null)
        {
            //朝向为面朝方向与移动方向夹角
            Vector3 angleAxis = Quaternion.AngleAxis(-Entity.rotation.eulerAngles.y, Vector3.up) * direction;
            if (direction.magnitude > 0)
            {
                Animator.SetFloat(X, angleAxis.x);
                Animator.SetFloat(Y, angleAxis.z);
                Animator.SetBool(IsRunning, true);
            }
            else
            {
                Animator.SetBool(IsRunning, false);
            }
        }
    }
}