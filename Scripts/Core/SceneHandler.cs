using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHandler : MonoBehaviour
{
    EventManager eventManager;
    InputManager inputManager;
    public GameObject[] requiredPrefabs;
    // Start is called before the first frame update
    void Awake()
    {
        eventManager = EventManager.Instance;
        inputManager = new InputManager();
    }

    private void Start()
    {
        InstantiateRequiredGameObjects();
    }

    private void InstantiateRequiredGameObjects()
    {
        foreach (GameObject prefab in requiredPrefabs)
        {
            Instantiate(prefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        eventManager.Update();
        inputManager.Update();
    }
    private void OnDestroy()
    {
        eventManager.RemoveAll();
    }
}
