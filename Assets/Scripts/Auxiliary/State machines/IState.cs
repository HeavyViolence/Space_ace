namespace SpaceAce.Auxiliary.StateMachines
{
    public interface IState
    {
        void OnStateEnter();
        void OnStateUpdate();
        void OnStateFixedUpdate();
        void OnStateExit();
    }
}