﻿//
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
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Fog manager interface
    /// </summary>
    public interface IFogManager : IWeatherMakerManager
    {
        // TODO: Expose things like fog density
    }

    /// <summary>
    /// Fog manager default implementation
    /// </summary>
    public class WeatherMakerFogManagerScript : MonoBehaviour, IFogManager
    {
        [Header("Dependencies")]
        [Tooltip("Full screen fog script")]
        public WeatherMakerFullScreenFogScript FogScript;

        private void OnEnable()
        {
            WeatherMakerScript.EnsureInstance(this, ref instance);
        }

        private void OnDisable()
        {
            
        }

        private void LateUpdate()
        {
        }

        private void OnDestroy()
        {
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        public void WeatherProfileChanged(WeatherMakerProfileScript oldProfile, WeatherMakerProfileScript newProfile, float transitionDelay, float transitionDuration)
        {
            FogScript.ShowFogAnimated(newProfile.FogProfile, transitionDelay, transitionDuration);
        }

        private static WeatherMakerFogManagerScript instance;
        /// <summary>
        /// Shared instance of fog manager script
        /// </summary>
        public static WeatherMakerFogManagerScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance); }
        }
    }
}
