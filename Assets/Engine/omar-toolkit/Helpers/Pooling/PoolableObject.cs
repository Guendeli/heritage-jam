using UnityEngine;
using UnityEngine.SceneManagement;


namespace Oryx
{
    /// <summary>
    /// Add this component to your prefab to make it poolable. 
    /// </summary>
    [AddComponentMenu( "Oryx/PoolableObject" )]
    public class PoolableObject : MonoBehaviour
    {
        [Tooltip("Specifies the maximum number of objects on the pool")]
        public int maxPoolSize = 10;

        [Tooltip("Specifies the number of objects that will be created on the pool at program start (improves speed of later instantiation)")]
        public int preloadCount = 0;

        [Tooltip("If enabled the pool of deactivated objects will surivive a scene change")]
        public bool doNotDestroyOnLoad = false;

        /// <summary>
        /// If enabled Awake(), Start(), and OnDestroy() messages are sent to the poolable object if the object is set 
        /// active or inactive whenever <see cref="ObjectPoolController.Destroy(GameObject)"/> or 
        /// <see cref="ObjectPoolController.Instantiate(GameObject)"/> is called. <para/>
        /// This way it is simulated that the object really gets instantiated or  destroyed.
        /// </summary>
        public bool sendAwakeStartOnDestroyMessage = true;
        
        public bool sendPoolableActivateDeactivateMessages = false;

        /// <summary>
        /// If enabled reflection gets used to invoke the <c>Awake()</c>, <c>Start()</c>, <c>OnDestroy()</c>, 
        /// <c>OnPoolableObjectActivated()</c> and <c>OnPoolableObjectDeactivated()</c> Methods instead of using the Unity-
        /// Messaging-System. This is useful when objects are instantiated as inactive or deactivated before they are destroyed.
        /// (Unity-Messaging-System works on active components and GameObjects only!)
        /// </summary>
        public bool useReflectionInsteadOfMessages = false;

        internal bool _isInPool = false;

        /// <summary>
        /// if null - Object was not created from ObjectPoolController
        /// </summary>
        internal ObjectPoolController.ObjectPool _pool = null;

        internal int _serialNumber = 0;
        internal int _usageCount = 0;

        //needed when an object gets instantiated deactivated to prevent double awake
        internal bool _awakeJustCalledByUnity = false;
        public bool isPooledInstance
        {
            get
            {
                return _pool != null;
            }
        }

        internal bool _wasInstantiatedByObjectPoolController = false;

        private bool _justInvokingOnDestroy = false;

        protected void Awake()
        {
            _awakeJustCalledByUnity = true;

#if UNITY_EDITOR
            if ( !isPooledInstance && !ObjectPoolController._isDuringInstantiate && !_wasInstantiatedByObjectPoolController )
                Debug.LogWarning( "Poolable object " + name + " was instantiated without ObjectPoolController" );
#endif

        }

        protected void OnDestroy()
        {
            //only if destroy message comes from unity and not from invocation
            if ( !_justInvokingOnDestroy && _pool != null )
            {
                // Poolable object was destroyed by using the default Unity Destroy() function -> Use ObjectPoolController.Destroy() instead
                // This can also happen if objects are automatically deleted by Unity e.g. due to level change or if an object is parented to an object that gets destroyed
                _pool.Remove( this );
            }
        }

        /// <summary>
        /// Gets the object's pool serial number. Each object has a unique serial number. Can be useful for debugging purposes.
        /// </summary>
        public int GetSerialNumber() // each new instance receives a unique serial number
        {
            return _serialNumber;
        }

        /// <summary>
        /// Gets the usage counter which gets increased each time an object is re-used from the pool.
        /// </summary>
        public int GetUsageCount()
        {
            return _usageCount;
        }

        /// <summary>
        /// Moves all poolable objects of this kind (instantiated from the same prefab as this instance) back to the pool. 
        /// </summary>
        public int DeactivateAllPoolableObjectsOfMyKind()
        {
            if ( _pool != null )
            {
                return _pool._SetAllAvailable();
            }
            return 0;
        }

        /// <summary>
        /// Checks if the object is deactivated and in the pool.
        /// </summary>
        public bool IsDeactivated()
        {
            return _isInPool;
        }

        internal void _PutIntoPool()
        {
            if ( _pool == null )
            {
                Debug.LogError( "Tried to put object into pool which was not created with ObjectPoolController", this );
                return;
            }

            if ( _isInPool )
            {
                if ( transform.parent != _pool.poolParent )
                {
                    Debug.LogWarning( "Object was already in pool but parented to Pool-Parent. Reparented.", this );
                    transform.parent = _pool.poolParent;

                    if ( transform.parent != _pool.poolParent )
                    {
                        Debug.LogError( "Object couldn�t be reparented. Deleted" );
                        DestroyImmediate( gameObject );
                    }

                    return;
                }

                Debug.LogWarning( "Object is already in Pool", this );
                return;
            }

            //dont fire callbacks when object is put into pool initially
            if ( !ObjectPoolController._isDuringInstantiate )
            {
                if ( sendAwakeStartOnDestroyMessage )
                {
                    _justInvokingOnDestroy = true;
                    _pool.CallMethodOnObject( gameObject, "OnDestroy", true, true, useReflectionInsteadOfMessages );
                    _justInvokingOnDestroy = false;
                }

                if ( sendPoolableActivateDeactivateMessages )
                    _pool.CallMethodOnObject( gameObject, "OnPoolableObjectDeactivated", true, true, useReflectionInsteadOfMessages );
            }

            _isInPool = true;
            transform.SetParent( _pool.poolParent, true );
            //transform.parent = _pool.poolParent;

            gameObject.SetActive( false );
        }

        internal void TakeFromPool( Transform parent, bool activateObject )
        {
            if ( !_isInPool )
            {
                Debug.LogError( "Tried to take an object from Pool which is not available!", this );
                return;
            }

            _isInPool = false;

            _usageCount++;
            transform.SetParent( parent, true );
            if( parent == null /*&& doNotDestroyOnLoad*/ )
            {
                // make sure that the object is not in the DontDestroyOnLoadScene when taken from pool
                SceneManager.MoveGameObjectToScene( gameObject, SceneManager.GetActiveScene() );
            }
            //transform.parent = parent;

            if ( activateObject )
            {
                //this may be set to true when unity calls Awake after gameObject.SetActive(true);
                _awakeJustCalledByUnity = false;
                gameObject.SetActive( true );

                if ( sendAwakeStartOnDestroyMessage )
                {
                    //when an instance gets activated not the first time Awake() wont be called again. so we call it here via reflection!
                    if ( !_awakeJustCalledByUnity )
                    {
                        _pool.CallMethodOnObject( gameObject, "Awake", true, false, useReflectionInsteadOfMessages );

                        if ( gameObject.activeInHierarchy ) // Awake could deactivate object
                            _pool.CallMethodOnObject( gameObject, "Start", true, false, useReflectionInsteadOfMessages );
                    }
                }

                if ( sendPoolableActivateDeactivateMessages )
                {
                    _pool.CallMethodOnObject( gameObject, "OnPoolableObjectActivated", true, true, useReflectionInsteadOfMessages );
                }
            }
        }
    }
}