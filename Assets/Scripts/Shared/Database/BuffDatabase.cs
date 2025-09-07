using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class BuffDatabase
{
    private static List<Buff> _items;

    public static Buff GetById(int id)
    {
        _items ??= LoadAllItems();
        return _items.FirstOrDefault(item => item.Id == id);
    }

    private static List<Buff> LoadAllItems() => AssetDatabase.FindAssets($"t:{nameof(Buff)}")
        .Select(AssetDatabase.GUIDToAssetPath)
        .Select(AssetDatabase.LoadAssetAtPath<Buff>)
        .Where(item => item != null)
        .ToList();
}