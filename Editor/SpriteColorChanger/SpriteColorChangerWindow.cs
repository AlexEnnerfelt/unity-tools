using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace UnpopularOpinion.Tools.SpriteColorPalette {

    public class SpriteColorChangerWindow : EditorWindow {
        internal static ColorSelection CurrentColor {
            get => _currentColor;
            set {
                _currentColor = value;
                if (value != null) {
                    instance.Inspect(value);
                }
            }
        }
        private static ColorSelection _currentColor;
        public static SpriteColorChangerWindow instance;

        private Dictionary<string, List<ColorSelection>> _colorPalettes = new();
        private List<ColorSelection> _colors = new();
        private VisualElement _optionInspector;
        private VisualElement _paletteList;
        private ColorField _colorField;
        private VisualTreeAsset _paletteAsset;

        private bool _debug = false;
        public override void SaveChanges() {
            base.SaveChanges();
            SaveColorsToPlayerPrefs();
        }

        private bool ToolReady {
            get {
                return _paintMode && CurrentColor != null;
            }
        }
        private bool _paintMode = false;

        public void CreateGUI() {
            _paletteAsset = Resources.Load<VisualTreeAsset>("color-palette");
            var windowTemplate = Resources.Load<VisualTreeAsset>("sprite-color-changer-window");

            var window = windowTemplate.Instantiate();
            rootVisualElement.Add(window);

            _paletteList = window.Q<VisualElement>("palettes");
            _paletteList.Clear();

            _optionInspector = window.Q<VisualElement>("palette-inspector");
            _optionInspector.Q<ColorField>().SetBinding(nameof(ColorField.value), new DataBinding() {
                dataSourcePath = new(nameof(ColorSelection.Color)),
                dataSourceType = typeof(ColorSelection),
                bindingMode = BindingMode.TwoWay,
            });
            _optionInspector.Q<Slider>("R-Slider").SetBinding(nameof(Slider.value), new DataBinding() {
                dataSourcePath = new(nameof(ColorSelection.rVariance)),
                dataSourceType = typeof(ColorSelection),
                bindingMode = BindingMode.TwoWay,
            });
            _optionInspector.Q<Slider>("G-Slider").SetBinding(nameof(Slider.value), new DataBinding() {
                dataSourcePath = new(nameof(ColorSelection.gVariance)),
                dataSourceType = typeof(ColorSelection),
                bindingMode = BindingMode.TwoWay,
            });
            _optionInspector.Q<Slider>("B-Slider").SetBinding(nameof(Slider.value), new DataBinding() {
                dataSourcePath = new(nameof(ColorSelection.bVariance)),
                dataSourceType = typeof(ColorSelection),
                bindingMode = BindingMode.TwoWay,
            });
            _optionInspector.Q<Button>("delete-button").clicked += DeleteCurrent;
            var radioButtonGroup = _optionInspector.Q<RadioButtonGroup>();
            radioButtonGroup.RegisterValueChangedCallback(change => {
                _paintMode = change.newValue == 0;
            });
            radioButtonGroup.value = 0;

            var debugToggle = rootVisualElement.Q<ToolbarToggle>();
            debugToggle.SetValueWithoutNotify(_debug);
            debugToggle.RegisterValueChangedCallback(val => _debug = val.newValue);

            CreatePalette(_colors);
            Inspect(null);
        }

        public void Inspect(ColorSelection colSel) {
            _optionInspector.visible = colSel is not null;
            _optionInspector.dataSource = colSel;
        }
        private void DeleteCurrent() {
            CurrentColor.Delete();
            Inspect(null);

        }

        private void CreatePalette(List<ColorSelection> palette) {
            var paletteTemplate = _paletteAsset.Instantiate();
            _paletteList.Add(paletteTemplate);

            var pal = new ColorPalette(ref palette, paletteTemplate);
        }


        private void OnEnable() {
            SceneView.duringSceneGui += OnSceneGUI;
            LoadColorsFromPlayerPrefs();
            instance = GetWindow(typeof(SpriteColorChangerWindow)) as SpriteColorChangerWindow;
        }
        private void OnDisable() {
            SceneView.duringSceneGui -= OnSceneGUI;
            SaveChanges();
        }

        private void ShowSelection(ColorSelection value) {

            _colorField.SetBinding(nameof(ColorField.value), new DataBinding() {
                bindingMode = BindingMode.TwoWay,
                dataSource = value,
                updateTrigger = BindingUpdateTrigger.EveryUpdate,
            });
        }
        private void OnSceneGUI(SceneView sceneView) {
            SpriteRenderer[] allRenderers;
            if (_debug) {
                allRenderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
                foreach (var renderer in allRenderers) {
                    // Draw the bounds of the renderer as gizmos
                    Bounds bounds = renderer.bounds;
                    Handles.DrawWireCube(bounds.center, bounds.size);
                }
            }


            if (!ToolReady) {
                return;
            }
            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0) {
                // Get the mouse position in world space
                Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;

                // Find all renderers in the scene
                allRenderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);

                // Find the renderer under the mouse cursor
                var withinBounds = allRenderers.Where(rend => rend.bounds.Contains(mousePosition)).ToList();
                var withinSprite = withinBounds.Where(rend => IsPointInSprite(rend, mousePosition)).ToList();
                IEnumerable<SpriteRenderer> orderedSprites;

                if (withinSprite.Count > 0) {
                    orderedSprites = withinSprite.OrderByDescending(rend => rend.sortingOrder);
                } else if (withinBounds.Count > 0) {
                    orderedSprites = withinBounds.OrderByDescending(rend => rend.sortingOrder);
                } else {
                    return;
                }

                if (orderedSprites.Count() > 0) {
                    var topSprite = orderedSprites.FirstOrDefault();

                    if (topSprite != null) {
                        // Change the color of the sprite
                        Undo.RecordObject(topSprite, "Change Sprite Color");
                        topSprite.color = CurrentColor.GetColor();
                        EditorUtility.SetDirty(topSprite);
                    }
                }
            }
        }
        private bool IsPointInSprite(SpriteRenderer spriteRenderer, Vector2 point) {
            // Convert the point to local space of the sprite
            Vector2 localPoint = spriteRenderer.transform.InverseTransformPoint(point);

            // Get the sprite's texture
            var sprite = spriteRenderer.sprite;
            var rect = sprite.rect;
            var texture = sprite.texture;

            // Convert local point to texture space
            var texturePoint = new Vector2(
                localPoint.x * rect.width / sprite.bounds.size.x + rect.width / 2,
                localPoint.y * rect.height / sprite.bounds.size.y + rect.height / 2
            );

            // Check if the point is within the texture bounds
            if (texturePoint.x >= 0 && texturePoint.x < rect.width && texturePoint.y >= 0 && texturePoint.y < rect.height) {
                try {
                    // Get the pixel color at the texture point
                    var pixelColor = texture.GetPixel((int)texturePoint.x, (int)texturePoint.y);
                    return pixelColor.a > 0; // Check if the pixel is not transparent
                } catch (UnityException) {
                    Debug.LogWarning($"Can't read the texture of {sprite.name}, It's recommended to enable Read/Write to make this tool function as intended", sprite);
                }
            }


            return false;
        }
        private void SaveColorsToPlayerPrefs() {
            var json = JsonConvert.SerializeObject(_colors, Formatting.Indented, new ColorConverter());

            PlayerPrefs.SetString("SavedColors", json);
            PlayerPrefs.Save();
        }
        private void LoadColorsFromPlayerPrefs() {
            var json = PlayerPrefs.GetString("SavedColors", string.Empty);
            _colors = JsonConvert.DeserializeObject<List<ColorSelection>>(json, new ColorConverter());
        }

        public class ColorPalette {
            private List<ColorSelection> _colors;

            private VisualElement _optionsList;
            private Button _addOptionButton;
            private RadioButton _selection;
            public ColorPalette(ref List<ColorSelection> palette, VisualElement root) {
                _colors = palette;
                _optionsList = root.Q<VisualElement>("palette-options");
                var foldout = root.Q<Foldout>();
                foldout.text = "Palette";
                _optionsList.Clear();
                foreach (var item in palette) {
                    _optionsList.Add(AddSelectionButton(item));
                }
                _addOptionButton = new Button();
                _optionsList.Add(_addOptionButton);
                _addOptionButton.clicked += AddOption;
            }
            private RadioButton AddSelectionButton(ColorSelection newSelection) {
                var radioButton = new RadioButton();
                radioButton.SetBinding(nameof(RadioButton.style.backgroundColor.value), new DataBinding() {
                    bindingMode = BindingMode.ToTarget,
                    dataSourcePath = new(nameof(ColorSelection.Color)),
                    dataSourceType = typeof(ColorSelection),
                    dataSource = newSelection
                });

                newSelection.onChanged += (col) => {
                    radioButton.style.backgroundColor = new(col);
                };
                radioButton.style.backgroundColor = new(newSelection.Color);

                radioButton.RegisterValueChangedCallback(change => {
                    if (change.newValue) {
                        CurrentColor = newSelection;
                        _selection = radioButton;
                    }
                });

                newSelection.onDelete += () => {
                    _colors.Remove(newSelection);
                    _optionsList.Remove(_selection);
                };

                return radioButton;
            }
            private void AddOption() {
                var newSelection = new ColorSelection();
                _colors.Add(newSelection);
                ;
                _optionsList.Insert(_optionsList.IndexOf(_addOptionButton), AddSelectionButton(newSelection));
                instance.SaveChanges();
            }
        }


        [Serializable]
        public class ColorSelection {
            [CreateProperty]
            public Color Color {
                get => _color;
                set {
                    _color = value;
                    onChanged?.Invoke(_color);
                }
            }
            private Color _color = Color.white;

            [CreateProperty]
            public float rVariance = 0.01f;
            [CreateProperty]
            public float gVariance = 0.01f;
            [CreateProperty]
            public float bVariance = 0.01f;

            public event Action<Color> onChanged;
            public event Action onDelete;


            public Color GetColor() {
                var color = new Color(
                    Mathf.Clamp01(_color.r + UnityEngine.Random.Range(-rVariance, rVariance)),
                    Mathf.Clamp01(_color.g + UnityEngine.Random.Range(-gVariance, gVariance)),
                    Mathf.Clamp01(_color.b + UnityEngine.Random.Range(-bVariance, bVariance)),
                    Mathf.Clamp01(_color.a)
                );

                return color;
            }

            internal void Delete() {
                onDelete?.Invoke();
            }
        }
        public class ColorConverter : JsonConverter<Color> {
            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer) {
                var colorString = (string)reader.Value;
                if (ColorUtility.TryParseHtmlString(colorString, out Color color)) {
                    return color;
                }
                throw new JsonSerializationException($"Invalid color string: {colorString}");
            }
            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer) {
                writer.WriteValue($"#{ColorUtility.ToHtmlStringRGBA(value)}");
            }
        }
        [MenuItem("Tools/Sprite Color Palette")]
        public static void ShowWindow() {
            var newWindow = GetWindow<SpriteColorChangerWindow>("Sprite Color Palette");
            newWindow.maxSize = new(300, 600);
            newWindow.minSize = new(300, 340);
            instance = newWindow;
        }

        private SpriteRenderer OnTop(SortingGroup grp) {

            if (grp.TryGetComponentsInChildren<SpriteRenderer>(out var rends)) {
                return rends.ToList().OrderByDescending(r => r.sortingOrder).FirstOrDefault();
            } else {
                return null;
            }
        }

        private int GetDrawingOrder(SpriteRenderer spriteRenderer) {


            var sortingOrder = spriteRenderer.sortingOrder;
            var sortingLayerValue = SortingLayer.GetLayerValueFromID(spriteRenderer.sortingLayerID);
            var currentTransform = spriteRenderer.transform;




            while (currentTransform.TryGetComponentInParent(out SortingGroup sortingGroup)) {

                if (sortingGroup.sortAtRoot) {
                    sortingOrder = sortingGroup.sortingOrder;
                    sortingLayerValue = SortingLayer.GetLayerValueFromID(sortingGroup.sortingLayerID);
                    break;
                } else {
                    sortingOrder = sortingGroup.sortingOrder;
                    sortingLayerValue = SortingLayer.GetLayerValueFromID(sortingGroup.sortingLayerID);
                }

                currentTransform = sortingGroup.transform;
            }

            return sortingLayerValue * 1000 + sortingOrder;
        }
        private bool GetRootSortingGroup(Transform currentTransform, out SortingGroup sortingGroup) {
            SortingGroup group;
            while (currentTransform.TryGetComponentInParent(out group)) {
                if (group.sortAtRoot) {
                    sortingGroup = group;
                    return true;
                }
                currentTransform = group.transform;
            }
            sortingGroup = group;
            return group != null;
        }
    }
}