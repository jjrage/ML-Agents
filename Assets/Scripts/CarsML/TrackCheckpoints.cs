using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    #region Public Fields
    #region Events
    public event EventHandler<CheckPointSystemArgs> OnPlayerCorrectCheckpoint;
    public event EventHandler<CheckPointSystemArgs> OnPlayerWrongCheckpoint;
    public event EventHandler<CheckPointSystemArgs> OnPlayerFinishedLap;
    #endregion

    public Transform checkpointsTransform;
    public List<Transform> carTransformList;
    public int numberOfNextCheckpoint;
    public float currentReward = 1f;
    #endregion

    #region Private Fields
    private List<Checkpoint> checkpointsList;
    private List<int> nextCheckpointIndexList;
    private CheckPointSystemArgs m_checkpointSystemArgs;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        checkpointsList = new List<Checkpoint>();
        m_checkpointSystemArgs = new CheckPointSystemArgs(transform);
        Transform checkpointsTransfrom = checkpointsTransform;

        foreach (Transform checkpointTransform in checkpointsTransfrom)
        {
            if (checkpointTransform.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
            {
                checkpoint.SetTrackCheckpoints(this);
                checkpointsList.Add(checkpoint);
            }
        }

        nextCheckpointIndexList = new List<int>(checkpointsList.Count);

        foreach (var checkpoint in checkpointsList)
        {
            nextCheckpointIndexList.Add(checkpointsList.IndexOf(checkpoint));
        }

        numberOfNextCheckpoint = checkpointsList.IndexOf(checkpointsList[0]);
    }
    #endregion

    #region Public Methods
    public void CarThroughtCheckpoint(Checkpoint checkpoint, Transform carTransform)
    {
        int nextCheckpointIndex = checkpointsList.IndexOf(checkpoint);

        if (checkpointsList.IndexOf(checkpoint) == numberOfNextCheckpoint)
        {

            Checkpoint correctCheckpoint = checkpointsList[nextCheckpointIndex];
            correctCheckpoint.Hide();
            if(numberOfNextCheckpoint < checkpointsList.Count - 1)
            {
                numberOfNextCheckpoint++;
                m_checkpointSystemArgs.IncreaseReward();
            }
            else
            {
                numberOfNextCheckpoint = 0;
                m_checkpointSystemArgs.SetReward(20);
                OnPlayerFinishedLap?.Invoke(this, m_checkpointSystemArgs);
            }

            OnPlayerCorrectCheckpoint?.Invoke(this, m_checkpointSystemArgs);
        }
        else
        {

            OnPlayerWrongCheckpoint?.Invoke(this, m_checkpointSystemArgs);
            Checkpoint correctCheckpoint = checkpointsList[nextCheckpointIndex];
            correctCheckpoint.Show();
        }
    }

    public void ResetCheckpoint(Transform transform)
    {
        numberOfNextCheckpoint = checkpointsList.IndexOf(checkpointsList[0]);
    }

    public void ResetReward()
    {
        m_checkpointSystemArgs.ResetRewardValue();
    }
    #endregion

    #region Public Functions
    public int GetNextCheckpointIndex()
    {
        return numberOfNextCheckpoint;
    }

    public GameObject GetNextCheckpoint()
    {
        return checkpointsList[numberOfNextCheckpoint].gameObject;
    }
    #endregion
}
public class CheckPointSystemArgs : EventArgs

{
    private Transform _CarTransform;
    private float _rewardMulti = 1.02f;
    private float _currentReward;

    public void SetReward(float reward)
    {
        _currentReward = reward;
    }
    public void IncreaseReward()
    {
        _currentReward *= _rewardMulti;
    }

    public CheckPointSystemArgs(Transform carTransform)
    {
        _CarTransform = carTransform;
        _currentReward = 1;
    }

    public Transform CarTransform()
    {
        return _CarTransform;
    }

    public void ResetRewardValue()
    {
        _currentReward = 1;
    }

    public float CurrentReward()
    {
        return _currentReward;
    }
}
