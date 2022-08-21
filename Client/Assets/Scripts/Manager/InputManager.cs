using System;
using UnityEngine;

namespace Manager
{
    public struct MyInput
    {
        public Quaternion faceDirection;
        public Vector3 moveDirection;
        public bool isShoot;
        public bool isRoll;
    }

    
    //每帧获取当前输入，放入当前指令中
    public class InputManager : MonoSingleton<InputManager>
    {
        public Transform entity;
        public MyInput input;

        override protected void Init()
        {
            input = new MyInput()
            {
                moveDirection = Vector3.one,
                faceDirection = new Quaternion(),
                isShoot = false,
                isRoll = false,
            };
        }

        private RaycastHit mouseCollision;
        void Update()
        {
            //移动
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            input.moveDirection = direction.normalized;
            
            //朝向
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(mouseRay.origin, mouseRay.direction.normalized * 50);
            if (Physics.Raycast(mouseRay, out mouseCollision))
            {
                Vector3 mouseCollisionPoint = mouseCollision.point;
                mouseCollisionPoint.y = entity.position.y;
                input.faceDirection = Quaternion.Euler(mouseCollisionPoint);
            }
            
            //翻滚
            input.isRoll = Input.GetMouseButtonDown(1);
            
            //射击
            input.isShoot = Input.GetMouseButton(0);
            
        }
    }
}
