using System.Collections.Generic;
namespace Oryx
{
    
    public class GameEvent
    {

        private List<GameAction> _listeners = new List<GameAction>();
    
        public GameEvent()
        {
        
        }

        public virtual void AddListener(GameAction listener)
        {
            if (listener == null)
                return;

            lock (this)
            {
                _listeners.Add(listener);
            }
        }
    
        public virtual void RemoveListener(GameAction listener)
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

    public class GameEvent<T>
    {

        private List<GameAction<T>> _listeners = new List<GameAction<T>>();
    
        public GameEvent()
        {
        
        }

        public virtual void AddListener(GameAction<T> listener)
        {
            if (listener == null)
                return;

            lock (this)
            {
                _listeners.Add(listener);
            }
        }
    
        public virtual void RemoveListener(GameAction<T> listener)
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
    
    public class GameEvent<T, TU>
    {

        private List<GameAction<T, TU>> _listeners = new List<GameAction<T,TU>>();
    
        public GameEvent()
        {
        
        }

        public virtual void AddListener(GameAction<T,TU> listener)
        {
            if (listener == null)
                return;

            lock (this)
            {
                _listeners.Add(listener);
            }
        }
    
        public virtual void RemoveListener(GameAction<T,TU> listener)
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

        public void Invoke(T arg0, TU arg1)
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].Invoke(arg0, arg1);
            }
        }
    
    }
}