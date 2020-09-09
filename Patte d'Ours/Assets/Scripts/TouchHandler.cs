using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class TouchHandler : MonoBehaviour
{
    //[SerializeField] CubeBehavior cubeControlled = null;

    #region TouchVars

    bool IsOnTouch = false;

    private float m_touchDuration = 0f;
    //private float m_durationForASwipe = 0.25f;

    private Vector3 m_firstTouchPosition = Vector3.zero;
    private Vector3 m_currentTouchPosition = Vector3.zero;
    private Vector3 m_previousTouchPosition = Vector3.zero;
    public float length = 0;
    #endregion

    #region SwipeVars

    Touch previousTouch;
    float rotSpeed = 4f;

    Vector3 previousMousePos = Vector3.zero;
    bool isAlreadyClick = false;

    #endregion

    #region ZoomVars

    [SerializeField] Transform cameraTarget = null;
    [SerializeField] Transform cameraTransform = null;

    public float ZoomSpeedMobile = 0.0001f;
    public float ZoomSpeedPC = 0.3f;

    #endregion

    void Start()
    {
        Application.targetFrameRate = 62;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTouch();
        CheckTimer();

        Vector3 UnitVec = cameraTarget.position - cameraTransform.position;
        length = UnitVec.magnitude;
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
        else if (Input.touchCount > 0)
            UpdateTouchSwipe();


        CheckInputPC();
    }

    void CheckInputPC()
    {
        CheckZoomPCWithScroll();

        CheckSwipeWithMouse();
    }

    void CheckZoomPCWithScroll()
    {
        Vector3 UnitVec = cameraTarget.position - cameraTransform.position;
        length = UnitVec.magnitude;
        UnitVec = Vector3.Normalize(UnitVec);

        float zoomModifier = Input.mouseScrollDelta.y * ZoomSpeedPC;

        if ((length < 50 && zoomModifier > 0) || (length > 15 && zoomModifier < 0))
            cameraTransform.position = cameraTransform.position - zoomModifier * UnitVec;
    }

    void CheckSwipeWithMouse()
    {
        if (Input.GetMouseButton(0))
        {
            if (!isAlreadyClick)
            {
                isAlreadyClick = true;
                previousMousePos = Input.mousePosition;
            }

            float deltaX = previousMousePos.x - Input.mousePosition.x;
            float rotY = -deltaX * Time.deltaTime * rotSpeed;

            cameraTarget.eulerAngles = new Vector3(cameraTarget.eulerAngles.x, cameraTarget.eulerAngles.y + rotY);
            previousMousePos = Input.mousePosition;
        }
        else
            isAlreadyClick = false;

    }

    void UpdateTouchSwipe()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                IsOnTouch = true;
                previousTouch = touch;
                break;
            case TouchPhase.Moved:
                RotateCamera(ref touch);
                break;
            default:
                break;
        }
    }

    void RotateCamera(ref Touch touch)
    {
        float deltaX = previousTouch.position.x - touch.position.x;
        float rotY = -deltaX * Time.deltaTime * rotSpeed;

        cameraTarget.eulerAngles = new Vector3(cameraTarget.eulerAngles.x, cameraTarget.eulerAngles.y + rotY);
        previousTouch = touch;
    }

    void UpdateTouchZoom()
    {
        float touchesPrevPosDifference = 0; float touchesCurPosDifference = 0;
        float zoomModifier = CalcZoomModifier(ref touchesPrevPosDifference, ref touchesCurPosDifference);

        Vector3 UnitVec = cameraTarget.position - cameraTransform.position;
        length = UnitVec.magnitude;
        UnitVec = Vector3.Normalize(UnitVec);

        // Move and check min and max of zoom
        if (touchesPrevPosDifference > touchesCurPosDifference && length < 50)
            cameraTransform.position = cameraTransform.position - zoomModifier * UnitVec;
        else if (touchesPrevPosDifference < touchesCurPosDifference && length > 15)
            cameraTransform.position = cameraTransform.position + zoomModifier * UnitVec;
    }

    float CalcZoomModifier(ref float touchesPrevPosDifference, ref float touchesCurPosDifference)
    {
        Touch firstTouch = Input.GetTouch(0);
        Touch secondTouch = Input.GetTouch(1);

        Vector3 firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
        Vector3 secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

        touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
        touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

        return (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * ZoomSpeedMobile;
    }

    #endregion
}
