using UnityEngine;
using System.Collections;

public class GizmoUtil
{
	public static void GizmosDrawArrow(Vector3 start, Vector3 end, Color color)
	{
		float arrowheadAngle = 45f;
		float arrowheadLength = 1f;

		Gizmos.color = color;

		// Main line
		Gizmos.DrawLine(start, end);

		//arrowhead lines
		Vector3 reverse = start - end;
		Vector3 rotate1 = (Quaternion.Euler(0, 0, arrowheadAngle) * reverse).normalized;
		Vector3 rotate2 = (Quaternion.Euler(0, 0, -arrowheadAngle) * reverse).normalized;

		Gizmos.DrawLine(end, end + rotate1 * arrowheadLength);
		Gizmos.DrawLine(end, end + rotate2 * arrowheadLength);
	}
}
