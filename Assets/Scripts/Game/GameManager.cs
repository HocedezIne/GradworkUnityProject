using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Agent> m_Agents;

    [SerializeField] private string m_GameDifficulty;
    [SerializeField] public float m_TestDuration;
    [SerializeField] private int m_NextSceneInFlow;

    [SerializeField] private AgentSoccer m_PlayerAgent;
    [SerializeField] private AgentSoccer m_OppenentAgent;
    private float m_PlayerOldElo = 100f;
    private float m_OpponentOldElo = 100f;

    [SerializeField] private float m_DataLoggingFrequency = 5f;
    [SerializeField] private string m_DataFileName = "Data.csv";
    private string m_FullDataPath;
    private int m_LogInterval = 0;
    private PlayerController m_PlayerController;
    private CultureInfo m_Culture;

    private BlendedAgentSoccer m_BlendedAgent;

    private int m_PlayerScore;
    [SerializeField] private ScoreCounter m_PlayerScoreCounter;
    public event Action PlayerScored;
    private int m_OpponentScore;
    [SerializeField] private ScoreCounter m_OpponentScoreCounter;
    public event Action OpponentScored;

    private float m_PlayerCumulativeReward;
    private float m_OpponentCumulativeReward;

    [SerializeField] private ScoreCounter m_CountdownTimer;
    private CanvasGroup m_CountdownCanvasGroup;
    public event Action TimerUpdated;
    private Slider m_TimeSlider;
    private bool m_HasTestStarted = false;

    private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_CountdownSound;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        m_TimeSlider = FindFirstObjectByType<Slider>();
        m_CountdownCanvasGroup = m_CountdownTimer.GetComponent<CanvasGroup>();
        ShowCountdownTimer(false);

        m_Culture = new CultureInfo("nl-NL");
        m_PlayerController = FindFirstObjectByType<PlayerController>();

        m_BlendedAgent = m_OppenentAgent.GetComponent<BlendedAgentSoccer>();

        m_FullDataPath = Path.Combine(Settings.Instance.m_DataPath, m_DataFileName);
        if(!File.Exists(m_FullDataPath))
        {
            CreateDataFile();
        }

        Debug.Log($"Game difficulty: {m_GameDifficulty}");
        File.AppendAllTextAsync(m_FullDataPath, $"Game difficulty: {m_GameDifficulty}\n");

        StartCoroutine(RoundCountdown(2.4f));
    }

    public IEnumerator RoundCountdown(float duration)
    {
        Debug.Log("Countdown started");
        m_AudioSource.clip = m_CountdownSound;
        m_AudioSource.Play();

        ShowCountdownTimer(true);

        foreach (AgentSoccer agent in m_Agents)
        {
            if(agent.TryGetComponent(out DecisionRequester dr))
            {
                dr.enabled = false;
            }

            agent.SetPause(true);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            m_CountdownTimer.UpdateScore(Convert.ToInt32(duration-elapsed +1), 0f);
            TimerUpdated?.Invoke();

            yield return null;
        }

        foreach (AgentSoccer agent in m_Agents)
        {
            if (agent.TryGetComponent(out DecisionRequester dr))
            {
                dr.enabled = true;
            }

            agent.SetPause(false);
        }

        ShowCountdownTimer(false);

        Debug.Log("Countdown ended");

        if(!m_HasTestStarted)
        { 
            m_HasTestStarted = true;
            StartCoroutine(TestTimer());
            StartCoroutine(LoggingTimer());
        }
    }

    private void ShowCountdownTimer(bool visible)
    {
        m_CountdownCanvasGroup.alpha = visible ? 1f : 0f;
        m_CountdownCanvasGroup.interactable = visible;
        m_CountdownCanvasGroup.blocksRaycasts = visible;
    }

    public IEnumerator TestTimer()
    {
        float elapsed = 0f;

        while (elapsed < m_TestDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            m_TimeSlider.value = elapsed / m_TestDuration;

            yield return null; // wait one frame
        }

        m_TimeSlider.value = 1f;
        OnTestTimerOver();
    }

    public void OnTestTimerOver()
    {
        if (m_LogInterval < m_TestDuration/ m_DataLoggingFrequency)
        {
            LogData();
        }
        SceneManager.LoadScene(m_NextSceneInFlow);
    }

    public void IncreaseScore(Team scoredPlayer, float reward)
    {
        if (scoredPlayer == Team.Blue)
        {
            ++m_PlayerScore;
            m_PlayerCumulativeReward += reward;
            --m_OpponentCumulativeReward;

            m_PlayerScoreCounter.UpdateScore(m_PlayerScore, 0f);
            PlayerScored?.Invoke();
        }
        else 
        { 
            ++m_OpponentScore;
            m_OpponentCumulativeReward += reward;
            --m_PlayerCumulativeReward;

            m_OpponentScoreCounter.UpdateScore(m_OpponentScore, 0f);
            OpponentScored?.Invoke();
        }

        StartCoroutine(RoundCountdown(2.4f));
    }

    private void CreateDataFile()
    {
        if (!Directory.Exists(Settings.Instance.m_DataPath))
        {
            Directory.CreateDirectory(Settings.Instance.m_DataPath);
        }

        File.AppendAllText(m_FullDataPath, "Time;" +
                                           "Input Frequency Average;Total Inputs;" +
                                           "Player Score;Opponent Score;" +
                                           "Player ELO;Opponent ELO;" +
                                           "Player Cumulative Reward;Opponent Cumulative Reward;" +
                                           "Blend Weight\n");
    }

    public IEnumerator LoggingTimer()
    {
        yield return new WaitForSecondsRealtime(m_DataLoggingFrequency);

        LogData();
        StartCoroutine(LoggingTimer());
    }

    private void LogData()
    {
        ++m_LogInterval;

        int totalInputs = m_PlayerController.GetInputEventCount();
        float averageInputPerSecond = totalInputs / m_DataLoggingFrequency;
        m_PlayerController.ResetInputLogging();

        CalculateELO();

        string data = $"{m_LogInterval * m_DataLoggingFrequency};" +
                      $"{averageInputPerSecond.ToString(m_Culture)};{totalInputs};" +
                      $"{m_PlayerScore};{m_OpponentScore};" +
                      $"{m_PlayerOldElo.ToString(m_Culture)};{m_OpponentOldElo.ToString(m_Culture)};" +
                      $"{m_PlayerCumulativeReward.ToString(m_Culture)};{m_OpponentCumulativeReward.ToString(m_Culture)};" +
                      $"{m_BlendedAgent?.BlendWeight.ToString(m_Culture) ?? "0"}\n";

        File.AppendAllText(m_FullDataPath, data);

        if (m_BlendedAgent) AdjustBlendWeight();

        m_PlayerCumulativeReward = 0f;
        m_OpponentCumulativeReward = 0f;
    }

    private void CalculateELO()
    {
        m_PlayerCumulativeReward += m_PlayerAgent.GetCumulativeReward();
        m_OpponentCumulativeReward += m_OppenentAgent.GetCumulativeReward();

        float playerResult;
        float opponentResult;

        if (m_PlayerCumulativeReward > m_OpponentCumulativeReward)
        {
            playerResult = 1f;
            opponentResult = 0f;
        }
        else if (m_PlayerCumulativeReward < m_OpponentCumulativeReward)
        {
            playerResult = 0f;
            opponentResult = 1f;
        }
        else
        {
            playerResult = 0.5f;
            opponentResult = 0.5f;
        }

        float playerExpectedReward = 1f / (1f + Mathf.Pow(10f, (m_OpponentOldElo - m_PlayerOldElo) / 400f));
        float opponentExpectedReward = 1f / (1f + Mathf.Pow(10f, (m_PlayerOldElo - m_OpponentOldElo) / 400f));

        m_PlayerOldElo += 8 * (playerResult - playerExpectedReward);
        m_OpponentOldElo += 8 * (opponentResult - opponentExpectedReward);
    }

    private void AdjustBlendWeight()
    {
        float rewardDifference = m_PlayerCumulativeReward - m_OpponentCumulativeReward;
        m_BlendedAgent.BlendWeight += rewardDifference/8;
    }
}