using System;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    #region Public Fields
    public Action<Collision> OnCollisionEnterCallback;
    public Action<Collision> OnCollisionStayCallback;
    public Action<Collider> OnTriggerEnterCallback;
    public Action<Collider> OnTriggerStayCallback;
    #endregion

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterCallback?.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayCallback?.Invoke(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterCallback?.Invoke(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionStayCallback?.Invoke(collision);
    }
    #endregion
}
