using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HandOrientation
{
    RightHanded,
    LeftHanded,
}

public class CanvasVuforiaListBehaviour : MonoBehaviour
{
    public GameObject ListContent;
    public GameObject ItemDescriptionPrefab;
    public Color UnselectedColor;
    public Color SelectedColor;
    public GameObject CubeSwitchRight;
    public GameObject CubeSwitchLeft;
    public HandOrientation Hand;

    List<GameObject> itemDescriptionList;
    ListAR listARObject;

    ScrollRect scrollObj;
    ScrollRect ScrollObj
    {
        get
        {
            if (scrollObj == null)
                scrollObj = gameObject.GetComponentInChildren<ScrollRect>();

            return scrollObj;
        }
    }

    void Start()
    {
        switch (Hand)
        {
            case HandOrientation.RightHanded:
                CubeSwitchRight.SetActive(true);
                CubeSwitchLeft.SetActive(false);
                break;

            case HandOrientation.LeftHanded:
                CubeSwitchLeft.SetActive(true);
                CubeSwitchRight.SetActive(false);
                break;
        }
    }
	
	void Update()
    {
	}

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

        var selectedRectTransf = itemDescriptionList[listAR.CurrentIndex].GetComponent<RectTransform>();

        if (ScrollObj != null)
        {
            float scrollValue = 1 + selectedRectTransf.anchoredPosition.y / ScrollObj.content.rect.height;
            ScrollObj.verticalScrollbar.value = scrollValue;
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
