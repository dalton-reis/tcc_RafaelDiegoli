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

    public void RefreshList(ListAR listAR)
    {
        if (ListContent == null)
            return;

        if (itemDescriptionList == null)
            itemDescriptionList = new List<GameObject>(listAR.Count);

        for (int i = 0; i < itemDescriptionList.Count; i++)
        {
            var obj = itemDescriptionList[i];
            itemDescriptionList.RemoveAt(i);
            Destroy(obj);
            i--;
        }

        foreach (ListARItem item in listAR)
        {
            var itemDescription = Instantiate(ItemDescriptionPrefab, ListContent.transform);
            itemDescription.GetComponentInChildren<Text>().text = item.ToString();
            itemDescriptionList.Add(itemDescription);
        }
    }
}
