Shader "Custom/FinishedWood" {
	Properties {
		_GratingDistance("Grating Distance", Float) = 1100;
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
				float3 pos : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float3 tangent : TANGENT0;
				float3 bitangent : TANGENT1;
			};
	


			v2f vert(appdata_tan v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.pos = mul(UNITY_MATRIX_MV,float4(v.vertex.xyz,1));
				o.normal = mul(UNITY_MATRIX_IT_MV, float4(v.normal,0)).xyz;
				o.tangent = mul(UNITY_MATRIX_MV, float4(v.tangent.x, v.tangent.y, v.tangent.z, 0));
				o.bitangent = mul(UNITY_MATRIX_MV, float4(cross(v.normal, v.tangent.xyz) * v.tangent.w,1));
				return o;
			}

			fixed3 spectral_zucconi(float w)
			{
				fixed x = saturate((w - 400.0) / 300.0);

				const float3 cs = float3(3.54541723, 2.86670055, 2.29421995);
				const float3 xs = float3(0.69548916, 0.49416934, 0.28269708);
				const float3 ys = float3(0.02320775, 0.15936245, 0.53520021);

				return bump3y(cs * (x - xs), ys);
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
					return half4(normalize(mul(UNITY_MATRIX_V, _WorldSpaceLightPos0).xyz)/2 + .5, 0);
				
				l = mul(UNITY_MATRIX_V, _WorldSpaceLightPos0) - i.pos;

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
					out_color += trans * (k_d * _LightColor0) / (r*r);
					if (_Debug == 5)
						return half4(k_s * _LightColor0.xyz  * pow(max(dot(n, h), 0), 1.55) / (r*r),1);
//						return half4(dot(n, h)*float3(1,1,1),1);
						//return half4((k_s * unity_LightColor[j].xyz * pow(max(dot(n, h), 0), 1.55)), 1);
					out_color += (k_s * _LightColor0.xyz  * pow(max(dot(n, h), 0), 1.55)) / (r*r);
					out_color += trans * (k_f * _LightColor0 * gaussian(beta, psi_h) / pow(cos(psi_d / 2), 2)) / (r*r);
				}

			return half4(out_color + ambient, 1);
			}

			

			
			ENDCG
		
		
		}
		Pass{
			Blend One One
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" // UnityObjectToWorldNormal
			#include  "Lighting.cginc"//"UnityLightingCommon.cginc" // _LightColor0


			
			struct v2f {
			float2 uv : TEXCOORD0;
			float3 normal : NORMAL;
			float4 vertex : SV_POSITION;
			};


			v2f vert(appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(UNITY_MATRIX_IT_MV, float4(v.normal, 0)).xyz;
				o.uv = v.texcoord;
				return o;
			}


			sampler2D _DiffuseTex;
			fixed4 frag(v2f i) : SV_Target{
				float3 n = normalize(i.normal);
				float3 k_d = tex2D(_DiffuseTex, i.uv);

				float3 ambient =  k_d * ShadeSH9(half4(n, 1));
				return fixed4(_DiffuseMultipier*ambient, 1);
			}

			ENDCG
		}


	}
	FallBack "Diffuse"
}
