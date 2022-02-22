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
