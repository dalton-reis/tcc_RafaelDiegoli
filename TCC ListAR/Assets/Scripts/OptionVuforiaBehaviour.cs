using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionVuforiaBehaviour : MonoBehaviour
{
    public ListAR ListARObject;
    public Text DescriptionText;

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

    public void OnChangeIterableType()
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

    void SetCollectionToIterate()
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

    public void OnNextItem()
    {
        ListARObject.CurrentIterableCollection.NextItem();
    }

    public void OnPreviousItem()
    {
        ListARObject.CurrentIterableCollection.PreviousItem();
    }

    public void OnBackToMenu()
    {
        SceneManager.LoadScene(AppTeste.CENA_MENU_PRINCIPAL);
    }

    public void OnAddItems()
    {
        ListARObject.AddItem(ItemFactory.GetListItems(ListARObject.ItemDisplayObj.transform.localScale));
    }

    public void OnDeleteItem()
    {
        ListARObject.RemoveItemAt(ListARObject.CurrentIndex);

        //if (ListARObject.IterationType == IterableType.ListARItemMaterials)
        //    CollectionToIterate = ListARObject.CurrentItem;
    }

    public void OnSelectItem()
    {
        AppTeste.CheckSelectedItem();
    }
}
