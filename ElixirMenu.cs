using BepInEx;
using UnityEngine;

namespace ElixirMenuMod
{
    [BepInPlugin("com.elixir.menu", "Elixir Menu", "1.0.0")]
    public class ElixirMenu : BaseUnityPlugin
    {
        private const float WindowWidth = 340f;
        private const float HeaderHeight = 42f;
        private const float ButtonHeight = 32f;
        private const float CornerRadius = 14f;

        private readonly string[] _buttons =
        {
            "Tag Aura",
            "Speed Boost",
            "Platforms",
            "No Clip",
            "Ghost Monke"
        };

        private bool[] _enabledStates;
        private Rect _windowRect;
        private Texture2D _windowTexture;
        private Texture2D _headerTexture;

        private void Awake()
        {
            _enabledStates = new bool[_buttons.Length];
            _windowRect = new Rect(40f, 120f, WindowWidth, HeaderHeight + (_buttons.Length * (ButtonHeight + 8f)) + 20f);
            _windowTexture = CreateRoundedTexture(8, 8, CornerRadius, new Color(0.08f, 0.08f, 0.11f, 0.95f));
            _headerTexture = CreateRoundedTexture(8, 8, CornerRadius, new Color(0.53f, 0.19f, 0.79f, 0.98f));
            Logger.LogInfo("Elixir menu loaded.");
        }

        private void OnGUI()
        {
            DrawRoundedPanel(_windowRect, _windowTexture);

            var headerRect = new Rect(_windowRect.x, _windowRect.y, _windowRect.width, HeaderHeight);
            DrawRoundedTopBar(headerRect, _headerTexture);
            GUI.Label(new Rect(headerRect.x + 14f, headerRect.y + 10f, headerRect.width - 28f, 24f), "ELIXIR", HeaderLabelStyle());

            float y = _windowRect.y + HeaderHeight + 10f;
            for (int i = 0; i < _buttons.Length; i++)
            {
                var buttonRect = new Rect(_windowRect.x + 12f, y, _windowRect.width - 24f, ButtonHeight);
                if (GUI.Button(buttonRect, BuildButtonLabel(i), ButtonStyle()))
                {
                    _enabledStates[i] = !_enabledStates[i];
                }

                y += ButtonHeight + 8f;
            }
        }

        private string BuildButtonLabel(int index)
        {
            return _enabledStates[index]
                ? $"{_buttons[index]}  [ON]"
                : $"{_buttons[index]}  [OFF]";
        }

        private static GUIStyle HeaderLabelStyle()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            return style;
        }

        private static GUIStyle ButtonStyle()
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(12, 8, 6, 6)
            };

            style.normal.textColor = Color.white;
            style.normal.background = SolidTexture(new Color(0.17f, 0.17f, 0.22f, 0.95f));
            style.hover.background = SolidTexture(new Color(0.24f, 0.24f, 0.33f, 1f));
            style.active.background = SolidTexture(new Color(0.31f, 0.20f, 0.42f, 1f));
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

        private static void DrawRoundedPanel(Rect rect, Texture2D texture)
        {
            GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill, true);
        }

        private static void DrawRoundedTopBar(Rect rect, Texture2D texture)
        {
            GUI.BeginGroup(rect);
            GUI.DrawTexture(new Rect(0f, 0f, rect.width, rect.height + CornerRadius), texture, ScaleMode.StretchToFill, true);
            GUI.EndGroup();
        }
    }
}
