using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasVuforiaListBehaviour : MonoBehaviour
{
    public GameObject ListContent;
    public GameObject ItemDescriptionPrefab;

    void Start()
    {
	}
	
	void Update()
    {
	}

    List<GameObject> itemDescriptionList;
    ListAR listARObject;

    public Color UnselectedColor;
    public Color SelectedColor;

    public void RefreshList(ListAR listAR, bool createNewList)
    {
        if (ListContent == null)
            return;

        if (itemDescriptionList == null)
            itemDescriptionList = new List<GameObject>(listAR.Count);

        if (createNewList)
        {
            for (int i = 0; i < itemDescriptionList.Count;)
            {
                var obj = itemDescriptionList[i];
                itemDescriptionList.RemoveAt(i);
                Destroy(obj);
            }
        }

        for (int i = 0; i < listAR.Count; i++)
        {
            ListARItem item = listAR[i];
            item.CurrentMaterialChanged += RefreshItemDescription;

            var itemDescription = createNewList ? Instantiate(ItemDescriptionPrefab, ListContent.transform) : itemDescriptionList[i];

            itemDescription.GetComponentInChildren<Text>().text = item.ToString();
            itemDescription.GetComponent<Image>().color = i == listAR.CurrentIndex ? SelectedColor : UnselectedColor;

            itemDescriptionList.Add(itemDescription);
        }

        listARObject = listAR;
    }

    private void RefreshItemDescription(ListARItem sender, ListARItemMaterialChangedEventArgs args)
    {
        int itemIndex = listARObject.IndexOf(sender);
        if (itemIndex < 0 || itemIndex > listARObject.Count)
            return;

        itemDescriptionList[itemIndex].GetComponentInChildren<Text>().text = sender.ToString();
    }
}
