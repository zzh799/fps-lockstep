using UnityEngine;

public interface IController
{
    public void MoveTo(Vector3 pos);
    public void RotateTo(Quaternion angle);
}