﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    public enum NeedType {
        None, Health, Hunger, Playfulness, Fear, Energy, Curiousity, Thirst
    }

    public enum WolfState {
        Idle, Sleep, Feed, Drink, Play, Fight, Explore, Walk, Run, Dead
    }

    public enum TagName {
        Water, Food, RestArea, Prey, Enemy, Wolf, None
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}