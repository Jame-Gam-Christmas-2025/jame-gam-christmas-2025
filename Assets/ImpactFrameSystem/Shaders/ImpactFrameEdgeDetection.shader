Shader "Hidden/Impact Frame Edge Detection"
{
    Properties
    {
        _OutlineThickness ("Outline Thickness", Float) = 1
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
        _ImpactIntensity ("Impact Intensity", Range(0, 1)) = 1.0
        _BlurCenter ("Blur Center", Vector) = (0.5, 0.5, 0, 0)
        _BlurStrength ("Blur Strength", Range(0, 1)) = 0.5
        _BlurSamples ("Blur Samples", Range(8, 32)) = 16
        _BlurRadius ("Blur Radius", Range(0, 1)) = 0.5
        _AdditionalBlur ("Additional Blur", Range(0, 5)) = 1
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Range(0.1, 5)) = 1
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.5
        _EdgeStep ("Edge Step Threshold", Range(0, 1)) = 0.5
        _AnimationTime ("Animation Time", Float) = 0
        _AnimationSpeed ("Animation Speed", Range(0, 10)) = 2
        _BlastWorldPosition ("Blast World Position", Vector) = (0, 0, 0, 0)
        _BlastRadius ("Blast Radius", Float) = 5
        _BlastIntensity ("Blast Intensity", Range(0, 1)) = 1
        _BlastEdgeThickness ("Blast Edge Thickness", Range(0.1, 2.0)) = 0.8
        _BlastDebugMode ("Blast Debug Mode", Float) = 0
        _BlastShowSphere ("Show Blast Sphere", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque"
        }

        ZWrite Off
        Cull Off
        Blend Off

        Pass
        {
            Name "IMPACT FRAME EDGE DETECTION"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float _OutlineThickness;
            float4 _OutlineColor;
            float4 _BackgroundColor;
            float _ImpactIntensity;
            float2 _BlurCenter;
            float _BlurStrength;
            float _BlurSamples;
            float _BlurRadius;
            float _AdditionalBlur;
            float _NoiseScale;
            float _NoiseIntensity;
            float _EdgeStep;
            float _AnimationTime;
            float _AnimationSpeed;
            float3 _BlastWorldPosition;
            float _BlastRadius;
            float _BlastIntensity;
            float _BlastEdgeThickness;
            float _BlastDebugMode;
            float _BlastShowSphere;

            #pragma vertex Vert
            #pragma fragment frag

            float RobertsCross(float3 samples[4])
            {
                const float3 difference_1 = samples[1] - samples[2];
                const float3 difference_2 = samples[0] - samples[3];
                return sqrt(dot(difference_1, difference_1) + dot(difference_2, difference_2));
            }

            float RobertsCross(float samples[4])
            {
                const float difference_1 = samples[1] - samples[2];
                const float difference_2 = samples[0] - samples[3];
                return sqrt(difference_1 * difference_1 + difference_2 * difference_2);
            }

            float3 SampleSceneNormalsRemapped(float2 uv)
            {
                return SampleSceneNormals(uv) * 0.5 + 0.5;
            }

            float SampleSceneLuminance(float2 uv)
            {
                float3 color = SampleSceneColor(uv);
                return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
            }

            float GetEdgeAtUV(float2 uv)
            {
                float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                const float half_width_f = floor(_OutlineThickness * 0.5);
                const float half_width_c = ceil(_OutlineThickness * 0.5);

                float2 uvs[4];
                uvs[0] = uv + texel_size * float2(half_width_f, half_width_c) * float2(-1, 1);
                uvs[1] = uv + texel_size * float2(half_width_c, half_width_c) * float2(1, 1);
                uvs[2] = uv + texel_size * float2(half_width_f, half_width_f) * float2(-1, -1);
                uvs[3] = uv + texel_size * float2(half_width_c, half_width_f) * float2(1, -1);

                float3 normal_samples[4];
                float depth_samples[4], luminance_samples[4];

                for (int i = 0; i < 4; i++) {
                    depth_samples[i] = SampleSceneDepth(uvs[i]);
                    normal_samples[i] = SampleSceneNormalsRemapped(uvs[i]);
                    luminance_samples[i] = SampleSceneLuminance(uvs[i]);
                }

                float edge_depth = RobertsCross(depth_samples);
                float edge_normal = RobertsCross(normal_samples);
                float edge_luminance = RobertsCross(luminance_samples);

                float depth_threshold = 1 / 100.0f;
                edge_depth = edge_depth > depth_threshold ? 1 : 0;

                float normal_threshold = 1 / 6.0f;
                edge_normal = edge_normal > normal_threshold ? 1 : 0;

                float luminance_threshold = 1 / 1.0f;
                edge_luminance = edge_luminance > luminance_threshold ? 1 : 0;

                return max(edge_depth, max(edge_normal, edge_luminance));
            }

            float3 ReconstructWorldPosition(float2 uv, float depth)
            {
                float2 ndc = uv * 2.0 - 1.0;
                float4 clipPos = float4(ndc.x, ndc.y, depth, 1.0);
                float4 worldPos = mul(unity_MatrixInvVP, clipPos);
                return worldPos.xyz / worldPos.w;
            }

            float GetBlastSphereEdges(float2 uv)
            {
                if (_BlastIntensity <= 0.0 || _BlastRadius <= 0.001) return 0.0;

                if (_BlastDebugMode > 0.0 && _BlastShowSphere > 0.0)
                {
                    float sceneDepth = SampleSceneDepth(uv);
                    if (sceneDepth >= 0.999) return 0.0;

                    float3 worldPos = ReconstructWorldPosition(uv, sceneDepth);
                    float distanceFromBlast = length(worldPos - _BlastWorldPosition);

                    float sphereBorder = abs(distanceFromBlast - _BlastRadius);
                    if (sphereBorder <= _BlastEdgeThickness)
                    {
                        return 1.0 * _BlastIntensity;
                    }
                    return 0.0;
                }

                float sceneDepth = SampleSceneDepth(uv);

                if (sceneDepth >= 0.999) return 0.0;

                float3 worldPos = ReconstructWorldPosition(uv, sceneDepth);

                float distanceFromBlast = length(worldPos - _BlastWorldPosition);

                float sphereSurface = _BlastRadius;
                float intersectionThickness = _BlastEdgeThickness;
                float halfThickness = intersectionThickness * 0.5;

                float distanceToSurface = abs(distanceFromBlast - sphereSurface);
                if (distanceToSurface > halfThickness) return 0.0;

                float distanceFromCamera = length(worldPos - _WorldSpaceCameraPos);
                float lodThreshold = 50.0;
                if (distanceFromCamera > lodThreshold)
                {
                    float simpleIntersection = 1.0 - smoothstep(0.0, halfThickness, distanceToSurface);
                    return simpleIntersection * _BlastIntensity * 0.5;
                }

                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                float edgeThickness = 2.0;

                float2 offsets[4] = {
                    float2(-edgeThickness, 0) * texelSize,
                    float2(edgeThickness, 0) * texelSize,
                    float2(0, -edgeThickness) * texelSize,
                    float2(0, edgeThickness) * texelSize
                };

                float sphereIntersections[4];
                float currentIntersection = 0.0;

                if (distanceToSurface <= halfThickness)
                {
                    currentIntersection = 1.0;
                }

                [unroll]
                for (int i = 0; i < 4; i++)
                {
                    float2 neighborUV = uv + offsets[i];

                    if (neighborUV.x < 0.0 || neighborUV.x > 1.0 || neighborUV.y < 0.0 || neighborUV.y > 1.0)
                    {
                        sphereIntersections[i] = 0.0;
                        continue;
                    }

                    float neighborDepth = SampleSceneDepth(neighborUV);

                    if (neighborDepth >= 0.999)
                    {
                        sphereIntersections[i] = 0.0;
                        continue;
                    }

                    float3 neighborWorldPos = ReconstructWorldPosition(neighborUV, neighborDepth);
                    float neighborDistanceFromBlast = length(neighborWorldPos - _BlastWorldPosition);
                    float neighborDistanceToSurface = abs(neighborDistanceFromBlast - sphereSurface);

                    sphereIntersections[i] = (neighborDistanceToSurface <= halfThickness) ? 1.0 : 0.0;
                }

                float edge1 = abs(sphereIntersections[0] - sphereIntersections[1]);
                float edge2 = abs(sphereIntersections[2] - sphereIntersections[3]);
                float edgeStrength = max(edge1, edge2);

                float finalEdge = edgeStrength * currentIntersection;

                float falloff = 1.0 - smoothstep(0.0, halfThickness, distanceToSurface);
                finalEdge *= falloff;

                return finalEdge * _BlastIntensity;
            }

            float BlurEdgesOnly(float2 uv, float2 center, float strength, float radius, int samples)
            {
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);

                float blurredEdge = 0.0;
                float blurRadius = 3.0;

                blurredEdge += GetEdgeAtUV(uv + float2(-blurRadius, 0) * texelSize);
                blurredEdge += GetEdgeAtUV(uv + float2(blurRadius, 0) * texelSize);
                blurredEdge += GetEdgeAtUV(uv + float2(0, -blurRadius) * texelSize);
                blurredEdge += GetEdgeAtUV(uv + float2(0, blurRadius) * texelSize);
                blurredEdge += GetEdgeAtUV(uv + float2(-blurRadius, -blurRadius) * texelSize);
                blurredEdge += GetEdgeAtUV(uv + float2(blurRadius, blurRadius) * texelSize);
                blurredEdge += GetEdgeAtUV(uv + float2(-blurRadius, blurRadius) * texelSize);
                blurredEdge += GetEdgeAtUV(uv + float2(blurRadius, -blurRadius) * texelSize);
                blurredEdge += GetEdgeAtUV(uv);
                blurredEdge /= 9.0;

                float radialBlurredEdge = 0.0;
                float2 direction = uv - center;
                direction = normalize(direction);

                for (int i = 0; i < samples; i++)
                {
                    float t = (float)i / samples;
                    float2 offset = direction * t * radius * strength;
                    radialBlurredEdge += GetEdgeAtUV(uv - offset);
                }
                radialBlurredEdge /= samples;

                return max(blurredEdge, radialBlurredEdge);
            }

            float ApplyNoiseToEdges(float edge, float2 uv, float2 enemyCenter, float animationSpeed)
            {
                if (_NoiseIntensity <= 0.0 || edge <= 0.0) return edge;

                float2 pixelToEnemy = enemyCenter - uv;
                float distance = length(pixelToEnemy);

                float angle = atan2(pixelToEnemy.y, pixelToEnemy.x);

                float timeOffset = _AnimationTime * animationSpeed;

                float2 lineUV = float2(distance + timeOffset, angle / (2 * PI) + 0.5) * _NoiseScale;

                float4 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, lineUV);
                float noiseValue = noise.r;

                float noisyEdge = edge * lerp(1.0, noiseValue, _NoiseIntensity);

                return step(_EdgeStep, noisyEdge);
            }

            half4 frag(Varyings IN) : SV_TARGET
            {
                float2 uv = IN.texcoord;
                float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);

                const float half_width_f = floor(_OutlineThickness * 0.5);
                const float half_width_c = ceil(_OutlineThickness * 0.5);

                float2 uvs[4];
                uvs[0] = uv + texel_size * float2(half_width_f, half_width_c) * float2(-1, 1);
                uvs[1] = uv + texel_size * float2(half_width_c, half_width_c) * float2(1, 1);
                uvs[2] = uv + texel_size * float2(half_width_f, half_width_f) * float2(-1, -1);
                uvs[3] = uv + texel_size * float2(half_width_c, half_width_f) * float2(1, -1);

                float3 normal_samples[4];
                float depth_samples[4], luminance_samples[4];

                for (int i = 0; i < 4; i++) {
                    depth_samples[i] = SampleSceneDepth(uvs[i]);
                    normal_samples[i] = SampleSceneNormalsRemapped(uvs[i]);
                    luminance_samples[i] = SampleSceneLuminance(uvs[i]);
                }

                float edge_depth = RobertsCross(depth_samples);
                float edge_normal = RobertsCross(normal_samples);
                float edge_luminance = RobertsCross(luminance_samples);

                float depth_threshold = 1 / 100.0f;
                edge_depth = edge_depth > depth_threshold ? 1 : 0;

                float normal_threshold = 1 / 6.0f;
                edge_normal = edge_normal > normal_threshold ? 1 : 0;

                float luminance_threshold = 1 / 1.0f;
                edge_luminance = edge_luminance > luminance_threshold ? 1 : 0;

                float edge = max(edge_depth, max(edge_normal, edge_luminance));

                float blurredEdge = edge;
                if (_BlurStrength > 0.0)
                {
                    blurredEdge = BlurEdgesOnly(uv, _BlurCenter, _BlurStrength, _BlurRadius, (int)_BlurSamples);
                }

                float finalEdge = ApplyNoiseToEdges(blurredEdge, uv, _BlurCenter, _AnimationSpeed);

                float blastEdges = GetBlastSphereEdges(uv);

                float noisyBlastEdges = ApplyNoiseToEdges(blastEdges, uv, _BlurCenter, _AnimationSpeed);

                if (_BlastIntensity > 0.0 && _BlastRadius > 0.001)
                {
                    float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                    const float half_width_f = floor(_OutlineThickness * 0.5);
                    const float half_width_c = ceil(_OutlineThickness * 0.5);

                    float2 uvs[4];
                    uvs[0] = uv + texel_size * float2(half_width_f, half_width_c) * float2(-1, 1);
                    uvs[1] = uv + texel_size * float2(half_width_c, half_width_c) * float2(1, 1);
                    uvs[2] = uv + texel_size * float2(half_width_f, half_width_f) * float2(-1, -1);
                    uvs[3] = uv + texel_size * float2(half_width_c, half_width_f) * float2(1, -1);

                    float sphere_mask[4];
                    for (int i = 0; i < 4; i++) {
                        float depth = SampleSceneDepth(uvs[i]);
                        if (depth >= 0.999) {
                            sphere_mask[i] = 0.0;
                        } else {
                            float3 worldPos = ReconstructWorldPosition(uvs[i], depth);
                            float distanceFromBlast = length(worldPos - _BlastWorldPosition);
                            float distanceToSphere = abs(distanceFromBlast - _BlastRadius);
                            sphere_mask[i] = (distanceToSphere < _BlastEdgeThickness) ? 1.0 : 0.0;
                        }
                    }

                    float sphere_edge = RobertsCross(sphere_mask);

                    float sphere_threshold = 1 / 6.0f;
                    float rawBlastEdge = sphere_edge > sphere_threshold ? 1.0 : 0.0;

                    noisyBlastEdges = ApplyNoiseToEdges(rawBlastEdge, uv, _BlurCenter, _AnimationSpeed);
                }

                float combinedEdges = saturate(finalEdge + noisyBlastEdges * 0.8);

                float3 originalColor = SampleSceneColor(uv);

                float3 impactBackground = lerp(originalColor, _BackgroundColor.rgb, _ImpactIntensity);
                float3 finalColor = lerp(impactBackground, _OutlineColor.rgb, combinedEdges * _ImpactIntensity);

                float lineAlpha = combinedEdges * _ImpactIntensity;

                return half4(finalColor, lineAlpha);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ANIMATED BLEND"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment fragBlend

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            TEXTURE2D(_TempTexture);
            SAMPLER(sampler_TempTexture);

            float _AnimationTime;
            float _AnimationSpeed;
            float _ImpactIntensity;
            float4 _BackgroundColor;
            float4 _OutlineColor;
            float3 _BlastWorldPosition;
            float _BlastRadius;
            float _BlastIntensity;
            float _BlastEdgeThickness;
            float _BlastDebugMode;
            float _BlastShowSphere;

            half4 fragBlend(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;

                float4 tempColor = SAMPLE_TEXTURE2D(_TempTexture, sampler_TempTexture, uv);
                float lineAlpha = tempColor.a;

                float3 originalColor = SampleSceneColor(uv);

                float impactIntensity = _ImpactIntensity;
                float3 backgroundColor = _BackgroundColor.rgb;
                float3 edgeColor = _OutlineColor.rgb;

                float3 blackBackground = lerp(originalColor, backgroundColor, impactIntensity);

                float pulseAlpha = (sin(_AnimationTime * _AnimationSpeed) * 0.3 + 0.7);
                float thrillingAlpha = lineAlpha * pulseAlpha;

                float3 finalColor = lerp(blackBackground, edgeColor, thrillingAlpha);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
