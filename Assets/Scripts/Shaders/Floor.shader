// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Floor" {
	Properties {
		_Color ("Color (A=Opacity)", Color) = (1,1,1,1)
		_MainTex ("Texture Floor Array", 2DArray) = "white" {}
		_BumpMap ("BumpMap Array", 2DArray) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0                                                                                                   
	}
	SubShader {
		Tags {"RenderType"="Opaque" }
		LOD 100

		//ZWrite Off
		//Blend SrcAlpha OneMinusSrcAlpha //Alpha blending


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows  vertex:vert //alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.5

		UNITY_DECLARE_TEX2DARRAY(_MainTex);
		UNITY_DECLARE_TEX2DARRAY(_BumpMap);


		struct Input {
			float4 color : COLOR;
			float3 worldPos;
			float3 floor;
		};

		void vert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.floor = v.texcoord2.xyz;
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float4 GetTerrainColor(Input IN, int index)
		{
			float3 uvw = float3(IN.worldPos.xz * 0.1,IN.floor[index]);
			float4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, uvw);
			return c * IN.color[index];
		}

		float4 GetTerrainBump(Input IN, int index)
		{
			float3 uvw = float3(IN.worldPos.xz * 0.1,IN.floor[index]);
			float4 b = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, uvw);
			return b * IN.color[index];
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 uv = IN.worldPos.xz * 0.1;
			fixed4 c = GetTerrainColor(IN, 0) +
						GetTerrainColor(IN, 1) +
						GetTerrainColor(IN,2);
			fixed4 b = GetTerrainBump(IN, 0) +
						GetTerrainBump(IN, 1) +
						GetTerrainBump(IN,2);
			o.Albedo = c.rgb * _Color;
			o.Normal = b;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
