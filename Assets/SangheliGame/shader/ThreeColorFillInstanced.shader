Shader "Custom/ThreeColorFillSpriteInstanced"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (1,0,0,0.8)
        _ColorB ("Color B", Color) = (0,1,0,0.8)
        _ColorC ("Color C", Color) = (0,0,1,0.8)

        _FillX ("Fill X (0-1)", Range(0,1)) = 0.3
        _FillY ("Fill Y (0-1)", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "CanUseSpriteAtlas"="True" 
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _ColorA)
                UNITY_DEFINE_INSTANCED_PROP(float4, _ColorB)
                UNITY_DEFINE_INSTANCED_PROP(float4, _ColorC)
                UNITY_DEFINE_INSTANCED_PROP(float, _FillX)
                UNITY_DEFINE_INSTANCED_PROP(float, _FillY)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float4 texColor = tex2D(_MainTex, i.uv);

                float4 colA = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorA);
                float4 colB = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorB);
                float4 colC = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorC);
                float fillX = UNITY_ACCESS_INSTANCED_PROP(Props, _FillX);
                float fillY = UNITY_ACCESS_INSTANCED_PROP(Props, _FillY);

                float x = i.uv.x;

                float4 fillCol;
                if (x < fillX)
                    fillCol = colA;
                else if (x < fillY)
                    fillCol = colB;
                else
                    fillCol = colC;

                // умножаем на альфу спрайта
                return float4(fillCol.rgb, fillCol.a * texColor.a);
            }
            ENDCG
        }
    }
}
