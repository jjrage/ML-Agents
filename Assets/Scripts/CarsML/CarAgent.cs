using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarAgent : Agent
{
    #region Public Fields
    public float Reward { get { return rewardValue; } set { rewardValue += value; Debug.Log($"Current reward: {rewardValue}"); } }
    public float rewardValue = 0;
    public bool showRaycasts;
    public float towardsCheckpointReward;
    public float speedReward;
    #endregion

    #region Editor Fields
    [SerializeField]
    private Transform m_spawnPosition;
    [SerializeField]
    private TrackCheckpoints m_trackCheckpoints;
    [SerializeField]
    private Rigidbody m_rigidbody;
    [SerializeField]
    private Transform m_carTransform;
    #endregion

    #region Private Fields
    private WheelDrive m_wheelDrive;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        m_wheelDrive = GetComponent<WheelDrive>();
    }

    private void Start()
    {
        m_trackCheckpoints.OnPlayerCorrectCheckpoint += OnCarCorrectCheckpoint;
        m_trackCheckpoints.OnPlayerWrongCheckpoint += OnPlayerWrongCheckpoint;
        m_trackCheckpoints.OnPlayerFinishedLap += OnPlayerFinishedLap;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<WrongPlace>(out WrongPlace wrongPlace))
        {
            //Debug.Log("wrong place stay");
            AddReward(-0.01f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<WrongPlace>(out WrongPlace wrongPlace))
        {
            //Debug.Log("wrong place enter");
            Reward = -1;
            AddReward(-1f);
            EndEpisode();
        }
    }
    #endregion

    #region Private Methods
    private void OnPlayerFinishedLap(object sender, CheckPointSystemArgs e)
    {
        Reward = 10;
        AddReward(10);
        EndEpisode();
    }

    private void OnPlayerWrongCheckpoint(object sender, CheckPointSystemArgs e)
    {
        Reward = -1;
        AddReward(-1f);
        EndEpisode();

    }

    private void OnCarCorrectCheckpoint(object sender, CheckPointSystemArgs e)
    {
        Reward = 1;
        AddReward(1);
    }
    #endregion

    #region Public Methods
    #region Override
    public override void OnEpisodeBegin()
    {
        Reward = 0;
        rewardValue = 0;
        m_rigidbody.velocity = Vector3.zero;
        transform.localPosition = m_spawnPosition.localPosition;
        transform.localRotation = Quaternion.identity;
        transform.forward = m_spawnPosition.forward;
        m_trackCheckpoints.ResetCheckpoint(transform);
        m_trackCheckpoints.ResetReward();
        m_wheelDrive.StopCompletely();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_rigidbody.velocity.magnitude); //Add speed value to observation
        var direction = (m_trackCheckpoints.GetNextCheckpoint().transform.position - m_carTransform.position.normalized);
        var directionDot = Vector3.Dot(m_rigidbody.velocity.normalized, direction);
        sensor.AddObservation(directionDot);
        sensor.AddObservation(m_wheelDrive.angle);
        sensor.AddObservation(m_wheelDrive.torque);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float horizontalMove = Input.GetAxis("Horizontal") * Time.deltaTime;
        float verticalMove = Input.GetAxis("Vertical") * Time.deltaTime;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;

        var nextCheckpoint = m_trackCheckpoints.GetNextCheckpoint();
        var direction = (nextCheckpoint.transform.position - m_carTransform.position).normalized;
        var reward = Vector3.Dot(m_rigidbody.velocity.normalized, direction);

        AddReward(reward * towardsCheckpointReward);

        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = +1f; break;
            case 2: forwardAmount = -1f; break;
        }

        switch (actions.DiscreteActions[1])
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = +1f; break;
            case 2: turnAmount = -1f; break;
        }

        m_wheelDrive.SetInputs(forwardAmount, turnAmount);
    }
    #endregion
    #endregion
}
