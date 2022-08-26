using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using FGUFW.Core;

namespace FGUFW.HCT
{
    public class HCTColorEditor : EditorWindow
    {
        private ColorField _colorField;
        private SliderInt _hueSlider;
        private SliderInt _chromaSlider;
        private SliderInt _toneSlider;
        private SliderInt _rSlider;
        private SliderInt _gSlider;
        private SliderInt _bSlider;
        private VisualElement _colorShow;

        [MenuItem("HCT/HCTColorEditor")]
        public static void ShowExample()
        {
            HCTColorEditor wnd = GetWindow<HCTColorEditor>();
            wnd.titleContent = new GUIContent("HCTColorEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Develop/FGUFW/HCT/Editor/HCTColorEditor.uxml");
            visualTree.CloneTree(root);
            this.minSize = new Vector2(500,300);
            this.maxSize = new Vector2(500,300);

            _colorField = root.Q<ColorField>("ColorField");
            _hueSlider = root.Q<SliderInt>("Hue");
            _chromaSlider = root.Q<SliderInt>("Chroma");
            _toneSlider = root.Q<SliderInt>("Tone");
            _rSlider = root.Q<SliderInt>("R");
            _gSlider = root.Q<SliderInt>("G");
            _bSlider = root.Q<SliderInt>("B");

            _colorShow = root.Q<VisualElement>("ColorShow");
            root.Q<VisualElement>("HueColor").style.backgroundImage = new StyleBackground(getHueTex2D());

            _hueSlider.RegisterValueChangedCallback(onHueValueChange);
            _chromaSlider.RegisterValueChangedCallback(onChromaValueChange);
            _toneSlider.RegisterValueChangedCallback(onToneValueChange);
            _rSlider.RegisterValueChangedCallback(onRValueChange);
            _gSlider.RegisterValueChangedCallback(onGValueChange);
            _bSlider.RegisterValueChangedCallback(onBValueChange);

            _colorField.RegisterValueChangedCallback(onColorValueChanged);

            onColorChanged(_colorField.value);
        }

        private void onColorValueChanged(ChangeEvent<Color> evt)
        {
            //Debug.Log($"onColorValueChanged:{evt.newValue}");
            onColorChanged(evt.newValue);
        }

        private void onBValueChange(ChangeEvent<int> evt)
        {
            //Debug.Log($"onBValueChange:{evt.newValue}");
            onRGBChanged(_rSlider.value,_gSlider.value,_bSlider.value);
        }

        private void onGValueChange(ChangeEvent<int> evt)
        {
            //Debug.Log($"onGValueChange:{evt.newValue}");
            onRGBChanged(_rSlider.value,_gSlider.value,_bSlider.value);
        }

        private void onRValueChange(ChangeEvent<int> evt)
        {
            //Debug.Log($"onRValueChange:{evt.newValue}");
            onRGBChanged(_rSlider.value,_gSlider.value,_bSlider.value);
        }

        private void onToneValueChange(ChangeEvent<int> evt)
        {
            //Debug.Log($"onToneValueChange:{evt.newValue}");
            onHCTChanged(_hueSlider.value,_chromaSlider.value,_toneSlider.value);
        }

        private void onChromaValueChange(ChangeEvent<int> evt)
        {
            //Debug.Log($"onChromaValueChange:{evt.newValue}");
            onHCTChanged(_hueSlider.value,_chromaSlider.value,_toneSlider.value);
        }

        private void onHueValueChange(ChangeEvent<int> evt)
        {
            //Debug.Log($"onHueValueChange:{evt.newValue}");
            onHCTChanged(_hueSlider.value,_chromaSlider.value,_toneSlider.value);
        }

        private void onRGBChanged(int r,int g,int b)
        {
            var color = new Color32((byte)r,(byte)g,(byte)b,byte.MaxValue);
            var hct = new Hct(color.ToARGBInt());

            _hueSlider.SetValueWithoutNotify((int)hct.Hue);
            _chromaSlider.SetValueWithoutNotify((int)hct.Chroma);
            _toneSlider.SetValueWithoutNotify((int)hct.Tone);

            _colorField.SetValueWithoutNotify(color);

            _colorShow.style.backgroundColor = new StyleColor(color);
        }

        private void onHCTChanged(int h,int c,int t)
        {
            var hct = new Hct(h,c,t);
            var color = ColorHelper.FromARGBInt(hct.Argb);

            _rSlider.SetValueWithoutNotify(color.r);
            _gSlider.SetValueWithoutNotify(color.g);
            _bSlider.SetValueWithoutNotify(color.b);
            _colorField.SetValueWithoutNotify(color);

            _colorShow.style.backgroundColor = new StyleColor(color);
        }

        private void onColorChanged(Color32 color)
        {
            _colorField.value = color;
            var hct = new Hct(color.ToARGBInt());

            _hueSlider.SetValueWithoutNotify((int)hct.Hue);
            _chromaSlider.SetValueWithoutNotify((int)hct.Chroma);
            _toneSlider.SetValueWithoutNotify((int)hct.Tone);

            _rSlider.SetValueWithoutNotify(color.r);
            _gSlider.SetValueWithoutNotify(color.g);
            _bSlider.SetValueWithoutNotify(color.b);

            _colorShow.style.backgroundColor = new StyleColor(color);
        }

        private Texture2D getHueTex2D()
        {
            float length = 256;
            var tex2D = new Texture2D((int)length,1);
            float start_H = 327;
            for (float i = 0; i < length; i++)
            {
                var color = new Color();
                color.r = ((360*i/length + start_H)%360f)/360f;
                // Debug.Log(color.r);
                color.g = 1;
                color.b = 1;
                color = color.HSV2RGB();
                color.a = 1;
                tex2D.SetPixel((int)i,0,color);
            }
            tex2D.Apply();
            // System.IO.File.WriteAllBytes("D:/hueTex2D.png",tex2D.EncodeToPNG());
            return tex2D;
        }


    }
}