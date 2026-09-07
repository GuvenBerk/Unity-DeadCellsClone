using System.IO;
using UnityEditor;
using UnityEngine;

public static class SceneSetup
{
    private const string SquarePath = "Assets/Square.png";
    private const int SquareSize = 256;

    [MenuItem("Tools/Rebuild Scene Setup")]
    public static void Rebuild()
    {
        EnsureTag("Player");
        EnsureTag("Ground");

        Sprite squareSprite = EnsureSquareSprite();

        BuildPlayer(squareSprite);
        BuildGround(squareSprite);
        BuildEnemy(squareSprite);

        CheckActiveInputHandling();

        Debug.Log("Rebuild Scene Setup tamamlandi: Player, Ground, Enemy olusturuldu.");
    }

    private static Sprite EnsureSquareSprite()
    {
        if (!File.Exists(SquarePath))
        {
            Texture2D texture = new Texture2D(SquareSize, SquareSize, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[SquareSize * SquareSize];
            Color32 white = new Color32(255, 255, 255, 255);
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = white;
            }
            texture.SetPixels32(pixels);
            texture.Apply();

            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(SquarePath, pngData);
            Object.DestroyImmediate(texture);

            AssetDatabase.ImportAsset(SquarePath, ImportAssetOptions.ForceSynchronousImport);
        }

        TextureImporter importer = AssetImporter.GetAtPath(SquarePath) as TextureImporter;
        if (importer != null)
        {
            bool changed = false;

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }
            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }
            if (importer.spritePixelsPerUnit != SquareSize)
            {
                importer.spritePixelsPerUnit = SquareSize;
                changed = true;
            }
            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }
            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }
        }

        AssetDatabase.Refresh();
        return AssetDatabase.LoadAssetAtPath<Sprite>(SquarePath);
    }

    private static GameObject ResetGameObject(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }
        return new GameObject(name);
    }

    private static void BuildPlayer(Sprite sprite)
    {
        GameObject player = ResetGameObject("Player");
        player.transform.position = new Vector3(0f, 0f, 0f);

        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = Color.white;

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        BoxCollider2D col = player.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1f);

        player.AddComponent<PlayerMovement>();
        player.AddComponent<PlayerHealth>();

        player.tag = "Player";
    }

    private static void BuildGround(Sprite sprite)
    {
        GameObject ground = ResetGameObject("Ground");
        ground.transform.position = new Vector3(0f, -3f, 0f);
        ground.transform.localScale = new Vector3(10f, 1f, 1f);

        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = Color.white;

        BoxCollider2D col = ground.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1f);

        ground.tag = "Ground";
    }

    private static void BuildEnemy(Sprite sprite)
    {
        GameObject enemy = ResetGameObject("Enemy");
        enemy.transform.position = new Vector3(2f, -2.5f, 0f);

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = Color.red;

        BoxCollider2D col = enemy.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1f);

        enemy.AddComponent<EnemyHealth>();
        enemy.AddComponent<EnemyAttack>();
    }

    private static void EnsureTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
            {
                return;
            }
        }

        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    private static void CheckActiveInputHandling()
    {
        const string settingsPath = "ProjectSettings/ProjectSettings.asset";
        if (!File.Exists(settingsPath))
        {
            return;
        }

        string content = File.ReadAllText(settingsPath);
        int idx = content.IndexOf("activeInputHandler:");
        if (idx < 0)
        {
            return;
        }

        string tail = content.Substring(idx + "activeInputHandler:".Length).TrimStart();
        char valueChar = tail.Length > 0 ? tail[0] : '0';

        if (valueChar != '0')
        {
            string message =
                "Active Input Handling ayari 'Input Manager (Old)' degil.\n\n" +
                "Bu ayar script ile otomatik degistirilmedi cunku degisiklik Unity'nin yeniden baslatilmasini gerektirebilir.\n\n" +
                "Lutfen Edit > Project Settings > Player > Other Settings > Active Input Handling ayarini " +
                "'Input Manager (Old)' olarak degistirip Unity'yi yeniden baslatin.";
            Debug.LogWarning(message);
            EditorUtility.DisplayDialog("Active Input Handling", message, "Tamam");
        }
    }
}
