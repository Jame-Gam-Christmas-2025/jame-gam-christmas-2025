using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ImpactFrameSystem
{
    public class ImpactFrameManager : MonoBehaviour
    {
        [Header("Impact Frame Settings")]
        public float defaultDuration = 0.2f;
        public float defaultIntensity = 1.0f;
        public AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public Color edgeColor = Color.white;
        public Color backgroundColor = Color.black;
        public float edgeThickness = 3f;

        [Header("Radial Blur Settings")]
        public bool useRadialBlur = true;
        public float blurStrength = 0.5f;
        [Range(8, 32)] public int blurSamples = 16;
        public float blurRadius = 0.5f;
        public float additionalBlur = 1.0f;

        [Header("Noise Settings")]
        public Texture2D noiseTexture;
        public float noiseScale = 1.0f;
        [Range(0, 1)] public float noiseIntensity = 0.5f;
        [Range(0, 1)] public float edgeStepThreshold = 0.5f;

        [Header("Animation Settings")]
        [Range(0, 10)] public float animationSpeed = 2.0f;

        [Header("Blast Effect Settings")]
        public bool useBlastEffect = true;
        public float blastDelay = 0.1f;
        public float blastRadius = 5.0f;
        public float blastDuration = 1.0f;
        public AnimationCurve blastScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Range(0, 1)] public float blastIntensity = 1.0f;
        [Range(0.1f, 2.0f)] public float blastEdgeThickness = 0.8f;

        [Header("Test Button (Play Mode Only)")]
        [SerializeField] private bool testImpactFrame = false;

        private bool isImpactFrameActive = false;
        private Coroutine currentEffectCoroutine;
        private float blastStartTime;
        private Vector3 blastWorldPosition;

        public static ImpactFrameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            EnableEdgeDetection(false, Vector2.one * 0.5f);
            SetupEdgeDetection();
        }

        private void SetupEdgeDetection()
        {
            var renderPipelineAsset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            if (renderPipelineAsset != null)
            {
                Debug.Log("[ImpactFrame] URP Asset found, edge detection setup ready");
            }
            else
            {
                Debug.LogError("[ImpactFrame] URP Asset not found! Make sure you're using URP.");
            }
        }

        private void Update()
        {
            if (!Application.isPlaying) return;

            if (testImpactFrame)
            {
                testImpactFrame = false;
                Debug.Log("[ImpactFrame] Test button clicked - triggering impact frame");

                Camera mainCamera = Camera.main;
                Vector3 testPosition = mainCamera != null ? mainCamera.transform.position + mainCamera.transform.forward * 5f : Vector3.forward * 5f;

                TriggerImpactFrame(defaultDuration, defaultIntensity, testPosition);
            }
        }

        public void TriggerImpactFrame(float duration = -1f, float intensity = -1f, Vector3 worldPosition = default)
        {
            if (isImpactFrameActive)
            {
                StopCurrentEffect();
            }

            if (duration <= 0) duration = defaultDuration;
            if (intensity <= 0) intensity = defaultIntensity;

            Vector2 blurCenter = Vector2.one * 0.5f;
            if (worldPosition != Vector3.zero)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
                    blurCenter = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
                }
            }

            blastStartTime = Time.time;
            blastWorldPosition = worldPosition;
            EnableEdgeDetection(true, blurCenter);
            currentEffectCoroutine = StartCoroutine(ExecuteImpactFrame(duration, intensity));
        }

        public void StopCurrentEffect()
        {
            if (currentEffectCoroutine != null)
            {
                StopCoroutine(currentEffectCoroutine);
                currentEffectCoroutine = null;
            }

            Shader.SetGlobalFloat("_BlastIntensity", 0.0f);
            Shader.SetGlobalFloat("_BlastRadius", 0.0f);
            EnableEdgeDetection(false, Vector2.one * 0.5f);
            isImpactFrameActive = false;
        }

        public bool IsActive => isImpactFrameActive;

        private void EnableEdgeDetection(bool enable, Vector2 blurCenter)
        {
            if (enable)
            {
                Shader.SetGlobalFloat("_ImpactFrameActive", 1.0f);
                Shader.SetGlobalColor("_ImpactFrameEdgeColor", edgeColor);
                Shader.SetGlobalColor("_ImpactFrameBackgroundColor", backgroundColor);
                Shader.SetGlobalFloat("_ImpactFrameEdgeThickness", edgeThickness);
                Shader.SetGlobalFloat("_ImpactFrameIntensity", 1.0f);

                if (useRadialBlur)
                {
                    Shader.SetGlobalVector("_BlurCenter", new Vector4(blurCenter.x, blurCenter.y, 0, 0));
                    Shader.SetGlobalFloat("_BlurStrength", blurStrength);
                    Shader.SetGlobalFloat("_BlurSamples", blurSamples);
                    Shader.SetGlobalFloat("_BlurRadius", blurRadius);
                    Shader.SetGlobalFloat("_AdditionalBlur", additionalBlur);
                }
                else
                {
                    Shader.SetGlobalFloat("_BlurStrength", 0.0f);
                }

                if (noiseTexture != null)
                {
                    Shader.SetGlobalTexture("_NoiseTex", noiseTexture);
                    Shader.SetGlobalFloat("_NoiseScale", noiseScale);
                    Shader.SetGlobalFloat("_NoiseIntensity", noiseIntensity);
                    Shader.SetGlobalFloat("_EdgeStep", edgeStepThreshold);
                }
                else
                {
                    Shader.SetGlobalFloat("_NoiseIntensity", 0.0f);
                }

                Shader.SetGlobalFloat("_AnimationSpeed", animationSpeed);
                Shader.SetGlobalFloat("_AnimationTime", Time.time);

                if (useBlastEffect)
                {
                    float timeSinceStart = Time.time - blastStartTime;
                    float blastProgress = 0.0f;
                    float currentBlastScale = 0.0f;

                    if (timeSinceStart >= blastDelay)
                    {
                        float blastTime = timeSinceStart - blastDelay;
                        blastProgress = Mathf.Clamp01(blastTime / blastDuration);
                        currentBlastScale = blastScaleCurve.Evaluate(blastProgress) * blastRadius;
                    }

                    Shader.SetGlobalVector("_BlastWorldPosition", new Vector4(blastWorldPosition.x, blastWorldPosition.y, blastWorldPosition.z, 0));
                    Shader.SetGlobalFloat("_BlastRadius", currentBlastScale);
                    Shader.SetGlobalFloat("_BlastIntensity", blastIntensity);
                    Shader.SetGlobalFloat("_BlastEdgeThickness", blastEdgeThickness);
                    Shader.SetGlobalFloat("_BlastDebugMode", 0.0f);
                    Shader.SetGlobalFloat("_BlastShowSphere", 0.0f);
                }
                else
                {
                    Shader.SetGlobalFloat("_BlastIntensity", 0.0f);
                }
            }
            else
            {
                Shader.SetGlobalFloat("_ImpactFrameActive", 0.0f);
                Shader.SetGlobalFloat("_ImpactFrameIntensity", 0.0f);
                Shader.SetGlobalFloat("_BlurStrength", 0.0f);
            }
        }

        private IEnumerator ExecuteImpactFrame(float duration, float maxIntensity)
        {
            isImpactFrameActive = true;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float normalizedTime = elapsedTime / duration;
                float currentIntensity = intensityCurve.Evaluate(normalizedTime) * maxIntensity;

                Shader.SetGlobalFloat("_ImpactFrameIntensity", currentIntensity);
                Shader.SetGlobalFloat("_AnimationTime", Time.time);

                if (useBlastEffect)
                {
                    float timeSinceStart = Time.time - blastStartTime;
                    float blastProgress = 0.0f;
                    float currentBlastScale = 0.0f;

                    if (timeSinceStart >= blastDelay)
                    {
                        float blastTime = timeSinceStart - blastDelay;
                        blastProgress = Mathf.Clamp01(blastTime / blastDuration);
                        float curveValue = Mathf.Clamp01(blastScaleCurve.Evaluate(blastProgress));
                        currentBlastScale = curveValue * blastRadius;
                    }

                    Shader.SetGlobalVector("_BlastWorldPosition", new Vector4(blastWorldPosition.x, blastWorldPosition.y, blastWorldPosition.z, 0));
                    Shader.SetGlobalFloat("_BlastRadius", currentBlastScale);
                    Shader.SetGlobalFloat("_BlastIntensity", blastIntensity);
                    Shader.SetGlobalFloat("_BlastEdgeThickness", blastEdgeThickness);
                }

                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            StopCurrentEffect();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            EnableEdgeDetection(false, Vector2.one * 0.5f);
        }
    }
}
