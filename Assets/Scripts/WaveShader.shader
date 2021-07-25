//UNITY_SHADER_NO_UPGRADE

Shader "Unlit/WaveShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PointLightColor ("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition ("Point Light Position", Vector) = (0.0, 0.0, 0.0)
	}
	SubShader
	{
	
		Pass
		{
			Cull Off
				
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;	
			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;
			uniform sampler2D _CameraDepthTexture;

			struct vertIn
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color: COLOR;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color: COLOR;
				float4 worldVertex : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				float amp = 0.3f, speed = 2.0f, speedtime = speed * _Time.y, wavelength = 2.0f*UNITY_PI / 10.0f;

				// better wave function from --> https://catlikecoding.com/unity/tutorials/flow/waves/
				float f = wavelength * (v.vertex.x - speed * _Time.y); 
				v.vertex.x += amp * cos(f);
				v.vertex.y += amp * sin(f);
				float3 tangent = normalize(float3(1- wavelength * amp * sin(f), amp * wavelength * cos(f), 0));
				float3 normal = float3(-tangent.y, tangent.x, 0);

				float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), normal));

				//change color here
				v.color = float4(0.26f, 0.57f, 1.0f, 1.0f);
				v.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				

				vertOut o;
				o.vertex = v.vertex;
				o.uv = v.uv;
				o.color = v.color;
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;

				return o;
			}
			
			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, v.uv);
				float3 interpNormal = normalize(v.worldNormal);

				// Calculate ambient RGB intensities
				float Ka = 0.5;
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;
				
				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = 1;
				float Kd = 1;
				float3 L = normalize(_PointLightPosition - v.worldVertex.xyz);
				float LdotN = dot(L, interpNormal.xyz);
				float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN);
				
				// Calculate specular reflections
				float Ks = 0.5;
				float specN = 20; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				float3 R = normalize(V + L);
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(interpNormal, R)), specN);

				// Combine Phong illumination model components
				float4 returnColor = float4(0,0,0,0);
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
				returnColor.a = v.color.a;

				return returnColor;
			}
			ENDCG
		}
	}
}