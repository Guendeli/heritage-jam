using UnityEngine;

namespace Oryx
{
    /// <summary>
    /// Auxiliary class to overcome the problem of references to pooled objects that should become <c>null</c> when 
    /// objects are moved back to the pool after calling <see cref="ObjectPoolController.Destroy(GameObject)"/>.
    /// </summary>
    public class PoolableReference<T> where T : Component
    {
        PoolableObject _pooledObj;
        int _initialUsageCount;
        
        T _objComponent;
        
        public PoolableReference()
        {
            Reset();
        }


        public PoolableReference( T componentOfPoolableObject )
        {
            Set( componentOfPoolableObject, false );
        }
        
        public PoolableReference( PoolableReference<T> poolableReference )
        {
            _objComponent = poolableReference._objComponent;
            _pooledObj = poolableReference._pooledObj;
            _initialUsageCount = poolableReference._initialUsageCount;
        }
        
        public void Reset()
        {
            _pooledObj = null;
            _objComponent = null;
            _initialUsageCount = 0;
        }

        public T Get()
        {
            if ( !_objComponent )
                return null;

            if ( _pooledObj ) // could be set to a none-poolable object
            {
                if ( _pooledObj._usageCount != _initialUsageCount || _pooledObj._isInPool )
                {
                    _objComponent = null;
                    _pooledObj = null;
                    return null;
                }
            }
            return ( T ) _objComponent;
        }


        public void Set( T componentOfPoolableObject )
        {
            Set( componentOfPoolableObject, false );
        }

        /// <summary>
        /// Sets the reference to a poolable object with the specified component.
        /// </summary>
       public void Set( T componentOfPoolableObject, bool allowNonePoolable )

        {
            if ( !componentOfPoolableObject )
            {
                Reset();
                return;
            }
            _objComponent = ( T ) componentOfPoolableObject;
            _pooledObj = _objComponent.GetComponent<PoolableObject>();
            if ( !_pooledObj )
            {
                if ( allowNonePoolable )
                {
                    _initialUsageCount = 0;
                }
                else
                {
                    Debug.LogError( "Object for PoolableReference must be poolable" );
                    return;
                }
            }
            else
            {
                _initialUsageCount = _pooledObj._usageCount;
            }
        }
    }
}