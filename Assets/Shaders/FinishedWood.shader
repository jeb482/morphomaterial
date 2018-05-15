Shader "Custom/FinishedWood" {
	Properties {
		_FiberAxisTex("Fiber axis", 2D) = "white" {}
		_HighlightWidthTex("Highlight width", 2D) = "white" {}
		_DiffuseTex("Diffuse color", 2D) = "white" {}
		_FiberColorTex("Fiber color", 2D) = "white" {}
		_SpecularReflection("Specular", Float) = 1
		_Debug("Debug Level", Int) = 0
	}
	SubShader {
		Pass {
			// light direction in _WorldSpaceLightPos0
			// color in _LightColor0
			Tags{ "LightMode" = "ForwardAdd" }

			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag2
			#include "UnityCG.cginc" // UnityObjectToWorldNormal
			#include  "Lighting.cginc"//"UnityLightingCommon.cginc" // _LightColor0

			struct v2f {
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float3 pos : POSITION1;
				float4 vertex : SV_POSITION;
				float3 tangent : TANGENT0;
				float3 bitangent : TANGENT1;
				float4 light_pos : POSITION2;
				half4 light_color : TEX_COORD1;
			};


			v2f vert(appdata_tan  v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.pos = mul(UNITY_MATRIX_MV,float4(v.vertex.xyz,1));
				
				o.normal = mul(UNITY_MATRIX_IT_MV, float4(v.normal,0)).xyz;
				o.tangent = mul(UNITY_MATRIX_MV, float4(v.tangent.x, v.tangent.y, v.tangent.z, 0));//v.tangent.xyz;//UnityObjectToViewPos(v.tangent.xyz).xyz;
				o.bitangent = mul(UNITY_MATRIX_MV, float4(cross(v.normal, v.tangent.xyz) * v.tangent.w,1));
				
				o.light_pos = _WorldSpaceLightPos0;
				o.light_color = _LightColor0;
				return o;
			}

			sampler2D _FiberAxisTex;
			sampler2D _HighlightWidthTex;
			sampler2D _DiffuseTex;
			sampler2D _FiberColorTex;
			float _SpecularReflection;
			int _Debug;

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
				float3 tangent = normalize(i.tangent);
				float3 bitangent = normalize(i.bitangent);

				if (_Debug == 1)
					return half4(n, 1);
				if (_Debug == 2)
					return half4(tangent / 2 + .5, 1);

				if (_Debug == 8)
					return half4(i.pos	,1);


				float3 k_d = tex2D(_DiffuseTex, i.uv);
				//float3 ambient = k_d * ShadeSH9(half4(n, 1));
				float3 ambient = 100000000 * k_d * ShadeSH9(half4(n, 1));

				float3 v = -normalize(i.pos);

				//float3 axis = tex2D(_FiberAxisTex, i.uv); // Potential error. What coordinate space is Axis in?
				//axis = (axis - 0.5) * 2;
				float3 axis = UnpackNormal(tex2D(_FiberAxisTex, i.uv));

				float3 u = normalize(tangent*axis.x + bitangent * axis.y + n * axis.z);
				if (_Debug == 3)
					return half4(u, 1);
				//float3 u;
				//u.x = dot(float3(tangent.x, bitangent.x, n.x), axis);
				//u.y = dot(float3(tangent.y, bitangent.y, n.y), axis);
				//u.z = dot(float3(tangent.z, bitangent.z, n.z), axis);

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

				//for (int j = 0; j < 8; j++) {
				if (_Debug == 10)
					return half4(normalize(mul(UNITY_MATRIX_V, i.light_pos).xyz)/2 + .5, 0);
				
				l = mul(UNITY_MATRIX_V, i.light_pos) - i.pos;

				r = length(l);
				l /= r;
				if (_Debug == 7)
					return half4(l, 1);

				psi_i = asin(dot(l, u) / 1.55); // Need to calc fresnel s(l)
				psi_d = psi_r - psi_i;
				psi_h = psi_r + psi_i;

				trans = T_r * fresnelTransmitance(l, n);
				if (_Debug == 4)
					return 1 - trans;

				if (dot(n, l) > 0) {
					out_color += trans * (k_d * i.light_color) / (r*r);
					if (_Debug == 5)
						return half4(k_s * i.light_color.xyz  * pow(max(dot(n, h), 0), 1.55) / (r*r),1);
//						return half4(dot(n, h)*float3(1,1,1),1);
						//return half4((k_s * unity_LightColor[j].xyz * pow(max(dot(n, h), 0), 1.55)), 1);
					out_color += (k_s * i.light_color  * pow(max(dot(n, h), 0), 1.55)) / (r*r);
					out_color += trans * (k_f * i.light_color * gaussian(beta, psi_h) / pow(cos(psi_d / 2), 2)) / (r*r);
				}

			return half4(out_color + ambient, 1);
			}

			fixed4 frag3(v2f i) : SV_Target
			{
				
				float3 n = normalize(i.normal);
				float3 tangent = normalize(i.tangent);
				float3 bitangent = normalize(i.bitangent);

				if (_Debug == 1)
					return half4(n, 1);
				if (_Debug == 2)
					return half4(tangent/2 + .5, 1);

				if (_Debug == 8)
					return half4(i.pos	,1);


				float3 k_d = tex2D(_DiffuseTex, i.uv);
				//float3 ambient = k_d * ShadeSH9(half4(n, 1));
				float3 ambient = k_d * ShadeSH9(half4(n, 1));

				float3 v = -normalize(i.pos);
				
				//float3 axis = tex2D(_FiberAxisTex, i.uv); // Potential error. What coordinate space is Axis in?
				//axis = (axis - 0.5) * 2;
				float3 axis = UnpackNormal(tex2D(_FiberAxisTex, i.uv));

				float3 u = normalize(tangent*axis.x + bitangent * axis.y + n * axis.z);
				if(_Debug == 3)
					return half4(u, 1);
				//float3 u;
				//u.x = dot(float3(tangent.x, bitangent.x, n.x), axis);
				//u.y = dot(float3(tangent.y, bitangent.y, n.y), axis);
				//u.z = dot(float3(tangent.z, bitangent.z, n.z), axis);

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
					if (_Debug == 10)
						return half4(unity_LightPosition[j].xyz,1);
					l = unity_LightPosition[j] - i.pos;

					r = length(l);
					l /= r;
					if (_Debug == 7)
						return half4(l, 1);

					psi_i = asin(dot(l, u) / 1.55); // Need to calc fresnel s(l)
					psi_d = psi_r - psi_i;
					psi_h = psi_r + psi_i;
					
					trans = T_r*fresnelTransmitance(l, n);
					if (_Debug == 4)
						return 1-trans;
					
					if (dot(n, l) > 0) {
						out_color += trans * (k_d * unity_LightColor[j]) / (r*r);
						if (_Debug ==5)
							return half4(dot(n, h)*float3(1,1,1),1);
							//return half4((k_s * unity_LightColor[j].xyz * pow(max(dot(n, h), 0), 1.55)), 1);
						out_color += (k_s * unity_LightColor[j] * pow(max(dot(n, h), 0), 1.55)) / (r*r);
						out_color += trans * (k_f * unity_LightColor[j] * gaussian(beta, psi_h) / pow(cos(psi_d / 2), 2)) / (r*r);
					}
				}
				return half4(out_color + ambient, 1);
			}

			
			ENDCG
		
		
		}


	}
	FallBack "Diffuse"
}
