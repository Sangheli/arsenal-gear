Shader "Suggo Creations/Retrowave Skybox Lite (Built-in)"
{
    Properties
    {
        [Header(Sky)]
        _SkyboxRotation("Skybox Rotation Y", Range(0, 360)) = 0
        _SkyColor("Sky Color", Color) = (1, 0.1529412, 0, 1)
        _HorizonColor("Horizon", Color) = (0.4248956, 0.1074671, 0.6509434, 1)
        _HorizonHeight("Horizon/Sky Height", Range(-1, 1)) = 0
        _HorizonStrength("Horizon Strength", Range(0, 1)) = 1

        [Header(Sun)]
        _SunColorOne("Sun Color One", Color) = (0.9568628, 0.8235295, 0.2901961, 1)
        _SunColorTwo("Sun Color Two", Color) = (0.8784314, 0.3960785, 0.6156863, 1)
        _SunMask("Sun Mask", 2D) = "white" {}
        _SunSize("Sun Disc Size", Range(0, 1)) = 0.55
        _SunMaskSize("Sun Mask Size", Range(-5, 1)) = 0
        _SunGradientHeight("Sun Gradient Height", Range(-1, 5)) = 2

        [Header(Stars)]
        [HDR] _StarColor("Star Color", Color) = (0.9056604, 0.9056604, 0.9056604, 0)
        _StarDensity("Star Density", Float) = 10
        _StarSize("Star Size", Range(0, 200)) = 100
        [Toggle(BOOLEAN_PREVENTSTARS_ON)] _PreventStars("Prevent Stars in front of Sun", Float) = 1

        [Header(Fog)]
        _FogHeight("Fog Height", Range(-10, 10)) = 1
        _FogPower("Fog Power", Range(0, 5)) = 1
        _FogContrast("Fog Contrast", Float) = 5
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature BOOLEAN_PREVENTSTARS_ON
            #pragma target 3.0
            #include "UnityCG.cginc"

            static const float Pi = 3.14159265359;
            static const float TwoPi = 6.28318530718;

            sampler2D _SunMask;
            float4 _SkyColor;
            float4 _HorizonColor;
            float4 _SunColorOne;
            float4 _SunColorTwo;
            float4 _StarColor;
            float _HorizonHeight;
            float _HorizonStrength;
            float _SunSize;
            float _SunMaskSize;
            float _SunGradientHeight;
            float _StarDensity;
            float _StarSize;
            float _FogHeight;
            float _FogPower;
            float _FogContrast;
            float _SkyboxRotation;
            float4x4 _LightMatrix;

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 objectPosition : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.objectPosition = v.vertex.xyz;
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float3 RotateSkybox(float3 direction)
            {
                float angle = radians(_SkyboxRotation);
                float sine = sin(angle);
                float cosine = cos(angle);
                return float3(cosine * direction.x + sine * direction.z, direction.y, cosine * direction.z - sine * direction.x);
            }

            float2 Random2(float2 value)
            {
                float3 p3 = frac(float3(value.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.xx + p3.yz) * p3.zy);
            }

            float Voronoi(float2 uv, float density, float angleOffset)
            {
                float2 cell = floor(uv * density);
                float2 offset = frac(uv * density);
                float minDistance = 8;

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 cellOffset = float2(x, y);
                        float2 random = Random2(cell + cellOffset);
                        float angle = angleOffset + random.x * TwoPi;
                        float2 voronoiPoint = cellOffset + float2(cos(angle), sin(angle));
                        float2 delta = voronoiPoint - offset;
                        minDistance = min(minDistance, dot(delta, delta));
                    }
                }

                return sqrt(minDistance);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float3 direction = RotateSkybox(normalize(i.objectPosition));
                float3 worldDirection = RotateSkybox(normalize(i.worldPosition));
                float2 skyUV = float2(atan2(worldDirection.x, worldDirection.z) / Pi, asin(worldDirection.y) / (Pi * 0.5));
                float horizonUV = skyUV.y * _HorizonStrength + _HorizonHeight;
                float3 skyColor = lerp(_HorizonColor.rgb, _SkyColor.rgb, horizonUV);

                float starDistance = saturate(Voronoi(skyUV * float2(8, 3), _StarDensity, 20));
                float stars = pow(1 - starDistance, _StarSize);

                float3 lightDirection = _WorldSpaceLightPos0.xyz;
                lightDirection = dot(lightDirection, lightDirection) > 0.0001 ? normalize(lightDirection) : float3(0, 1, 0);
                float sunAngle = acos(saturate(dot(lightDirection, direction)));
                float sunDisc = 1 - step(_SunSize, sunAngle);
                float3 rotatedDirection = normalize(mul(direction, _LightMatrix));

                float2 sunUV = rotatedDirection.xy / rotatedDirection.z;
                sunUV = sunUV * (1 - _SunMaskSize) + float2(0.5, -0.5);
                sunUV.y *= -1;
                float4 sunMask = tex2D(_SunMask, saturate(sunUV));

                float sunGradient = skyUV.y * 10 + 1 - _SunGradientHeight;
                float3 sunColor = saturate(lerp(_SunColorTwo.rgb, _SunColorOne.rgb, sunGradient));
                float3 sun = sunColor * sunDisc * sunMask.rgb;

                #ifdef BOOLEAN_PREVENTSTARS_ON
                stars *= 1 - sunMask.r;
                #endif

                float fogUV = skyUV.y * _FogContrast + 1 - _FogHeight;
                float fogFactor = saturate(1 - fogUV) * _FogPower;
                float3 color = lerp(skyColor + stars * _StarColor.rgb, unity_FogColor.rgb, fogFactor);
                color += sun;
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
}
