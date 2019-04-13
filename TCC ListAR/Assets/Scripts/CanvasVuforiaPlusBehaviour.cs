using System;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CanvasVuforiaPlusBehaviour : OpcaoVuforiaBehaviour, IVirtualButtonEventHandler
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
    }

    public float holdOnTime = 1;
    public GameObject[] Buttons;

    Dictionary<string, float> pressedTime = new Dictionary<string, float>();

    CanvasVuforiaListBehaviour ListBehaviour
    {
        get { return gameObject.GetComponent<CanvasVuforiaListBehaviour>(); }
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

        if ((Time.time - time) >= holdOnTime)
        {
            CanvasVuforiaAction action = (CanvasVuforiaAction)Enum.Parse(typeof(CanvasVuforiaAction), vb.VirtualButtonName);

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
            }
        }
    }
}
