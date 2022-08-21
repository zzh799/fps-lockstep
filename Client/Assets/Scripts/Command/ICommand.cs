using System.Collections.Generic;

namespace DefaultNamespace
{
    public interface ICommand
    {
        void Do(IEntity entity);
        void Undo(IEntity entity);
    }
}