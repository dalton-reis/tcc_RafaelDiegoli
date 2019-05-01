using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestUserActivity2Manager : MonoBehaviour
{
    const string PATH_CORRECT_IMGS = "TestUserActivity2\\Correct";
    const string PATH_WRONG_IMGS = "TestUserActivity2\\Wrong";

    private Dictionary<string, Sprite> cacheCorrectImgs;
    public Dictionary<string, Sprite> CacheCorrectImgs
    {
        get
        {
            if (cacheCorrectImgs == null)
            {
                var sprites = Resources.LoadAll<Sprite>(PATH_CORRECT_IMGS);
                cacheCorrectImgs = new Dictionary<string, Sprite>(sprites.Length);

                foreach (var sprite in sprites)
                    cacheCorrectImgs.Add(sprite.name, sprite);
            }

            return cacheCorrectImgs;
        }
    }

    private Dictionary<string, Sprite> cacheWrongImgs;
    public Dictionary<string, Sprite> CacheWrongImgs
    {
        get
        {
            if (cacheWrongImgs == null)
            {
                var sprites = Resources.LoadAll<Sprite>(PATH_WRONG_IMGS);
                cacheWrongImgs = new Dictionary<string, Sprite>(sprites.Length);

                foreach (var sprite in sprites)
                    cacheWrongImgs.Add(sprite.name, sprite);
            }

            return cacheWrongImgs;
        }
    }

    readonly AppTeste.Objective[] OBJECTIVES = new AppTeste.Objective[]
    {
        new AppTeste.Objective("1 - Selecione a opção que representa um uso consciente da água"),
        new AppTeste.Objective("2 - Selecione a opção que representa um uso consciente da água"),
        new AppTeste.Objective("3 - Selecione a opção que representa um uso consciente da água"),
        new AppTeste.Objective("Você concluiu todos os objetivos, obrigado pela participação."),
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
    public ListAR ListARObject;
    public GameObject ListAROptionPrefab;
    public CanvasVuforiaListBehaviour VuforiaListObject;
    public OptionVuforiaPlusBehaviour VuforiaConfirmButton;
    
    ListARItem CreateOptionItem(Sprite sprite)
    {
        var obj = Instantiate(ListAROptionPrefab, ListARObject.DisplayObj.transform);
        obj.GetComponent<SpriteRenderer>().sprite = sprite;

        return new ListARItem(obj);
    }

    int correctImgIndex = -1;

    void ChangeObjective()
    {
        if (ObjectiveText == null)
            return;

        currentObjectiveIndex++;

        ObjectiveText.text = CurrentObjective.descripton;

        if (currentObjectiveIndex == OBJECTIVES.Length)
            SceneManager.LoadSceneAsync(TestUserConfigManager.MAIN_MENU_SCENE);

        if (currentObjectiveIndex < OBJECTIVES.Length - 1)
        {
            List<ListARItem> itemsToAdd = new List<ListARItem>(3);

            foreach (Sprite sprite in CacheWrongImgs.Values.Skip(currentObjectiveIndex * 2).Take(2))
                itemsToAdd.Add(CreateOptionItem(sprite));

            var correctOption = CreateOptionItem(CacheCorrectImgs[string.Format("correta ({0})", currentObjectiveIndex + 1)]);

            itemsToAdd.Insert(Random.Range(0, 3), correctOption);

            ListARObject.AddItem(itemsToAdd);

            correctImgIndex = ListARObject.IndexOf(correctOption);
        }
    }

    public void CheckSelectedOption(int selectedOption)
    {
        if (selectedOption == correctImgIndex || currentObjectiveIndex == OBJECTIVES.Length - 1)
        {
            ListARObject.RemoveAt(correctImgIndex);
            ChangeObjective();
        }
    }

    void ApplySceneType()
    {
        VuforiaConfirmButton.ExecuteAction += OnVuforiaConfirm;

        if (TestConfigurations.IsVuforiaPlus)
        {
            VuforiaListObject.HandRuntime = TestConfigurations.Hand;
        }
        else
        {
            //VuforiaListObject.gameObject.SetActive(false);
        }

        ChangeObjective();
        ListARObject.ShowItem();
    }

    private void OnVuforiaConfirm(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        CheckSelectedOption(ListARObject.CurrentIndex);
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
}
