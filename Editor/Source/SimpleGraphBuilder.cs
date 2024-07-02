using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using UnityEditorInternal;
using Unity.VisualScripting;

namespace LumenCat92.SimpleGraphBuilder
{
    public class SimpleGraphBuilder : EditorWindow
    {
        SimpleGraphBuilderView GraphBuilderView;
        public VisualTreeAsset trbuilderUxml;
        public StyleSheet trbuilderUss;
        public VisualTreeAsset nodeUxml;

        [MenuItem("Window/UI Toolkit/GraphBuilder")]
        static void ShowExample()
        {
            SimpleGraphBuilder wnd = GetWindow<SimpleGraphBuilder>();
            wnd.titleContent = new GUIContent("GraphBuilder");
        }

        void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            trbuilderUxml.CloneTree(root);
            root.styleSheets.Add(trbuilderUss);

            var objectField = rootVisualElement.Query<ObjectField>("NodeTree").First();
            objectField.RegisterValueChangedCallback(evt => { ReadNodeTree(evt); });

            GraphBuilderView = root.Q<SimpleGraphBuilderView>();
            GraphBuilderView.styleSheets.Add(trbuilderUss);
            GraphBuilderView.nodeUxml = nodeUxml;
            GraphBuilderView.OnNodeSelected = OnNodeSelectionChanged;
            Undo.undoRedoPerformed += () => GraphBuilderView.Refreshing();
        }

        void ReadNodeTree(ChangeEvent<UnityEngine.Object> obj)
        {
            var graph = obj.newValue as NodeGraph;
            if (graph)
            {
                GraphBuilderView.PopulateView(graph);
            }
            else
            {
                GraphBuilderView.DrawingWithEmpty();
            }
        }

        void OnNodeSelectionChanged(NodeView nodeView)
        {
            if (nodeView == null) return;
            if (nodeView.node == null) return;
            EditorGUIUtility.PingObject(nodeView.node);
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= () => GraphBuilderView.Refreshing();
        }
    }
}