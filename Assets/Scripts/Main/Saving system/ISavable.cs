using System;

namespace SpaceAce.Main.Saving
{
    public interface ISavable : IEquatable<ISavable>
    {
        event EventHandler SavingRequested;
        
        string ID { get; }
        string SaveName { get; }

        string GetState();
        void SetState(string state);
    }
}