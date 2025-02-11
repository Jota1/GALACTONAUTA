//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store at https://www.assetstore.unity3d.com/en/#!/content/60955?aid=1011lGnL. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace DigitalRuby.WeatherMaker
{
    [CreateAssetMenu(fileName = "WeatherMakerFullScreenFogProfile", menuName = "WeatherMaker/Full Screen Fog Profile", order = 70)]
    [System.Serializable]
    public class WeatherMakerFullScreenFogProfileScript : WeatherMakerFogProfileScript
    {
        [Header("Full screen fog - thresholds")]
        [Tooltip("Fog height. Set to 0 for unlimited height.")]
        [Range(0.0f, 5000.0f)]
        public float FogHeight = 0.0f;

        [Header("Full screen fog - screen space shafts (all dir lights).")]
        [Tooltip("The number of shaft samples. Set to 0 to disable sun shafts.")]
        [Range(0, 100)]
        public int SunShaftSampleCount = 0;

        [Tooltip("Sun shaft spread (0 - 1).")]
        [Range(0.0f, 1.0f)]
        public float SunShaftSpread = 0.65f;

        [Tooltip("Increases the sun shaft brightness")]
        [Range(0.0f, 1.0f)]
        public float SunShaftBrightness = 0.075f;

        [Tooltip("Combined with each ray march, this determines how much light is accumulated each step.")]
        [Range(0.0f, 100.0f)]
        public float SunShaftStepMultiplier = 21.0f;

        [Tooltip("Determines light fall-off from start of sun shaft. Set to 1 for no fall-off.")]
        [Range(0.5f, 1.0f)]
        public float SunShaftDecay = 0.97f;

        [Tooltip("Sun shaft tint color. Alpha value determines tint intensity.")]
        public Color SunShaftTintColor = Color.white;

        [Tooltip("Controls dithering intensity of sun shafts.")]
        [Range(-1.0f, 1.0f)]
        public float SunShaftDither = 0.4f;

        [Tooltip("Intensity of background color. Set to 0 to not blend in the background at all on the shafts.")]
        [Range(0.0f, 10.0f)]
        public float SunShaftBackgroundColorMultiplier;

        [Tooltip("Tint of background color")]
        public Color SunShaftBackgroundTintColor = Color.white;

        [Tooltip("Controls dithering appearance of sun shafts.")]
        public Vector4 SunShaftDitherMagic = new Vector4(2.34325f, 5.235345f, 1024.0f, 1024.0f);

        /// <summary>
        /// Update full screen fog material properties
        /// </summary>
        /// <param name="material">Fog material</param>
        /// <param name="camera">Camera</param>
        /// <param name="global">Whether to set global shader properties</param>
        public override void UpdateMaterialProperties(Material material, Camera camera, bool global)
        {
            if (WeatherMakerLightManagerScript.Instance == null)
            {
                return;
            }

            base.UpdateMaterialProperties(material, camera, global);

            // reduce shadow strength as the fog blocks out dir lights
            float h;
            float m = Mathf.Pow(MaxFogFactor, 3.0f);
            const float p = 0.005f;
            float shadowMultiplier;

            switch (FogMode)
            {
                case WeatherMakerFogMode.Constant:
                    shadowMultiplier = 1.0f - (Mathf.Min(1.0f, fogDensity * 1.2f * m));
                    FogIntensityMultiplier = 1.0f - (fogDensity * m);
                    break;

                case WeatherMakerFogMode.Linear:
                    h = (FogHeight < Mathf.Epsilon ? 1000.0f : FogHeight) * m;
                    shadowMultiplier = 1.0f - (fogDensity * 16.0f * h * FogShadowStrengthFactor);
                    FogIntensityMultiplier = 1.0f - (fogDensity * 2.0f * h * p);
                    break;

                case WeatherMakerFogMode.Exponential:
                    h = (FogHeight < Mathf.Epsilon ? 1000.0f : FogHeight) * 2.0f * m;
                    shadowMultiplier = 1.0f - (Mathf.Min(1.0f, Mathf.Pow(fogDensity * 32.0f * h * FogShadowStrengthFactor, 0.5f)));
                    FogIntensityMultiplier = 1.0f - (fogDensity * 4.0f * h * p);
                    break;

                case WeatherMakerFogMode.ExponentialSquared:
                    h = (FogHeight < Mathf.Epsilon ? 1000.0f : FogHeight) * 4.0f * m;
                    shadowMultiplier = 1.0f - (Mathf.Min(1.0f, Mathf.Pow(fogDensity * 64.0f * h * FogShadowStrengthFactor, 0.5f)));
                    FogIntensityMultiplier = 1.0f - (fogDensity * 8.0f * h * p);
                    break;

                default:
                    shadowMultiplier = 1.0f;
                    FogIntensityMultiplier = 1.0f;
                    break;
            }

            FogGlobalShadow = shadowMultiplier = Mathf.Clamp(shadowMultiplier * 1.3f, 0.2f, 1.0f);
            Shader.SetGlobalFloat(WMS._WeatherMakerFogGlobalShadow, FogGlobalShadow);

            float fogHeight = Mathf.Max(0.0f, FogHeight);
            if (global)
            {
                Shader.SetGlobalFloat(WMS._WeatherMakerFogHeight, fogHeight);
            }
            material.SetFloat(WMS._WeatherMakerFogHeight, fogHeight);

            // check if shafts are disabled
            if (SunShaftSampleCount <= 0 ||
                FogDensity <= 0.0f ||
                SunShaftBrightness <= 0.0f ||
                SunShaftTintColor.a <= 0.0f)
            {
                material.SetInt(WMS._WeatherMakerFogSunShaftMode, 0);
                return;
            }

            // check each dir light to see if it can do shafts
            bool atLeastOneLightHasShafts = false;
            foreach (WeatherMakerCelestialObjectScript obj in WeatherMakerLightManagerScript.Instance.Suns.Union(WeatherMakerLightManagerScript.Instance.Moons))
            {
                if (obj != null && obj.OrbitTypeIsPerspective && obj.LightIsOn && obj.ViewportPosition.z > 0.0f)
                {
                    atLeastOneLightHasShafts = true;
                    break;
                }
            }

            if (atLeastOneLightHasShafts)
            {
                // sun shafts are visible
                material.SetInt(WMS._WeatherMakerFogSunShaftMode, 1);
                bool gamma = (QualitySettings.activeColorSpace == ColorSpace.Gamma);
                float scatterCover = (WeatherMakerFullScreenCloudsScript.Instance != null && WeatherMakerFullScreenCloudsScript.Instance.enabled && WeatherMakerFullScreenCloudsScript.Instance.CloudProfile != null ? WeatherMakerFullScreenCloudsScript.Instance.CloudProfile.CloudCoverTotal : 0.0f);
                float brightness = SunShaftBrightness * (gamma ? 1.0f : 0.33f) * Mathf.Max(0.0f, 1.0f - (scatterCover * 1.5f));
                material.SetVector(WMS._WeatherMakerFogSunShaftsParam1, new Vector4(SunShaftSpread / (float)SunShaftSampleCount, (float)SunShaftSampleCount, brightness, 1.0f / (float)SunShaftSampleCount));
                material.SetVector(WMS._WeatherMakerFogSunShaftsParam2, new Vector4(SunShaftStepMultiplier, SunShaftDecay, SunShaftDither, 0.0f));
                material.SetVector(WMS._WeatherMakerFogSunShaftsTintColor, new Vector4(SunShaftTintColor.r * SunShaftTintColor.a, SunShaftTintColor.g * SunShaftTintColor.a,
                    SunShaftTintColor.b * SunShaftTintColor.a, SunShaftTintColor.a));
                material.SetVector(WMS._WeatherMakerFogSunShaftsDitherMagic, new Vector4(SunShaftDitherMagic.x * Screen.width , SunShaftDitherMagic.y * Screen.height, SunShaftDitherMagic.z * Screen.width, SunShaftDitherMagic.w * Screen.height));
                material.SetFloat(WMS._WeatherMakerFogSunShaftsBackgroundIntensity, SunShaftBackgroundColorMultiplier);
                material.SetColor(WMS._WeatherMakerFogSunShaftsBackgroundTintColor, SunShaftBackgroundTintColor);
            }
            else
            {
                // sun shafts not visible
                material.SetInt(WMS._WeatherMakerFogSunShaftMode, 0);
            }
        }

        public float FogGlobalShadow { get; private set; }

        private float fogIntensityMultiplier = 1.0f;
        public float FogIntensityMultiplier { get { return fogIntensityMultiplier; } set { fogIntensityMultiplier = value; } }
    }
}
