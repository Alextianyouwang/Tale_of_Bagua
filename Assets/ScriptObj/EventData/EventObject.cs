
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TTOBG/EventObject")]
public class EventObject : ScriptableObject
{

    List<IEventListener> listeners = new List<IEventListener>();

    public void RegisterListener(IEventListener l) 
    {
        if (!listeners.Contains(l))
            listeners.Add(l);
    }
    public void UnregisterListener(IEventListener l)
    {
        if (listeners.Contains(l))
            listeners.Remove(l);
    }

    public void Raise() 
    {
        for (int i = listeners.Count - 1; i >= 0; i --)
            listeners[i].OnEventRaise();
    }
}
