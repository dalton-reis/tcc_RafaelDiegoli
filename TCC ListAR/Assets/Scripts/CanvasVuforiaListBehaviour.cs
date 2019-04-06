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
            var itemDescription = createNewList ? Instantiate(ItemDescriptionPrefab, ListContent.transform) : itemDescriptionList[i];

            itemDescription.GetComponentInChildren<Text>().text = item.ToString();
            itemDescription.GetComponent<Image>().color = i == listAR.CurrentIndex ? SelectedColor : UnselectedColor;

            itemDescriptionList.Add(itemDescription);
        }
    }
}
