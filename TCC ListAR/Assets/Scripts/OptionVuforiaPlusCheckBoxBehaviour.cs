using System.Linq;
using UnityEngine;

public class CheckBoxCheckChangedEventArgs
{
    public bool IsChecked { get; set; }

    public CheckBoxCheckChangedEventArgs()
    {
    }

    public CheckBoxCheckChangedEventArgs(bool checkValue)
    {
        IsChecked = checkValue;
    }
}

public class OptionVuforiaPlusCheckBoxBehaviour : OptionVuforiaPlusBehaviour
{
    bool isChecked;

    TextMesh internalText;
    TextMesh InternalText
    {
        get
        {
            if (internalText == null)
                internalText = gameObject.GetComponentInChildren<TextMesh>();

            return internalText;
        }
    }

    public Sprite UncheckedSprite;
    public Sprite CheckedSprite;
    public string text;

    public string Text
    {
        get
        {
            if (InternalText == null)
                return string.Empty;

            return InternalText.text;
        }

        set
        {
            if (InternalText == null)
                return;

            InternalText.text = value;
            text = value;
        }
    }

    public delegate void CheckBoxCheckChangedEventHandler(OptionVuforiaPlusCheckBoxBehaviour sender, CheckBoxCheckChangedEventArgs args);
    public event CheckBoxCheckChangedEventHandler CheckChanged;

    protected virtual void RaiseCheckChanged(CheckBoxCheckChangedEventArgs args)
    {
        if (CheckChanged != null)
            CheckChanged(this, args);
    }

    SpriteRenderer iconSprite;
    SpriteRenderer IconSprite
    {
        get
        {
            if (iconSprite == null)
                iconSprite = gameObject.GetComponentsInChildren<SpriteRenderer>().Where(x => x.gameObject != gameObject).FirstOrDefault();

            return iconSprite;
        }
    }

    public bool IsChecked
    {
        get { return isChecked; }
        set
        {
            IconSprite.sprite = value ? CheckedSprite : UncheckedSprite;

            if (isChecked != value)
            {
                isChecked = value;
                RaiseCheckChanged(new CheckBoxCheckChangedEventArgs(isChecked));
            }
        }
    }

    public override VirtualButtonType ButtonType
    {
        get { return VirtualButtonType.CheckBox; }
    }

    void Start()
    {
        InternalStart();
        IsChecked = IconSprite.sprite == CheckedSprite;
        Text = text;
    }

    void Update()
    {
    }

    public void ChangeCheck()
    {
        IsChecked = !IsChecked;
    }
}
