using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class AIPersonality : MonoBehaviour
{
	[SerializeField] private AI ai;
	[SerializeField] private AIBehavior behavior;
	[SerializeField] private AIAligment aligment;
	[SerializeField] private AIConfidence confidence;
	[SerializeField] private AIFactionRelations relations;
	[Space]
	[SerializeField] private AIWanderState.Settings wanderSettings;
	//SeekFlee
	public IFSM FSM => fsm;
	private IFSM fsm;

	private AIWanderState wanderState;

	private void Awake()
	{
		fsm = new AIBehaviorFSM(ai);

		wanderState = new AIWanderState(fsm, ai, wanderSettings);

		fsm.SetState(wanderState);
	}
}

[System.Serializable]
public class AIBehavior
{
	[InfoBox("@Info")]
	public BehaviorType behavior = BehaviorType.Passive;
	
	private string Info
	{
		get
		{
			switch (behavior)
			{
				case BehaviorType.Passive:
				{
					return "��������� �� �� ����� ���������. ��� ������ ����� ������� ������." +
						"\n���� �� ��� �������, ��� ����� ����������� � ������������ �� ����� ������� �����������, �� ���� ���� ��������������, ���� ������.";
				}
				case BehaviorType.Cautious:
				{
					return "���������� �� ���� ������, ���� ����� ����������� ��������������, � ����������� �� ������ �Confidence�." +
						"\n��������������� �� ����� ������������� ����, ������ ��� ��������� �� ����." +
						"\n�� ��������� ���������������, ���� ��� ������� ����������� ���������� �� �Brave� ��� ����.";
				}
				case BehaviorType.Aggressive:
				{
					return "����������� �� ����� ��������� ����� ����, ������� �������� � ������� �� ������������.";
				}
				case BehaviorType.Companion:
				{
					return "��-��������� ����� ��������� ������ ���� � �������� �� ���������." +
						"\n���� �� ����� ����������� ���� ��� ����������, ����� ��������.";
				}
				case BehaviorType.Pet:
				{
					return "AI ������� ����� ��������� ������ ���� ������." +
						"\n��� �� ����� �������, �������� � ��� ��� ����������� �������." +
						"\n��� ������ � ������������� �����.";
				}
			}
			return "";
		}
	}
}
[System.Serializable]
public class AIAligment
{
	[MinValue(-1), MaxValue(1)]
	public Vector2 aligment = Vector2.zero;

	//	[OnInspectorGUI]
	//	private void ShowImage()
	//	{

	//		//int selGridInt = 0;
	//		//string[] selStrings = { "radio1", "radio2", "radio3", "radio1", "radio2", "radio3", "radio1", "radio2", "radio3", "radio1", "radio2", "radio3" };
	//		//selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 3);

	//		//var texture = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Game/Editor/Alignment.png");
	//		//var content = new GUIContent(texture, "������������");
	//		//GUILayout.Label(content, GUILayout.Width(300), GUILayout.Height(300));
	//	}
}
[System.Serializable]
public class AIConfidence
{
	public ConfidenceLevel confidenceLevel = ConfidenceLevel.Coward;
}

public class AIFactionRelations
{
	
}

public enum BehaviorType
{
	Passive = 0,
	Cautious = 1,
	Aggressive = 2,
	Companion = 3,

	Pet = 99,
}

public enum ConfidenceLevel
{
	Coward,
	Brave,
	Foolhardy,
}



//public enum PassiveAIType
//{
//	/// <summary>
//	/// ��������� ��������� �� ����� �������� � ������������ �� ������ ����������� ���������, �� ������� ������ ��� ���������.
//	/// </summary>
//	Coward,
//	/// <summary>
//	/// ������� ��������� �� ����� �������� � ������������ �� ������ ����������� ���������, �� ������� ������ �����, ����� ��� �������.
//	/// �� ���������� �������, ��� ������ ��� �������� ��������� �������������� ��������.
//	/// </summary>
//	Brave,
//	/// <summary>
//	/// ������������ ��������� �� ����� �������� � ������������ �� ������ ����������� ���������, �� ������� ������ �����, ����� ��� �������.
//	/// �� ������� �� ������ � ����� ���������� ���������, ���� �� �������� ���� ��� �� ������ �� ��.
//	/// </summary>
//	Foolhardy,
//}

//public enum CautiousAIType
//{
//	/// <summary>
//	/// ��������� ���������� �� ������, ����� �������� ����.
//	/// </summary>
//	Coward,
//	/// <summary>
//	/// ������� ���������� �� ������ ���������������, ����� ���� ������ � ������ �� ������������.
//	/// ���� ���� �� ������� ���� ������ �� ����, ��� ����� ���������� �� ��������������� �������, �� ������� ����.
//	/// ��� ���������� �������, ��� ������ ��� �������� ��������� �������������� ���� ��������.
//	/// </summary>
//	Brave,
//	/// <summary>
//	/// ������������ ���������� �� ������ ���������������, ����� ���� ������ � ������ �� ������������.
//	/// ���� ���� �� ������� ���� ������ �� ����, ��� ����� ���������� �� ��������������� �������, �� ������� ����.
//	/// ���� �� ������� �� ������ � ��������� ���������, ���� �� �������� ��� ���� �� ������ �� ��.
//	/// </summary>
//	Foolhardy,
//}

//public enum AggressiveAIType
//{
//	/// <summary>
//	/// ����������� �� �� ����� ���� ���������� �� ����.
//	/// �� � ���� ���������� ����� ������������� ���������� �� �Brave� ��� �������.
//	/// </summary>
//	Coward,
//	/// <summary>
//	/// ������� ����������� �� ����� ��������� � ����� ����� ��� �����������, �� ���������� �������, ��� ������ �������� ��������� �������������� ��������.
//	/// </summary>
//	Brave,
//	/// <summary>
//	/// ������������ ����������� �� ����� ��������� � ����� ����� ��� ����������� � ������� �� ������.
//	/// �� ����� ���������� ���������, ���� �� �������� ��� ���� �� ������ �� ��.
//	/// </summary>
//	Foolhardy,
//}

[System.Flags]
public enum Faction
{
	NPC			= 0,

	Animal		= 10,
	Predator	= 11,
	Prey		= 12,

}
public enum FactionRelations
{
	Enemy,
	Neutral,
	Friendly,
}
