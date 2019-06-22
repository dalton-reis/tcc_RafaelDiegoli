using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionVuforiaBehaviour : MonoBehaviour
{
    public ListAR ListARObject;
    public Text DescriptionText;
    public string MainMenuName;

    void Start()
    {
        InternalInitialize();
    }

    protected void InternalInitialize()
    {
        SetCollectionToIterate();
    }

    void Update()
    {
    }

    protected virtual void SetCollectionToIterate()
    {
        if (ListARObject == null)
            return;

        switch (ListARObject.IterationType)
        {
            case IterableType.ListARItemMaterials:

                if (DescriptionText != null)
                    DescriptionText.text = "Materiais";
                break;

            default:

                if (DescriptionText != null)
                    DescriptionText.text = "Objetos";
                break;
        }
    }

    public virtual void OnChangeIterableType()
    {
        switch (ListARObject.IterationType)
        {
            case IterableType.ListARObjects:
                ListARObject.IterationType = IterableType.ListARItemMaterials;
                break;

            default:
                ListARObject.IterationType = IterableType.ListARObjects;
                break;
        }

        SetCollectionToIterate();
    }

    public virtual void OnNextItem()
    {
        ListARObject.CurrentIterableCollection.NextItem();
    }

    public virtual void OnPreviousItem()
    {
        ListARObject.CurrentIterableCollection.PreviousItem();
    }

    public virtual void OnBackToMenu()
    {
        SceneManager.LoadSceneAsync(MainMenuName);
    }

    public virtual void OnAddItems()
    {
        ListARObject.AddItem(ItemFactory.GetListItems(ListARObject.ItemDisplayObj.transform.localScale));
    }

    public virtual void OnDeleteItem()
    {
        ListARObject.RemoveItemAt(ListARObject.CurrentIndex);
    }

    public virtual void OnSelectItem()
    {
    }
}
