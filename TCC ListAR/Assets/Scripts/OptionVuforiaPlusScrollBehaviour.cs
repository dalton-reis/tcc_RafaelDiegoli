using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Vuforia;

public enum VuforiaScrollType
{
    /// <summary>
    /// With Scroll Behaviour only one button can be selected at a time.
    /// </summary>
    ScrollBehaviour,

    /// <summary>
    /// With Slider Behaviour the buttons from start until the one actually pressed are selected.
    /// </summary>
    SliderBehaviour,
}

public class ScrollValueChangedEventArgs
{
    public int OldValue { get; set; }
    public int NewValue { get; set; }

    public ScrollValueChangedEventArgs()
    {
    }

    public ScrollValueChangedEventArgs(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

public class OptionVuforiaPlusScrollBehaviour : MonoBehaviour, IVirtualButtonEventHandler
{
    public GameObject Content;
    public GameObject VirtualStepPrefab;

    public Sprite UnselectedSprite;
    public Sprite SelectedSprite;
    public Sprite UnselectedIcon;
    public Sprite SelectedIcon;

    public int Steps;
    public float HoldOnTime = 1;
    public VuforiaScrollType BehaviourType;

    public delegate void ScrollValueChangedEventHandler(OptionVuforiaPlusScrollBehaviour sender, ScrollValueChangedEventArgs args);
    public event ScrollValueChangedEventHandler ValueChanged;

    protected virtual void RaiseValueChanged(ScrollValueChangedEventArgs args)
    {
        if (ValueChanged != null)
            ValueChanged(this, args);
    }

    public int Value
    {
        get
        {
            for (int i = virtualStepsList.Count - 1; i >= 0; i--)
            {
                var stepVirtualCheck = virtualStepsList[i].GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>();
                if (stepVirtualCheck.IsChecked)
                    return i;
            }

            return -1;
        }

        set
        {
            var step = virtualStepsList[value].GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>();
            step.IsChecked = true;

            SelectSteps(virtualStepsList[value]);
        }
    }

    List<GameObject> virtualStepsList;
    Dictionary<string, float> pressedTime = new Dictionary<string, float>();

    void Start()
    {
        virtualStepsList = new List<GameObject>(Steps);
        
        Transform transform = Content.transform;
        float newScale = transform.localScale.y / Steps;

        for (int i = 0; i < Steps; i++)
        {
            var vbPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - newScale * (i * Steps));

            var virtualStep = Instantiate(VirtualStepPrefab, vbPosition, transform.rotation, transform);
            virtualStep.gameObject.name = string.Format("{0}Step{1}", gameObject.name, i);
            virtualStep.transform.localScale = new Vector3(0.25f, newScale, 0.25f);

            var virtualBtn = virtualStep.GetComponentInChildren<VirtualButtonBehaviour>();

            //Vuforia does not allow setting VirtualButtonName outside editor
            var fieldNameInfo = virtualBtn.GetType().GetField("mName", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldNameInfo.SetValue(virtualBtn, virtualStep.gameObject.name);

            virtualBtn.RegisterEventHandler(this);

            var stepVirtualCheck = virtualStep.GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>();

            stepVirtualCheck.StandyBySprite = UnselectedSprite;
            stepVirtualCheck.PressedSprite = SelectedSprite;
            stepVirtualCheck.UncheckedSprite = UnselectedIcon;
            stepVirtualCheck.CheckedSprite = SelectedIcon;

            virtualStepsList.Add(virtualStep);
        }

        Value = 0;
	}
	
	void Update()
    {
	}

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        if (pressedTime.ContainsKey(vb.VirtualButtonName))
            pressedTime.Remove(vb.VirtualButtonName);

        pressedTime.Add(vb.VirtualButtonName, Time.time);
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        float time;
        pressedTime.TryGetValue(vb.VirtualButtonName, out time);

        if ((Time.time - time) >= HoldOnTime)
        {
            if (vb.GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>().ButtonType == VirtualButtonType.CheckBox)
            {
                var virtualCheckbox = vb.GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>();
                if (!virtualCheckbox.IsChecked)
                {
                    virtualCheckbox.ChangeCheck();
                }

                SelectSteps(vb.gameObject.transform.parent.gameObject);
            }
        }
    }

    void SelectSteps(GameObject pressedButton)
    {
        int pressedIndex = virtualStepsList.IndexOf(pressedButton);

        switch (BehaviourType)
        {
            case VuforiaScrollType.ScrollBehaviour:
                InternalSelectSteps(pressedIndex);
                break;

            case VuforiaScrollType.SliderBehaviour:
                InternalSelectSteps(Enumerable.Range(0, pressedIndex + 1).ToArray());
                break;
        }
    }

    void InternalSelectSteps(params int[] buttons)
    {
        int oldValue = 0;

        for (int i = 0; i < virtualStepsList.Count; i++)
        {
            var checkBehaviour = virtualStepsList[i].GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>();

            if (checkBehaviour.IsChecked && i != buttons.Max() && i > oldValue)
                oldValue = i;

            checkBehaviour.IsChecked = buttons.Contains(i);
        }

        int newValue = buttons.Max();
        if (oldValue != newValue)
            RaiseValueChanged(new ScrollValueChangedEventArgs(oldValue, newValue));
    }
}
