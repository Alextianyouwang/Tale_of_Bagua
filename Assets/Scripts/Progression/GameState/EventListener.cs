using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour,IEventListener
{
    public UnityEvent EventResponser;
    public EventObject Event;

    public void OnEventRaise() 
    {
        EventResponser.Invoke();
    }
    public void OnDisable()
    {
        if (Event == null)
            return;
        Event.UnregisterListener(this);
    }
    public void OnEnable()
    {
        if (Event == null)
            return;
        Event.RegisterListener(this);
    }


}
