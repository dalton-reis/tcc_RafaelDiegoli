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

    void Start()
    {
        InternalInitialize();

        foreach (var button in Buttons)
            button.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);

        ListARObject.ItemsAdded += OnListARChanged;
        ListARObject.ItemsRemoved += OnListARChanged;
        ListARObject.CurrentItemChanged += OnListARItemChanged;

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
            CanvasVuforiaAction action = SafeParseEnum<CanvasVuforiaAction>(vb.VirtualButtonName);
            OptionVuforiaPlusCheckBoxBehaviour virtualCheckbox = null;

            if (vb.GetComponentInChildren<OptionVuforiaPlusBehaviour>().ButtonType == VirtualButtonType.CheckBox)
            {
                virtualCheckbox = vb.GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>();
                virtualCheckbox.ChangeCheck();
            }

            switch (action)
            {
                case CanvasVuforiaAction.Next:
                    OnNextItem();
                    break;

                case CanvasVuforiaAction.Previous:
                    OnPreviousItem();
                    break;

                case CanvasVuforiaAction.ChangeType:
                    OnChangeIterableType();
                    break;

                case CanvasVuforiaAction.BackToMenu:
                    OnBackToMenu();
                    break;

                case CanvasVuforiaAction.AddItems:
                    OnAddItems();
                    break;

                case CanvasVuforiaAction.DeleteItem:
                    OnDeleteItem();
                    break;

                case CanvasVuforiaAction.SelectItem:
                    OnSelectItem();
                    break;

                case CanvasVuforiaAction.SetVisible:
                    {
                        if (virtualCheckbox != null)
                        {
                            if (virtualCheckbox.IsChecked)
                                ListARObject.ShowItem();
                            else
                                ListARObject.HideItem();
                        }
                    }
                    break;
            }
        }
    }
}
