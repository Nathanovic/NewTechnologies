Shader "Custom/AuraShader"
{
Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BaseColor("_Color", Color) = (1, 1, 1, 1)
		_Transparency("Transparency", Float) = 0.2


	}
	SubShader
		{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"LightMode" = "ForwardBase" ///DIT IS HEEL BELANGRIJK, ZONDER DIT VERANDERT DE BELICHTING ALS JE EEN ANDERE RICHTING OP KIJKT
		}

		Pass
		{
			ZWrite Off
			Cull Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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
			};
			fixed4 _LightColor0; ////contains the light's color

			float _time;
			float _colorModifier;

			v2f vert(appdata v)
			{
				v2f o; //maak een v2f aan genaamd o.
				o.pos = UnityObjectToClipPos(v.pos); ///Zet v.vertex om naar world-space in plaats van object-space, en sla dat op in o.vertex.
				o.normal = UnityObjectToWorldNormal(v.normal); ///Zet v.normal om naar world-normal en sla dat op in o.normal.
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex); ////Does some UV magic to make the UV work for _MainTex
				o.uv *= 2;
				o.uv.x += _time;
				o.uv.y += _time;

				o.color = _BaseColor;
				o.color *= _LightColor0;

				return o; //Geef de v2f o door aan de fragment functie nu het de correcte data bevat.
			}


			float _Transparency;

			float4 frag(v2f i) : SV_Target
			{
				float4 texcol = tex2D(_MainTex, i.uv);
				if(texcol.a < _Transparency)
				{
					texcol.a += _Transparency;
				}
				texcol.r += _colorModifier;
				texcol.g += _colorModifier;
				texcol.b += _colorModifier;
				return texcol * i.color;
				
			}
			ENDCG
		}
			//UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		
	}}
