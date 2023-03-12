using UnityEngine;

public interface ITrigger
{
    TriggerType triggerType { get; set; }

    bool Check(TriggerType type)
    {
        return triggerType == type;
    }
}

public enum TriggerType { PLAYER = 0, THROWABLE_OBJECT = 1, ALL = 2 }

public interface IThrowable
{
    void ThrowMe(Vector3 startThrowLocation, Vector3 throwDirection);
}

public interface IPickUpable
{
    void PickMeUp();
}