using UnityEngine;
using Vuforia;

public class OptionVuforiaPlusBehaviour : MonoBehaviour, IVirtualButtonEventHandler
{
    public Sprite PressedSprite;
    public GameObject VirtualButton;

    Sprite standyBySprite;

    void Start()
    {
        VirtualButton.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
        standyBySprite = GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        GetComponent<SpriteRenderer>().sprite = PressedSprite;
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        GetComponent<SpriteRenderer>().sprite = standyBySprite;
    }
}
