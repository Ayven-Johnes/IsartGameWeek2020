using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class ItemSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private ShopHandler MyShop = null;
    [SerializeField]
    private float percentTreshold = 0.2f;
    [SerializeField]
    private float easing = 0.5f;

    private int CurrentItem = 1;

    private bool reset = true;
    private bool setupReset = false;
    private Vector3 Location;
    private Vector3 ResetLocation;

    [SerializeField]
    private AudioClip SwipSound;
    private AudioSource Source { get { return GetComponent<AudioSource>(); } }


    public void Setup()
    {
        Location = transform.position;
        if (!setupReset)
            ResetLocation = Location;
    }

    public void Update()
    {
        if(MyShop.currentCategory == ShopHandler.Category.ICEBERG && !reset)
        {
            transform.position = ResetLocation;
            reset = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (MyShop.currentCategory == ShopHandler.Category.ICEBERG)
            return;

        float difference = eventData.pressPosition.x - eventData.position.x;
        transform.position = Location - new Vector3(difference, 0, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (MyShop.currentCategory == ShopHandler.Category.ICEBERG)
            return;

        reset = false;

        float percentage = (eventData.pressPosition.x - eventData.position.x) / Screen.width;
        if (Mathf.Abs(percentage) >= percentTreshold)
        {
            Vector3 newLocation = Location;
            if (percentage > 0 && CurrentItem != 3)
            {
                newLocation += new Vector3(-Screen.width, 0, 0);
                CurrentItem++;
            }
            else if (percentage < 0 && CurrentItem != 1)
            {
                newLocation += new Vector3(Screen.width, 0, 0);
                CurrentItem--;
            }

            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            Location = newLocation;
            Source.PlayOneShot(SwipSound);
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, Location, easing));
        }
    }

    IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1.0f, t));
            yield return null;
        }
    }
}
