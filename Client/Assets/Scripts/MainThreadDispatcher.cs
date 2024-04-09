using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;

    private readonly Queue<Action> actionQueue = new Queue<Action>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        lock (actionQueue)
        {
            while (actionQueue.Count > 0)
            {
                Action action = actionQueue.Dequeue();
                action?.Invoke();
            }
        }
    }

    public static void Enqueue(Action action)
    {
        if (instance == null)
        {
            Debug.LogError("MainThreadDispatcher instance is null.");
            return;
        }

        lock (instance.actionQueue)
        {
            instance.actionQueue.Enqueue(action);
        }
    }
}
