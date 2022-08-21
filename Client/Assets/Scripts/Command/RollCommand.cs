namespace DefaultNamespace
{
    public class RollCommand:ICommand
    {

        public void Do(IEntity entity)
        {

        }
        public void Undo(IEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
