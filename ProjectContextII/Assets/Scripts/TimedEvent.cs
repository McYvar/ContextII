using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TimedEvent : MonoBehaviour
{
    [Space(10), Header("Do events after a certain amount of time")]
    [SerializeField] bool repeatable;
    [SerializeField] TimedEventStruct[] timedEvents;

    bool running;
    float timer;
    BoxCollider myCollider;
    int currentEventIterator = 0;


    private void Update()
    {
        if (running)
        {
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (timedEvents.Length == 0) return;
        StartCoroutine(nextEvent(timedEvents[0]));

        if (!repeatable)
        {
            myCollider.enabled = false;
            currentEventIterator = 0;
        }
    }

    IEnumerator nextEvent(TimedEventStruct currentEvent)
    {
        yield return new WaitUntil(() => timer >= currentEvent.timedEventTime);
        currentEvent.myEvent.Invoke();

        currentEventIterator++;
        if (currentEventIterator != timedEvents.Length) StartCoroutine(nextEvent(timedEvents[currentEventIterator]));
    }
}


    [System.Serializable]
public struct TimedEventStruct
{
    public float timedEventTime;
    public UnityEvent myEvent;
}
