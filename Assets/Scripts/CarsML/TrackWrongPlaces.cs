using System;
using UnityEngine;

public class TrackWrongPlaces : MonoBehaviour
{
    #region Public Events
    public event EventHandler OnPlayerInWrongPlace;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        Transform wornglacesTransform = transform.Find("WrongPlaces");

        foreach (Transform wrongPlaceTransform in wornglacesTransform)
        {
            if (wrongPlaceTransform.TryGetComponent<WrongPlace>(out WrongPlace wrongPlace))
            {
                wrongPlace.SetTrackCheckpoints(this);
            }
        }
    }
    #endregion
}
