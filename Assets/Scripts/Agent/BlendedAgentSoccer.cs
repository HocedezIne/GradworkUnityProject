using NUnit.Framework;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System.Collections.Generic;
using Unity.InferenceEngine;
using Unity.MLAgents.Sensors;

public class BlendedAgentSoccer : AgentSoccer
{
    [SerializeField] GhostAgentSoccer m_GhostAgent;
    private float m_BlendWeight = 0.5f;
    public float BlendWeight 
    { 
        get { return m_BlendWeight; }
        set
        {
            m_BlendWeight = Mathf.Clamp01(value);
        } 
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (m_IsPaused) return;

        ActionSegment<float> ghostActions = m_GhostAgent.LastConinuousActions;
        float[] ContinuousOut = new float[actionBuffers.ContinuousActions.Length];

        for(int i = 0; i < actionBuffers.ContinuousActions.Length; i++)
        {
            var primaryWeighted = m_BlendWeight * actionBuffers.ContinuousActions[i];
            var ghostWeighted = (1- m_BlendWeight) * ghostActions[i];
            ContinuousOut[i] = primaryWeighted + ghostWeighted;
        }

        MoveAgent(new ActionSegment<float>(ContinuousOut, 0, 3));
    }
}
