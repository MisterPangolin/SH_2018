using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(HealthParameter))]
public class HealthParameterEditor : Editor {

	private new SerializedProperty name;
	private SerializedProperty description;
	private SerializedProperty indications;
	private SerializedProperty isABool;
	private SerializedProperty daysRate;

	private SerializedProperty measure,decimals,minValue,maxValue;
	private SerializedProperty randomLaw, variationValue;
	private SerializedProperty targetValue, criticalValue;

	private void OnEnable()
	{
		name = serializedObject.FindProperty("name");
		description = serializedObject.FindProperty("description");
		indications = serializedObject.FindProperty("indications");

		isABool = serializedObject.FindProperty("isABool");
		measure = serializedObject.FindProperty("measure");
		decimals = serializedObject.FindProperty("decimals");
		minValue = serializedObject.FindProperty("minValue");
		maxValue = serializedObject.FindProperty("maxValue");
		daysRate = serializedObject.FindProperty("daysRate");

		randomLaw = serializedObject.FindProperty("randomLaw");
		variationValue = serializedObject.FindProperty("variationValue");
		targetValue = serializedObject.FindProperty("targetValue");
		criticalValue = serializedObject.FindProperty("criticalValue");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.UpdateIfRequiredOrScript();

		//champs de texte
		EditorGUILayout.PropertyField(name, new GUIContent("Nom"));
		EditorGUILayout.PropertyField(description, new GUIContent("Description"));
		EditorGUILayout.PropertyField(indications, new GUIContent("Indications"));
		EditorGUILayout.Space();

		//mesure
		EditorGUILayout.PropertyField(isABool, new GUIContent("booleen"));

		if (!isABool.boolValue)
		{
			EditorGUILayout.PropertyField(measure, new GUIContent("measure"));
			EditorGUILayout.PropertyField(decimals, new GUIContent("Nombre de décimales"));
			EditorGUILayout.PropertyField(minValue, new GUIContent("Valeur minimale"));
			EditorGUILayout.PropertyField(maxValue, new GUIContent("Valeur maximale"));

			//loi des données
			EditorGUILayout.PropertyField(randomLaw, new GUIContent("loi de données"));

			if (randomLaw.enumValueIndex != (int)HealthParameter.RandowLaw.pureRandom)
			{
				EditorGUILayout.PropertyField(variationValue, new GUIContent("Variation"));
			}

			if (randomLaw.enumValueIndex == (int)HealthParameter.RandowLaw.increasing)
			{
				EditorGUILayout.PropertyField(targetValue, new GUIContent("Valeur visée"));
				EditorGUILayout.PropertyField(criticalValue, new GUIContent("Valeur critique"));
			}
		}
		else
		{
			EditorGUILayout.PropertyField(daysRate, new GUIContent("fréquence journalière"));
		}

		serializedObject.ApplyModifiedProperties();

	}
}
