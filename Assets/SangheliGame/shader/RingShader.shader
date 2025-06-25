Shader "Custom/RingShader"
{
    Properties
    {
        [Header(Inner Circle)]
        _InnerColor ("Inner Color", Color) = (1,1,1,1)
        _InnerRadius ("Inner Radius (0-1)", Range(0,1)) = 0.5
        
        [Header(Outer Ring)]
        _RingColor ("Ring Color", Color) = (1,1,1,1)
        _RingOuter ("Ring Outer Radius", Range(0, 0.5)) = 0.4
        _RingThickness ("Ring Thickness", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            fixed4 _InnerColor;
            fixed4 _RingColor;
            float _InnerRadius; // 0–1 normalized
            float _RingOuter;
            float _RingThickness;

            struct appdata {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                // o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                
                // Центрируем UV координаты
                float2 centeredUV = i.texcoord - float2(0.5, 0.5);
                
                // Рассчитываем расстояние от центра
                float dist = length(centeredUV);
                
                float ringInner = _RingOuter - _RingThickness;

                // Масштабируем нормализованный радиус
                float realInnerRadius = _InnerRadius * ringInner;
                
                // Рисуем внутренний круг
                float circle = step(dist, realInnerRadius);
                
                // Рисуем внешнее кольцо
                float ring = step(ringInner, dist) * step(dist, _RingOuter);
                
                // Комбинируем результат с разными цветами
                fixed4 col = _InnerColor * circle + _RingColor * ring;
                
                // Устанавливаем альфу (0 в прозрачных областях)
                col.a = circle * _InnerColor.a + ring * _RingColor.a;
                
                return col;
            }
            ENDCG
        }
    }
}
