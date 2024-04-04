Shader "Unlit/BrushShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Coordinates("Coordinate", Vector) = (0,0,0,0)
        _Color("Draw Color", Color) = (0,0,0,0)
        _Strength("Strength", Range(0,1)) = 1
        _Size("Size", Range(2, 16)) = 2
        _Hardness("Hardness", Range(1, 15)) = 3

        _IsRoundBrush("RoundBrush", Range(0,1)) = 1
        _BrushWidth("BrushWidth", Range(1, 50)) = 45
        _BrushHeight("BrushHeight", Range(1, 50)) = 15
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Coordinates,_Color;
            half _Size, _Strength, _Hardness;
            half _BrushWidth, _BrushHeight;
            half _IsRoundBrush;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float brushstroke;
                float draw;

                if (_IsRoundBrush == 1)
                {
                    // Calcular a distancia do fragmento a coordenada de desenho
                    float dist = distance(i.uv, _Coordinates.xy) * 18/_Size;

                    // Definir uma distancia de corte para a borda
                    float hardness = 0.1;

                    // Calcular a intensidade da pincelada com base na distancia
                    draw = (dist > hardness) ? 0 : 1;
                }
                else
                {
                    float2 diff = abs(i.uv - _Coordinates.xy);
                    float2 brushSize = float2(_BrushWidth, _BrushHeight);
                    float2 falloff = 1.0 - saturate(diff / (brushSize * 0.5));
                    draw = pow(min(falloff.x, falloff.y), 500 / _BrushWidth * 10 * _Hardness * 0.35);

                    //draw = pow(saturate(1 - distance(i.uv, _Coordinates.xy)), 500 / _Size * _Hardness * 0.35);
                }

                fixed4 drawcol = _Color * (draw * _Strength);
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return saturate(col + drawcol);
            }
            ENDCG
        }
    }
}
