using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Team
{
    Blue = 0,
    Purple = 1
}

public class AgentSoccer : Agent
{
    // Note that that the detectable tags are different for the blue and purple teams. The order is
    // * ball
    // * own goal
    // * opposing goal
    // * wall
    // * own teammate
    // * opposing player

    public enum Position
    {
        Striker,
        Goalie,
        Generic
    }

    [HideInInspector]
    public Team team;
    float m_KickPower;
    // The coefficient for the reward for colliding with a ball. Set using curriculum.
    float m_BallTouch;
    public Position position;

    const float k_Power = 2000f;
    float m_Existential;
    float m_LateralSpeed;
    float m_ForwardSpeed;

    public bool m_UseDiscreteActions = true;

    [HideInInspector]
    public Rigidbody agentRb;
    SoccerSettings m_SoccerSettings;
    protected BehaviorParameters m_BehaviorParameters;
    public Vector3 initialPos;
    public float rotSign;

    public string m_OtherAgentTag;

    EnvironmentParameters m_ResetParams;

    private PlayerController m_PlayerController;
    private bool m_PlayerControlled = false;
    [SerializeField]
    private Transform ballTransform;
    [SerializeField]
    private float m_RotationModifier = 5f;

    protected bool m_IsPaused = false;

    public override void Initialize()
    {
        m_PlayerController = GetComponent<PlayerController>();

        // set lifetime
        SoccerEnvController envController = GetComponentInParent<SoccerEnvController>();
        if (envController != null)
        {
            m_Existential = 1f / envController.MaxEnvironmentSteps;
        }
        else
        {
            m_Existential = 1f / MaxStep;
        }

        // set team and starting position
        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        if (m_BehaviorParameters.TeamId == (int)Team.Blue)
        {
            team = Team.Blue;
            initialPos = new Vector3(transform.position.x - 5f, .5f, transform.position.z);
            rotSign = 1f;
        }
        else
        {
            team = Team.Purple;
            initialPos = new Vector3(transform.position.x + 5f, .5f, transform.position.z);
            rotSign = -1f;
        }

        // set movement speeds
        if (position == Position.Goalie)
        {
            m_LateralSpeed = 1.0f;
            m_ForwardSpeed = 1.0f;
        }
        else if (position == Position.Striker)
        {
            m_LateralSpeed = 0.3f;
            m_ForwardSpeed = 1.3f;
        }
        else
        {
            m_LateralSpeed = 0.3f;
            m_ForwardSpeed = 1.0f;
        }
        if(m_PlayerControlled)
        {
            Console.Write("ADDDDDDDDDDDDDDDDDDD");
        }


        m_SoccerSettings = FindFirstObjectByType<SoccerSettings>();
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    public void MoveAgent(ActionSegment<float> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        m_KickPower = 0f;

        var forwardValue = act[0];
        var rightValue = act[1];
        var rotateValue = act[2];

        agentRb.linearDamping = 5f;
        agentRb.angularDamping = 5f;

        if(m_PlayerControlled)
        {
            dirToGo += Vector3.forward * forwardValue * m_ForwardSpeed;
            if (forwardValue > 0) m_KickPower = 2f;

            dirToGo += Vector3.right * rightValue * m_ForwardSpeed;

            dirToGo.Normalize();
            agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed * 5, ForceMode.VelocityChange);

            if(dirToGo.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dirToGo, Vector3.up);
                agentRb.MoveRotation(Quaternion.Slerp(agentRb.rotation, targetRotation, Time.deltaTime * m_RotationModifier));
            }
        }
        else
        {
            dirToGo += transform.forward * forwardValue * m_ForwardSpeed;
            if (forwardValue > 0f) m_KickPower = 1f;

            dirToGo += transform.right * rightValue * m_LateralSpeed;

            dirToGo.Normalize();
            agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed,ForceMode.VelocityChange);
            
            rotateDir = transform.up * rotateValue;
            transform.Rotate(rotateDir, Time.deltaTime * 100f);
        }    
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        m_KickPower = 0f;

        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        agentRb.linearDamping = 5f;
        agentRb.angularDamping = 5f;

        if (m_PlayerControlled)
        {
            switch (forwardAxis)
            {
                case 1:
                    //dirToGo = transform.forward * m_ForwardSpeed * 2;
                    dirToGo += Vector3.forward * m_ForwardSpeed;
                    m_KickPower = 2f;
                    break;
                case 2:
                    //dirToGo = transform.forward * -m_ForwardSpeed *2;
                    dirToGo += Vector3.forward * -m_ForwardSpeed;
                    break;
            }

            switch (rightAxis)
            {
                case 1:
                    //dirToGo = transform.right * m_LateralSpeed*2;
                    dirToGo += Vector3.right * m_ForwardSpeed;
                    break;
                case 2:
                    //dirToGo = transform.right * -m_LateralSpeed*2;
                    dirToGo += Vector3.right * -m_ForwardSpeed;
                    break;
            }

            //agentRb.linearVelocity = dirToGo * m_SoccerSettings.agentRunSpeed;
            dirToGo.Normalize();
            agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed * 5, ForceMode.VelocityChange);

            if(dirToGo.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dirToGo, Vector3.up);
                agentRb.MoveRotation(Quaternion.Slerp(agentRb.rotation, targetRotation, Time.deltaTime * m_RotationModifier));
            }
        }
        else
        {
            switch (forwardAxis)
            {
                case 1:
                    dirToGo = transform.forward * m_ForwardSpeed;
                    m_KickPower = 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -m_ForwardSpeed;
                    break;
            }

            switch (rightAxis)
            {
                case 1:
                    dirToGo = transform.right * m_LateralSpeed;
                    break;
                case 2:
                    dirToGo = transform.right * -m_LateralSpeed;
                    break;
            }

            switch (rotateAxis)
            {
                case 1:
                    rotateDir = transform.up * -1f;
                    break;
                case 2:
                    rotateDir = transform.up * 1f;
                    break;
            }

            transform.Rotate(rotateDir, Time.deltaTime * 100f);

            agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed,
            ForceMode.VelocityChange);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (m_IsPaused) return;

        if (position == Position.Goalie)
        {
            // Existential bonus for Goalies.
            AddReward(m_Existential);
        }
        else if (position == Position.Striker)
        {
            // Existential penalty for Strikers
            AddReward(-m_Existential);
        }

        if(m_UseDiscreteActions) MoveAgent(actionBuffers.DiscreteActions);
        else MoveAgent(actionBuffers.ContinuousActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (m_IsPaused) return;
        m_PlayerControlled = true;

        if (m_UseDiscreteActions)
        {
            int[] playerActions = new int[3];
            playerActions[0] = m_PlayerController.forwardAxis;
            playerActions[1] = m_PlayerController.rightAxis;
            playerActions[2] = 0;

            //RotateTowardBall();

            MoveAgent(new ActionSegment<int>(playerActions, 0, 3));
        }
        else
        {
            float[] playerActions = new float[3];
            playerActions[0] = m_PlayerController.forwardValue;
            playerActions[1] = m_PlayerController.rightValue;
            playerActions[2] = 0;

            MoveAgent(new ActionSegment<float>(playerActions, 0, 3));
        }
    }

    /// <summary>
    /// Used to provide a "kick" to the ball.
    /// </summary>
    public virtual void OnCollisionEnter(Collision c)
    {
        var force = k_Power * m_KickPower;
        if (position == Position.Goalie)
        {
            force = k_Power;
        }

        if (c.gameObject.CompareTag("ball"))
        {
            AddReward(.4f * m_BallTouch);
            var dir = c.contacts[0].point - transform.position;
            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
        else if(c.gameObject.CompareTag(m_OtherAgentTag))
        {
            AddReward(-0.1f); // add a negative reward when agent is inside the other agents goal
        }
    }

    public override void OnEpisodeBegin()
    {
        m_BallTouch = m_ResetParams.GetWithDefault("ball_touch", 0);
    }

    public virtual void SetPause(bool paused)
    {
        m_IsPaused = paused;

        if(m_IsPaused)
        {
            agentRb.linearVelocity = Vector3.zero;
            agentRb.angularVelocity = Vector3.zero;
        }
    }

    public void RotateTowardBall()
    {
        Vector3 target = ballTransform.position;
        target.y = transform.position.y;

        Vector3 direction = (target - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 100f);
        }
    }
}
