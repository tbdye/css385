using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(OtherCat))]
public class OtherCatEditor : Editor
{
	SerializedProperty fameRate;
	SerializedProperty timeBoostPerMember;
	SerializedProperty fameBoostPerMember;
	SerializedProperty interestBleedRate;
	SerializedProperty progressOnSuccessInPosse;
	SerializedProperty penaltyOnFailureInPosse;
	SerializedProperty angryDuration;
	SerializedProperty startingInterest;
	SerializedProperty isHuman;
	SerializedProperty progressOnSuccess;
	SerializedProperty penaltyOnFailure;
	SerializedProperty bailoutPromptWindow;
	SerializedProperty sequenceLengths;
	SerializedProperty perSequenceTimeLimit;
	SerializedProperty fameInputPool;
	SerializedProperty inputPool;
	

	void OnEnable()
	{
		fameRate = serializedObject.FindProperty("fameRate");
		timeBoostPerMember = serializedObject.FindProperty("timeBoostPerMember");
		fameBoostPerMember = serializedObject.FindProperty("fameBoostPerMember");
		interestBleedRate = serializedObject.FindProperty("interestBleedRate");
		progressOnSuccessInPosse = serializedObject.FindProperty("progressOnSuccessInPosse");
		penaltyOnFailureInPosse = serializedObject.FindProperty("penaltyOnFailureInPosse");
		angryDuration = serializedObject.FindProperty("angryDuration");
		startingInterest = serializedObject.FindProperty("startingInterest");
		isHuman = serializedObject.FindProperty("isHuman");
		progressOnSuccess = serializedObject.FindProperty("progressOnSuccess");
		penaltyOnFailure = serializedObject.FindProperty("penaltyOnFailure");
		bailoutPromptWindow = serializedObject.FindProperty("bailoutPromptWindow");
		sequenceLengths = serializedObject.FindProperty("sequenceLengths");
		perSequenceTimeLimit = serializedObject.FindProperty("perSequenceTimeLimit");
		inputPool = serializedObject.FindProperty("inputPool");
		fameInputPool = serializedObject.FindProperty("fameInputPool");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		CommonVariables();

		EditorGUILayout.PropertyField(isHuman, true);

		if (isHuman.boolValue)
		{
			GUILayout.Label("Fame sequence variables");

			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(fameRate, true);
			EditorGUILayout.PropertyField(timeBoostPerMember, true);
			EditorGUILayout.PropertyField(fameBoostPerMember, true);
			FameSequencePicker();
			EditorGUI.indentLevel--;
		}
		else
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(interestBleedRate, true);
			EditorGUILayout.PropertyField(progressOnSuccessInPosse, true);
			EditorGUILayout.PropertyField(penaltyOnFailureInPosse, true);
			EditorGUILayout.PropertyField(angryDuration, true);
			EditorGUI.indentLevel--;
		}

		serializedObject.ApplyModifiedProperties();
	}

	void CommonVariables()
	{
		EditorGUILayout.PropertyField(startingInterest, true);
		EditorGUILayout.PropertyField(progressOnSuccess, true);
		EditorGUILayout.PropertyField(penaltyOnFailure, true);
		EditorGUILayout.PropertyField(bailoutPromptWindow, true);
		EditorGUILayout.PropertyField(sequenceLengths, true);
		EditorGUILayout.PropertyField(perSequenceTimeLimit, true);


		perSequenceTimeLimit.floatValue = EditorGUILayout.FloatField("Time Limit Per Sequence", perSequenceTimeLimit.floatValue);


		EnumFlagHandler(inputPool, "Sequence Input Types");
	}
	void FameSequencePicker()
	{
		GUILayout.Label("Fame Input Types");
		int i = 1;
		EditorGUI.indentLevel++;
		foreach (var e in GetArrayElements(fameInputPool))
		{
			EnumFlagHandler(e, "Level " + i);

			i++;
		}
		EditorGUI.indentLevel--;
	}

	Dictionary<string, bool> foldouts;
	void EnumFlagHandler(SerializedProperty enumSP, string label)
	{
		string key = enumSP.propertyPath;
		if (foldouts == null)
			foldouts = new Dictionary<string, bool>();
		if (!foldouts.ContainsKey(key))
			foldouts.Add(key, true);

		int result = enumSP.intValue;
		foldouts[key] = EditorGUILayout.Foldout(foldouts[key], label);

		if (foldouts[key])
		{
			int i = 1;
			EditorGUI.indentLevel++;
			foreach (var e in Enum.GetNames(typeof(InputTypes)))
			{

				bool set = EditorGUILayout.Toggle(e, 
					((InputTypes)enumSP.intValue & (InputTypes)i) == (InputTypes)i);

				if(set)
					result |= i;
				else
					result &= ~i;
				
				i*=2;
			}
			EditorGUI.indentLevel--;
		}

		enumSP.intValue = result;
	}


	static IEnumerable<SerializedProperty> GetArrayElements(SerializedProperty sp)
	{
		int i = 0;
		while (i < sp.arraySize)
			yield return sp.GetArrayElementAtIndex(i++);

	}
}

