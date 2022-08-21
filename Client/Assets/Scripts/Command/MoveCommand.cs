using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoveCommand:ICommand
    {
        private readonly Vector3 dir;
        private readonly float speed;

        public MoveCommand(Vector3 dir,float speed)
        {
            this.dir = dir;
            this.speed = speed;
        }

        public void Do(IEntity entity)
        {
            entity.Position = entity.Position + dir * speed * AppConst.FrameTimeInterval;
        }

        public void Undo(IEntity entity)
        {
            entity.Position = entity.Position - dir * speed * AppConst.FrameTimeInterval;
        }
    }
}