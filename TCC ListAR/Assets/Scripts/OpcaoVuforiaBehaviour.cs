using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpcaoVuforiaBehaviour : MonoBehaviour
{
    protected const string TAG_DESCRIPION_TEXT = "DescriptionText";

    public enum IterableType
    {
        ListARObjects,
        ListARItemMaterials,
    }

    protected static ListAR ListARObject;
    protected static IterableType iterableType = IterableType.ListARObjects;
    protected static Text DescriptionText;

    static IIterableCollection collectionToIterate;
    protected static IIterableCollection CollectionToIterate
    {
        get
        {
            if (collectionToIterate == null)
                collectionToIterate = ListARObject;

            return collectionToIterate;
        }

        set { collectionToIterate = value; }
    }


    void Start()
    {
        InternalInitialize();
    }

    protected void InternalInitialize()
    {
        ListARObject = GameObject.FindGameObjectWithTag(AppTeste.TAG_DISPLAY).GetComponent<ListAR>();

        var descText = GameObject.FindGameObjectWithTag(TAG_DESCRIPION_TEXT);
        if (descText != null)
            DescriptionText = descText.GetComponent<Text>();

        SetCollectionToIterate();
    }

    void Update()
    {
    }

    public void OnChangeIterableType()
    {
        switch (iterableType)
        {
            case IterableType.ListARObjects:
                iterableType = IterableType.ListARItemMaterials;
                break;

            default:
                iterableType = IterableType.ListARObjects;
                break;
        }

        SetCollectionToIterate();
    }

    void SetCollectionToIterate()
    {
        switch (iterableType)
        {
            case IterableType.ListARItemMaterials:
                CollectionToIterate = ListARObject.CurrentItem;

                if (DescriptionText != null)
                    DescriptionText.text = "Materiais";
                break;

            default:
                CollectionToIterate = ListARObject;

                if (DescriptionText != null)
                    DescriptionText.text = "Objetos";
                break;
        }
    }

    public void OnNextItem()
    {
        CollectionToIterate.NextItem();
    }

    public void OnPreviousItem()
    {
        CollectionToIterate.PreviousItem();
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

        if (iterableType == IterableType.ListARItemMaterials)
            CollectionToIterate = ListARObject.CurrentItem;
    }

    public void OnSelectItem()
    {
        AppTeste.CheckSelectedItem();
    }
}
