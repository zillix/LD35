using UnityEngine;
using System.Collections.Generic;

/*
	See: https://github.com/AdriaandeJongh/UnityTools/blob/master/ColliderToMesh.cs
*/


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class ColliderToMesh : MonoBehaviour
{
	
	void Start()
	{
		CreateMesh();
	}

	public void CreateMesh()
	{
		Collider2D collider = gameObject.GetComponent(typeof(Collider2D)) as Collider2D;
		Vector2[] path;
		if (collider is EdgeCollider2D)
		{
			path = (collider as EdgeCollider2D).points;
		}
		else if (collider is PolygonCollider2D)
		{
			//path = (collider as PolygonCollider2D).GetPath(0);
			path = (collider as PolygonCollider2D).points;
		}
		else
		{
			Debug.LogError("Failed to find supported collider");
			return;
		}

		MeshFilter mf = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		Triangulator tr = new Triangulator(path);

		int[] indices = tr.Triangulate();
		Vector3[] vertices = new Vector3[path.Length];
		Vector2[] uvs = new Vector2[path.Length];

		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = new Vector3(path[i].x, path[i].y, 0);
			uvs[i] = new Vector2(path[i].x, path[i].y);
		}

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.uv = uvs;
		mesh.RecalculateNormals();

		/*if (invertNormals)
		{
			Vector3[] normals = mesh.normals;
			for (int i = 0; i < normals.Length; ++i)
			{
				Vector3 normal = normals[i];
				normal *= -1;
				normals[i] = normal;
			}

			mesh.SetNormals(new List<Vector3>(normals));
		}*/


		mesh.RecalculateBounds();

		

		mf.mesh = mesh;
	}
}
