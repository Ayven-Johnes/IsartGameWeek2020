using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class TouchHandler : MonoBehaviour
{
    //[SerializeField] CubeBehavior cubeControlled = null;

    #region TouchVars

    bool IsOnTouch = false;

    private float m_touchDuration = 0f;
    //private float m_durationForASwipe = 0.25f;

    private Vector3 m_firstTouchPosition = Vector3.zero;
    private Vector3 m_currentTouchPosition = Vector3.zero;

    #endregion

    #region ZoomVars

    [SerializeField] Transform cameraTarget = null;
    [SerializeField] Transform cameraTransform = null;

    private float PreviousDistanceBetweenTouch = 0.0f;

    #endregion

    // Update is called once per frame
    void Update()
    {
        CheckTouch();
        CheckTimer();
    }

    #region TimerFunction

    void CheckTimer()
    {
        if (IsOnTouch)
            TouchDurationTimer();
    }

    void TouchDurationTimer()
    {
        m_touchDuration += Time.deltaTime;

        /*if (m_touchDuration > m_durationForAnImpulse)
        {
            cubeControlled.UpdateImpulse(ref m_firstTouchPosition, ref m_currentTouchPosition);
        }*/
    }

    #endregion

    #region TouchFunctions
    void CheckTouch()
    {
        if (Input.touchCount == 2)
            UpdateTouchZoom();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    IsOnTouch = true;
                    break;
                case TouchPhase.Moved:
                    TouchMoved(ref touch);
                    break;
                case TouchPhase.Ended:
                    TouchEnded(ref touch);
                    break;
                case TouchPhase.Canceled:
                    EndTouch();
                    break;
                default:
                    break;
            }
        }
    }

    void UpdateTouchZoom()
    {
        Touch touch_first = Input.GetTouch(0);
        Touch touch_second = Input.GetTouch(1);

        switch (touch_first.phase)
        {
            case TouchPhase.Began:
                IsOnTouch = true;
                break;
            case TouchPhase.Moved:
                TouchZoomMoved(ref touch_first, ref touch_second);
                break;
            case TouchPhase.Ended:
                EndZoomTouch();
                break;
            case TouchPhase.Canceled:
                EndZoomTouch();
                break;
            default:
                break;
        }
    }

    void TouchMoved(ref Touch touch)
    {
        m_currentTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);

        if (m_firstTouchPosition == Vector3.zero)
            m_firstTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);
    }

    void TouchEnded(ref Touch touch)
    {
        /*if (m_touchDuration <= m_durationForAnImpulse)
            cubeControlled.Jump(ref m_currentTouchPosition);
        else
            cubeControlled.Propulse();*/

        EndTouch();
    }

    void EndTouch()
    {
        // Reset Touch Variables
        IsOnTouch = false;
        m_touchDuration = 0f;
        m_firstTouchPosition = Vector3.zero;

        //cubeControlled.EndTouch();
    }


    void TouchZoomMoved(ref Touch firstTouch, ref Touch secondTouch)
    {
        Vector3 tmpVec = firstTouch.position - secondTouch.position;
        float currentDistance = tmpVec.sqrMagnitude;

        MoveZoomCamera(currentDistance);

        PreviousDistanceBetweenTouch = currentDistance;
    }

    void MoveZoomCamera(float currentDistance)
    {
        Vector3 UnitVec = cameraTarget.position - cameraTransform.position;
        UnitVec = Vector3.Normalize(UnitVec);

        cameraTransform.position = cameraTransform.position + Time.deltaTime * (currentDistance-PreviousDistanceBetweenTouch) * UnitVec;
    }

    void EndZoomTouch()
    {
        // Reset Touch Variables
        IsOnTouch = false;
        m_touchDuration = 0f;
        m_firstTouchPosition = Vector3.zero;

        PreviousDistanceBetweenTouch = 0.0f;
    }
    #endregion
}
