using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListARItemEventArgs
{
    public Material[] Materials { get; set; }

    public ListARItemEventArgs(Material[] materials)
    {
        Materials = materials;
    }
}

public class ListARItemMaterialChangedEventArgs
{
    public Material OldMaterial { get; set; }
    public Material NewMaterial { get; set; }

    public ListARItemMaterialChangedEventArgs(Material oldMaterial, Material newMaterial)
    {
        OldMaterial = oldMaterial;
        NewMaterial = newMaterial;
    }
}

public class ListARItem : IList<Material>, IIterableCollection
{
    public GameObject ObjPrefab;

    public List<Material> ObjMaterials;

    Vector3 originalLocalScale;

    int currentMaterialIndex = 0;

    bool visible = false;

    public delegate void ListARItemEventHandler(ListARItem sender, ListARItemEventArgs args);
    public event ListARItemEventHandler MaterialsAdded;
    public event ListARItemEventHandler MaterialsRemoved;

    public delegate void ListARItemMaterialChangedEventHandler(ListARItem sender, ListARItemMaterialChangedEventArgs args);
    public event ListARItemMaterialChangedEventHandler CurrentMaterialChanged;

    protected virtual void RaiseMaterialsAdded(ListARItemEventArgs args)
    {
        if (MaterialsAdded != null)
            MaterialsAdded(this, args);
    }

    protected virtual void RaiseMaterialsRemoved(ListARItemEventArgs args)
    {
        if (MaterialsRemoved != null)
            MaterialsRemoved(this, args);
    }

    protected virtual void RaiseCurrentMaterialChanged(ListARItemMaterialChangedEventArgs args)
    {
        if (CurrentMaterialChanged != null)
            CurrentMaterialChanged(this, args);
    }

    public int Count
    {
        get { return ObjMaterials == null ? 0 : ObjMaterials.Count; }
    }

    public Material CurrentMaterial
    {
        get { return ObjMaterials[currentMaterialIndex]; }
    }

    public Vector3 OriginalLocalScale
    {
        get { return originalLocalScale; }
    }

    public bool Visible
    {
        get { return visible; }
        set
        {
            if (ObjPrefab == null)
                return;

            if (value)
                ObjPrefab.transform.localScale = OriginalLocalScale;
            else
                ObjPrefab.transform.localScale = new Vector3(0, 0, 0);

            visible = value;
        }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public Material this[int index]
    {
        get { return ObjMaterials[index]; }
        set { ObjMaterials[index] = value; }
    }

    public ListARItem(GameObject prefab)
    {
        originalLocalScale = prefab.transform.localScale;
        ObjPrefab = prefab;
        Visible = false;

        var renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
            ObjMaterials = new List<Material>(renderer.materials);
    }

    public ListARItem(GameObject prefab, IEnumerable<Material> materials)
    {
        originalLocalScale = prefab.transform.localScale;
        ObjPrefab = prefab;
        Visible = false;

        var renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            ObjMaterials = new List<Material>(renderer.materials);

            AddMaterial(materials);
        }
        else
        {
            ObjMaterials = new List<Material>(materials);
        }
    }

    public void AddMaterial(Material material)
    {
        ObjMaterials.Add(material);

        RaiseMaterialsAdded(new ListARItemEventArgs(new Material[] { material }));
    }

    public void AddMaterial(IEnumerable<Material> materials)
    {
        ObjMaterials.AddRange(materials);

        RaiseMaterialsAdded(new ListARItemEventArgs(materials.ToArray()));
    }

    public bool RemoveMaterial(Material material)
    {
        return RemoveMaterialAt(IndexOf(material));
    }

    public bool RemoveMaterialAt(int materialIndex)
    {
        if (materialIndex < 0 || materialIndex >= ObjMaterials.Count)
            return false;

        Material removedMaterial = ObjMaterials[materialIndex];

        if (materialIndex == 0)
            NextMaterial();
        else if (currentMaterialIndex == materialIndex)
            PreviousMaterial();

        ObjMaterials.RemoveAt(materialIndex);

        RaiseMaterialsRemoved(new ListARItemEventArgs(new Material[] { removedMaterial }));

        return true;
    }

    public bool HasNextMaterial()
    {
        return currentMaterialIndex < ObjMaterials.Count - 1;
    }

    public void NextMaterial()
    {
        Material oldMaterial = CurrentMaterial;

        currentMaterialIndex = HasNextMaterial() ? currentMaterialIndex + 1 : 0;

        RaiseCurrentMaterialChanged(new ListARItemMaterialChangedEventArgs(oldMaterial, CurrentMaterial));

        SetMaterial();
    }

    public bool HasPreviousMaterial()
    {
        return currentMaterialIndex > 0;
    }

    public void PreviousMaterial()
    {
        Material oldMaterial = CurrentMaterial;

        currentMaterialIndex = HasPreviousMaterial() ? currentMaterialIndex - 1 : ObjMaterials.Count - 1;

        RaiseCurrentMaterialChanged(new ListARItemMaterialChangedEventArgs(oldMaterial, CurrentMaterial));

        SetMaterial();
    }

    public void SetMaterial()
    {
        ObjPrefab.GetComponent<Renderer>().material = CurrentMaterial;
    }

    public int IndexOf(Material item)
    {
        return ObjMaterials.IndexOf(item);
    }

    public void Insert(int index, Material item)
    {
        ObjMaterials.Insert(index, item);

        if (index == currentMaterialIndex && HasNextMaterial())
            currentMaterialIndex++;
    }

    public void RemoveAt(int index)
    {
        RemoveMaterialAt(index);
    }

    public void Add(Material item)
    {
        AddMaterial(item);
    }

    public void Clear()
    {
        ObjMaterials.Clear();
    }

    public bool Contains(Material item)
    {
        return ObjMaterials.Contains(item);
    }

    public void CopyTo(Material[] array, int arrayIndex)
    {
        ObjMaterials.CopyTo(array, arrayIndex);
    }

    public bool Remove(Material item)
    {
        return RemoveMaterial(item);
    }

    public IEnumerator<Material> GetEnumerator()
    {
        return ObjMaterials.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void NextItem()
    {
        NextMaterial();
    }

    public void PreviousItem()
    {
        PreviousMaterial();
    }

    public int GetCount()
    {
        return Count;
    }

    public int GetCurrentIndex()
    {
        return currentMaterialIndex;
    }

    public static string GetColorName(Color color)
    {
        if (Color.black == color)
            return "Black";

        if (Color.blue == color)
            return "Blue";

        if (Color.clear == color)
            return "Clear";

        if (Color.cyan == color)
            return "Cyan";

        if (Color.gray == color)
            return "Gray";

        if (Color.green == color)
            return "Green";

        if (Color.magenta == color)
            return "Magenta";

        if (Color.red == color)
            return "Red";

        if (Color.white == color)
            return "White";

        return color.ToString();
    }

    public virtual string GetMaterialDescription()
    {
        return GetColorName(CurrentMaterial.color);
    }

    public virtual string GetDescription()
    {
        return string.Format("{0} {1}", GetMaterialDescription(), ObjPrefab.name);
    }

    public override string ToString()
    {
        return GetDescription();
    }
}
