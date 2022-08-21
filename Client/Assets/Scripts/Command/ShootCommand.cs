namespace DefaultNamespace
{
    public class ShootCommand:ICommand
    {
        public void Do(IEntity entity)
        {
            throw new System.NotImplementedException();
        }
        public void Undo(IEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
