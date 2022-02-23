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
					return "Пассивный ИИ не будет атаковать. Они просто будут бродить вокруг." +
						"\nЕсли на них нападут, они будут реагировать в соответствии со своим уровнем уверенности, то есть либо сопротивляться, либо бежать.";
				}
				case BehaviorType.Cautious:
				{
					return "Осторожный ИИ либо убежит, либо будет действовать территориально, в зависимости от уровня «Confidence»." +
						"\nТерриториальный ИИ будет предупреждать цели, прежде чем атаковать их цель." +
						"\nИИ считается территориальным, если его уровень уверенности установлен на «Brave» или выше.";
				}
				case BehaviorType.Aggressive:
				{
					return "Агрессивный ИИ будет атаковать любую цель, которая окажется в радиусе их срабатывания.";
				}
				case BehaviorType.Companion:
				{
					return "ИИ-компаньон будет следовать вокруг цели и помогать ей сражаться." +
						"\nПока не будет установлена цель его следования, будет блуждать.";
				}
				case BehaviorType.Pet:
				{
					return "AI питомца будет следовать вокруг цели игрока." +
						"\nОни не будут драться, вступать в бой или становиться мишенью." +
						"\nОни просто в косметических целях.";
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
	//		//var content = new GUIContent(texture, "Мировозрение");
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
//	/// Трусливый пассивный ИИ будет блуждать в соответствии со своими настройками блуждания, но убегает только при нападении.
//	/// </summary>
//	Coward,
//	/// <summary>
//	/// Храбрый пассивный ИИ будет блуждать в соответствии со своими настройками блуждания, но атакует только тогда, когда его атакуют.
//	/// Он попытается сбежать, как только его здоровье достигнет установленного процента.
//	/// </summary>
//	Brave,
//	/// <summary>
//	/// Безрассудный пассивный ИИ будет блуждать в соответствии со своими настройками блуждания, но атакует только тогда, когда его атакуют.
//	/// Он никогда не убежит и будет продолжать сражаться, пока не погибнет цель или не сбежит от ИИ.
//	/// </summary>
//	Foolhardy,
//}

//public enum CautiousAIType
//{
//	/// <summary>
//	/// Трусливый осторожный ИИ убежит, когда встретит цель.
//	/// </summary>
//	Coward,
//	/// <summary>
//	/// Храбрый осторожный ИИ станет территориальным, когда цель войдет в радиус их срабатывания.
//	/// Если цель не покинет свой радиус до того, как будут достигнуты ее территориальные секунды, ИИ атакует цель.
//	/// Они попытаются сбежать, как только его здоровье достигнет установленного вами процента.
//	/// </summary>
//	Brave,
//	/// <summary>
//	/// Безрассудный Осторожный ИИ станет территориальным, когда цель войдет в радиус их срабатывания.
//	/// Если цель не покинет свой радиус до того, как будут достигнуты ее территориальные секунды, ИИ атакует цель.
//	/// Этот ИИ никогда не убежит и продолжит сражаться, пока не погибнет или цель не сбежит от ИИ.
//	/// </summary>
//	Foolhardy,
//}

//public enum AggressiveAIType
//{
//	/// <summary>
//	/// Агрессивный ИИ не может быть установлен на Трус.
//	/// ИИ с этой настройкой будет автоматически установлен на «Brave» при запуске.
//	/// </summary>
//	Coward,
//	/// <summary>
//	/// Храбрый агрессивный ИИ будет сражаться с любой целью при обнаружении, но попытается сбежать, как только здоровье достигнет установленного процента.
//	/// </summary>
//	Brave,
//	/// <summary>
//	/// Безрассудный агрессивный ИИ будет сражаться с любой целью при обнаружении и никогда не убежит.
//	/// Он будет продолжать сражаться, пока не погибнет или цель не сбежит от ИИ.
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