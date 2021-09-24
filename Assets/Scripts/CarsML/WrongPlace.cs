using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongPlace : MonoBehaviour
{
    #region Private Fields
    private TrackWrongPlaces m_trackWrongPlaces;
    #endregion

    #region Public Methods
    public void SetTrackCheckpoints(TrackWrongPlaces trackWrongPlaces)
    {
        this.m_trackWrongPlaces = trackWrongPlaces;
    }
    #endregion

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<CarAgent>(out CarAgent player))
        {
            player.AddReward(-1f);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.TryGetComponent<CarAgent>(out CarAgent player))
        {
            player.AddReward(-0.1f);
        }
    }
    #endregion

}
