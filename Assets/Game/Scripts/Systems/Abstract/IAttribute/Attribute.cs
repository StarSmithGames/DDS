using System.Collections.Generic;
using UnityEngine.Events;

public abstract class Attribute : IAttribute
{
	public event UnityAction onAttributeChanged;

	public override abstract string ToString();

	protected virtual void ValueChanged()
	{
		onAttributeChanged?.Invoke();
	}
}
public abstract class AttributeModifiable : Attribute, IModifiable
{
	public List<IModifier> Modifiers { get; private set; }

	public AttributeModifiable()
	{
		Modifiers = new List<IModifier>();
	}

	public float ModifyValue
	{
		get
		{
			float mod = 0;
			for (int i = 0; i < Modifiers.Count; i++)
			{
				mod += Modifiers[i].Value;
			}
			return mod;
		}
	}

	public void AddModifier(IModifier modifier)
	{
		if (!Modifiers.Contains(modifier))
		{
			Modifiers.Add(modifier);
			ValueChanged();
		}
	}
	public void RemoveModifier(IModifier modifier)
	{
		if (Modifiers.Contains(modifier))
		{
			Modifiers.Remove(modifier);
			ValueChanged();
		}
	}
}