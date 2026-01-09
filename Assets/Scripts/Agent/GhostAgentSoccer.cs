using System;
using System.Collections.Generic;
using Unity.InferenceEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostAgentSoccer : AgentSoccer
{
    [SerializeField] Transform primaryAgent;

    [HideInInspector] public ActionSegment<float> LastConinuousActions {  get; private set; }

    public override void Initialize()
    {
        float[] emptyActions = new float[3];
        LastConinuousActions = new ActionSegment<float>(emptyActions, 0, 3);

        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        if (m_BehaviorParameters.TeamId == (int)Team.Blue)
        {
            team = Team.Blue;
        }
        else
        {
            team = Team.Purple;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (m_IsPaused) return;

        LastConinuousActions = actions.ContinuousActions;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        return;
    }

    public override void SetPause(bool paused)
    {
        m_IsPaused = paused;
    }

    private void LateUpdate()
    {
        transform.SetPositionAndRotation(primaryAgent.position, primaryAgent.rotation);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        return;
    }

    public override void OnEpisodeBegin()
    {
        return;
    }
}
