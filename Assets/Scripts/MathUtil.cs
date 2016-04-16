using UnityEngine;
using System.Collections;

public class MathUtil {

	public static Vector3 AngleToVector(float degrees)
	{
		return new Vector3(Mathf.Cos(Mathf.Deg2Rad * degrees),
			Mathf.Sin(Mathf.Deg2Rad * degrees)).normalized;
	}

	public static float VectorToAngle(ref Vector3 vector)
	{
		return Mathf.Rad2Deg * Mathf.Atan2(vector.y, vector.x);
	}
}
