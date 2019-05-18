using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppTeste : MonoBehaviour
{
    const string CENA_TRADICIONAL = "Tradicional";
    const string CENA_VUFORIA = "Vuforia AR";
    const string CENA_VUFORIA_PLUS = CENA_VUFORIA + " Plus";
    const string CENA_MENU_PRINCIPAL = "Menu Principal";

    const string TAG_DISPLAY = "Display";
    const string TAG_OBJECTIVE = "ObjectiveText";

    public struct Objective
    {
        public string descripton { get; set; }
        public string color { get; set; }
        public string objectName { get; set; }
        public int count { get; set; }
        public int optionNumber { get; set; }

        public bool checkOptionNumber
        {
            get { return optionNumber > -1; }
        }

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

        public Objective(string desc)
        {
            descripton = desc;
            color = null;
            objectName = null;
            count = 0;
            optionNumber = 0;
        }

        public Objective(string desc, string colorName, string objName, int itemCount)
        {
            descripton = desc;
            color = colorName;
            objectName = objName;
            count = itemCount;
            optionNumber = -1;
        }

        public Objective(string desc, string colorName, string objName, int itemCount, int correctOption)
        {
            descripton = desc;
            color = colorName;
            objectName = objName;
            count = itemCount;
            optionNumber = correctOption;
        }
    }

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

    static void CheckSelectedItem(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        string[] palavrasDescItem = listAR.CurrentItem.ToString().Split(' ');

        if (CurrentObjective.checkColor)
            if (string.Compare(CurrentObjective.color, palavrasDescItem[0], true) != 0)
                return;

        if (CurrentObjective.checkObject)
            if (string.Compare(CurrentObjective.objectName, palavrasDescItem[1], true) != 0)
                return;

        if (CurrentObjective.checkCount)
            if (CurrentObjective.count != listAR.Count)
                return;

        ChangeObjective();
    }

    GameObject displayGameObj;
    static ListAR listAR;
    static OptionVuforiaPlusBehaviour VuforiaSelectButton;

    float GetAjusteEscala(string cena)
    {
        if (cena == CENA_TRADICIONAL)
            return 2;

        if (cena == CENA_VUFORIA)
            return 1;

        if (cena == CENA_VUFORIA_PLUS)
            return 5;

        return 0;
    }

    void Start()
    {
        float ajusteEscala = GetAjusteEscala(SceneManager.GetActiveScene().name);

        displayGameObj = GameObject.FindGameObjectWithTag(TAG_DISPLAY);
        Vector3 scale = displayGameObj.transform.localScale + new Vector3(ajusteEscala, ajusteEscala, ajusteEscala);
        displayGameObj.transform.localScale = Vector3.zero;

        listAR = displayGameObj.GetComponent<ListAR>();

        var confirmObj = GameObject.FindGameObjectWithTag("Player");
        if (confirmObj != null)
        {
            VuforiaSelectButton = confirmObj.GetComponentInChildren<OptionVuforiaPlusBehaviour>();
            VuforiaSelectButton.ExecuteAction += CheckSelectedItem;
        }

        listAR.AddItem(ItemFactory.GetListItems(scale));
        listAR.ShowItem();

        var objectiveObj = GameObject.FindGameObjectWithTag(TAG_OBJECTIVE);
        if (objectiveObj != null)
            objectiveText = objectiveObj.GetComponent<Text>();

        currentObjectiveIndex = -1;
        ChangeObjective();
    }
	
	void Update()
    {
	}
}
