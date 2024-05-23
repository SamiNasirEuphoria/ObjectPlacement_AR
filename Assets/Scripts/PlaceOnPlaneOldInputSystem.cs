using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// For tutorial video, see my YouTube channel: <seealso href="https://www.youtube.com/@xiennastudio">YouTube channel</seealso>
/// How to use this script:
/// - Add ARPlaneManager to XROrigin GameObject.
/// - Add ARRaycastManager to XROrigin GameObject.
/// - Attach this script to XROrigin GameObject.
/// - Add the prefab that will be spawned to the <see cref="placedPrefab"/>
/// 
/// Touch to place the <see cref="placedPrefab"/> object on the touch position.
/// Will only placed the object if the touch position is on detected trackables.
/// Move the existing spawned object on the touch position.
/// Using Unity old input system.
/// </summary>
[HelpURL("https://youtu.be/HkNVp04GOEI")]
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlaneOldInputSystem : MonoBehaviour
{
    /// <summary>
    /// The prefab that will be instantiated on touch.
    /// </summary>
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject placedPrefab;

    /// <summary>
    /// The instantiated object.
    /// </summary>
    private GameObject spawnedObject, selected,lastSelected;
    private Vector3 initialScale;
    private float initialDist, currentDist;
    private bool check,touchHold, pauseSpawning;
    ARRaycastManager aRRaycastManager;
    private Vector3 startPos, endPos;
    private List<GameObject> cubes = new List<GameObject>();
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    void FixedUpdate()
    {
        AreaCheck();
        SpawnObjects();
        ResizingObject();

    }
    public void SpawnObjects()
    {
        if ((Input.touchCount==1) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            startPos = Input.GetTouch(0).position;
        }
        if ((Input.touchCount==1) && (Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            //endPos = Input.mousePosition;
            endPos = Input.GetTouch(0).position;
            if (startPos == endPos && !(touchHold))
            {
                Vector3 mousePos = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                if (aRRaycastManager.Raycast(ray, hits, TrackableType.AllTypes))
                {
                    // Raycast hits are sorted by distance, so the first hit means the closest.
                    var hitPose = hits[0].pose;
                    spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
                    cubes.Add(spawnedObject);
                    Debug.Log("Cube placed here");
                }
            }
        }
    }
    public void ResetSelected(GameObject recent )
    {
        foreach (GameObject gb in cubes)
        {
            if (gb != recent)
            {
                gb.GetComponent<LeanPinchScale>().enabled = false;
                gb.GetComponent<LeanTwistRotateAxis>().enabled = false;
                gb.GetComponent<LeanDragTranslate>().enabled = false;
            }

        }
    }
    public bool AreaCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.CompareTag(TagManager.Cube_tag))
            {
                touchHold  = true;
                selected = hit.transform.gameObject;
                ResetSelected(selected);
                selected.GetComponent<LeanPinchScale>().enabled = true;
                selected.GetComponent<LeanTwistRotateAxis>().enabled = true;
                selected.GetComponent<LeanDragTranslate>().enabled = true;
                return touchHold;
            }
            else
            {
                touchHold = false;
                return touchHold;
            }
        }
        else
        {
            touchHold = false;
            return touchHold;
        }
    }
    public void ResizingObject()
    {
        if(Input.touchCount == 2 && selected)
        {
            var touchCount_1 = Input.GetTouch(0);
            var touchCount_2 = Input.GetTouch(1);
            if (touchCount_1.phase == TouchPhase.Canceled || touchCount_1.phase == TouchPhase.Ended ||
                 touchCount_2.phase == TouchPhase.Canceled || touchCount_2.phase == TouchPhase.Ended)
            {
                return;
            }
            if (touchCount_1.phase == TouchPhase.Began || touchCount_2.phase == TouchPhase.Began)
            {
                initialDist = Vector2.Distance(touchCount_2.position, touchCount_1.position);
                initialScale = selected.transform.localScale;
                
            }
            else
            {
                check = false;
                currentDist = Vector2.Distance(touchCount_2.position, touchCount_1.position);
                if (Mathf.Approximately(initialDist, 0))
                {
                    return;
                }
                var factor = currentDist / initialDist;
                selected.transform.localScale = initialScale*factor;
            }  
        }
    }
}
