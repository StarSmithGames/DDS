using UnityEngine.Events;

public interface IStat  : IAttribute, IModifiable
{
    float CurrentValue { get; set; }

    float MaxBaseValue { get; }
    float MaxValue { get; }


    float PercentValue { get; }
}