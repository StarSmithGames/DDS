using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(AIPersonality))]
public class AIPersonalityEditor : Editor
{
	private void OnSceneGUI()
	{
		var personality = (AIPersonality)target;

		var settings = serializedObject.FindProperty("wanderSettings").GetValue<AIWanderState.Settings>();
		var ai = serializedObject.FindProperty("ai").GetValue<AI>();

		Transform transform = personality.transform;

		Color color = Color.red;
		Handles.color = color;
		Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, ai.NavMeshAgent.stoppingDistance);

		color = Color.white;
		Handles.color = color;
		Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, settings.innerRadius);
		Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, settings.outerRadius);

		//Handles.color = Color.red;
		//Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, settings.awayInnerRadius);
		//Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, settings.awayOuterRadius);
	}
}