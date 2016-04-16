using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MoveTexture : MonoBehaviour {

	public Renderer rend;
	public Vector2 initialOffset;
	public int radius = 1;
	public float angle;
	public float angleSpeed = 200f;
	Vector3 offset = new Vector3(0.5f, 0.5f, 0);
	Vector3 tiling = new Vector3(1, 1, 1);


	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		initialOffset = rend.sharedMaterial.GetTextureOffset("_MainTex");
	}

	// Update is called once per frame
	[ExecuteInEditMode]
	public void Update () {
		//rend.sharedMaterial.SetTextureOffset("_MainTex", initialOffset + new Vector2(0, Mathf.Sin(Mathf.Rad2Deg * angle)));
		angle += .166f * angleSpeed;
		Quaternion quat = Quaternion.Euler(0, 0, angle);
		/*Matrix4x4 matrix1 = Matrix4x4.TRS(offset, Quaternion.identity, tiling);
		Matrix4x4 matrix2 = Matrix4x4.TRS(Vector3.zero, quat, tiling);
		Matrix4x4 matrix3 = Matrix4x4.TRS(-offset, Quaternion.identity, tiling);
		rend.sharedMaterial.SetMatrix("_Matrix", matrix1 * matrix2 * matrix3);*/
		Matrix4x4 matrix = Matrix4x4.TRS(Vector2.zero, quat, Vector3.one);
		//rend.material.SetMatrix("_Rotation", matrix);
	}
}
