﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClearEventsScript : MonoBehaviour
{
    public List<EventDelayPair> EventDelayPairs;

    [Serializable]
    public class EventDelayPair
    {
        public UnityEvent unityEvent;
        public float Delay;
    }
    private void Start()
    {
        foreach (var eventDelayPair in EventDelayPairs)
        {
            StartCoroutine(InvokeDelayed(eventDelayPair));
        }
    }

    private IEnumerator InvokeDelayed(EventDelayPair pair)
    {
        var timer = pair.Delay;

        do
        {
            timer -= Time.deltaTime * (1 / Time.timeScale);
            pair.Delay = timer;
            yield return null;
        } while (timer > 0);

        pair.Delay = 0;

        pair.unityEvent.Invoke();
    }

}
