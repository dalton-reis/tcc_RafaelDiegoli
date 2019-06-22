using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ItemFactory
{
    const string DEFAULT_SHADER = "Standard";

    static Shader shader;

    public static Shader Shader
    {
        get
        {
            if (shader == null)
                shader = Shader.Find(DEFAULT_SHADER);

            return shader;
        }

        set { shader = value; }
    }

    static Color GetMaterialColor(int index)
    {
        switch (index)
        {
            case 0:
                return Color.blue;
            case 1:
                return Color.black;
            case 2:
                return Color.red;
            default:
                return Color.white;
        }
    }

    public static IEnumerable<Material> GetMaterials()
    {
        List<Material> result = new List<Material>(3);

        for (int i = 0; i < result.Capacity; i++)
        {
            var mat = new Material(Shader);
            mat.color = GetMaterialColor(i);
            result.Add(mat);
        }

        return result;
    }

    static readonly PrimitiveType[] OBJS_PRIMITVOS = new PrimitiveType[]
    {
        PrimitiveType.Sphere,
        PrimitiveType.Cube,
        PrimitiveType.Cylinder,
    };

    public static IEnumerable<ListARItem> GetListItems()
    {
        return GetListItems(new Vector3(0.3f, 0.3f, 0.3f));
    }

    public static IEnumerable<ListARItem> GetListItems(Vector3 scale)
    {
        var items = new ListARItem[OBJS_PRIMITVOS.Length];

        for (int i = 0; i < OBJS_PRIMITVOS.Length; i++)
        {
            var primitive = GameObject.CreatePrimitive(OBJS_PRIMITVOS[i]);
            primitive.transform.localScale = scale;
            items[i] = new ListARItem(primitive, GetMaterials().Skip(i));
        }

        return items;
    }
}
