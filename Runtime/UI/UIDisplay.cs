using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnpopularOpinion.UICore {
    public class UIDisplay : MonoBehaviour, IUIElementsInit, IUIDisplay {
        public VisualElement Root { get; protected set; }
        protected UIDocument _document;
        protected static UIDisplay fade;
        public string RootElementName;

        [field: SerializeField, ReadOnlyField]
        public bool IsOpen { get; protected set; } = true;

        public string RootTag => _document.rootVisualElement.name;

        public virtual void Awake() {
            if (TryGetComponent(out _document)) {
                _document.enabled = true;
                //SetUp(_document.rootVisualElement);
                if (string.IsNullOrEmpty(RootElementName)) {
                    SetUp(_document.rootVisualElement);
                } else {
                    SetUp(_document.rootVisualElement.Q<VisualElement>(RootElementName));

                }
            } else {
                Debug.LogError($"Missing UIDocument for {name}", this);
            }
        }

        public void Start() {
            Initialization();
        }
        public virtual void SetUp(VisualElement root) {
            Root = root;
            RegisterForGlobalEventsRecursive(root);

        }

        public virtual void Initialization() { }
        public virtual void Show() {
            Root.visible = true;
        }
        public virtual void Hide() {
            Root.visible = false;
        }

        protected virtual void Evaluate() {
            Root.visible = IsOpen;
        }
        public static void RegisterForGlobalEventsRecursive(VisualElement visualElement) {
            if (visualElement == null) {
                return;
            }
            RegisterForGlobalEvents(visualElement);
            var children = visualElement.Children().ToList();
            children.ForEach(child => RegisterForGlobalEventsRecursive(child));
        }
        public static void RegisterForGlobalEvents(VisualElement visualElement) {
            if (visualElement is Button button) {
                HandleButton(button);
            } else if (visualElement is Toggle toggle) {
                HandleToggle(toggle);
            } else if (visualElement is Slider slider) {
                HandleSlider(slider);
            } else if (visualElement is SliderInt sliderInt) {
                HandleSlider(sliderInt);
            } else if (visualElement is DropdownField dropdown) {
                HandleDropdown(dropdown);
            }

            void HandleButton(Button elem) {
                elem.RegisterCallback<FocusInEvent>(e => {
                    UINavigationEvent.Invoke(NavigationAction.Selected, elem.name);
                });
                elem.clicked += () => {
                    UINavigationEvent.Invoke(NavigationAction.Clicked, elem.name);
                };
            }

            void HandleToggle(Toggle elem) {
                elem.RegisterCallback<FocusInEvent>(e => {
                    UINavigationEvent.Invoke(NavigationAction.Selected, elem.name);
                });
                elem.RegisterValueChangedCallback(e => {
                    UINavigationEvent.Invoke(NavigationAction.Clicked, elem.name);
                });
            }

            void HandleDropdown(DropdownField elem) {
                elem.RegisterCallback<FocusInEvent>(e => {
                    UINavigationEvent.Invoke(NavigationAction.Selected, elem.name);
                });
                elem.RegisterValueChangedCallback(e => {
                    UINavigationEvent.Invoke(NavigationAction.ValueChanged, elem.name);
                });
            }

            void HandleSlider<T>(BaseSlider<T> elem) where T : IComparable<T> {
                elem.RegisterCallback<FocusInEvent>(e => {
                    UINavigationEvent.Invoke(NavigationAction.Selected, elem.name);
                });
                elem.RegisterValueChangedCallback(e => {
                    if (!e.newValue.Equals(e.previousValue)) {
                        UINavigationEvent.Invoke(NavigationAction.ValueChanged, elem.name);
                    }
                });
            }
        }
    }

    public interface IUIElementsInit {
        public void SetUp(VisualElement root);
        public string RootTag { get; }
        public VisualElement Root { get; }
    }

    public interface IUIDisplay {
        public void Show();
        public void Hide();
    }

    public static class NavigationUtils {
        public static void SetNavigation(Focusable origin, Focusable left = null, Focusable right = null, Focusable up = null, Focusable down = null) {
            origin.RegisterCallback<NavigationMoveEvent>(evt => {
                switch (evt.direction) {
                    case NavigationMoveEvent.Direction.None:
                        break;
                    case NavigationMoveEvent.Direction.Left:
                        left?.Focus();
                        break;
                    case NavigationMoveEvent.Direction.Up:
                        up?.Focus();
                        break;
                    case NavigationMoveEvent.Direction.Right:
                        right?.Focus();
                        break;
                    case NavigationMoveEvent.Direction.Down:
                        down?.Focus();
                        break;
                    case NavigationMoveEvent.Direction.Next:
                        break;
                    case NavigationMoveEvent.Direction.Previous:
                        break;
                    default:
                        break;
                }
            });
        }
    }
}