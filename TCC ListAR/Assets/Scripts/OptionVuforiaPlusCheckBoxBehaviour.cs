using System.Linq;
using UnityEngine;

public class OptionVuforiaPlusCheckBoxBehaviour : OptionVuforiaPlusBehaviour
{
    bool isChecked;

    public Sprite UncheckedSprite;
    public Sprite CheckedSprite;

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
    }
	
	void Update()
    {
	}

    public void ChangeCheck()
    {
        IsChecked = !IsChecked;
    }
}
