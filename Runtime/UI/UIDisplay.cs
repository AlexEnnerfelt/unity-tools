using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnpopularOpinion.UICore
{

    public class UIDisplay : MonoBehaviour, IUIElementsInit, IUIDisplay
    {
        [SerializeField]
        protected HandlingMethod handlingMethod = HandlingMethod.Display;
        public VisualElement Root { get; protected set; }
        protected UIDocument _document;
        protected static UIDisplay fade;
        public string RootElementName;

        [field: SerializeField, ReadOnlyField] public bool IsOpen { get; protected set; } = false;

        public string RootTag => _document.rootVisualElement.name;

        public virtual void Awake() {
            IsOpen = false;
            if (TryGetComponent(out _document)) {
                if (handlingMethod is not HandlingMethod.Hierarchy) {
                    _document.enabled = true;
                    if (string.IsNullOrEmpty(RootElementName)) {
                        SetUp(_document.rootVisualElement);
                    }
                    else {
                        SetUp(_document.rootVisualElement.Q<VisualElement>(RootElementName));
                    }
                }
            }
            else {
                Debug.LogError($"Missing UIDocument for {name}", this);
            }
            
        }
        public void Start() {
            if (handlingMethod is not HandlingMethod.Hierarchy) {
                Initialization();
            }
            SetInitialState();
        }
        public virtual void SetUp(VisualElement root) {
            Root = root;
            RegisterForGlobalEventsRecursive(root);
        }

        public virtual void Initialization() {
        }
        protected virtual void SetInitialState() {
            
        }
        public virtual void Show() {
            IsOpen = true;
            Evaluate();
        }
        public virtual void Hide() {
            IsOpen = false;
            Evaluate();
        }

        protected virtual void Evaluate() {
            if (handlingMethod is HandlingMethod.Display) {
                Root.style.display = IsOpen ? DisplayStyle.Flex : DisplayStyle.None;
            }
            else if (handlingMethod is HandlingMethod.Visibility) {
                Root.visible = IsOpen;
            }
            else if (handlingMethod is HandlingMethod.Hierarchy) {
                _document.enabled = IsOpen;
                if (IsOpen) {
                    
                    if (string.IsNullOrEmpty(RootElementName)) {
                        Root = _document.rootVisualElement;
                        SetUp(Root);
                    }
                    else {
                        Root = _document.rootVisualElement.Q<VisualElement>(RootElementName);
                        SetUp(Root);
                    }
                    Initialization();
                }
            }
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
            }
            else if (visualElement is Toggle toggle) {
                HandleToggle(toggle);
            }
            else if (visualElement is Slider slider) {
                HandleSlider(slider);
            }
            else if (visualElement is SliderInt sliderInt) {
                HandleSlider(sliderInt);
            }
            else if (visualElement is DropdownField dropdown) {
                HandleDropdown(dropdown);
            }

            void HandleButton(Button elem) {
                elem.RegisterCallback<FocusInEvent>(e => { UINavigationEvent.Invoke(NavigationAction.Selected, elem.name); });
                elem.clicked += () => { UINavigationEvent.Invoke(NavigationAction.Clicked, elem.name); };
            }

            void HandleToggle(Toggle elem) {
                elem.RegisterCallback<FocusInEvent>(e => { UINavigationEvent.Invoke(NavigationAction.Selected, elem.name); });
                elem.RegisterValueChangedCallback(e => { UINavigationEvent.Invoke(NavigationAction.Clicked, elem.name); });
            }

            void HandleDropdown(DropdownField elem) {
                elem.RegisterCallback<FocusInEvent>(e => { UINavigationEvent.Invoke(NavigationAction.Selected, elem.name); });
                elem.RegisterValueChangedCallback(e => { UINavigationEvent.Invoke(NavigationAction.ValueChanged, elem.name); });
            }

            void HandleSlider<T>(BaseSlider<T> elem) where T : IComparable<T> {
                elem.RegisterCallback<FocusInEvent>(e => { UINavigationEvent.Invoke(NavigationAction.Selected, elem.name); });
                elem.RegisterValueChangedCallback(e => {
                    if (!e.newValue.Equals(e.previousValue)) {
                        UINavigationEvent.Invoke(NavigationAction.ValueChanged, elem.name);
                    }
                });
            }
        }

        protected enum HandlingMethod
        {
            Display,
            Visibility,
            Hierarchy
        }
    }

    public interface IUIElementsInit
    {
        public void SetUp(VisualElement root);
        public string RootTag { get; }
        public VisualElement Root { get; }
    }

    public interface IUIDisplay
    {
        public void Show();
        public void Hide();
    }

    public static class NavigationUtils
    {
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
        
    public interface IViewController<in T>
    {
        public void SetModel(T model);
    }

}