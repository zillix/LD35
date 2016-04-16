using UnityEngine;
using System.Collections;

public class LayerUtil {

	public static string MOON = "Moon";
	public static string WOLF = "Wolf";
	public static string FOREST_GROUND = "ForestGround";
	public static string FOREST_BACKGROUND = "ForestBackground";

	public static int GetLayerMask(string layerName)
	{
		int layer = LayerMask.NameToLayer(layerName);
		return 1 << layer;
	}
}
