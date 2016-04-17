using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColliderToMesh))]
public class ColliderToMeshEditor : Editor {

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Update"))
		{
			((ColliderToMesh)target).CreateMesh();
		}
		ColliderToMesh mesh = target as ColliderToMesh;
		mesh.Reverse = EditorGUILayout.Toggle("Reverse:", mesh.Reverse);
		mesh.ReverseNormals = EditorGUILayout.Toggle("ReverseNormals:", mesh.ReverseNormals);
	}
}
