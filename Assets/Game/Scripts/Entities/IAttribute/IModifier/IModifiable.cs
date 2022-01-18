using System.Collections.Generic;
using UnityEngine.Events;

public interface IModifiable
{
	List<IModifier> Modifiers { get; } 

	float ModifyValue { get; }

	void AddModifier(IModifier modifier);
	void RemoveModifier(IModifier modifier);
}