﻿using System.Linq;
using UnityEngine;

public class OptionVuforiaPlusComboBoxBehaviour : MonoBehaviour
{
    public GameObject MainCheck;
    public GameObject OptionsScroll;

    OptionVuforiaPlusCheckBoxBehaviour CheckBehaviour
    {
        get { return MainCheck.GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>(); }
    }

    OptionVuforiaPlusScrollBehaviour ScrollBehaviour
    {
        get { return OptionsScroll.GetComponentInChildren<OptionVuforiaPlusScrollBehaviour>(); }
    }

    Vector3 originalOptionsScale;

    public int SelectedIndex
    {
        get { return ScrollBehaviour.Value; }
        set { ScrollBehaviour.Value = value; }
    }

    public string SelectedValue
    {
        get { return CheckBehaviour.Text; }
    }

    void Start()
    {
    }

    private void OnMainCheckChanged(OptionVuforiaPlusCheckBoxBehaviour sender, CheckBoxCheckChangedEventArgs args)
    {
        OptionsScroll.transform.localScale = args.IsChecked ? originalOptionsScale : Vector3.zero;
    }

    private void OnOptionScrollValueChanged(OptionVuforiaPlusScrollBehaviour sender, ScrollValueChangedEventArgs args)
    {
        string text = sender.virtualStepsList[args.NewValue].GetComponentInChildren<OptionVuforiaPlusCheckBoxBehaviour>().text;
        CheckBehaviour.Text = text;
        CheckBehaviour.ChangeCheck();
    }

    bool firstUpdate = true;

    void Update()
    {
        if (firstUpdate)
        {
            firstUpdate = false;
            InternalStart();
        }
	}

    void InternalStart()
    {
        originalOptionsScale = OptionsScroll.transform.localScale;

        ScrollBehaviour.ValueChanged += OnOptionScrollValueChanged;
        SelectedIndex = 0;
        CheckBehaviour.Text = ScrollBehaviour.Elements.First();

        CheckBehaviour.CheckChanged += OnMainCheckChanged;
        CheckBehaviour.IsChecked = false;
    }
}
