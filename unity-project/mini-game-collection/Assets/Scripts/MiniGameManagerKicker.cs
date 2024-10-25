using UnityEngine;

namespace MiniGameCollection
{
    public class MiniGameManagerKicker : MonoBehaviour
    {
        [field: SerializeField] MiniGameManager MiniGameManager;

        private void Awake()
        {
            GetMiniGameManagerIfNull();

            // Warn if unable to kick mini game
            if (MiniGameManager == null)
            {
                string msg =
                    $"{nameof(MiniGameManagerKicker)} could not find " +
                    $"{typeof(MiniGameManager).Name} in scene to kick.";
                Debug.LogWarning(msg);
            }

            // Start mini game
            MiniGameManager.StartTimer();
        }

        private void Reset()
        {
            GetMiniGameManagerIfNull();
        }

        private void GetMiniGameManagerIfNull()
        {
            if (MiniGameManager == null)
            {
                MiniGameManager = FindAnyObjectByType<MiniGameManager>();
            }
        }
    }
}
