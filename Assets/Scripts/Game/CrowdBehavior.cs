using Google.Protobuf.WellKnownTypes;
using Unity.VisualScripting;
using UnityEngine;

public class CrowdBehavior : MonoBehaviour
{
    [SerializeField] float m_JumpHeight = 0.5f;
    [SerializeField] float m_JumpSpeed = 0.25f;
    [SerializeField] float m_JumpDuration = 2f;

    private GameObject m_Ball;
    private GameManager m_GameManager;
    private Vector3 m_StartPos;
    private int m_CurrentTweenId = -1;

    private void Awake()
    {
        m_StartPos = transform.localPosition;

        m_Ball = FindFirstObjectByType<SoccerBallController>().gameObject;

        m_GameManager = FindFirstObjectByType<GameManager>();
        m_GameManager.PlayerScored += PlayJump;
        m_GameManager.OpponentScored += PlayJump;
    }

    private void OnDestroy()
    {
        if(m_GameManager != null)
        {
            m_GameManager.PlayerScored -= PlayJump;
            m_GameManager.OpponentScored -= PlayJump;
        }
    }

    private void Update()
    {
        RotateTowardsBall();
    }

    public void RotateTowardsBall()
    {
        Vector3 target = m_Ball.transform.position;
        target.y = transform.position.y; // keep upright

        Vector3 direction = (target - transform.position).normalized;

        if (direction.sqrMagnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 100f
            );
        }
    }

    private void PlayJump()
    {
        StopCurrentTween();

        float randomDelay = Random.Range(0f, 0.15f);

        LeanTween.delayedCall(gameObject, randomDelay, () =>
        {
            m_CurrentTweenId = LeanTween
                .moveLocalY(gameObject, m_StartPos.y + m_JumpHeight, m_JumpSpeed)
                .setEaseInOutSine()
                .setLoopPingPong()
                .id;

            LeanTween.delayedCall(gameObject, m_JumpDuration, StopJump);
        });
    }

    private void StopJump()
    {
        StopCurrentTween();
        transform.localPosition = m_StartPos;
    }

    private void StopCurrentTween()
    {
        if (m_CurrentTweenId != -1)
        {
            LeanTween.cancel(m_CurrentTweenId);
            m_CurrentTweenId = -1;
        }
    }
}
