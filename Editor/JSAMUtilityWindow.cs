using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace JSAM.JSAMEditor
{
    public class JSAMUtilityWindow : JSAMBaseEditorWindow<JSAMUtilityWindow>
    {
        public class CustomField
        {
            public GUIContent content;
            public string text;
            public bool useTextArea;
            public Object value;
            public Type valueType;
        }

        string windowName;
        bool allowEnterKey;
        bool allowEscapeKey;
        bool focused = false;
        static bool quickReferenceGuide = true;

        List<CustomField> fields = new();

        public static System.Action<string[]> onSubmitField;
        public static System.Action<Object[]> onSubmitObjectField;

        public static JSAMUtilityWindow Init(string _windowName, bool _allowEnterKey, bool _allowEscapeKey)
        {
            window = CreateInstance<JSAMUtilityWindow>();
            window.ShowUtility();
            window.titleContent = new GUIContent(_windowName);
            window.allowEnterKey = _allowEnterKey;
            window.allowEscapeKey = _allowEscapeKey;
            return window;
        }

        private void OnDisable()
        {
            onSubmitField = null;
        }

        public void AddField(GUIContent content, string startingText = "", bool useTextArea = false)
        {
            fields.Add(new CustomField
            {
                content = content,
                text = startingText,
                useTextArea = useTextArea
            });
        }

        public void AddTypeField(GUIContent content, Object value, Type type)
        {
            fields.Add(new CustomField
            {
                content = content,
                value = value,
                valueType = type
            });
        }

        private void OnGUI()
        {
            foreach (CustomField field in fields)
            {
                if (field.content != GUIContent.none)
                {
                    EditorGUILayout.LabelField(field.content);
                }

                if (!focused) GUI.SetNextControlName("TextBox");
                if (field.valueType != null)
                {
                    field.value =
                        EditorGUILayout.ObjectField(field.value, field.valueType, allowSceneObjects: false);
                }
                else
                {
                    field.text = !field.useTextArea 
                        ? EditorGUILayout.TextField(field.text)
                        : EditorGUILayout.TextArea(field.text);
                }

                if (!focused)
                {
                    GUI.FocusControl("TextBox");
                    focused = true;
                }
                EditorGUILayout.Space();
            }
            List<string> tipText = new List<string>();
            if (allowEscapeKey)
            {
                tipText.Add("Tip: You can double-press ESC to quickly close this window!");
            }
            if (allowEnterKey)
            {
                tipText.Add("Tip: You can double-press ENTER to quickly submit your text!");
            }
            quickReferenceGuide = JSAMEditorHelper.RenderQuickReferenceGuide(quickReferenceGuide, tipText.ToArray());

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    if (allowEscapeKey)
                    {
                        window.Close();
                    }
                }

                if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                {
                    if (allowEnterKey)
                    {
                        SubmitText();
                    }
                }
            }
            
            if (GUILayout.Button("Submit"))
            {
                SubmitText();
            }
        }

        void SubmitText()
        {
            List<string> text = new List<string>();
            List<Object> values = new List<Object>();
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].valueType != null)
                    values.Add(fields[i].value);
                else
                    text.Add(fields[i].text);
            }
            onSubmitField?.Invoke(text.ToArray());
            onSubmitObjectField?.Invoke(values.ToArray());
            window.Close();
        }

        protected override void SetWindowTitle()
        {
        }
    }
}