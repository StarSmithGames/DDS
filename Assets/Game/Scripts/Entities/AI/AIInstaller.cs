using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class AIInstaller : MonoInstaller
{
	[SerializeField] private AI ai;
	[Space]
	[SerializeField] protected Animator animator;
	[SerializeField] protected NavMeshAgent navMeshAgent;
	[SerializeField] protected CharacterController characterController;
	[Space]
	[SerializeField] protected FieldOfView fov;
	[Space]
	[SerializeField] private AIPersonality personality;
	[SerializeField] private AIWanderState.Settings wanderSettings;
	[SerializeField] private AIFollowState.Settings seekSettings;

	public override void InstallBindings()
	{
		Container.BindInstance(ai);
		Container.BindInstance(animator);
		Container.BindInstance(navMeshAgent);
		Container.BindInstance(characterController);
		Container.BindInstance(fov);

		Container.BindInstance(wanderSettings);
		Container.BindInstance(seekSettings);

		Container.BindInterfacesAndSelfTo<AIIdleState>().AsSingle();
		Container.BindInterfacesAndSelfTo<AIWanderState>().AsSingle();
		Container.BindInterfacesAndSelfTo<AIFollowState>().AsSingle();

		BindBehavior();
	}

	private void BindBehavior()
	{
		switch (personality.behavior)
		{
			case BehaviorType.Passive:
			{
				Container.BindInterfacesAndSelfTo<AIPassiveBehavior>().AsSingle();
				break;
			}
			case BehaviorType.Cautious:
			{
				Container.BindInterfacesAndSelfTo<AICautiousBehavior>().AsSingle();
				break;
			}
			case BehaviorType.Aggressive:
			{
				Container.BindInterfacesAndSelfTo<AIAggressiveBehavior>().AsSingle();
				break;
			}
			case BehaviorType.Companion:
			{
				Container.BindInterfacesAndSelfTo<AICompanionBehavior>().AsSingle();
				break;
			}
			case BehaviorType.Pet:
			{
				Container.BindInterfacesAndSelfTo<AIPetBehavior>().AsSingle();
				break;
			}
			default:
			{
				Container.BindInterfacesAndSelfTo<AIPassiveBehavior>().AsSingle();
				break;
			}
		}
	}
}


[System.Serializable]
public class AIPersonality
{
	[InfoBox("@Info")]
	public BehaviorType behavior = BehaviorType.Passive;
	public AIAligment aligment;
	public AIConfidence confidence;
	public AIFactionRelations relations;

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
	NPC = 0,

	Animal = 10,
	Predator = 11,
	Prey = 12,

}
public enum FactionRelations
{
	Enemy,
	Neutral,
	Friendly,
}