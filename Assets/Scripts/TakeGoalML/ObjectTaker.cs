using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class ObjectTaker : Agent
{
    #region Editor Fields
    [SerializeField]
    private Transform m_targetTransform;
    [SerializeField]
    private float m_playerSpeed;
    [SerializeField]
    private Material m_groundMaterial;
    [SerializeField]
    private Color m_winColor;
    [SerializeField]
    private Color m_loseColor;
    #endregion

    #region Public Methods
    #region Overrides
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4));
        m_targetTransform.localPosition = new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(m_targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * m_playerSpeed;
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
    #endregion
    #endregion

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Wall>(out Wall wall))
        {
            m_groundMaterial.color = m_loseColor;
            SetReward(-1f);
            EndEpisode();
        }

        if (other.TryGetComponent<ObjectToTake>(out ObjectToTake objectToTake))
        {
            m_groundMaterial.color = m_winColor;
            SetReward(1f);
            EndEpisode();
        }
    }
    #endregion
}
