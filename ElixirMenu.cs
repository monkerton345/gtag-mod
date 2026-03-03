using System.Collections.Generic;
using BepInEx;
using UnityEngine;

namespace ElixirMenuMod
{
    [BepInPlugin("com.elixir.menu", "Elixir Menu", "1.1.0")]
    public class ElixirMenu : BaseUnityPlugin
    {
        private const float MenuWidth = 470f;
        private const float MenuHeight = 390f;
        private const float CornerRadius = 16f;

        private readonly string[] _categories =
        {
            "Movement",
            "Player",
            "Visual",
            "Fun"
        };

        private readonly List<ModToggle> _mods = new List<ModToggle>
        {
            new ModToggle("Speed Boost", "Movement"),
            new ModToggle("Long Arms", "Movement"),
            new ModToggle("No Clip", "Movement"),
            new ModToggle("Platforms", "Movement"),
            new ModToggle("Wall Walk", "Movement"),
            new ModToggle("Tag Aura", "Player"),
            new ModToggle("Auto Tag", "Player"),
            new ModToggle("Anti Tag", "Player"),
            new ModToggle("Ghost Monke", "Player"),
            new ModToggle("Invis Monke", "Visual"),
            new ModToggle("ESP", "Visual"),
            new ModToggle("Box Chams", "Visual"),
            new ModToggle("Tracers", "Visual"),
            new ModToggle("Rainbow Hands", "Fun"),
            new ModToggle("RGB Body", "Fun"),
            new ModToggle("Head Spin", "Fun"),
            new ModToggle("Low Gravity", "Fun")
        };

        private Rect _menuRect;
        private bool _showMenu = true;
        private int _activeCategoryIndex;

        private Texture2D _bgTexture;
        private Texture2D _topTexture;
        private Texture2D _sideTexture;
        private Texture2D _onTexture;
        private Texture2D _offTexture;

        private void Awake()
        {
            _menuRect = new Rect(60f, 90f, MenuWidth, MenuHeight);

            _bgTexture = CreateRoundedTexture(18, 18, CornerRadius, new Color(0.07f, 0.07f, 0.11f, 0.97f));
            _topTexture = CreateRoundedTexture(18, 18, CornerRadius, new Color(0.51f, 0.16f, 0.8f, 0.98f));
            _sideTexture = CreateRoundedTexture(18, 18, CornerRadius, new Color(0.12f, 0.12f, 0.18f, 0.96f));
            _onTexture = SolidTexture(new Color(0.25f, 0.73f, 0.45f, 0.95f));
            _offTexture = SolidTexture(new Color(0.25f, 0.25f, 0.32f, 0.95f));

            Logger.LogInfo("Elixir 1.1.0 loaded.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                _showMenu = !_showMenu;
            }
        }

        private void OnGUI()
        {
            if (!_showMenu)
            {
                return;
            }

            DrawMainPanel();
            DrawTopBar();
            DrawSidebar();
            DrawModList();

            _menuRect = GUI.Window(777, _menuRect, _ => { }, GUIContent.none, WindowStyle());
            DrawHint();
        }

        private void DrawMainPanel()
        {
            GUI.DrawTexture(_menuRect, _bgTexture, ScaleMode.StretchToFill, true);
        }

        private void DrawTopBar()
        {
            float topHeight = 52f;
            Rect topBar = new Rect(_menuRect.x, _menuRect.y, _menuRect.width, topHeight);
            GUI.DrawTexture(topBar, _topTexture, ScaleMode.StretchToFill, true);

            GUI.Label(
                new Rect(topBar.x + 16f, topBar.y + 12f, 250f, 28f),
                "ELIXIR",
                TitleStyle());

            GUI.Label(
                new Rect(topBar.x + topBar.width - 170f, topBar.y + 16f, 160f, 20f),
                "forked iidk style",
                SubtitleStyle());
        }

        private void DrawSidebar()
        {
            Rect sideRect = new Rect(_menuRect.x + 10f, _menuRect.y + 62f, 145f, _menuRect.height - 72f);
            GUI.DrawTexture(sideRect, _sideTexture, ScaleMode.StretchToFill, true);

            float y = sideRect.y + 12f;
            for (int i = 0; i < _categories.Length; i++)
            {
                Rect catRect = new Rect(sideRect.x + 10f, y, sideRect.width - 20f, 34f);
                bool active = i == _activeCategoryIndex;
                if (GUI.Button(catRect, _categories[i], CategoryButtonStyle(active)))
                {
                    _activeCategoryIndex = i;
                }

                y += 40f;
            }
        }

        private void DrawModList()
        {
            Rect contentRect = new Rect(_menuRect.x + 165f, _menuRect.y + 66f, _menuRect.width - 175f, _menuRect.height - 76f);
            GUI.Label(
                new Rect(contentRect.x + 4f, contentRect.y + 4f, contentRect.width - 8f, 22f),
                _categories[_activeCategoryIndex] + " Mods",
                SectionTitleStyle());

            float y = contentRect.y + 32f;
            string selected = _categories[_activeCategoryIndex];

            for (int i = 0; i < _mods.Count; i++)
            {
                if (_mods[i].Category != selected)
                {
                    continue;
                }

                Rect modRect = new Rect(contentRect.x, y, contentRect.width, 32f);
                GUI.DrawTexture(modRect, _mods[i].Enabled ? _onTexture : _offTexture, ScaleMode.StretchToFill, true);

                string label = _mods[i].Enabled
                    ? _mods[i].Name + "  [ON]"
                    : _mods[i].Name + "  [OFF]";

                if (GUI.Button(modRect, label, ModButtonStyle()))
                {
                    _mods[i].Enabled = !_mods[i].Enabled;
                }

                y += 37f;
            }
        }

        private void DrawHint()
        {
            GUI.Label(
                new Rect(_menuRect.x + 14f, _menuRect.y + _menuRect.height - 24f, 240f, 18f),
                "Insert = toggle menu",
                HintStyle());
        }

        private static GUIStyle WindowStyle()
        {
            return new GUIStyle(GUI.skin.window)
            {
                normal = { background = Texture2D.blackTexture }
            };
        }

        private static GUIStyle TitleStyle()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = 24;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            return style;
        }

        private static GUIStyle SubtitleStyle()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = 12;
            style.alignment = TextAnchor.MiddleRight;
            style.normal.textColor = new Color(0.95f, 0.90f, 1f, 0.95f);
            return style;
        }

        private static GUIStyle SectionTitleStyle()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = new Color(0.93f, 0.93f, 1f, 0.98f);
            return style;
        }

        private static GUIStyle CategoryButtonStyle(bool active)
        {
            var style = new GUIStyle(GUI.skin.button);
            style.fontSize = 13;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.background = SolidTexture(active
                ? new Color(0.44f, 0.19f, 0.69f, 1f)
                : new Color(0.2f, 0.2f, 0.28f, 1f));
            style.hover.background = SolidTexture(new Color(0.35f, 0.24f, 0.50f, 1f));
            style.active.background = SolidTexture(new Color(0.50f, 0.22f, 0.77f, 1f));
            return style;
        }

        private static GUIStyle ModButtonStyle()
        {
            var style = new GUIStyle(GUI.skin.button);
            style.fontSize = 13;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;
            style.padding = new RectOffset(10, 8, 5, 5);
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            style.normal.background = Texture2D.blackTexture;
            style.hover.background = Texture2D.blackTexture;
            style.active.background = Texture2D.blackTexture;
            return style;
        }

        private static GUIStyle HintStyle()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = 11;
            style.normal.textColor = new Color(0.75f, 0.75f, 0.85f, 0.95f);
            return style;
        }

        private static Texture2D SolidTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private static Texture2D CreateRoundedTexture(int width, int height, float radius, Color fillColor)
        {
            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            float maxX = width - 1f;
            float maxY = height - 1f;
            float squaredRadius = radius * radius;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Min(x, maxX - x);
                    float dy = Mathf.Min(y, maxY - y);

                    if (dx >= radius || dy >= radius)
                    {
                        texture.SetPixel(x, y, fillColor);
                        continue;
                    }

                    float cornerDx = radius - dx;
                    float cornerDy = radius - dy;
                    float distance = (cornerDx * cornerDx) + (cornerDy * cornerDy);
                    texture.SetPixel(x, y, distance <= squaredRadius ? fillColor : Color.clear);
                }
            }

            texture.Apply();
            return texture;
        }

        private class ModToggle
        {
            public string Name { get; }
            public string Category { get; }
            public bool Enabled { get; set; }

            public ModToggle(string name, string category)
            {
                Name = name;
                Category = category;
            }
        }
    }
}
