using UnityEngine;

namespace DefaultNamespace
{
    public interface IEntity
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
    }
}