using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;

namespace TheKiwiCoder
{
    [UxmlElement] // Unity 6에서 UXML 요소로 등록
    public partial class InspectorView : VisualElement
    {

        Editor editor;

        public InspectorView()
        {

        }

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() => {
                if (editor && editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}
