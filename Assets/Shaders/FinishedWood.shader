Shader "Custom/FinishedWood" {
	Properties {
		_FiberAxisTex("Fiber axis", 2D) = "white" {}
		_HighlightWidthTex("Highlight width", 2D) = "white" {}
		_DiffuseTex("Diffuse color", 2D) = "white" {}
		_FiberColorTex("Fiber color", 2D) = "white" {}
		_SpecularReflection("Specular", Float) = 1.0 
	}
	SubShader {
		Pass {
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
				float3 eyeDir : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};


			v2f vert(appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.eyeDir = UnityObjectToViewPos(-1 * normalize(v.vertex));
				o.normal = mul(UNITY_MATRIX_IT_MV, float4(v.normal,0)).xyz;
				return o;
			}

			sampler2D _FiberAxisTex;
			sampler2D _HighlightWidthTex;
			sampler2D _DiffuseTex;
			sampler2D _FiberColorTex;
			float _SpecularReflection;

			float gaussian(float sigma, float x) {
				return 1 / (sigma*sqrt(2 * 3.1415963))*exp((x*x) / (2 * sigma*sigma));
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 n = normalize(i.normal);
				float3 l = _WorldSpaceLightPos0;
				if (dot(n, l) < 0) {
					return half4(0,0,0,1);
				}


				float eta = 1.55;
				float n_cellulose = 1.55;
				float3 v = normalize(i.eyeDir);
				float3 u = normalize(tex2D(_FiberAxisTex, i.uv));
				float3 h = normalize((n + v) / 2);
				float3 k_d = tex2D(_DiffuseTex, i.uv);
				float3 k_f = tex2D(_FiberColorTex, i.uv);
				float beta = tex2D(_HighlightWidthTex, i.uv);



				float k_s = _SpecularReflection;

				float3 I = _LightColor0;


				float psi_r = asin(dot(v, u) / n_cellulose);
				float psi_i = asin(dot(l, u) / n_cellulose);
				float psi_d = psi_r - psi_i;
				float psi_h = psi_r + psi_i;

				float3 L_d = k_d * I * max(dot(n, l), 0);
				float3 L_s = k_s * I * pow(max(dot(n, h), 0), eta);

				float3 L_f = k_f * I * gaussian(beta, psi_h) / pow(cos(psi_d / 2), 2);

				return half4(L_d + L_s + L_f, 1);

			}
			ENDCG
		
		
		}


	}
	FallBack "Diffuse"
}
