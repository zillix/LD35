Shader "Test" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white"
	}
	SubShader{
		Pass{
			SetTexture[_MainTex]{
				Matrix[_Rotation]
			}
		}
	}
}