﻿using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour {

    [SerializeField] int maxLives = 3;

    public static GameManager Instance { get; private set; }
    public static Direction CurrentDirection { get; private set; }
    public static int PlayerScore { get; private set; }
    public static Arrow SelectedArrow { get; private set; }

    public int Lives { get; private set; }

    public static event System.Action OnCorrectFinalInputEvent;

    static Direction inputDirection;
    static Direction desiredDirection;
    static Direction displayedDirection;
    static float countdown;
    static int currentMoveIndex;
    static Vector2 previousMousePos;

    void Awake() {
        #region Singleton
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(this);
            return;
        }
        #endregion
    }

    void Update() {
        if (!AnimationManager.IsAnimating) {
            countdown -= Time.deltaTime;
            if (countdown <= 0f) {
                NextArrow();
            }
        }
        if (!AnimationManager.IsAnimating) {
            HandleInput();
        }
    }

    void HandleInput() {
        if (Input.touchCount == 0) {
            return;
        }
        if (!InputManager.GetInput(ref inputDirection)) {
            return;
        }

        if (currentMoveIndex < SelectedArrow.MoveCount) {
            int move = SelectedArrow.GetMove(currentMoveIndex);
            if ((int)inputDirection == (move + (int)displayedDirection) %
                    DirectionUtility.kDirectionCount) {
                // The input matches the move
                currentMoveIndex++;
            } else {
                
            }
        } else if (inputDirection == desiredDirection) {
            // The arrow has been oriented successfully and the final move is right
            OnCorrectFinalInputEvent?.Invoke();
            countdown = AnimationManager.NextDelay;
            currentMoveIndex = 0;
        } else {
            // Wrong input on the scoring/final move
        }
    }

    // Hide the previous arrow, randomly select a new one, rotate it and show it.
    void NextArrow() {
        if (SelectedArrow != null) {
            SelectedArrow.IsActive = false;
            SelectedArrow.Reset();
        }

        desiredDirection = DirectionUtility.GetRandomDirection();
        Arrow[] arrows = ArrowPool.Instance.Arrows;

        int weightSum = arrows.Sum(a => a.Weight);
        for (int i = 0; i < arrows.Length; i++) {
            if (Random.Range(0, weightSum) < arrows[i].Weight) {
                SelectedArrow = arrows[i];
                break;
            }
            weightSum -= arrows[i].Weight;
        }

        displayedDirection = (Direction)(((int)desiredDirection +
            SelectedArrow.DisplayedDirectionModifier) % DirectionUtility.kDirectionCount);

        SelectedArrow.Orientation = displayedDirection;
        SelectedArrow.IsActive = true;
        countdown = SelectedArrow.Duration;
    }

    // While or for loop ? make a choice. Algorithm (to be improved by
    // using random function only once) from the website
    // https://forum.unity.com/threads/random-numbers-with-a-weighted-chance.442190/
    int SelectRandomWeightedIndex(int[] weights) {
        int weightSum = weights.Sum();
        for (int i = 0; i < weights.Length - 1; i++) {
            if (Random.Range(0, weightSum) < weights[i]) {
                return i;
            }
            weightSum -= weights[i];
        }
        return weights.Length - 1;
    }
}
