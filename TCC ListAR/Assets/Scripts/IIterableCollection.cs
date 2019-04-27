public enum IterableType
{
    ListARObjects,
    ListARItemMaterials,
}

public interface IIterableCollection
{
    void NextItem();
    void PreviousItem();
    int GetCount();
    int GetCurrentIndex();
}
