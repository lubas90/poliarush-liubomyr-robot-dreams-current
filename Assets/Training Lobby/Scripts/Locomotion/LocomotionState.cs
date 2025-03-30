namespace StateMachineSystem.Locomotion
{
    public enum LocomotionState : byte
    {
        None = 0,
        Idle = 1,
        Movement = 2,
        Jump = 3,
        Fall = 4,
        
        NullState = 255
    }
}