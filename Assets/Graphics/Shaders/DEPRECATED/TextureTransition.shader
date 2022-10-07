Shader "Unlit/TextureTransition"
{
    Properties
    {
        [MainTexture] _TransitionLayout ("Transition Layout", 2D) = "white" {}
	    [MainColor] _TransitionImage ("Transition Image", 2D) = "white" {}
        _ContourMap("Contour Map", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0.0, 1.1)) = 0.3
    }
    SubShader
    {
		Tags 
		{ 
			"RenderType" = "Opaque" 
            "Queue" = "Overlay+1"
			"PreviewType" = "Plane"
		}

		Cull Back
        LOD 200
        
        Cull Back
        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
            };

            sampler2D _TransitionLayout;
            float4 _TransitionLayout_ST;

			sampler2D _TransitionImage;
			float4 _TransitionImage_ST;

			sampler2D _ContourMap;
			float4 _ContourMap_ST;

			float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _TransitionLayout);
				o.uv2 = TRANSFORM_TEX(v.uv, _TransitionImage);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 sampledLayout = tex2D(_TransitionLayout, i.uv);
                float4 sampledContour = tex2D(_ContourMap, i.uv);
				float4 color = tex2D(_TransitionImage, i.uv2);

                float step = 1 - (sampledLayout.r >= _Cutoff);

                if (step > 0)
                {
                    return step.xxxx * color * sampledContour;
                }
                else
                {
                    discard;
                }

                return 0;
            }
            ENDCG
        }
    }
}
