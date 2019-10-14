﻿using UnityEngine;

public class Tutorial : MonoBehaviour {

    [SerializeField] int requiredConsecutiveSuccess = 8;

    public static event System.Action Sth;

    Countdown countdown;
    ProgressStep tutorialStep;
    GameManager gameManager;

    void Start() {
        countdown = GameManager.Countdown;
        gameManager = GetComponent<GameManager>();
        UnityEngine.Assertions.Assert.IsNotNull(gameManager, "GameManager not found!");
        Sth?.Invoke();
    }

    public void StartTutorial() {
        tutorialStep = ProgressStep.First;
    }

    public void NextStep() {
        if (tutorialStep == ProgressStep.Fourth) {
            return;
        }
        tutorialStep = (ProgressStep)((int)tutorialStep + 1);
        //gameManager.NextArrow();
    }

    enum ProgressStep {
        First,
        Second,
        Third,
        Fourth
    }
}
