using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nuclear_reactor
{
    public class Singltone<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null) Instance = this as T;
            else Destroy(gameObject);
        }
        protected virtual void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
