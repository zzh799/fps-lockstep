using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

namespace DefaultNamespace
{
    public class Entity : IEntity
    {
        public IController controller { get; set; }

        private Vector3 position = Vector3.zero;
        private Quaternion rotation = new Quaternion();

        public Vector3 Position
        {
            get => this.position;
            set => this.position = value;
        }
        public Quaternion Rotation
        {
            get => this.rotation;
            set => this.rotation = value;
        }

        public void Update(ICommand command)
        {
            command.Do(this);

            if (this.controller != null)
            {
                this.controller.MoveTo(position);
                this.controller.RotateTo(rotation);
            }
        }

    }


}
