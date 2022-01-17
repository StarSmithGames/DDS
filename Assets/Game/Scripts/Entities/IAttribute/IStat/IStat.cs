using UnityEngine.Events;

public interface IStat
{
	event UnityAction<float> onCurrentValueChanged;

    float CurrentValue { get; set; }

    float MaxBaseValue { get; }
    float MaxValue { get; }


    float PercentValue { get; }
}