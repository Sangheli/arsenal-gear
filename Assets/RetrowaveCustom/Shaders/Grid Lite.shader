Shader "Suggo Creations/Retrowave Grid Lite (Built-in)"
{
    Properties
    {
        _GridTexture("Grid Texture", 2D) = "white" {}
        [HDR] _GridColor("Grid Color", Color) = (2.1185474, 0, 2.1185474, 1)
        _GroundColor("Ground Color", Color) = (0.1372549, 0, 0.2470588, 1)
        _GridSpeed("Grid Speed XY", Vector) = (0, 0.5, 0, 0)
        _GridRotation("Grid Rotation", Range(-180, 180)) = 0
        _Tiling("Tiling XY", Vector) = (1, 1, 0, 0)
        _Smoothness("Smoothness", Range(0, 1)) = 0
        _Metallic("Metallic", Range(0, 1)) = 0
        _ReflectionTex("Planar Reflection", 2D) = "black" {}
        _ReflectionStrength("Planar Reflection Strength", Range(0, 1)) = 0
        [Toggle] BOOLEAN_WORLDSPACEUV("Enable Worldspace UVs", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _GridTexture;
        float4 _GridColor;
        float4 _GroundColor;
        float4 _GridSpeed;
        float4 _Tiling;
        float _GridRotation;
        float _Smoothness;
        float _Metallic;
        float BOOLEAN_WORLDSPACEUV;
        sampler2D _ReflectionTex;
        float _ReflectionStrength;

        struct Input
        {
            float2 uv_GridTexture;
            float3 worldPos;
            float4 reflectionPos;
        };

        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            data.reflectionPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
        }

        float2 RotateUV(float2 uv, float degrees)
        {
            float angle = radians(degrees);
            float sine = sin(angle);
            float cosine = cos(angle);
            uv -= 0.5;
            uv = mul(float2x2(cosine, -sine, sine, cosine), uv);
            return uv + 0.5;
        }

        void surf(Input input, inout SurfaceOutputStandard output)
        {
            float2 uv = input.uv_GridTexture;

            if (BOOLEAN_WORLDSPACEUV > 0.5)
                uv = RotateUV(float2(input.worldPos.x, input.worldPos.z) * 0.5, 180);

            uv = uv * _Tiling.xy + _Time.y * _GridSpeed.xy;
            uv = RotateUV(uv, _GridRotation);

            float2 reflectionUV = input.reflectionPos.xy / input.reflectionPos.w;
            float3 reflection = tex2D(_ReflectionTex, reflectionUV).rgb;
            float3 ground = lerp(_GroundColor.rgb, reflection, _ReflectionStrength);
            float3 color = tex2D(_GridTexture, uv).rgb * _GridColor.rgb + ground;

            output.Albedo = color;
            output.Emission = color;
            output.Metallic = _Metallic * (1 - _ReflectionStrength);
            output.Smoothness = _Smoothness;
            output.Alpha = 1;
        }
        ENDCG
    }

    FallBack "Standard"
}
