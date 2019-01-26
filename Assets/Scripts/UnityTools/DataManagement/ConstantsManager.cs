using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityTools.DataManagement
{
    [CreateAssetMenu(menuName = "UnityTools/DataManagement/ConstantsManager")]
    public class ConstantsManager : ScriptableObject
    {
        [SerializeField]
        private float rotationSpeed;
        [SerializeField]
        private float timeToPressToPickupShell;
        [SerializeField]
        private float baseCrabLife;

        [SerializeField]
        private Shell[] allShellPrefabs;
        [SerializeField]
        private GameObject[] allPropsPrefabs;

        [SerializeField]
        private float minDistanceBetweenMapElements;

        [SerializeField]
        private PressToGetShellUI pressToGetShellUIPrefab;

        #region Properties

        public static float RotationSpeed { get { return instance.rotationSpeed; } }
        public static float TimeToPressToPickupShell { get { return instance.timeToPressToPickupShell; } }
        public static float BaseCrabLife { get { return instance.baseCrabLife; } }

        public static Shell[] AllShellPrefabs { get { return instance.allShellPrefabs; } }
        public static GameObject[] AllPropsPrefabs { get { return instance.allPropsPrefabs; } }

        public static float MinDistanceBetweenMapElements { get { return instance.minDistanceBetweenMapElements; } }

        public static PressToGetShellUI PressToGetShellUIPrefab { get { return instance.pressToGetShellUIPrefab; } }

        #endregion

        private const string assetPath = "Constants";
        private static ConstantsManager _instance;
        private static ConstantsManager instance { get { return _instance ?? (_instance = Resources.Load<ConstantsManager>(assetPath)); } }

        // Methods to enable lookup for a scene that is not the one currently loaded
        private static string sceneToLookup;
        private static bool lookupDifferentScene = false;

        public static void LookupScene(string sceneName)
        {
            sceneToLookup = sceneName;
            lookupDifferentScene = true;
        }

        [Serializable]
        private class FloatOverride : Override<float> { }
        [Serializable]
        private class FloatOverridableConstant : OverridableConstant<float, FloatOverride> { }

        [Serializable]
        private class IntOverride : Override<int> { }
        [Serializable]
        private class IntOverridableConstant : OverridableConstant<int, IntOverride> { }

        // Strings have their own implementation (copied) because of the TextArea attribute
        [Serializable]
        private class StringOverride
        {
            [SerializeField, TextArea(1, 5)]
            private string value;
            [SerializeField]
            private string scene;

            public string Value { get { return value; } }
            public string Scene { get { return scene; } }
        }
        [Serializable]
        private class StringOverridableConstant
        {
            [SerializeField, TextArea(1, 5)]
            private string value;
            public string Value
            {
                get
                {
                    if (overridesDictionary == null)
                    {
                        overridesDictionary = new Dictionary<string, string>();

                        foreach (var o in overrides)
                        {
                            if (overridesDictionary.ContainsKey(o.Scene))
                            {
                                Debug.LogWarning("There are multiple overrides for scene \"" + o.Scene + "\" for some constant");
                                overridesDictionary[o.Scene] = o.Value;
                            }
                            else
                            {
                                overridesDictionary.Add(o.Scene, o.Value);
                            }
                        }
                    }

                    string currentScene = lookupDifferentScene ? sceneToLookup : SceneManager.GetActiveScene().name;
                    lookupDifferentScene = false;

                    if (overridesDictionary.ContainsKey(currentScene))
                    {
                        return overridesDictionary[currentScene];
                    }
                    else
                    {
                        return value;
                    }
                }
            }

            public static implicit operator string(StringOverridableConstant oc)
            {
                return oc.Value;
            }

            [SerializeField]
            private StringOverride[] overrides;

            private Dictionary<string, string> overridesDictionary;
        }

        [Serializable]
        // The double generic trick is to enable serialization
        private class OverridableConstant<T, TOverride> where TOverride : Override<T>
        {
            [SerializeField]
            private T value;
            public T Value
            {
                get
                {
                    if (overridesDictionary == null)
                    {
                        overridesDictionary = new Dictionary<string, T>();

                        foreach (var o in overrides)
                        {
                            if (overridesDictionary.ContainsKey(o.Scene))
                            {
                                Debug.LogWarning("There are multiple overrides for scene \"" + o.Scene + "\" for some constant");
                                overridesDictionary[o.Scene] = o.Value;
                            }
                            else
                            {
                                overridesDictionary.Add(o.Scene, o.Value);
                            }
                        }
                    }

                    string currentScene = lookupDifferentScene ? sceneToLookup : SceneManager.GetActiveScene().name;
                    lookupDifferentScene = false;

                    if (overridesDictionary.ContainsKey(currentScene))
                    {
                        return overridesDictionary[currentScene];
                    }
                    else
                    {
                        return value;
                    }
                }
            }

            public static implicit operator T(OverridableConstant<T, TOverride> oc)
            {
                return oc.Value;
            }

            [SerializeField]
            private TOverride[] overrides;

            private Dictionary<string, T> overridesDictionary;
        }
        [Serializable]
        private class Override<T>
        {
            [SerializeField]
            private T value;
            [SerializeField]
            private string scene;

            public T Value { get { return value; } }
            public string Scene { get { return scene; } }
        }
    }
}