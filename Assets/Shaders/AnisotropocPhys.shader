Shader "Custom/AnisotropicPhysicalShader" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
		_SpecularTex("Specular (R) Gloss (G) Anisotropic Mask (B)", 2D) = "gray" {}
		_BumpMap("Normal (Normal)", 2D) = "bump" {}
		_AnisoMaskTex("Anisotropic Mask", 2D) = "white" {}
		_AnisoDirWhite("Default Anisotropic Direction", Vector) = (0.57735026919,0.57735026919,0.57735026919)
		_AnisoDirBlack("Masked Anisotropic Direction", Vector) = (0.57735026919,0.57735026919,0.57735026919)
		_IndexOfRefraction("Index of Refraction", Float) = 1.1978
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		Pass{
			// light direction in _WorldSpaceLightPos0
			// color in _LightColor0
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" // UnityObjectToWorldNormal
			#include "UnityLightingCommon.cginc" // _LightColor0

			struct v2f {
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float3 viewPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float3 tangent : TEXCOORD2;
				float3 bitangent : TEXCOORD3;
			};


		v2f vert(appdata_base v) {
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			o.viewPos = UnityObjectToViewPos(v.vertex);
			o.normal = mul(UNITY_MATRIX_IT_MV, float4(v.normal,0)).xyz;
		
			return o;
		}


		sampler2D _MainTex;
		sampler2D _SpecularTex;
		sampler2D _BumpMap;
		sampler2D _AnisoMaskTex;
		float _SpecularReflection;
		float _IndexOfRefraction
		float3 _AnisoDirWhite, _AnisoDirBlack;





		fixed4 frag(v2f i) : SV_Target
		{
			float3 n = normalize(i.normal);
			float3 l = normalize(mul(UNITY_MATRIX_V,_WorldSpaceLightPos0));
			float3 k_d = tex2D(_DiffuseTex, i.uv);
			float3 ambient = k_d * ShadeSH9(half4(n, 1));

			if (dot(n, l) < -.001) {
				return half4(ambient,1);
			}



			float n_cellulose = 1.55;
			float3 v = normalize(i.eyeDir);
			float3 u = normalize(tex2D(_FiberAxisTex, i.uv));
			float3 h = normalize((n + v) / 2);

			float3 k_f = tex2D(_FiberColorTex, i.uv);
			float beta = tex2D(_HighlightWidthTex, i.uv);



			float k_s = _SpecularReflection;

			float3 I = _LightColor0;


			float psi_r = asin(dot(v, u) / n_cellulose);
			float psi_i = asin(dot(l, u) / n_cellulose);
			float psi_d = psi_r - psi_i;
			float psi_h = psi_r + psi_i;

			float3 L_d = k_d * I * max(dot(n, l), 0);
			float3 L_s = k_s * I * pow(max(dot(n, h), 0), 1.55);

			float3 L_f = k_f * I * gaussian(beta, psi_h) / pow(cos(psi_d / 2), 2);


			// Ambient: ShadeSH9(half4(worldNormal,1));
			//
			//float3 ambient = k_d * 0.);
			return half4(L_d + L_s + L_f + ambient, 1);

		}
			ENDCG


		}
	}
		FallBack "Transparent/Cutout/VertexLit"
}

