using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    #region Editor Fields
    [SerializeField]
    private MeshRenderer m_meshRenderer;
    #endregion

    #region Private Fields
    private TrackCheckpoints m_trackCheckpoints;
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        Hide();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<CarAgent>(out CarAgent carAgent))
        {         
            m_trackCheckpoints.CarThroughtCheckpoint(this, other.transform);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.TryGetComponent<CarAgent>(out CarAgent carAgent))
        {
            carAgent.AddReward(-0.01f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<CarAgent>(out CarAgent carAgent))
        {
            m_trackCheckpoints.CarThroughtCheckpoint(this, collision.transform);
        }
    }
    #endregion

    #region Public Methods
    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.m_trackCheckpoints = trackCheckpoints;
    }

    public void Hide()
    {
        m_meshRenderer.enabled = false;
    }
    public void Show()
    {
        m_meshRenderer.enabled = true;
    }
    #endregion
}
