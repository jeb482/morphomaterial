﻿Shader "Custom/FinishedWood" {
	Properties {
		_FiberAxisTex("Fiber axis", 2D) = "white" {}
		_HighlightWidthTex("Highlight width", 2D) = "white" {}
		_DiffuseTex("Diffuse color", 2D) = "white" {}
		_FiberColorTex("Fiber color", 2D) = "white" {}
		_SpecularReflection("Specular", Float) = 0.05 
	}
	SubShader {
		Pass {
			// light direction in _WorldSpaceLightPos0
			// color in _LightColor0
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag2
			#include "UnityCG.cginc" // UnityObjectToWorldNormal
			#include "UnityLightingCommon.cginc" // _LightColor0

			struct v2f {
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float3 pos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};


			v2f vert(appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.pos = UnityObjectToViewPos(v.vertex);
				o.normal = mul(UNITY_MATRIX_IT_MV, float4(v.normal,0)).xyz;
				return o;
			}

			sampler2D _FiberAxisTex;
			sampler2D _HighlightWidthTex;
			sampler2D _DiffuseTex;
			sampler2D _FiberColorTex;
			float _SpecularReflection;

			float gaussian(float sigma, float x) {
				return 1 / (sigma*sqrt(2 * 3.1415963))*exp((-x*x) / (2 * sigma*sigma));
			}

			float fresnelTransmitance(float3 l, float3 n) {
				float costheta1 = dot(l, n);
				float sintheta1 = sqrt(1 - costheta1 * costheta1);
				float sintheta2 = sintheta1 / 1.55;
				float costheta2 = sqrt(1 - sintheta2 * sintheta2);

				float F_Rpar = (1.55 * costheta1 - costheta2)/(1.55 * costheta1 + costheta2);
				F_Rpar = F_Rpar *F_Rpar;

				float F_Rperp = (costheta2 - 1.55*costheta1) / (costheta2 + 1.55*costheta1);
				F_Rperp = F_Rperp * F_Rperp;

				float Fr = 0.5*(F_Rperp + F_Rpar);
				return 1 - Fr;
			}
			fixed4 frag2(v2f i) : SV_Target
			{
				
				float3 n = normalize(i.normal);

				float3 k_d = tex2D(_DiffuseTex, i.uv);
				float3 ambient = k_d * ShadeSH9(half4(n, 1));
				float3 v = -normalize(i.pos);
				float3 u = normalize(tex2D(_FiberAxisTex, i.uv)); // Potential error. What coordinate space is Axis in?
				float3 h = normalize((n + v) / 2);
				float3 k_f = tex2D(_FiberColorTex, i.uv);
				float beta = tex2D(_HighlightWidthTex, i.uv);
				float k_s = _SpecularReflection;
				float psi_r = asin(dot(v, u) / 1.55); // Need to calc fresnel s(v)
				float3 out_color = float3(0, 0, 0);
				float3 l; 
				float ndl;
				float psi_i; 
				float psi_h; 
				float psi_d; 
				float r;
				float T_r = fresnelTransmitance(v, n);
				float trans;

				for (int j = 0; j < 8; j++) {
					l = unity_LightPosition[j] - i.pos;
					r = length(l);
					l /= r;
					psi_i = asin(dot(l, u) / 1.55); // Need to calc fresnel s(l)
					psi_d = psi_r - psi_i;
					psi_h = psi_r + psi_i;
					
					trans = T_r*fresnelTransmitance(l, n);
					
					if (dot(n, l) > 0) {
						out_color += trans * (k_d * unity_LightColor[j]) / (r*r);
						out_color += (k_s * unity_LightColor[j] * pow(max(dot(n, h), 0), 1.55)) / (r*r);
						out_color += trans * (k_f * unity_LightColor[j] * gaussian(beta, psi_h) / pow(cos(psi_d / 2), 2)) / (r*r);
					}
				}
				return half4(out_color + ambient, 1);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				
				// Calculate ambient term
				float3 n = normalize(i.normal);
				//return half4(normalize(i.pos - unity_LightPosition[0]), 1);
				
				
				float3 k_d = tex2D(_DiffuseTex, i.uv);
				float3 ambient = k_d*ShadeSH9(half4(n, 1));
				

				// View parameters
				float3 v = -normalize(i.pos);
				float3 u = normalize(tex2D(_FiberAxisTex, i.uv));
				float3 h = normalize((n + v) / 2);
				float3 k_f = tex2D(_FiberColorTex, i.uv);
				float beta = tex2D(_HighlightWidthTex, i.uv);
				float k_s = _SpecularReflection;

				// Physical quantities
				float n_cellulose = 1.55;
				float psi_r = asin(dot(v, u) / n_cellulose);


				// Calculate diffuse, spec, scatter for each light.
				float3 out_color = float3(0, 0, 0);
				float3 l; float ndl;
				float psi_i; float psi_h; float psi_d; float r;
				for (int j = 0; j < 8; j++) {
					l = unity_LightPosition[j] - i.pos ;
					if ((unity_LightPosition[j]).w == 0)
						r = 1;
					else
						r = length(l);

					l = normalize((l).xyz);
					ndl = dot(n, l);
					
					psi_i = asin(dot(l, u) / n_cellulose);
					psi_d = psi_r - psi_i;
					psi_h = psi_r + psi_i;


					if (ndl > 0) {
						out_color.xyz += (k_d * unity_LightColor[j]) / (r*r);
						out_color.xyz += (k_s * unity_LightColor[j] * pow(max(dot(n, h), 0), 1.55)) / (r*r);
					}

					
					
					out_color.xyz += (k_f * unity_LightColor[j] * gaussian(beta, psi_h) / pow(cos(psi_d / 2), 2)) / (r*r) ;
				}
				return half4(out_color + ambient, 1);
		
			}
			ENDCG
		
		
		}


	}
	FallBack "Diffuse"
}
