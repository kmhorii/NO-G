﻿Shader "N3K/AlwaysVisible"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "red" {}
		//Changing color of always visible
		_Color("Always visible color", Color) = (1,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

		Pass
		{	
			Cull Off
			ZWrite Off		 //Controls wheter pixels from this object are written to the depth buffer (default is On). 
							 //If you're drawing solid objects, leave this on. If you're drawing semitransparent effects, switch to ZWrite Off.

			ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
			//where it renders object
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _Color;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//all we need to return is a fixed color
				return _Color;
			}

			ENDCG
		}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
