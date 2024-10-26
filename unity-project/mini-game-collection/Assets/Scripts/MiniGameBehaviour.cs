using UnityEngine;

namespace MiniGameCollection
{
    public abstract class MiniGameBehaviour : MonoBehaviour
    {
        [field: SerializeField] protected MiniGameManager MiniGameManager { get; set; }

        protected virtual void OnEnable()
        {
            GetMiniGameManagerIfNull();

            if (MiniGameManager == null)
                return;

            MiniGameManager.OnTimerInit += OnTimerInitialized;
            MiniGameManager.OnCountDown += OnCountDown;
            MiniGameManager.OnGameStart += OnGameStart;
            MiniGameManager.OnTimerUpdateFloat += OnTimerUpdate;
            MiniGameManager.OnTimerUpdateInt += OnTimerUpdateInt;
            MiniGameManager.OnGameEnd += OnGameEnd;
            MiniGameManager.OnGameWinner += OnGameWinner;
            MiniGameManager.OnGameClose += OnGameClose;
        }

        protected virtual void OnDisable()
        {
            if (MiniGameManager == null)
                return;

            MiniGameManager.OnTimerInit -= OnTimerInitialized;
            MiniGameManager.OnCountDown -= OnCountDown;
            MiniGameManager.OnGameStart -= OnGameStart;
            MiniGameManager.OnTimerUpdateFloat -= OnTimerUpdate;
            MiniGameManager.OnTimerUpdateInt -= OnTimerUpdateInt;
            MiniGameManager.OnGameEnd -= OnGameEnd;
            MiniGameManager.OnGameWinner -= OnGameWinner;
            MiniGameManager.OnGameClose -= OnGameClose;
        }

        protected virtual void Reset()
        {
            GetMiniGameManagerIfNull();
        }

        protected virtual void OnTimerInitialized(int maxGameTime) { }
        protected virtual void OnCountDown(string message) { }
        protected virtual void OnGameStart() { }
        protected virtual void OnTimerUpdate(float timeRemaining) { }
        /// <summary>
        ///     
        /// </summary>
        /// <param name="timeRemaining"></param>
        /// <remarks>
        ///     Called once per second.
        /// </remarks>
        protected virtual void OnTimerUpdateInt(int timeRemaining) { }
        protected virtual void OnGameEnd() { }
        protected virtual void OnGameWinner(MiniGameWinner winner) { }
        protected virtual void OnGameClose() { }




        private void GetMiniGameManagerIfNull()
        {
            if (MiniGameManager == null)
            {
                MiniGameManager = FindAnyObjectByType<MiniGameManager>();
            }
        }
    }
}
