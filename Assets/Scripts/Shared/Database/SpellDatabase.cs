using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class SpellDatabase
{
    private static List<Spell> _items;
    
    public static Spell GetById(int id)
    {
        _items ??= LoadAllItems();
        return _items.FirstOrDefault(item => item.id == id);
    }

    private static List<Spell> LoadAllItems() => AssetDatabase.FindAssets($"t:{nameof(Spell)}")
        .Select(AssetDatabase.GUIDToAssetPath)
        .Select(AssetDatabase.LoadAssetAtPath<Spell>)
        .Where(item => item != null)
        .ToList();
}