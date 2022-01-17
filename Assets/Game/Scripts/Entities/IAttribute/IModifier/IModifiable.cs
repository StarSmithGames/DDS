using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifiable
{
	List<IModifier> Modifiers { get; } 

	void AddModifier(IModifier modifier);
	void RemoveModifier(IModifier modifier);
}