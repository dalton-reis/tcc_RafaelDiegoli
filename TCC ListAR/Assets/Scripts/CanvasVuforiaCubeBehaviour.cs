using UnityEngine;

public class CanvasVuforiaCubeBehaviour : MonoBehaviour
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

    void Start()
    {
        standyBySprite = GetComponentInChildren<SpriteRenderer>().sprite;
	}
	
	void Update()
    {
	}

    void OnTriggerEnter(Collider other)
    {
        GetComponentInChildren<SpriteRenderer>().sprite = ActivatedSprite;
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
        Vector3 angles = gameObject.transform.rotation.eulerAngles;

        //Debug.Log(string.Format("angle x: {0} y: {1} z: {2}", angles.x, angles.y, angles.z));

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
        GetComponentInChildren<SpriteRenderer>().sprite = standyBySprite;
    }
}
