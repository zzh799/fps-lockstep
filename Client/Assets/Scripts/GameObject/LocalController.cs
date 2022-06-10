using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

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
    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        direction = direction.normalized;
        transform.position += direction * Speed * Time.deltaTime;
        
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        Debug.DrawRay(mouseRay.origin,mouseRay.direction.normalized * 50);
        if (Physics.Raycast(mouseRay,out mouseCollision) )
        {
            Vector3 mouseCollisionPoint = mouseCollision.point;
            mouseCollisionPoint.y = 0;
            Entity.transform.LookAt(mouseCollisionPoint);
        }
        
        if (Animator != null)
        {
            if (direction.magnitude > 0)
            {
                Animator.SetFloat("speed", direction.magnitude);
                Animator.SetFloat("forward", direction.magnitude);
            }
            else
            {
                Animator.SetFloat("speed", 0);
                Animator.SetFloat("forward", 0);
            }
        }
        
        
    }
}