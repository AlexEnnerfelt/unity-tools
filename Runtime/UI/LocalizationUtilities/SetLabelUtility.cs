using UnityEngine.UIElements;

public static class SetLabelUtility {
    public static void SetLabelText(this VisualElement elem, string labelText) {
        if (elem is IPrefixLabel label) {
            label.labelElement.text = labelText;
        }
        else if (elem is TextElement textElem) {
            textElem.text = labelText;
        }
    }
}
