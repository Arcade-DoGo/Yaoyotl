using System;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Namespace that contains custom classes defined for specific purposes
///</summary>

namespace CustomClasses
{
    /// <summary>
    /// Class that lets a data class be serialized into Photon Pun.
    /// </summary>
    /// <remarks>
    /// To use it, the class must inherit from this class, 
    /// and in the Awake method, the next code must be called:
    /// <para>
    ///    <c>PhotonPeer.RegisterType(typeof(MyCustomClass), i, PhotonCustomClass.Serialize, PhotonCustomClass.Deserialize); </c>
    /// </para>
    /// Where:
    /// <list type="bullet">
    ///     <item>
    ///         <term>MyCustomClass</term>
    ///         <description>is the name of the class </description>
    ///     </item>
    ///     <item>
    ///         <term>i</term>
    ///         <description>is the number of the id to register (starts at 0) </description>
    ///     </item>
    /// </list>
    /// </remarks>

    [Serializable] public class PhotonCustomClass
    {
        public byte Id { get; set; }
        public static object Deserialize(byte[] data) => new PhotonCustomClass { Id = data[0] };
        public static byte[] Serialize(object customType)
        {
            var c = (PhotonCustomClass)customType;
            return new byte[] { c.Id };
        }
    }

    /// <summary>
    /// Class that adds a Singleton pattern to a class.
    /// </summary>
    /// <remarks>
    /// To use it, the class must inherit from this class,
    /// and in the Awake method (if extra code is needed), the next code must be called:
    /// <code>
    ///     protected override void Awake()
    ///     {
    ///         base.Awake();
    ///         // extra code in here...
    ///     }
    /// </code>
    /// </remarks>

    public class InstanceClass<T> : MonoBehaviour where T : InstanceClass<T>
    {
        public static T instance;
        protected virtual void Awake()
        {
            // Destroys this object if instance already in the Scene
            if (instance != null && instance != this)
            {
                if(GameManager.usingEditor) Debug.LogWarning("Multiple instances of " + typeof(T) + " found. Destroying the extra instance.");
                Destroy(gameObject);
                return;
            }
            // Saves this object as the instance if its the first and onlyone
            instance = (T)this;
        }
    }

    /// <summary>
    /// Class that adds a Singleton pattern to a class 
    /// that needs MonoBehaviourPunCallbacks from Photon Pun 2
    /// </summary>
    /// <remarks>
    /// Same behaviour as <c>InstanceClass</c>
    /// </remarks>

    public class InstanceOnlineClass<T> : MonoBehaviourPunCallbacks where T : InstanceOnlineClass<T>
    {
        public static T instance;
        protected virtual void Awake()
        {
            // Destroys this object if instance already in the Scene
            if (instance != null && instance != this)
            {
                if(GameManager.usingEditor) Debug.LogWarning("Multiple instances of " + typeof(T) + " found. Destroying the extra instance.");
                Destroy(gameObject);
                return;
            }
            // Saves this object as the instance if its the first and onlyone
            instance = (T)this;
        }
    }
}