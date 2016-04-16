﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColliderToMesh))]
public class ColliderToMeshEditor : Editor {

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Update"))
		{
			((ColliderToMesh)target).CreateMesh();
		}
	}
}
