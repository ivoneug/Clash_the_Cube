using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Doozy.Engine.Soundy;

public class BaseSingletonController : MonoBehaviour
{
    public static BaseSingletonController Instance;

    public bool IsSingleton
    {
        get
        {
            return this == BaseSingletonController.Instance;
        }
    }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (!IsSingleton) return;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (!IsSingleton) return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { }
}
