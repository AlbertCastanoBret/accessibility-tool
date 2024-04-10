using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFG_Videojocs.ACC_Utilities
{
    public class ACC_UIElementFactory
    {
        public Dictionary<string, int> nameCounters { get; private set; } = new Dictionary<string, int>();
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
            return integerField;
        }

        public ColorField CreateColorField(string classList, Color color = default, Action<Color> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var colorField = new ColorField { name = name, value = color == default ? Color.white : color };
            colorField.AddToClassList(classList);
            colorField.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            return colorField;
        }

        public SliderInt CreateSliderInt(string classList, string label, int minValue, int maxValue, int defaultValue = 20, 
            string subClassList = "", Action<int> onValueChanged = null)
        {
            var name = GenerateUniqueName(classList);
            
            var slider = new SliderInt(label, minValue, maxValue) { name = name, value = defaultValue };
            slider.AddToClassList(classList);
            slider[0].AddToClassList(subClassList);
            slider.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            return slider;
        }

        public Button CreateButton(string text, string classList, Action onClick = null)
        {
            var name = GenerateUniqueName(classList);
            if(name.Contains("delete")) Debug.Log(name);
            
            var button = new Button(() => onClick?.Invoke()) { name = name, text = text };
            button.AddToClassList(classList);
            return button;
        } 
        
        public ScrollView CreateScrollView(VisualElement content, string classList)
        {
            var name = GenerateUniqueName(classList);

            var scrollView = new ScrollView(ScrollViewMode.Vertical) { name = name };
            scrollView.Add(content);
            scrollView.AddToClassList(classList);
            return scrollView;
        }
         
        private string GenerateUniqueName(string baseClass)
        {
            nameCounters.TryAdd(baseClass, 0);
            var name = $"{baseClass}-{nameCounters[baseClass]}";
            nameCounters[baseClass]++;
            return name;
        }
    }
}