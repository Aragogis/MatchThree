#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;


public class LevelEditor : EditorWindow
{
    private int gridCols = 10;
    private int gridRows = 10;
    private int cellSize = 30; // Size of each cell in pixels
    private ObjType[,] grid; // 2D array to store grid data (e.g., 0 for empty, 1 for block type, etc.)
    private string[] objectTypes; // Example types
    private int selectedType = 0; // Default selected type (Gem1)
    private int tempGridWidth;
    private int tempGridHeight;
    private Dictionary<ObjType, Sprite> objSprites;
    private Dictionary<ObjType, int> questData = new Dictionary<ObjType, int>();
    private ObjType selectedObjType = ObjType.Red; // Adjust this to your enum's default value
    private int questCount = 0;
    private int turns;
    private int levelCount;

    private void LoadGemSprites()
    {
        objSprites = new Dictionary<ObjType, Sprite>();
        for (int type = 0; type <= Enum.GetValues(typeof(ObjType)).Length; type++)
        {
            if ((ObjType)type == ObjType.None) continue;
            string currentName = Enum.GetName(typeof(ObjType), (ObjType)type);
            objSprites.Add((ObjType)type, AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/" + currentName + ".png"));
        }
    }

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Editor");
    }

    private void OnEnable()
    {
        // Initialize the grid
        grid = new ObjType[gridCols, gridRows];
        objectTypes = Enum.GetNames(typeof(ObjType));
        LoadGemSprites();
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Editor", EditorStyles.boldLabel);

        // Grid size settings
        GUILayout.Label("Grid Settings", EditorStyles.label);
        tempGridWidth = EditorGUILayout.IntField("Grid Width", tempGridWidth);
        tempGridHeight = EditorGUILayout.IntField("Grid Height", tempGridHeight);

        if (GUILayout.Button("Create Grid"))
        {
            CreateGrid();
        }

        GUILayout.Label("Select level number", EditorStyles.label);
        levelCount = EditorGUILayout.IntField(levelCount);

        GUILayout.Label("Select number of turns", EditorStyles.label);
        turns = EditorGUILayout.IntSlider(turns, 0, 30);

        // Select gem type
        GUILayout.Label("Select Gem Type", EditorStyles.label);
        selectedType = GUILayout.Toolbar(selectedType, objectTypes);

        // Draw the grid
        GUILayout.Label("Grid Preview", EditorStyles.label);
        DrawGrid();

        // Quest creation
        GUILayout.Label("Quest Settings", EditorStyles.boldLabel);
        selectedObjType = (ObjType)EditorGUILayout.EnumPopup("Quest Type", selectedObjType);
        questCount = EditorGUILayout.IntField("Quest Count", questCount);

        if (GUILayout.Button("Add Quest"))
        {
            questData.Add(selectedObjType, questCount);
        }

        // Display current quests
        GUILayout.Label("Current Quests", EditorStyles.label);
        foreach (var quest in questData)
        {
            GUILayout.Label($"{quest.Key}: {quest.Value}");
        }
        // Save button
        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }
    }

    private void CreateGrid()
    {
        gridCols = tempGridWidth;
        gridRows = tempGridHeight;
        grid = new ObjType[gridCols, gridRows]; // Reset grid to new dimensions
    }

    private void DrawGrid()
    {
        if (grid == null) return;

        // Calculate offsets to center the grid
        float offsetX = (position.width - (gridCols * cellSize)) / 2;
        float offsetY = 600;

        for (int y = 0; y < gridRows; y++)
        {
            for (int x = 0; x < gridCols; x++)
            {
                // Calculate cell position
                Rect cellRect = new Rect(offsetX + x * cellSize, offsetY + y * cellSize, cellSize, cellSize);

                // Draw the cell's background
                EditorGUI.DrawRect(cellRect, Color.gray);

                // Draw the sprite for the current cell
                if (objSprites.TryGetValue(grid[x, gridRows - y - 1], out Sprite sprite) && sprite != null)
                {
                    Texture2D texture = sprite.texture;
                    Rect spriteRect = new Rect(
                        sprite.rect.x / texture.width,
                        sprite.rect.y / texture.height,
                        sprite.rect.width / texture.width,
                        sprite.rect.height / texture.height
                    );

                    GUI.DrawTextureWithTexCoords(cellRect, texture, spriteRect);
                }

                // Detect click to modify the cell
                if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                {
                    grid[x, gridRows-y-1] = (ObjType)(selectedType);
                    Repaint(); // Redraw the window
                }
            }
        }
    }

    private void SaveLevel()
    {
        LevelData level = ScriptableObject.CreateInstance<LevelData>();
        level.columns = gridCols;
        level.rows = gridRows;
        level.turns = turns;
        level.gems = new List<int>();
        for (int i = 0; i < level.columns; i++)
        {
            for (int j = 0; j < level.rows; j++)
            {
                level.gems.Add((int)grid[i, j]);
            }
        }

        level.quests = new List<QuestEntry>();
        foreach (var quest in questData)
        {
            level.quests.Add(new QuestEntry { type = quest.Key, count = quest.Value });
        }

        AssetDatabase.CreateAsset(level, "Assets/Resources/Levels/Level" + levelCount + ".asset");
        AssetDatabase.SaveAssets();
    }
}

#endif