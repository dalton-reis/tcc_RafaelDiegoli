using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class CanvasVuforiaPlusBehaviour : OpcaoVuforiaBehaviour, IVirtualButtonEventHandler
{
    public const string TAG_OBJECTIVE = "ObjectiveText";

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

    public struct Objective
    {
        public string descripton { get; set; }
        public string color { get; set; }
        public string objectName { get; set; }
        public int count { get; set; }

        public bool checkCount
        {
            get { return count > 0; }
        }

        public bool checkColor
        {
            get { return !string.IsNullOrEmpty(color); }
        }

        public bool checkObject
        {
            get { return !string.IsNullOrEmpty(objectName); }
        }

        public Objective(string desc, string colorName, string objName, int itemCount)
        {
            descripton = desc;
            color = colorName;
            objectName = objName;
            count = itemCount;
        }
    }

    public float holdOnTime = 1;
    public GameObject[] Buttons;

    static readonly Objective[] OBJECTIVES = new Objective[]
    {
        new Objective("Selecione uma esfera azul", "blue", "sphere", 0),
        new Objective("Selecione um cilindro vermelho", "red", "cylinder", 0),
        new Objective("Selecione um cubo preto", "black", "cube", 0),
        new Objective("Selecione uma esfera branca", "white", "sphere", 0),
        new Objective("Deixe apenas um cubo vermelho", "red", "cube", 1),
        new Objective("Deixe 3 itens", null, null, 3),
        new Objective("Você concluiu todos os objetivos", null, null, 0),
    };

    static Text objectiveText;
    static int currentObjectiveIndex = -1;

    Dictionary<string, float> pressedTime = new Dictionary<string, float>();

    static Objective CurrentObjective
    {
        get
        {
            if (currentObjectiveIndex > -1 && currentObjectiveIndex < OBJECTIVES.Length)
                return OBJECTIVES[currentObjectiveIndex];

            return OBJECTIVES.Last();
        }
    }

    static void ChangeObjective()
    {
        if (objectiveText == null)
            return;

        currentObjectiveIndex++;

        objectiveText.text = CurrentObjective.descripton;
    }

    public static void CheckSelectedItem()
    {
        string[] palavrasDescItem = ListARObject.CurrentItem.ToString().Split(' ');

        if (CurrentObjective.checkColor)
            if (string.Compare(CurrentObjective.color, palavrasDescItem[0], true) != 0)
                return;
        
        if (CurrentObjective.checkObject)
            if (string.Compare(CurrentObjective.objectName, palavrasDescItem[1], true) != 0)
                return;
        
        if (CurrentObjective.checkCount)
            if (CurrentObjective.count != ListARObject.Count)
                return;

        ChangeObjective();
    }

    CanvasVuforiaListBehaviour ListBehaviour
    {
        get { return gameObject.GetComponent<CanvasVuforiaListBehaviour>(); }
    }

    void Start()
    {
        InternalInitialize();

        foreach (var button in Buttons)
            button.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);

        var objectiveObj = GameObject.FindGameObjectWithTag(TAG_OBJECTIVE);
        if (objectiveObj != null)
            objectiveText = objectiveObj.GetComponent<Text>();

        ChangeObjective();

        ListARObject.ItemsAdded += OnListARChanged;
        ListARObject.ItemsRemoved += OnListARChanged;

        if (ListBehaviour != null)
            ListBehaviour.RefreshList(ListARObject);
    }

    private void OnListARChanged(ListAR sender, ListAREventArgs args)
    {
        if (ListBehaviour != null)
            ListBehaviour.RefreshList(sender);
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

            Debug.Log(action);

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
