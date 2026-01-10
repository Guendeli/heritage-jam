using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Oryx
{
    /// <summary>
    /// A static class used to pool creation and destruction of poolable objects.
    /// </summary>
    static public class ObjectPoolController
    {
        const string objectPoolsParentName = "ObjectPools";
        const string persistentObjectPoolsParentName = "PersistentObjectPools";

        static Transform poolsParent;
        static Transform persistentPoolsParent;

        static public bool isDuringPreload
        {
            get;
            private set;
        }
        

        /// <summary>
        /// Retrieves an instance of the specified prefab. Either returns a new instance or it claims an instance 
        /// from the pool.
        /// </summary>
        static public GameObject Instantiate( GameObject prefab, Transform parent = null )
        {
            PoolableObject prefabPool = prefab.GetComponent<PoolableObject>();
            if ( prefabPool == null )
            {
                //Debug.LogWarning( "Object " + prefab.name + " not poolable " );
                return ( GameObject ) _InstantiateGameObject( prefab, Vector3.zero, Quaternion.identity, parent ); // prefab not pooled, instantiate normally
            }

            GameObject go = _GetPool( prefabPool ).GetPooledInstance( null, null, prefab.activeSelf, parent );
            return go ?? InstantiateWithoutPool( prefab, parent );
        }

        /// <summary>
        /// Retrieves an instance of the specified prefab. Either returns a new instance or it claims an instance
        /// from the pool.
        /// </summary>
        static public GameObject Instantiate( GameObject prefab, Vector3 position, Quaternion quaternion, Transform parent = null )
        {
            PoolableObject prefabPool = prefab.GetComponent<PoolableObject>();
            if ( prefabPool == null )
            {
                // no warning displayed by design because this allows to decide later if the object will be poolable or not
                // Debug.LogWarning( "Object " + prefab.name + " not poolable "); 
                return ( GameObject ) _InstantiateGameObject( prefab, position, quaternion, parent ); // prefab not pooled, instantiate normally
            }

            GameObject go = _GetPool( prefabPool ).GetPooledInstance( position, quaternion, prefab.activeSelf, parent );
            return go ?? InstantiateWithoutPool( prefab, position, quaternion, parent );
        }

        /// <summary>
        /// Instantiates the specified prefab without using pooling.
        /// from the pool.
        /// </summary>
        static public GameObject InstantiateWithoutPool( GameObject prefab, Transform parent = null )
        {
            return InstantiateWithoutPool( prefab, Vector3.zero, Quaternion.identity, parent );
        }

        /// <summary>
        /// Instantiates the specified prefab without using pooling.
        /// from the pool.
        static public GameObject InstantiateWithoutPool( GameObject prefab, Vector3 position, Quaternion quaternion, Transform parent = null )
        {
            _isDuringInstantiate = true;
            GameObject go = _InstantiateGameObject( prefab, position, quaternion, parent ); // prefab not pooled, instantiate normally
            _isDuringInstantiate = false;

            PoolableObject pool = go.GetComponent<PoolableObject>();
            if ( pool != null )
            {
                pool._wasInstantiatedByObjectPoolController = true;
                Component.Destroy( pool );
            }

            return go;
        }

        static GameObject _InstantiateGameObject( GameObject prefab, Vector3 position, Quaternion rotation, Transform parent )
        {
            var go = ( GameObject ) GameObject.Instantiate( prefab, position, rotation, parent );
            return go;
        }

        /// <summary>
        /// Destroys the specified game object, respectively sets the object inactive and adds it to the pool.
        /// </summary>
        static public bool Destroy( GameObject obj ) // destroys poolable and none-poolable objects. Destroys poolable children correctly
        {
            return _DetachChildrenAndDestroy( obj.transform, false );
        }

        /// <summary>
        /// Destroys the specified game object, respectively sets the object inactive and adds it to the pool.
        /// </summary>
        static public void DestroyImmediate( GameObject obj ) // destroys poolable and none-poolable objects. Destroys poolable children correctly
        {
            _DetachChildrenAndDestroy( obj.transform, true );
        }

        /// <summary>
        /// Preloads as many instances to the pool so that there are at least as many as
        /// specified in <see cref="PoolableObject.preloadCount"/>. 
        /// </summary>
        static public void Preload( GameObject prefab ) // adds as many instances to the prefab pool as specified in the PoolableObject
        {
            PoolableObject poolObj = prefab.GetComponent<PoolableObject>();
            if ( poolObj == null )
            {
                Debug.LogWarning( "Can not preload because prefab '" + prefab.name + "' is not poolable" );
                return;
            }

            var pool = _GetPool( poolObj );

            //check how much Objects need to be preloaded
            int delta = poolObj.preloadCount - pool.GetObjectCount();
            if ( delta <= 0 )
                return;

            isDuringPreload = true;

            bool preloadActive = prefab.activeSelf;

            try
            {
                for ( int i = 0; i < delta; i++ )
                {
                    //dont use prefab.activeSelf because this may change inside Preloadinstance. use the cached value "preloadActive"
                    pool.PreloadInstance( preloadActive );
                }
            }
            finally
            {
                isDuringPreload = false;
            }

            //Debug.Log( "preloaded: " + prefab.name + " " + poolObj.preloadCount + " times" );
        }
        

        internal static int _globalSerialNumber = 0;
        internal static bool _isDuringInstantiate = false;

        internal class ObjectPool
        {
            private List<PoolableObject> _pool;
            private GameObject _prefab;
            private PoolableObject _poolableObjectComponent;

            private Transform _poolParent;
            internal Transform poolParent
            {
                get
                {
                    _ValidatePoolParentDummy();
                    return _poolParent;
                }
            }

            public ObjectPool( GameObject prefab )
            {
                this._prefab = prefab;
                this._poolableObjectComponent = prefab.GetComponent<PoolableObject>();
            }

            private void _ValidatePooledObjectDataContainer()
            {
                if ( _pool == null )
                {
                    _pool = new List<PoolableObject>();
                    _ValidatePoolParentDummy();
                }
            }

            private void _ValidatePoolParentDummy()
            {
                if ( _poolParent )
                    return;

                var isPersistent = _poolableObjectComponent.doNotDestroyOnLoad;

                if ( poolsParent == null && !isPersistent )
                {
                    var poolsParentGameObject = GameObject.Find( objectPoolsParentName );

                    if ( poolsParentGameObject == null )
                        poolsParentGameObject = new GameObject( objectPoolsParentName );

                    poolsParent = poolsParentGameObject.transform;
                }

                if ( persistentPoolsParent == null && isPersistent )
                {
                    var persistentPoolsParentGameObject = GameObject.Find( persistentObjectPoolsParentName );

                    if ( persistentPoolsParentGameObject == null )
                        persistentPoolsParentGameObject = new GameObject( persistentObjectPoolsParentName );

                    GameObject.DontDestroyOnLoad( persistentPoolsParentGameObject );

                    persistentPoolsParent = persistentPoolsParentGameObject.transform;
                }

                var relevantPoolsParent = poolsParent;

                if ( isPersistent )
                    relevantPoolsParent = persistentPoolsParent;

                var poolParentDummyGameObject = new GameObject( "POOL:" + _poolableObjectComponent.name );
                _poolParent = poolParentDummyGameObject.transform;
                _poolParent.SetParent( relevantPoolsParent, true );

                poolParentDummyGameObject.SetActive( false );
            }

            internal void Remove( PoolableObject poolObj )
            {
                _pool.Remove( poolObj );
            }

            internal int GetObjectCount()
            {
                return _pool == null ? 0 : _pool.Count;
            }

            internal GameObject GetPooledInstance( Vector3? position, Quaternion? rotation, bool activateObject, Transform parent = null )
            {
                _ValidatePooledObjectDataContainer();

                PoolableObject instance = null;

                for ( int i = 0; i < _pool.Count; i++ )
                {
                    var pooledElement = _pool.ElementAt( i );

                    if ( pooledElement == null ) // can happen e.g. at scene loads, so we need to clean up
                    {
                        _pool.RemoveAt( i-- );
                        continue;
                    }

                    if ( pooledElement._isInPool )
                    {
                        instance = pooledElement;

                        var transform = pooledElement.transform;
                        transform.position = ( position != null ) ? ( Vector3 ) position : _poolableObjectComponent.transform.position;
                        transform.rotation = ( rotation != null ) ? ( Quaternion ) rotation : _poolableObjectComponent.transform.rotation;
                        transform.localScale = _poolableObjectComponent.transform.localScale;
                        break;
                    }
                }

                if ( instance == null && _pool.Count < _poolableObjectComponent.maxPoolSize ) //create and return new element
                {
                    instance = _NewPooledInstance( position, rotation, activateObject, false );
                    instance.transform.SetParent( parent, true ); //instance.transform.parent = parent;
                    return instance.gameObject;
                }

                if ( instance != null )
                {
                    instance.TakeFromPool( parent, activateObject );
                    return instance.gameObject;
                }
                else
                    return null;
            }

            internal PoolableObject PreloadInstance( bool preloadActive )
            {
                _ValidatePooledObjectDataContainer();

                PoolableObject poolObj = _NewPooledInstance( null, null, preloadActive, true );

                return poolObj;
            }

            private PoolableObject _NewPooledInstance( Vector3? position, Quaternion? rotation, bool createActive, bool addToPool )
            {
                _isDuringInstantiate = true;

                _prefab.SetActive( false );

                GameObject go = ( GameObject ) GameObject.Instantiate(
                    _prefab,
                    position ?? Vector3.zero,
                    rotation ?? Quaternion.identity
                    );

                _prefab.SetActive( true );

                PoolableObject poolObj = go.GetComponent<PoolableObject>();

                poolObj._pool = this;
                poolObj._serialNumber = ++_globalSerialNumber;
                poolObj.name += poolObj._serialNumber;
                poolObj._wasInstantiatedByObjectPoolController = true;

                _pool.Add( poolObj );

                if ( addToPool )
                {
                    poolObj._PutIntoPool();
                }
                else
                {
                    poolObj._usageCount++;

                    if ( createActive )
                    {
                        go.SetActive( true );

                        if ( poolObj.sendPoolableActivateDeactivateMessages )
                        {
                            CallMethodOnObject( poolObj.gameObject, "OnPoolableObjectActivated", true, true, poolObj.useReflectionInsteadOfMessages );
                        }
                    }
                }

                _isDuringInstantiate = false;

                return poolObj;
            }

            /// <summary>
            /// Deactivate all active pooled objects
            /// </summary>
            internal int _SetAllAvailable()
            {
                int count = 0;
                for ( int i = 0; i < _pool.Count; i++ )
                {
                    var element = _pool.ElementAt( i );

                    if ( element != null && !element._isInPool )
                    {
                        element._PutIntoPool();
                        count++;
                    }
                }
                return count;
            }

            internal void CallMethodOnObject( GameObject obj, string method, bool includeChildren, bool includeInactive, bool useReflection )
            {
                if ( useReflection )
                {
                    if ( includeChildren )
                        obj.InvokeMethodInChildren( method, includeInactive );
                    else
                        obj.InvokeMethod( method, includeInactive );
                }
                else
                {
                    if ( !obj.activeInHierarchy )
                        Debug.LogWarning( "Tried to call method \"" + method + "\" on an inactive GameObject using Unity-Messaging-System. This only works on active GameObjects and Components! Check \"useReflectionInsteadOfMessages\"!", obj );

                    if ( includeChildren )
                        obj.BroadcastMessage( method, null, SendMessageOptions.DontRequireReceiver );
                    else
                        obj.SendMessage( method, null, SendMessageOptions.DontRequireReceiver );
                }
            }
        }

        static private Dictionary<int, ObjectPool> _pools = new Dictionary<int, ObjectPool>();

        static internal ObjectPool _GetPool( PoolableObject prefabPoolComponent )
        {
            ObjectPool pool;

            GameObject prefab = prefabPoolComponent.gameObject;

            var instanceID = prefab.GetInstanceID();
            if ( !_pools.TryGetValue( instanceID, out pool ) )
            {
                pool = new ObjectPool( prefab );
                _pools.Add( instanceID, pool );
            }

            return pool;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="destroyImmediate"></param>
        /// <returns>If old Hierarchy was changed. To prevent Problems when iterating children.</returns>
        static private bool _DetachChildrenAndDestroy( Transform transform, bool destroyImmediate )
        {
            var po = transform.GetComponent<PoolableObject>();

            if ( transform.childCount > 0 )
            {
                List<PoolableObject> poolableChilds = new List<PoolableObject>();
                transform.GetComponentsInChildren<PoolableObject>( true, poolableChilds );

                if ( po != null )
                    poolableChilds.Remove( po );

                //first destroy all poolable childs.
                for ( int i = 0; i < poolableChilds.Count; i++ )
                {
                    if ( poolableChilds[ i ] == null || poolableChilds[ i ]._isInPool )
                        continue; //can happen when a poolable is a child of another poolable

                    if ( destroyImmediate )
                        ObjectPoolController.DestroyImmediate( poolableChilds[ i ].gameObject );
                    else
                        ObjectPoolController.Destroy( poolableChilds[ i ].gameObject );
                }
            }

            if ( po != null )
            {
                //move poolable Object to pool
                po._PutIntoPool();
                return true;
            }
            else
            {
                //destroy non-poolable object itself
                if ( destroyImmediate )
                {
                    GameObject.DestroyImmediate( transform.gameObject );
                    return true;
                }
                else
                {
                    GameObject.Destroy( transform.gameObject );
                    return false;
                }
            }
        }
    }
}