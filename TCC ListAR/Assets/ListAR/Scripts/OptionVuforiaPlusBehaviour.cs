using UnityEngine;
using Vuforia;

public enum VirtualButtonType
{
    Button,
    CheckBox,
}

public class OptionVuforiaPlusActionEventArgs
{
    public string VuforiaVBName { get; set; }

    public OptionVuforiaPlusActionEventArgs()
    {
        VuforiaVBName = string.Empty;
    }

    public OptionVuforiaPlusActionEventArgs(string vbName)
    {
        VuforiaVBName = vbName;
    }
}

public class OptionVuforiaPlusBehaviour : OptionVuforiaBehaviour, IVirtualButtonEventHandler
{
    public Sprite PressedSprite;
    public GameObject VirtualButton;
    public virtual VirtualButtonType ButtonType
    {
        get { return VirtualButtonType.Button; }
    }

    public Sprite StandyBySprite;

    public delegate void OptionVuforiaPlusActionEventHandler(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args);
    public event OptionVuforiaPlusActionEventHandler ExecuteAction;

    protected void InternalStart()
    {
        VirtualButton.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);

        if (StandyBySprite == null)
            StandyBySprite = GetComponent<SpriteRenderer>().sprite;
    }

    void Start()
    {
        InternalStart();
    }

    void Update()
    {
    }

    public virtual void RaiseExecuteAction(OptionVuforiaPlusActionEventArgs args)
    {
        if (ExecuteAction != null)
            ExecuteAction(this, args);
    }

    public virtual void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = PressedSprite;
    }

    public virtual void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = StandyBySprite;
    }
}
