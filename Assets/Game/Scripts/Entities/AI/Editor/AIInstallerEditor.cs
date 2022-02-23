using UnityEditor;

using UnityEngine;

using Sirenix.OdinInspector.Editor;
using UnityEngine.AI;

[CustomEditor(typeof(AIInstaller), true)]
public class AIInstallerEditor : OdinEditor
{
	private void OnSceneGUI()
	{
		var installer = (AIInstaller)target;

		var ai = serializedObject.FindProperty("ai")?.GetValue<AI>();
		var navMeshAgent = serializedObject.FindProperty("navMeshAgent")?.GetValue<NavMeshAgent>();
		var personality = serializedObject.FindProperty("personality").GetValue<AIPersonality>();

		var wanderSettings = serializedObject.FindProperty("wanderSettings").GetValue<AIWanderState.Settings>();

		if (ai == null) return;
		if (navMeshAgent == null) return;

		Transform transform = ai.transform;

		Color color = !wanderSettings.useNavMeshStoppingDistance ? Color.red : Color.magenta;
		Handles.color = color;

		Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, wanderSettings.useNavMeshStoppingDistance ? navMeshAgent.stoppingDistance : wanderSettings.stoppingDistance);
		if (navMeshAgent.destination != Vector3.zero)
		{
			Handles.DrawSolidDisc(navMeshAgent.destination, Vector3.up, 0.25f);
			Handles.DrawWireArc(navMeshAgent.destination, Vector3.up, Vector3.forward, 360f, wanderSettings.useNavMeshStoppingDistance ? navMeshAgent.stoppingDistance : wanderSettings.stoppingDistance);
		}

		Vector3[] corners = navMeshAgent.path.corners;

		for (int i = 0; i < corners.Length - 1; i++)
		{
			Handles.DrawLine(corners[i], corners[i + 1]);
		}

		color = Color.white;
		Handles.color = color;
		Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, wanderSettings.innerRadius);
		Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, wanderSettings.outerRadius);


		var seekSettings = serializedObject.FindProperty("seekSettings").GetValue<AIFollowState.Settings>();
		color = !wanderSettings.useNavMeshStoppingDistance ? Color.red : Color.magenta;
		Handles.color = color;


		//
		//Handles.color = Color.red;
		//Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, settings.awayInnerRadius);
		//Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, settings.awayOuterRadius);
	}
}