using UnityEngine;

namespace Oryx
{
    public class UnitySingleton<T>
        where T : MonoBehaviour
    {
        static T _instance;

        static internal GameObject _autoCreatePrefab;
        static private int _GlobalInstanceCount = 0;
        static private bool _awakeSingletonCalled = false;


        static public T GetSingleton( bool throwErrorIfNotFound, bool autoCreate, bool searchInObjectHierarchy = true )
        {
            if ( !_instance ) 
            {
                T component = null;
                if ( searchInObjectHierarchy )
                {
                    var components = GameObject.FindObjectsOfType<T>();
                    for ( int i = 0; i < components.Length; i++ )
                    {
                        var sb = components[ i ] as ISingletonMonoBehaviour;
                        if ( sb != null && sb.isSingletonObject )
                        {
                            component = components[ i ];
                            break;
                        }
                    }
                }
                if ( !component )
                {
                    if ( autoCreate && _autoCreatePrefab != null )
                    {
                        GameObject go = (GameObject) GameObject.Instantiate( _autoCreatePrefab );
                        go.name = _autoCreatePrefab.name; // removes "(clone)"

                        var newComponent = GameObject.FindObjectOfType<T>();

                        if ( !newComponent )
                        {
                            Debug.LogError( "Auto created object does not have component " + typeof( T ).Name );
                            return null;
                        }
                    }
                    else
                    {
                        if ( throwErrorIfNotFound )
                        {
                            Debug.LogError( "No singleton component " + typeof( T ).Name + " found in the scene." );
                        }
                        return null;
                    }
                }
                else
                {
                    _AwakeSingleton( component as T );
                }

                _instance = component;
            }

            return _instance;
        }

        private UnitySingleton()
        {
        }

        static internal void _Awake( T instance )
        {
            _GlobalInstanceCount++;
            if ( _GlobalInstanceCount > 1 )
            {
                Debug.LogError( "More than one instance of SingletonMonoBehaviour " + typeof( T ).Name );
            }
            else
                _instance = instance;

            _AwakeSingleton( instance as T );
        }

        static internal void _Destroy()
        {
            if ( _GlobalInstanceCount > 0 )
            {
                _GlobalInstanceCount--;
                if ( _GlobalInstanceCount == 0 )
                {
                    _awakeSingletonCalled = false;
                    _instance = null;
                }
            }
        }

        static private void _AwakeSingleton( T instance )
        {
            if ( !_awakeSingletonCalled )
            {
                _awakeSingletonCalled = true;
                instance.SendMessage( "AwakeSingleton", SendMessageOptions.DontRequireReceiver );
            }
        }
    }


    public interface ISingletonMonoBehaviour
    {
        bool isSingletonObject { get; }
    }

    /// <summary>
    /// Provides singleton-like access to a unique instance of a MonoBehaviour. <para/>
    /// </summary>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour, ISingletonMonoBehaviour
            //where T : SingletonMonoBehaviour<T>
            where T : MonoBehaviour
    {

        
        static public T Instance => UnitySingleton<T>.GetSingleton( true, true );

        
        static public T DoesInstanceExist()
        {
            return UnitySingleton<T>.GetSingleton( false, false );
        }
        
        protected virtual void Awake() // should be called in derived class
        {
            if ( isSingletonObject )
            {
                UnitySingleton<T>._Awake( this as T );
            }
        }

        protected virtual void OnDestroy()  // should be called in derived class
        {
            if ( isSingletonObject )
            {
                UnitySingleton<T>._Destroy();
            }
        }

        /// <summary>
        /// must return true if this instance of the object is the singleton. Can be used to allow multiple objects of this type
        /// that are "add-ons" to the singleton.
        /// </summary>
        public virtual bool isSingletonObject
        {
            get
            {
                return true;
            }
        }
    }
}

