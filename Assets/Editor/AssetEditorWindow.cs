using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssetEditorWindow : EditorWindow
{
    private ListView _listView;
    private VisualElement _inspectorView;
    private List<Spell> _spells;
    private Spell _selectedSpell;
    private string _spellFolderPath = "Assets";

    [MenuItem("Tools/Spell Editor")]
    public static void ShowWindow() => GetWindow<AssetEditorWindow>("Spell Editor");

    public void CreateGUI()
    {
        LoadSettings();
        EditorApplication.projectChanged += RefreshList;
        
        rootVisualElement.Add(CreateToolbar());
        
        TwoPaneSplitView splitView = new(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        rootVisualElement.Add(splitView);
        
        splitView.Add(CreateSpellListView());
        
        _inspectorView = new VisualElement { style = { flexGrow = 1 } };
        splitView.Add(_inspectorView);
        
        RefreshList();
    }

    private Toolbar CreateToolbar()
    {
        Toolbar toolbar = new();
        toolbar.Add(new Button(() => Debug.Log("Spells clicked")) { text = "Spells" });
        toolbar.Add(new Button(() => Debug.Log("Races clicked")) { text = "Races" });
        toolbar.Add(new VisualElement { style = { flexGrow = 1 } });
        toolbar.Add(new Button(ShowSettingsMenu) { text = "Settings" });
        return toolbar;
    }

    private VisualElement CreateSpellListView()
    {
        VisualElement listContainer = new();
        _listView = new ListView
        {
            style = { flexGrow = 1 },
            fixedItemHeight = 20,
            selectionType = SelectionType.Single,
            makeItem = () => new Label(),
            bindItem = (element, index) => ((Label)element).text = $"{_spells[index].id}\t{_spells[index].name}"
        };
        _listView.selectionChanged += objects => SelectSpell(objects.FirstOrDefault() as Spell);

        listContainer.Add(_listView);
        listContainer.Add(new Button(CreateNewSpell) { text = "+ Nouveau Spell" });
        return listContainer;
    }

    private void ShowSettingsMenu()
    {
        GenericMenu menu = new();
        menu.AddItem(new GUIContent("Set spell folder's path"), false, () => SetPath(ref _spellFolderPath, nameof(_spellFolderPath)));
        menu.ShowAsContext();
    }

    private void SetPath(ref string path, string pathName)
    {
        string selectedPath = EditorUtility.OpenFolderPanel("Select Path", path, "");
        if (!string.IsNullOrEmpty(selectedPath) && selectedPath.StartsWith(Application.dataPath))
        {
            path = "Assets" + selectedPath.Substring(Application.dataPath.Length);
            EditorPrefs.SetString(pathName, path);
            Debug.Log($"Path updated: {path}");
        }
        else
        {
            EditorUtility.DisplayDialog("Invalid Path", "Please select a folder inside the Assets directory.", "OK");
        }
    }

    private List<Spell> findAllSpells() => AssetDatabase.FindAssets($"t:Spell")
        .Select(AssetDatabase.GUIDToAssetPath)
        .Select(AssetDatabase.LoadAssetAtPath<Spell>)
        .Where(o => o != null)
        .OrderBy(o => o.id)
        .ToList();
    
    private void RefreshList()
    {
        _spells = findAllSpells();
        _listView.itemsSource = _spells;
        _listView.RefreshItems();
        Debug.Log("List refreshed.");
    }

    private void SelectSpell(Spell spell)
    {
        RefreshList();
        _selectedSpell = spell;
        _inspectorView.Clear();
        if (spell == null) return;
        _inspectorView.Add(new InspectorElement(spell));
        _inspectorView.Add(new Button(DeleteSelectedSpell) { text = "Supprimer" });
    }

    private void CreateNewSpell()
    {
        Spell newSpell = CreateInstance<Spell>();
        string path = AssetDatabase.GenerateUniqueAssetPath($"{_spellFolderPath}/NewSpell.asset");
        AssetDatabase.CreateAsset(newSpell, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        RefreshList();
    }

    private void DeleteSelectedSpell()
    {
        if (_selectedSpell == null) return;
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_selectedSpell));
        _selectedSpell = null;
        RefreshList();
        _inspectorView.Clear();
    }
    
    private void LoadSettings() => _spellFolderPath = EditorPrefs.GetString(nameof(_spellFolderPath), "Assets");
}
