using UnityEngine.Events;

public interface IAttribute
{
	event UnityAction<string> onValueChanged;
	string ToString();
}