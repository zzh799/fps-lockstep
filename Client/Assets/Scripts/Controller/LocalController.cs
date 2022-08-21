using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Manager;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class LocalController : MonoBehaviour,IController
{
    public Animator Animator;
    public Transform Entity;

    public float Speed = 1;
    // Start is called before the first frame update
    public Entity model;
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
    void Awake()
    {
        model = new Entity()
        {
            Position = Entity.position,
            Rotation = Entity.rotation,
            controller = this
        };

    }
    // Update is called once per frame
    void LateUpdate()
    {
        //获取当前指令，生成命令
        MyInput input = InputManager.Instance.input;

        MoveCommand moveCommand = new MoveCommand(input.moveDirection, Speed);
        RotateCommand rotateCommand = new RotateCommand(input.faceDirection);
       
        model.Update(moveCommand);
        model.Update(rotateCommand);
        
        // if (Input.GetMouseButtonDown(1))
        // {
        //     Animator.SetTrigger(Roll);
        // }
        
        
        // if (Input.GetMouseButton(0))
        // {
        //     isShooting = true;
        //     Animator.SetBool(IsShooting, true);
        // }
        // else
        // {
        //     if (isShooting)
        //     {
        //         Animator.SetBool(IsShooting, false);
        //         isShooting = false;
        //     }
        // }

       
    }

    public void MoveTo(Vector3 pos)
    {
        Entity.transform.position = pos;
        if (Animator != null)
        {
            Vector3 direction = transform.position - pos;
            // 动画朝向：面朝方向与移动方向夹角
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

    public void RotateTo(Quaternion rotation)
    {
        Entity.transform.rotation = rotation;
    }
}