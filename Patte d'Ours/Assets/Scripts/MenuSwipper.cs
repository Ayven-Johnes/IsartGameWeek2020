using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MenuSwipper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 Location;
    [SerializeField]
    private float percentTreshold = 0.2f;
    [SerializeField]
    private float easing = 0.5f;

    private bool canUp = true;

    [SerializeField]
    private ItemSwiper Swiper = null;

    // Start is called before the first frame update
    void Start()
    {
        Location = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float difference = eventData.pressPosition.y - eventData.position.y;
        transform.position = Location - new Vector3(0, difference, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float percentage = (eventData.pressPosition.y - eventData.position.y) / (Screen.height / 2);
        if(Mathf.Abs(percentage) >= percentTreshold)
        {
            Vector3 newLocation = Location;
            if(percentage > 0 && !canUp)
            {
                newLocation += new Vector3(0, -Screen.height / 2 , 0);
                canUp = true;
            }
            else if(percentage < 0 && canUp)
            {
                newLocation += new Vector3(0, Screen.height / 2, 0);
                canUp = false;
            }

            StartCoroutine(SmoothMove(transform.position, newLocation, easing, Swiper));
            Location = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, Location, easing, Swiper));
        }
    }

    IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float seconds, ItemSwiper it)
    {
        float t = 0f;
        while( t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1.0f, t));
            yield return null;
        }
        it.Setup();
    }
}
