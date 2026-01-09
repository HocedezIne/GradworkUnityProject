using UnityEngine;
using UnityEngine.VFX;

public class ConfettiController : MonoBehaviour
{
    [SerializeField] private bool m_ActivateOnPlayerScore;
    private GameManager m_GameManager;
    private VisualEffect m_FX;

    private void Awake()
    {
        m_FX = GetComponent<VisualEffect>();
    }

    private void Start()
    {
        m_GameManager = FindFirstObjectByType<GameManager>();
        if (m_ActivateOnPlayerScore) m_GameManager.PlayerScored += PlayFX;
        else m_GameManager.OpponentScored += PlayFX;
    }

    private void OnDestroy()
    {
        if(m_GameManager != null)
        {
            if (m_ActivateOnPlayerScore) m_GameManager.PlayerScored -= PlayFX;
            else m_GameManager.OpponentScored -= PlayFX;
        }
    }

    private void PlayFX()
    {
        m_FX.Play();
    }
}
