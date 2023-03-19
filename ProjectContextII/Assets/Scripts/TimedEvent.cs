using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(MeshRenderer))]
public class TimedEvent : MonoBehaviour
{
    [Space(10), Header("Do events after a certain amount of time")]
    [SerializeField] bool repeatable;
    [SerializeField] TimedEventStruct[] timedEvents;

    [Space(10), Header("What should be able to trigger this event?")]
    [SerializeField] TriggerType responseType;

    bool running;
    float timer;
    BoxCollider myCollider;
    int currentEventIterator = 0;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        if (running)
        {
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!repeatable)
        {
            myCollider.enabled = false;
        }

        if (other == null) return;
        ITrigger collider = other.GetComponent<ITrigger>();
        if (collider == null) return;
        if (collider.triggerType != responseType && responseType != TriggerType.ALL) return;

        currentEventIterator = 0;
        timer = 0;

        if (timedEvents.Length == 0) return;
        StartCoroutine(nextEvent(timedEvents[0]));

        running = true;
    }

    IEnumerator nextEvent(TimedEventStruct currentEvent)
    {
        yield return new WaitUntil(() => timer >= currentEvent.timedEventTime);
        currentEvent.myEvent.Invoke();

        currentEventIterator++;
        if (currentEventIterator < timedEvents.Length) StartCoroutine(nextEvent(timedEvents[currentEventIterator]));
        else running = false;
    }
}


    [System.Serializable]
public struct TimedEventStruct
{
    public float timedEventTime;
    public UnityEvent myEvent;
}
