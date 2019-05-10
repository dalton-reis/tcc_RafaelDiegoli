using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TestSceneType
{
    TraditionalVuforia,
    VuforiaPlus
}

public static class TestConfigurations
{
    public static TestSceneType SceneType { get; set; }
    public static HandOrientation Hand { get; set; }

    public static bool IsVuforiaPlus { get { return SceneType == TestSceneType.VuforiaPlus; } }
    public static bool IsLeftHanded { get { return Hand == HandOrientation.LeftHanded; } }

    public static void SetLeftHandedAnchor(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0, rect.anchorMin.y);
        rect.anchorMax = new Vector2(0, rect.anchorMax.y);
        rect.anchoredPosition = new Vector2(Math.Abs(rect.anchoredPosition.x), rect.anchoredPosition.y);
    }
}

public class TestUserConfigManager : MonoBehaviour
{
    public const string MAIN_MENU_SCENE = "TestUserMainMenu";
    public const string TEST_1_SCENE = "TestUserActivity1";
    public const string TEST_2_SCENE = "TestUserActivity2";
    public const string TEST_END_SCENE = "TestUserEnd";

    public OptionVuforiaPlusCheckBoxBehaviour TraditionalSceneCheckBox;
    public OptionVuforiaPlusCheckBoxBehaviour PlusSceneCheckBox;
    public OptionVuforiaPlusCheckBoxBehaviour HandOrientationCheckBox;

    public OptionVuforiaPlusBehaviour StartTestButton;

    void RefreshSceneCheckBoxes()
    {
        switch (TestConfigurations.SceneType)
        {
            case TestSceneType.VuforiaPlus:
                TraditionalSceneCheckBox.IsChecked = false;
                PlusSceneCheckBox.IsChecked = true;
                break;

            default:
                TraditionalSceneCheckBox.IsChecked = true;
                PlusSceneCheckBox.IsChecked = false;
                break;
        }
    }

    void Start()
    {
    }

    private void OnHandOrientationChanged(OptionVuforiaPlusCheckBoxBehaviour sender, CheckBoxCheckChangedEventArgs args)
    {
        TestConfigurations.Hand = args.IsChecked ? HandOrientation.RightHanded : HandOrientation.LeftHanded;
    }

    private void OnSceneTypeChanged(OptionVuforiaPlusCheckBoxBehaviour sender, CheckBoxCheckChangedEventArgs args)
    {
        if (sender == TraditionalSceneCheckBox)
            TestConfigurations.SceneType = args.IsChecked ? TestSceneType.TraditionalVuforia : TestSceneType.VuforiaPlus;
        else
            TestConfigurations.SceneType = args.IsChecked ? TestSceneType.VuforiaPlus : TestSceneType.TraditionalVuforia;

        RefreshSceneCheckBoxes();
    }

    bool firstUpdate = true;

    void Update()
    {
        if (firstUpdate)
        {
            InternalStart();
            firstUpdate = false;
        }
	}

    private void InternalStart()
    {
        RefreshSceneCheckBoxes();

        HandOrientationCheckBox.IsChecked = TestConfigurations.Hand == HandOrientation.RightHanded;

        TraditionalSceneCheckBox.CheckChanged += OnSceneTypeChanged;
        PlusSceneCheckBox.CheckChanged += OnSceneTypeChanged;
        HandOrientationCheckBox.CheckChanged += OnHandOrientationChanged;

        StartTestButton.ExecuteAction += OnStartTest;
    }

    private void OnStartTest(OptionVuforiaPlusBehaviour sender, OptionVuforiaPlusActionEventArgs args)
    {
        SceneManager.LoadScene(TEST_1_SCENE);
    }
}
