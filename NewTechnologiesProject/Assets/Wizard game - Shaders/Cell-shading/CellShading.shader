// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Cartoon/CellShading"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BaseColor("Color", Color) = (1, 1, 1, 1)
		_Evaluator("Evaluator", Vector) = (0.1, 0.1, 0.1, 0.1)
		_LightMultiplier("Light multiplier", Float) = 1
		_DarkMultiplier("Dark multiplier", Float) = 0.5

		_OutlineColor("Outline color", Color) = (1, 1, 1, 1)
		_OutlineSize("Outline Size", Float) = 4

	}
	SubShader
		{
		Tags
		{
			//"Queue" = "Transparent"
			"RenderType" = "Opaque"
			"LightMode" = "ForwardBase" ///DIT IS HEEL BELANGRIJK, ZONDER DIT VERANDERT DE BELICHTING ALS JE EEN ANDERE RICHTING OP KIJKT
		}

		Pass //OUTLINE PASS
		{
			Cull Front //Rendert van deze pass de tris die richting de camera kijken NIET
			ZWrite On 
			ZTest Less 
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog		
			#include "UnityCG.cginc"



			fixed4 _OutlineColor; //maak _OutlineColor uit de Properties bruikbaar in deze pass
			float _OutlineSize;
			struct appdata //struct genaamd appdata. Deze vraagt de informatie van het 3D model op.
			{
				float4 vertex : POSITION; ///the position of the vertex
				float4 normal : NORMAL; //the rotation of the vertex
			};

			struct v2f //struct genaamd vertex to fragment (v2f)
			{
				float4 vertex : SV_POSITION; //geef v2f informatie over de positie van de vertexes.
				half3 normal : NORMAL; //geef v2f informatie over de normals.
				fixed4 color : COLOR0; //geef v2f een mogelijkheid om een kleur op te slaan.
			};

			v2f vert(appdata v)
			{
				v2f o; //maak een v2f aan genaamd o.
				o.vertex = UnityObjectToClipPos(v.vertex); ///Zet v.vertex om naar world-space in plaats van object-space, en sla dat op in o.vertex.
				o.normal = UnityObjectToWorldNormal(v.normal); ///Zet v.normal om naar world-normal en sla dat op in o.normal.
				float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal); 
				float2 offset = TransformViewToProjection(norm.xy);
				o.vertex.xy += (offset * o.vertex.z) * _OutlineSize;
				o.color = _OutlineColor;
				return o; //Geef de v2f o door aan de fragment functie nu het de correcte data bevat.
			}

			float4 frag(v2f i) : SV_Target
			{
				return float4(i.color.r, i.color.g, i.color.b, 1); //Geeft de kleurendata terug. Dit wordt uiteindelijk gerendert.
			}
			ENDCG
		}

		Pass
		{
			ZWrite On
			Cull Off
			//ZTest LEqual
			//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#define UNITY_PASS_SHADOWCASTER

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _BaseColor; //maak _BaseColor uit de Properties bruikbaar in deze pass
			struct appdata //struct genaamd appdata. Deze vraagt de informatie van het 3D model op.
			{
				float4 pos : POSITION; ///the position of the vertex
				float4 normal : NORMAL; //the rotation of the vertex
				float2 uv : TEXCOORD0;
				
			};

			struct v2f //struct genaamd vertex to fragment (v2f)
			{
				float4 pos : SV_POSITION; //geef v2f informatie over de positie van de vertexes.
				half3 normal : NORMAL; //geef v2f informatie over de normals.
				fixed4 color : COLOR0; //geef v2f een mogelijkheid om een kleur op te slaan.
				float2 uv : TEXCOORD0;
				LIGHTING_COORDS(3, 4)
			};


			half4 _Evaluator; /////Een evaluator voor de cell shading
			float _LightMultiplier; ////De multiplier voor alles wat licht moet zijn
			float _DarkMultiplier; ////De multiplier voor alles wat donker moet zijn

			fixed4 _LightColor0; ////contains the light's color

			v2f vert(appdata v)
			{
				v2f o; //maak een v2f aan genaamd o.
				o.pos = UnityObjectToClipPos(v.pos); ///Zet v.vertex om naar world-space in plaats van object-space, en sla dat op in o.vertex.
				o.normal = UnityObjectToWorldNormal(v.normal); ///Zet v.normal om naar world-normal en sla dat op in o.normal.
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex); ////Does some UV magic to make the UV work for _MainTex

				half4 NdotL = max(0, dot(o.normal, _WorldSpaceLightPos0.xyz)); //Krijg de dot product van o.normal en de WorldSpaceLightPos0.xyz.

				float sumNdotL = NdotL.r + NdotL.g + NdotL.b + NdotL.a; ///De som van alle 4 waardes in NdotL
				float sumEvaluator = _Evaluator.r + _Evaluator.g + _Evaluator.b + _Evaluator.a; ///De som van alle 4 waardes in evaluator
				if (sumNdotL > sumEvaluator) ///Als de som van NdotL meer is dan de som van de evaluator
				{
					o.color = _BaseColor * _LightMultiplier; ///Dan moet dit gedeelte licht worden
				}
				else ///Anders moet dit gedeelte donker worden
				{
					o.color = _BaseColor * _DarkMultiplier;
				}
				o.color *= _LightColor0;
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o; //Geef de v2f o door aan de fragment functie nu het de correcte data bevat.
			}

			float4 frag(v2f i) : SV_Target
			{
				float  atten = LIGHT_ATTENUATION(i);
				float4 texcol = tex2D(_MainTex, i.uv);
				float4 result = float4(i.color.r, i.color.g, i.color.b, 0.5); 
				return float4(atten, atten, atten, 1) * result * texcol;
				
			}
			ENDCG
		}
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		
	}
}
