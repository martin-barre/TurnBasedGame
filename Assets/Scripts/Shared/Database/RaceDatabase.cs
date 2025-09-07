using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class RaceDatabase
{
    private static List<Race> _items;

    public static Race GetById(int id)
    {
        _items ??= LoadAllItems();
        return _items.FirstOrDefault(item => item.Id == id);
    }

    private static List<Race> LoadAllItems() => AssetDatabase.FindAssets($"t:{nameof(Race)}")
        .Select(AssetDatabase.GUIDToAssetPath)
        .Select(AssetDatabase.LoadAssetAtPath<Race>)
        .Where(item => item != null)
        .ToList();
}