using System.Linq;
using UnityEngine;

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
        get { return InternalText.text; }
        set { InternalText.text = value; }
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
            isChecked = value;
            IconSprite.sprite = isChecked ? CheckedSprite : UncheckedSprite;
        }
    }

    public new VirtualButtonType ButtonType
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
