Shader "Unlit/ColorMappedPuzzlePiece"
{
    Properties
    {
        [MainTexture] _ColorKeyMap ("Color Key Map", 2D) = "white" {}
		_ColorKeyEdges ("Color Key Edges", 2D) = "white" {}
	    [MainColor] _PuzzleImage ("Puzzle Image", 2D) = "white" {}
		_PieceColorKey ("Piece Color Key", Color) = (1.0, 1.0, 1.0, 1.0)
		_EdgeBlend ("Edge Blend", Range(0.0, 1.0)) = 0.3
        _GlobalTilingOffset("Global Tiling Offset", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {
		Tags 
		{ 
			"RenderType" = "Opaque" 
			"PreviewType" = "Plane"
		}

		Cull Back
        LOD 200

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

            sampler2D _ColorKeyMap;
            float4 _ColorKeyMap_ST;

			sampler2D _ColorKeyEdges;
			float4 _ColorKeyEdges_ST;

			sampler2D _PuzzleImage;
			float4 _PuzzleImage_ST;

			float4 _PieceColorKey;
			float _EdgeBlend;
            float4 _GlobalTilingOffset;

            v2f vert (appdata v)
            {
                v2f o;

                _ColorKeyMap_ST = _GlobalTilingOffset;
                _ColorKeyEdges_ST = _GlobalTilingOffset;
                _PuzzleImage_ST = _GlobalTilingOffset;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _ColorKeyMap);
				o.uv2 = TRANSFORM_TEX(v.uv, _PuzzleImage);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the template
                float4 sampledColorKey = tex2D(_ColorKeyMap, i.uv);

				// use the puzzle image at the same uv, making anything beyond the template bounds fully transparent
				float4 puzzlePieceColor = tex2D(_PuzzleImage, i.uv2);

				// determine if the piece color matches the mapped color in the template
				if (sampledColorKey.r == _PieceColorKey.r &&
					sampledColorKey.g == _PieceColorKey.g &&
					sampledColorKey.b == _PieceColorKey.b &&
					sampledColorKey.a > 0.0)
				{
					puzzlePieceColor.a = sampledColorKey.a;

					if (puzzlePieceColor.a > 0.0)
					{
						float edgeAlpha = tex2D(_ColorKeyEdges, i.uv).a;
						puzzlePieceColor.rgb += (edgeAlpha > 0.0 ? (edgeAlpha * _EdgeBlend) : 0.0);
					}
				}
				else
				{
					discard;
				}

                return puzzlePieceColor;
            }
            ENDCG
        }
    }
}
