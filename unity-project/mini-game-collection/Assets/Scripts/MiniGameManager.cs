using System;
using System.Collections;
using UnityEngine;

namespace MiniGameCollection
{
    public class MiniGameManager : MonoBehaviour
    {
        // Delegates
        public delegate void TimerEvent();
        public delegate void TimerMessage(string message);
        public delegate void TimerUpdateInt(int seconds);
        public delegate void TimerUpdateFloat(float seconds);
        public delegate void DisplayWinner(MiniGameWinner winner);

        // Events, ordered by occurence
        public TimerUpdateInt OnTimerInit;
        public TimerMessage OnCountDown;
        public TimerEvent OnGameStart;
        public TimerUpdateFloat OnTimerUpdateFloat;
        public TimerUpdateInt OnTimerUpdateInt;
        public TimerEvent OnGameEnd;
        public DisplayWinner OnGameWinner;
        public TimerEvent OnGameClose;

        // Private state
        private int previousIntTime = -1;
        private readonly WaitForSeconds delay1Second = new(1);
        private IEnumerator stateCoroutine;

        // Inspector fields
        [field: SerializeField] public string CurrentCountDownMessage { get; private set; } = string.Empty;
        [field: SerializeField] public string CountDownStartMessage { get; private set; } = "GO!";
        [field: SerializeField] public MiniGameMaxTime MaxGameTime { get; private set; } = MiniGameMaxTime.Seconds30;
        [field: SerializeField] public MiniGameState State { get; private set; } = MiniGameState.NotStarted;
        [field: SerializeField] public float TimeFloat { get; private set; }
        [field: SerializeField] public MiniGameWinner Winner { get; set; } = MiniGameWinner.Unset;
        
        // Properties
        public int TimeInt => (int)Math.Round(TimeFloat, MidpointRounding.AwayFromZero);
        public bool IsCountingDown => State == MiniGameState.InCountDown;
        public bool IsTimerRunning => State == MiniGameState.TimerRunning;
        public bool IsTimerExpired => State == MiniGameState.TimerExpired;


        public void Awake()
        {
            // Set time based on enum
            TimeFloat = MaxGameTime switch
            {
                MiniGameMaxTime.Seconds30 => 30,
                MiniGameMaxTime.Seconds60 => 60,
                _ => throw new NotImplementedException($"{MaxGameTime}")
            };

            // Pass in initial time
            OnTimerInit?.Invoke(TimeInt);
            previousIntTime = TimeInt;
        }

        public void Update()
        {
            switch (State)
            {
                case MiniGameState.NotStarted:
                case MiniGameState.InCountDown:
                case MiniGameState.TimerExpired:
                case MiniGameState.GameOver:
                    break;

                case MiniGameState.TimerRunning:
                    StateRunning();
                    break;

                default:
                    throw new NotImplementedException($"{State}");
            }
        }

        public void Reset()
        {

        }

        public void StartTimer()
        {
            if (IsTimerRunning)
            {
                string msg = $"Attempt to invoke {nameof(MiniGameManager)}.{nameof(StartTimer)} when timer already running.";
                Debug.LogWarning(msg);
                return;
            }

            // Begin countdown
            State = MiniGameState.InCountDown;
            stateCoroutine = CoroutineCountDown();
            StartCoroutine(stateCoroutine);
        }

        public void StopTimer()
        {
            if (!IsTimerRunning)
            {
                string msg = $"Attempt to invoke {nameof(MiniGameManager)}.{nameof(StopTimer)} when timer not running.";
                Debug.LogWarning(msg);
                return;
            }

            State = MiniGameState.TimerExpired;
            OnGameEnd?.Invoke();
        }

        private void StateRunning()
        {
            // Update time
            TimeFloat = Mathf.Clamp(TimeFloat - Time.deltaTime, 0f, float.MaxValue);
            OnTimerUpdateFloat?.Invoke(TimeFloat);

            // Update whole number time if changed
            if (TimeInt != previousIntTime)
            {
                previousIntTime = TimeInt;
                OnTimerUpdateInt?.Invoke(TimeInt);
            }

            // End timer running state if timer expired
            if (TimeInt == 0)
            {
                State = MiniGameState.TimerExpired;
                OnGameEnd?.Invoke();
            }
        }

        private IEnumerator CoroutineCountDown()
        {
            // Do 3-2-1 countdown
            for (int i = 3; i > 0; i--)
            {
                CurrentCountDownMessage = $"{i}";
                OnCountDown?.Invoke(CurrentCountDownMessage);
                yield return delay1Second;
            }

            // Print "GO!" or otherwise defined message
            CurrentCountDownMessage = CountDownStartMessage;
            OnCountDown?.Invoke(CurrentCountDownMessage);
            yield return delay1Second;
            
            // Clear message
            CurrentCountDownMessage = string.Empty;

            // Kick off timer
            State = MiniGameState.TimerRunning;
            OnGameStart?.Invoke();

            // Clear reference to this coroutine
            stateCoroutine = null;
        }

        private IEnumerator CoroutineTimerExpired()
        {
            // Call game end
            OnGameEnd?.Invoke();
            yield return delay1Second;

            // Call game winner
            OnGameWinner?.Invoke(Winner);
            yield return delay1Second;

            // Kill state machine with terminal state
            State = MiniGameState.GameOver;
            OnGameClose?.Invoke();

            // Clear reference to this coroutine
            stateCoroutine = null;
        }

    }
}
