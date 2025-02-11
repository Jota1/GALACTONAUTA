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
    public class WeatherMakerExtensionScript<T> : MonoBehaviour where T : UnityEngine.MonoBehaviour
    {
        public T TypeScript { get; private set; }

        // executes when a TypeScript object is found
        protected virtual void OnTypeScriptFound() { }

        protected virtual void Awake()
        {
            if (typeof(T) != typeof(UnityEngine.MonoBehaviour))
            {
                // try to find manager object if it doesn't exist
                if (TypeScript == null)
                {
                    System.Type type = typeof(T);
                    TypeScript = UnityEngine.GameObject.FindObjectOfType(type) as T;
                    if (TypeScript == null || TypeScript.GetType() != type || !TypeScript.enabled)
                    {
                        TypeScript = null;
                        return;
                    }
                    OnTypeScriptFound();
                }
            }
        }

        protected virtual void LateUpdate() { }
    }
}
