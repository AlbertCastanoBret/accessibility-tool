#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TFG_Videojocs.ACC_Utilities
{
    [System.Serializable]
    public class ACC_UIElementFactory
    {
        public ACC_SerializableDictiornary<string, int> nameCounters = new();
        public VisualElement CreateVisualElement(string classList)
        {
            var name = GenerateUniqueName(classList);
            
            var visualElement = new VisualElement { name = name };
            visualElement.AddToClassList(classList);
            return visualElement;
        }
        
        public Label CreateLabel(string classList, string value = "", Action<string> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var label = new Label(value) { name = name };
            label.AddToClassList(classList);
            label.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            return label;
        }
        
        public TextField CreateTextField(string classList, string label = "", string value = "", 
            string subClassList = "", Action<string> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var textField = new TextField(label) { name = name, value = value };
            textField.AddToClassList(classList);
            textField[0].AddToClassList(subClassList);
            textField.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            onValueChanged?.Invoke(textField.value);
            return textField;
        }
        
        public IntegerField CreateIntegerField(string classList, string label = "", int value = 0, 
            string subClassList = "", Action<int> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var integerField = new IntegerField(label) { name = name, value = value };
            integerField.AddToClassList(classList);
            integerField[0].AddToClassList(subClassList);
            integerField.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            onValueChanged?.Invoke(integerField.value);
            return integerField;
        }
        
        public FloatField CreateFloatField(string classList, string label = "", float value = 0, 
            string subClassList = "", Action<float> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var floatField = new FloatField(label) { name = name, value = value };
            floatField.AddToClassList(classList);
            floatField[0].AddToClassList(subClassList);
            floatField.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            onValueChanged?.Invoke(floatField.value);
            return floatField;
        }

        public ColorField CreateColorField(string classList, string label, Color color = default, string subClassList = "", Action<Color> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var colorField = new ColorField(label) { name = name, value = color == default ? Color.white : color };
            colorField.AddToClassList(classList);
            colorField[0].AddToClassList(subClassList);
            colorField.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            onValueChanged?.Invoke(colorField.value);
            return colorField;
        }

        public SliderInt CreateSliderInt(string classList, string label, int minValue, int maxValue, 
            int defaultValue = 20, string subClassList1 = "multi-input-1-1", string subClassList2 = "multi-input-1-2", Action<int> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var slider = new SliderInt(label, minValue, maxValue) { name = name, value = defaultValue };
            slider.AddToClassList(classList);
            slider[0].name = GenerateUniqueName(subClassList1);
            slider[0].AddToClassList(subClassList1);
            
            slider[1].name = GenerateUniqueName(GenerateUniqueName(subClassList2));
            slider[1].AddToClassList(subClassList2);
            slider.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            onValueChanged?.Invoke(slider.value);
            return slider;
        }
        
        public Slider CreateSliderFloat(string classList, string label, float minValue, float maxValue, 
            float defaultValue = 20, string subClassList1 = "multi-input-1-1", string subClassList2 = "multi-input-1-2", Action<float> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var slider = new Slider(label, minValue, maxValue) { name = name, value = defaultValue };
            slider.AddToClassList(classList);
            slider[0].name = GenerateUniqueName(subClassList1);
            slider[0].AddToClassList(subClassList1);
            
            slider[1].name = GenerateUniqueName(GenerateUniqueName(subClassList2));
            slider[1].AddToClassList(subClassList2);
            slider.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            onValueChanged?.Invoke(slider.value);
            return slider;
        }

        public Button CreateButton(string text, string classList, Action onClick = null)
        {
            var name = GenerateUniqueName(classList);
            
            var button = new Button(() => onClick?.Invoke()) { name = name, text = text };
            button.AddToClassList(classList);
            return button;
        } 
        
        public ScrollView CreateScrollView(string classList, VisualElement content=null)
        {
            var name = GenerateUniqueName(classList);

            var scrollView = new ScrollView(ScrollViewMode.Vertical) { name = name };
            scrollView.Add(content);
            scrollView.AddToClassList(classList);
            return scrollView;
        }
        
        public VisualElement CreateObjectField(string classList, string label,  Type type, Object value = null,
            string subClassList1 = "multi-input-1-1", string subClassList2 = "multi-input-1-2", 
            Action<Object> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var objectField = new ObjectField(label) { name = name, objectType = type, value = value};
            objectField.AddToClassList(classList);

            objectField[0].name = GenerateUniqueName(subClassList1);
            objectField[0].AddToClassList(subClassList1);
            
            objectField[1].name = GenerateUniqueName(subClassList2);
            objectField[1].AddToClassList(subClassList2);
            objectField.RegisterValueChangedCallback(evt =>
            {
                onValueChanged?.Invoke(evt.newValue);
            });
            onValueChanged?.Invoke(objectField.value);
            return objectField;
        }
        
        public VisualElement CreateDropdownField(string classList, string label = "", List<string> options = null, 
            string value = "Default", string subClassList = "", Action<string> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var dropdownField = new DropdownField(label, options, 0) { name = name, value = value};
            dropdownField.AddToClassList(classList);
            dropdownField[0].AddToClassList(subClassList);
            dropdownField.RegisterValueChangedCallback(evt =>
            {
                onValueChanged?.Invoke(evt.newValue);
            });
            onValueChanged?.Invoke(dropdownField.value);
            return dropdownField;
        }
        
        public VisualElement CreateToggle(string classList, string label, bool value = false, 
            string subClassList = "", Action<bool> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var toggle = new Toggle(label) { name = name, value = value };
            toggle.AddToClassList(classList);
            toggle[0].AddToClassList(subClassList);
            toggle.RegisterValueChangedCallback(evt =>
            {
                onValueChanged?.Invoke(evt.newValue);
            });
            onValueChanged?.Invoke(toggle.value);
            return toggle;
        }
        
        public VisualElement CreateSliderWithIntegerField(string classList, string label, int min, int max, int defaultValue,
            string sliderClassList = "multi-input-1", string integerFieldClassList = "multi-input-2", Action<int> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var container = new VisualElement(){name = name};
            container.AddToClassList(classList);

            var slider = CreateSliderInt(sliderClassList, label, min, max, defaultValue, onValueChanged: onValueChanged);
            var inputField = CreateIntegerField(integerFieldClassList, "", defaultValue, onValueChanged: onValueChanged);
            
            slider.RegisterValueChangedCallback(evt =>
            {
                inputField.value = evt.newValue;
            });
            
            inputField.RegisterValueChangedCallback(evt =>
            {
                slider.value = evt.newValue;
            });
            
            container.Add(slider);
            container.Add(inputField);

            return container;
        }
        
        public VisualElement CreateSliderWithFloatField(string classList, string label, float min, float max, float defaultValue,
            string sliderClassList = "multi-input-1", string integerFieldClassList = "multi-input-2", Action<float> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var container = new VisualElement(){name = name};
            container.AddToClassList(classList);

            var slider = CreateSliderFloat(sliderClassList, label, min, max, defaultValue, onValueChanged: onValueChanged);
            var inputField = CreateFloatField(integerFieldClassList, "", defaultValue, onValueChanged: onValueChanged);
            
            slider.RegisterValueChangedCallback(evt =>
            {
                inputField.value = evt.newValue;
            });
            
            inputField.RegisterValueChangedCallback(evt =>
            {
                slider.value = evt.newValue;
            });
            
            container.Add(slider);
            container.Add(inputField);

            return container;
        }
        
        public VisualElement CreateObjectFieldWithButton(string classList, string label, string buttonLabel, 
            Type type, Action<Object> onObjectField=null, Action onClick = null, 
            string objectFieldClassList = "multi-input-1", string buttonClassList = "multi-input-2") 
        {
            var name = GenerateUniqueName(classList);
            
            var container = new VisualElement(){name = name};
            container.AddToClassList(classList);

            var objectField = CreateObjectField(objectFieldClassList, label, type, onValueChanged: onObjectField);
            var button = CreateButton(buttonLabel, buttonClassList, onClick);
            
            container.Add(objectField);
            container.Add(button);

            return container;
        }
        
        public VisualElement CreateImage(string classList, Texture2D texture)
        {
            var name = GenerateUniqueName(classList);
            
            var image = new Image() { name = name, image = texture };
            image.AddToClassList(classList);
            return image;
        }
         
        private string GenerateUniqueName(string baseClass)
        {
            nameCounters.AddOrUpdate(baseClass,
                nameCounters.Items.Exists(item => item.key == baseClass)
                    ? nameCounters.Items.Find(item => item.key == baseClass).value
                    : 0);

            var name = $"{baseClass}-{nameCounters.Items.Find(item => item.key == baseClass).value.ToString()}";
            nameCounters.Items.Find(item => item.key == baseClass).value++;
            
            return name;
        }
    }
}
#endif