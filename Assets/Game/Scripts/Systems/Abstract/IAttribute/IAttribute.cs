using UnityEngine.Events;

public interface IAttribute
{
	event UnityAction onAttributeChanged;
	string ToString();
}