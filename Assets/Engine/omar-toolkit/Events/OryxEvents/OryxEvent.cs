using System.Collections.Generic;
namespace Oryx
{
    
    public class OryxEvent
    {

        private List<OryxAction> _listeners = new List<OryxAction>();
    
        public OryxEvent()
        {
        
        }

        public virtual void AddListener(OryxAction listener)
        {
            if (listener == null)
                return;

            lock (this)
            {
                _listeners.Add(listener);
            }
        }
    
        public virtual void RemoveListener(OryxAction listener)
        {
            lock (this)
            {
                _listeners.Add(listener);
            }
        }
    
        public virtual void RemoveAllListeners()
        {
            lock (this)
            {
                _listeners.Clear();
            }
        }

        public void Invoke()
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].Invoke();
            }
        }
    
    }

    public class OryxEvent<T>
    {

        private List<OryxAction<T>> _listeners = new List<OryxAction<T>>();
    
        public OryxEvent()
        {
        
        }

        public virtual void AddListener(OryxAction<T> listener)
        {
            if (listener == null)
                return;

            lock (this)
            {
                _listeners.Add(listener);
            }
        }
    
        public virtual void RemoveListener(OryxAction<T> listener)
        {
            lock (this)
            {
                _listeners.Add(listener);
            }
        }
    
        public virtual void RemoveAllListeners()
        {
            lock (this)
            {
                _listeners.Clear();
            }
        }

        public void Invoke(T arg)
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].Invoke(arg);
            }
        }
    
    }
    
    public class OryxEvent<T1,T2>
    {

        private List<OryxAction<T1,T2>> _listeners = new List<OryxAction<T1,T2>>();
    
        public OryxEvent()
        {
        
        }

        public virtual void AddListener(OryxAction<T1,T2> listener)
        {
            if (listener == null)
                return;

            lock (this)
            {
                _listeners.Add(listener);
            }
        }
    
        public virtual void RemoveListener(OryxAction<T1,T2> listener)
        {
            lock (this)
            {
                _listeners.Add(listener);
            }
        }
    
        public virtual void RemoveAllListeners()
        {
            lock (this)
            {
                _listeners.Clear();
            }
        }

        public void Invoke(T1 arg, T2 arg2)
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].Invoke(arg, arg2);
            }
        }
    
    }
}