using UnityEngine;

namespace DefaultNamespace
{
    public class RotateCommand:ICommand
    {
        private readonly Quaternion new_rotation;
        private Quaternion old_rotation;

        public RotateCommand(Quaternion rotation)
        {
            this.new_rotation = rotation;
        }

        public void Do(IEntity entity)
        {
            this.old_rotation = entity.Rotation;
            entity.Rotation = new_rotation;
        }

        public void Undo(IEntity entity)
        {
            entity.Rotation = this.old_rotation;
        }
    }
}