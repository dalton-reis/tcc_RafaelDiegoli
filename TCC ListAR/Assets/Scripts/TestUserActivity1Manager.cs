using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestUserActivity1Manager : MonoBehaviour
{
    readonly AppTeste.Objective[] OBJECTIVES = new AppTeste.Objective[]
    {
        new AppTeste.Objective("Selecione a opção que melhor representa o processo de precipitação", null, null, 0, 4),
        new AppTeste.Objective("Selecione a opção que melhor representa o processo de transpiração", null, null, 0, 2),
        new AppTeste.Objective("Selecione a opção que melhor representa o processo de condensação", null, null, 0, 3),
        new AppTeste.Objective("Selecione a opção que melhor representa o processo de evaporação", null, null, 0, 1),
    };

    int currentObjectiveIndex = -1;

    AppTeste.Objective CurrentObjective
    {
        get
        {
            if (currentObjectiveIndex > -1 && currentObjectiveIndex < OBJECTIVES.Length)
                return OBJECTIVES[currentObjectiveIndex];

            return OBJECTIVES.Last();
        }
    }

    public Text ObjectiveText;
    public OptionVuforiaPlusBehaviour VuforiaConfirmButton;
    public OptionVuforiaPlusScrollBehaviour VuforiaOptionsScroll;

    void ChangeObjective()
    {
        if (ObjectiveText == null)
            return;

        currentObjectiveIndex++;

        ObjectiveText.text = CurrentObjective.descripton;

        if (currentObjectiveIndex >= OBJECTIVES.Length)
            SceneManager.LoadSceneAsync(TestUserConfigManager.TEST_2_SCENE);
    }

    public void CheckSelectedOption(int selectedOption)
    {
        if (CurrentObjective.checkOptionNumber)
        {
            if (CurrentObjective.optionNumber == selectedOption)
                ChangeObjective();
        }
    }

    void ApplySceneType()
    {
        VuforiaConfirmButton.ExecuteAction += OnVuforiaConfirm;

        if (TestConfigurations.IsVuforiaPlus)
        {
            //TODO: desabilitar opções do canvas
        }
        else
        {
            //VuforiaConfirmButton.transform.parent.parent.gameObject.SetActive(false);
        }
        
        ChangeObjective();
    }

    private void OnVuforiaConfirm(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        CheckSelectedOption(Convert.ToInt32(VuforiaOptionsScroll.Value + 1));
    }

    void Start()
    {
	}

    bool firstUpdate = true; 

	void Update()
    {
        if (firstUpdate)
        {
            ApplySceneType();
            firstUpdate = false;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == TestUserConfigManager.TEST_1_SCENE)
        {
            //For some reason the Virtual Scroll/Slider and ComboBox only work on the the scene where Vuforia is initialized
            Vuforia.VuforiaRuntime.Instance.Deinit();
            Vuforia.VuforiaRuntime.Instance.InitVuforia();
        }
    }
}
