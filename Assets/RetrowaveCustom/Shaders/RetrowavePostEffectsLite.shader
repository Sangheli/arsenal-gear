Shader "Hidden/Suggo Creations/Retrowave Post Effects Lite"
{
    Properties
    {
        _MainTex("Source", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        sampler2D _BloomTex;
        float _Threshold;
        float _Intensity;
        float _BlurStep;
        float _VignetteIntensity;
        float _VignetteSmoothness;

        float3 GaussianBlur(v2f_img input, float2 direction)
        {
            float2 offset = _MainTex_TexelSize.xy * direction * _BlurStep;
            float3 color = tex2D(_MainTex, input.uv).rgb * 0.227027;
            color += tex2D(_MainTex, input.uv + offset * 1.384615).rgb * 0.316216;
            color += tex2D(_MainTex, input.uv - offset * 1.384615).rgb * 0.316216;
            color += tex2D(_MainTex, input.uv + offset * 3.230769).rgb * 0.070270;
            color += tex2D(_MainTex, input.uv - offset * 3.230769).rgb * 0.070270;
            return color;
        }

        float3 ToneMapACES(float3 color)
        {
            return saturate((color * (2.51 * color + 0.03)) / (color * (2.43 * color + 0.59) + 0.14));
        }
        ENDCG

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            fixed4 frag(v2f_img input) : SV_Target
            {
                float3 color = tex2D(_MainTex, input.uv).rgb;
                float brightness = max(color.r, max(color.g, color.b));
                float contribution = max(brightness - _Threshold, 0) / max(brightness, 0.00001);
                return fixed4(color * contribution, 1);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            fixed4 frag(v2f_img input) : SV_Target
            {
                return fixed4(GaussianBlur(input, float2(1, 0)), 1);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            fixed4 frag(v2f_img input) : SV_Target
            {
                return fixed4(GaussianBlur(input, float2(0, 1)), 1);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            fixed4 frag(v2f_img input) : SV_Target
            {
                float3 color = tex2D(_MainTex, input.uv).rgb;
                color += tex2D(_BloomTex, input.uv).rgb * _Intensity;
                color = ToneMapACES(color);

                float distanceFromCenter = length(input.uv - 0.5);
                float vignette = 1 - smoothstep(0.8 - _VignetteSmoothness, 0.8, distanceFromCenter);
                color *= lerp(1, vignette, _VignetteIntensity);
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
}
