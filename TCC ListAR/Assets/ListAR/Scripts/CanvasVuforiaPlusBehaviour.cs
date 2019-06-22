using System;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CanvasVuforiaPlusBehaviour : OptionVuforiaBehaviour, IVirtualButtonEventHandler
{
    public enum CanvasVuforiaAction
    {
        Nothing,
        Next,
        Previous,
        ChangeType,
        BackToMenu,
        AddItems,
        DeleteItem,
        SelectItem,
        SetVisible,
    }

    public float HoldOnTime = 1;
    public GameObject[] Buttons;
    public GameObject SetScaleScroll;

    Dictionary<string, float> pressedTime = new Dictionary<string, float>();
    Vector3 originalScale;

    CanvasVuforiaListBehaviour ListBehaviour
    {
        get { return gameObject.GetComponent<CanvasVuforiaListBehaviour>(); }
    }

    OptionVuforiaPlusScrollBehaviour scrollBehaviour;
    OptionVuforiaPlusScrollBehaviour ScrollBehaviour
    {
        get
        {
            if (scrollBehaviour == null && SetScaleScroll != null)
                scrollBehaviour = SetScaleScroll.GetComponentInChildren<OptionVuforiaPlusScrollBehaviour>();

            return scrollBehaviour;
        }
    }

    void ApplyCanvasVuforiaAction(CanvasVuforiaAction action, GameObject virtualObj)
    {
        switch (action)
        {
            case CanvasVuforiaAction.Next:
                virtualObj.GetComponentInChildren<OptionVuforiaPlusBehaviour>().ExecuteAction += OnNextItem;
                break;

            case CanvasVuforiaAction.Previous:
                virtualObj.GetComponentInChildren<OptionVuforiaPlusBehaviour>().ExecuteAction += OnPreviousItem;
                break;

            case CanvasVuforiaAction.ChangeType:
                virtualObj.GetComponentInChildren<OptionVuforiaPlusBehaviour>().ExecuteAction += OnChangeType;
                break;

            case CanvasVuforiaAction.BackToMenu:
                virtualObj.GetComponentInChildren<OptionVuforiaPlusBehaviour>().ExecuteAction += OnBackToMenu;
                break;

            case CanvasVuforiaAction.AddItems:
                virtualObj.GetComponentInChildren<OptionVuforiaPlusBehaviour>().ExecuteAction += OnAddItems;
                break;

            case CanvasVuforiaAction.DeleteItem:
                virtualObj.GetComponentInChildren<OptionVuforiaPlusBehaviour>().ExecuteAction += OnDeleteItem;
                break;

            case CanvasVuforiaAction.SelectItem:
                virtualObj.GetComponentInChildren<OptionVuforiaPlusBehaviour>().ExecuteAction += OnSelectItem;
                break;

            case CanvasVuforiaAction.SetVisible:
                virtualObj.GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>().CheckChanged += OnSetVisible;
                break;

            //If action is CanvasVuforiaAction.Nothing probably its a custom action not a ListAR action
            default:
                break;
        }
    }

    protected virtual void OnNextItem(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        OnNextItem();
    }

    protected virtual void OnPreviousItem(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        OnPreviousItem();
    }

    protected virtual void OnChangeType(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        OnChangeIterableType();
    }

    protected virtual void OnBackToMenu(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        OnBackToMenu();
    }

    protected virtual void OnAddItems(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        OnAddItems();
    }

    protected virtual void OnDeleteItem(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        OnDeleteItem();
    }

    protected virtual void OnSelectItem(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        OnSelectItem();
    }

    protected virtual void OnSetVisible(OptionVuforiaPlusCheckBoxBehaviour sender, CheckBoxCheckChangedEventArgs args)
    {
        if (args.IsChecked)
            ListARObject.ShowItem();
        else
            ListARObject.HideItem();
    }

    void Start()
    {
        InternalInitialize();

        foreach (var button in Buttons)
        {
            var vbBehaviour = button.GetComponent<VirtualButtonBehaviour>();
            vbBehaviour.RegisterEventHandler(this);

            ApplyCanvasVuforiaAction(SafeParseEnum<CanvasVuforiaAction>(vbBehaviour.VirtualButtonName), button);
        }

        if (ListARObject != null)
        {
            ListARObject.ItemsAdded += OnListARChanged;
            ListARObject.ItemsRemoved += OnListARChanged;
            ListARObject.CurrentItemChanged += OnListARItemChanged;
        }
        
        if (ListBehaviour != null)
            ListBehaviour.RefreshList(ListARObject, true);

        if (ScrollBehaviour != null)
            ScrollBehaviour.ValueChanged += OnScaleValueChanged;
    }

    private void OnScaleValueChanged(OptionVuforiaPlusScrollBehaviour sender, ScrollValueChangedEventArgs args)
    {
        ListARObject.CurrentItem.ObjPrefab.transform.localScale = originalScale + new Vector3(args.NewValue, args.NewValue, args.NewValue);
    }

    private void OnListARItemChanged(ListAR sender, ListARItemChangedEventArgs args)
    {
        if (ListBehaviour != null)
            ListBehaviour.RefreshList(sender, false);
    }

    private void OnListARChanged(ListAR sender, ListAREventArgs args)
    {
        if (ListBehaviour != null)
            ListBehaviour.RefreshList(sender, true);

        if (originalScale == Vector3.zero)
            originalScale = ListARObject.CurrentItem.OriginalLocalScale;
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

    TEnum SafeParseEnum<TEnum>(string name)
    {
        foreach (string enumName in Enum.GetNames(typeof(TEnum)))
            if (string.Compare(enumName, name, true) == 0)
                return (TEnum)Enum.Parse(typeof(TEnum), name);

        return default(TEnum);
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        float time;
        pressedTime.TryGetValue(vb.VirtualButtonName, out time);

        if ((Time.time - time) >= HoldOnTime)
        {
            var vPlusBehaviour = vb.GetComponentInChildren<OptionVuforiaPlusBehaviour>();

            switch (vPlusBehaviour.ButtonType)
            {
                case VirtualButtonType.CheckBox:
                    {
                        var virtualCheckbox = vb.GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>();
                        virtualCheckbox.ChangeCheck();
                    }
                    break;

                default:
                    vPlusBehaviour.RaiseExecuteAction(new OptionVuforiaPlusActionEventArgs(vb.VirtualButtonName));
                    break;
            }
        }
    }
}
