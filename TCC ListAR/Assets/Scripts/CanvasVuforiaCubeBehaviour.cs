using UnityEngine;
using Vuforia;

public class CanvasVuforiaCubeBehaviour : MonoBehaviour, ITrackableEventHandler
{
    public enum ReferenceAxis
    {
        X,
        Y,
        Z
    }

    public Sprite ActivatedSprite;
    public ReferenceAxis Axis = ReferenceAxis.X;
    public float AngleValue = 1;
    public CanvasVuforiaPlusBehaviour VuforiaPlusCanvas;

    float lastAngle;
    Sprite standyBySprite;
    bool isActive, isColliding;

    void Start()
    {
        standyBySprite = GetComponentInChildren<SpriteRenderer>().sprite;
        GetComponent<MultiTargetBehaviour>().RegisterTrackableEventHandler(this);
    }
	
	void Update()
    {
	}

    void ChangeSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = isActive && isColliding ? ActivatedSprite : standyBySprite;
    }

    void OnTriggerEnter(Collider other)
    {
        isColliding = true;
        ChangeSprite();
    }

    float GetAxisAngle(Vector3 angles)
    {
        switch (Axis)
        {
            case ReferenceAxis.Y:
                return angles.y;

            case ReferenceAxis.Z:
                return angles.z;

            default:
                return angles.x;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!(isActive && isColliding))
            return;

        Vector3 angles = gameObject.transform.rotation.eulerAngles;

        float currentRotation = GetAxisAngle(angles);

        if (Mathf.Abs((currentRotation - lastAngle)) >= AngleValue)
        {
            if (currentRotation - lastAngle > 0)
            {
                Debug.Log("Next");
                VuforiaPlusCanvas.OnNextItem();
            }
            else
            {
                Debug.Log("Previous");
                VuforiaPlusCanvas.OnPreviousItem();
            }

            lastAngle = currentRotation;
        }
    }

    void OnTriggerExit(Collider other)
    {
        isColliding = false;
        ChangeSprite();
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus != TrackableBehaviour.Status.DETECTED && newStatus != TrackableBehaviour.Status.TRACKED)
        {
            isColliding = false;
            isActive = false;
        }
        else
        {
            isActive = true;
        }

        ChangeSprite();
    }
}
