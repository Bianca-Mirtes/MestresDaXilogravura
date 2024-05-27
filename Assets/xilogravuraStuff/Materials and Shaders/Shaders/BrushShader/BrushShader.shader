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
        _BrushPreset("BrushPreset", Range(1, 4)) = 1
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
            half _BrushPreset;

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


                float hardness = 0.1;

                //logica de brush quadrado
                float2 diff = abs(i.uv - _Coordinates.xy);
                float2 brushSize;
                float2 falloff;
                float drawSoft;
                float dist;

                #define BRUSH_HARDCIRCLE 0
                #define BRUSH_HARDSQUARE 1
                #define BRUSH_SOFTSQUARE 2
                #define BRUSH_INK 3

                switch (_BrushPreset) {
                    case BRUSH_HARDCIRCLE:
                        // Calcular a distancia do fragmento a coordenada de desenho
                        dist = distance(i.uv, _Coordinates.xy) * 18 / (_Size/2);

                        draw = (dist > hardness) ? 0 : 1;
                        break;
                    case BRUSH_HARDSQUARE: //hard
                        brushSize = float2(_BrushHeight, _BrushWidth);
                        falloff = 1.0 - saturate(diff / brushSize);
                        drawSoft = pow(min(falloff.x, falloff.y), 1800) * _Size;
                        draw = drawSoft;
                        break;
                    case BRUSH_SOFTSQUARE: //soft
                        brushSize = float2(_BrushWidth, _BrushHeight);
                        falloff = 1.0 - saturate(diff / brushSize);
                        drawSoft = pow(min(falloff.x, falloff.y), 1800) * _Size;
                        draw = drawSoft;
                        break;
                    case BRUSH_INK: //Tinta
                        //Baseado no 1
                        // Calcular a distancia do fragmento a coordenada de desenho
                        dist = distance(i.uv, _Coordinates.xy) * 18 / 8;

                        draw = (dist > hardness) ? 0 : 1;
                        break;
                }
                fixed4 drawcol = _Color * (draw * _Strength);
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return saturate(col + drawcol);
            }
            ENDCG
        }
    }
}
