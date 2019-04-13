using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListAREventArgs
{
    public ListARItem[] Items { get; set; }

    public ListAREventArgs(ListARItem[] items)
    {
        Items = items;
    }
}

public class ListARItemChangedEventArgs
{
    public ListARItem OldItem { get; set; }
    public ListARItem NewItem { get; set; }

    public ListARItemChangedEventArgs(ListARItem oldItem, ListARItem newItem)
    {
        OldItem = oldItem;
        NewItem = newItem;
    }
}

public class ListAR : MonoBehaviour, IList<ListARItem>, IIterableCollection
{
    GameObject itemDisplayObj;

    public GameObject ItemDisplayObj
    {
        get { return itemDisplayObj; }
    }

    public List<ListARItem> Items;

    public GameObject DisplayObj;

    int currentItemIndex = 0;

    public delegate void ListAREventHandler(ListAR sender, ListAREventArgs args);
    public event ListAREventHandler ItemsAdded;
    public event ListAREventHandler ItemsRemoved;

    public delegate void ListARItemChangedEventHandler(ListAR sender, ListARItemChangedEventArgs args);
    public event ListARItemChangedEventHandler CurrentItemChanged;

    protected virtual void RaiseItemsAdded(ListAREventArgs args)
    {
        if (ItemsAdded != null)
            ItemsAdded(this, args);
    }

    protected virtual void RaiseItemsRemoved(ListAREventArgs args)
    {
        if (ItemsRemoved != null)
            ItemsRemoved(this, args);
    }

    protected virtual void RaiseCurrentItemChanged(ListARItemChangedEventArgs args)
    {
        if (CurrentItemChanged != null)
            CurrentItemChanged(this, args);
    }

    public int CurrentIndex
    {
        get { return currentItemIndex; }
    }

    public int Count
    {
        get { return Items == null ? 0 : Items.Count; }
    }

    public ListARItem CurrentItem
    {
        get { return Items[currentItemIndex]; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public ListARItem this[int index]
    {
        get { return Items[index]; }
        set { Items[index] = value; }
    }

    public ListAR()
    {
        Items = new List<ListARItem>();
    }

    public ListAR(IEnumerable<ListARItem> items)
    {
        Items = new List<ListARItem>(items);
    }

    public void AddItem(ListARItem item)
    {
        Items.Add(item);

        RaiseItemsAdded(new ListAREventArgs(new ListARItem[] { item }));
    }

    public void AddItem(IEnumerable<ListARItem> items)
    {
        Items.AddRange(items);

        RaiseItemsAdded(new ListAREventArgs(items.ToArray()));
    }

    public bool RemoveItem(ListARItem item)
    {
        return RemoveItemAt(Items.IndexOf(item));
    }

    public bool RemoveItemAt(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= Count)
            return false;

        if (itemIndex == 0)
            NextItem();
        else if (currentItemIndex == itemIndex)
            PreviousItem();

        var item = Items[itemIndex];
        Items.RemoveAt(itemIndex);

        RaiseItemsRemoved(new ListAREventArgs(new ListARItem[] { item }));

        Destroy(item.ObjPrefab);

        return true;
    }

    public bool HasNextItem()
    {
        return currentItemIndex < Count - 1;
    }

    public void NextItem()
    {
        ListARItem oldItem = CurrentItem;

        CurrentItem.Visible = false;
        currentItemIndex = HasNextItem() ? currentItemIndex + 1 : 0;

        RaiseCurrentItemChanged(new ListARItemChangedEventArgs(oldItem, CurrentItem));

        ShowItem();
    }

    public bool HasPreviousItem()
    {
        return currentItemIndex > 0;
    }

    public void PreviousItem()
    {
        ListARItem oldItem = CurrentItem;

        CurrentItem.Visible = false;
        currentItemIndex = HasPreviousItem() ? currentItemIndex - 1 : Count - 1;

        RaiseCurrentItemChanged(new ListARItemChangedEventArgs(oldItem, CurrentItem));

        ShowItem();
    }

    public void ShowItem()
    {
        InternalShowHideItem(true);
    }

    public void HideItem()
    {
        InternalShowHideItem(false);
    }

    void InternalShowHideItem(bool show)
    {
        CurrentItem.Visible = show;
        itemDisplayObj = CurrentItem.ObjPrefab;
    }

    public void NextMaterialForItem()
    {
        CurrentItem.NextMaterial();
    }

    public void PreviousMaterialForItem()
    {
        CurrentItem.PreviousMaterial();
    }

    void Start()
    {
    }

    void Update()
    {
        CurrentItem.ObjPrefab.transform.SetPositionAndRotation(DisplayObj.transform.position, DisplayObj.transform.rotation);
    }

    public int IndexOf(ListARItem item)
    {
        return Items.IndexOf(item);
    }

    public void Insert(int index, ListARItem item)
    {
        Items.Insert(index, item);

        if (index == currentItemIndex && HasNextItem())
            currentItemIndex++;

        RaiseItemsAdded(new ListAREventArgs(new ListARItem[] { item }));
    }

    public void RemoveAt(int index)
    {
        RemoveItemAt(index);
    }

    public void Add(ListARItem item)
    {
        AddItem(item);
    }

    public void Clear()
    {
        foreach (ListARItem listItem in Items)
            Destroy(listItem.ObjPrefab);

        Items.Clear();
    }

    public bool Contains(ListARItem item)
    {
        return Items.Contains(item);
    }

    public void CopyTo(ListARItem[] array, int arrayIndex)
    {
        Items.CopyTo(array, arrayIndex);
    }

    public bool Remove(ListARItem item)
    {
        return RemoveItem(item);
    }

    public IEnumerator<ListARItem> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int GetCount()
    {
        return Count;
    }

    public int GetCurrentIndex()
    {
        return CurrentIndex;
    }
}