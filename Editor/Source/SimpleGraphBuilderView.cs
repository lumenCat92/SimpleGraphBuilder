using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
namespace LumenCat92.SimpleGraphBuilder
{
    [Serializable]
    public class SimpleGraphBuilderView : GraphView
    {
        public Action<NodeView> OnNodeSelected;
        public new class UxmlFactory : UxmlFactory<SimpleGraphBuilderView, GraphView.UxmlTraits> { }
        NodeGraph graph;
        public VisualTreeAsset nodeUxml;
        public SimpleGraphBuilderView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }
        public void DrawingWithEmpty()
        {
            graph = null;
            ReDrawingElement();
        }
        void ReDrawingElement()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
        }

        public void Refreshing()
        {
            if (graph != null)
            {
                PopulateView(graph);
            }
        }
        public void PopulateView(NodeGraph graph)
        {
            this.graph = graph;
            if (!graph.CheckConnectionType()) return;
            ReDrawingElement();
            graph.SetSyncConnectionTypeToAllNode();

            // creates node view
            graph.nodes.ForEach(n => { if (graph.root.Equals(n)) CreateRootNodeView(n); else CreateNodeView(n); });

            // creates edge
            graph.nodes.ForEach(parentNode =>
            {
                parentNode.exitConnectionPairs.ForEach(connectionPair =>
                {
                    connectionPair.connectionTypes.ForEach(connectionType =>
                    {
                        var parentView = GetNodeByGuid(parentNode.guid) as NodeView;
                        var childView = GetNodeByGuid(connectionPair.connectedNode.guid) as NodeView;

                        if (parentView != null && childView != null)
                        {
                            if (parentView.ports.ContainsKey(connectionType) &&
                                childView.ports.ContainsKey(connectionType))
                            {
                                var output = parentView.ports[connectionType].output;
                                var input = childView.ports[connectionType].input;
                                var edge = output.ConnectTo(input);
                                AddElement(edge);
                            }
                        }
                    });
                });
            });
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node &&
                endPort.portName == startPort.portName
                    ).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        graph.DeleteNode(nodeView.node);
                    }

                    var edge = elem as UnityEditor.Experimental.GraphView.Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;

                        var outPortName = edge.output.portName;
                        var inPortName = edge.input.portName;
                        graph.RemoveExit(parentView.node, childView.node, inPortName, outPortName);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    var inPortName = edge.input.portName;
                    var outPortName = edge.output.portName;
                    graph.AddExit(parentView.node, childView.node, inPortName, outPortName);
                });
            }
            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (graph == null) return;

            var type = typeof(Node);
            var mousePosition = contentViewContainer.WorldToLocal(evt.mousePosition);
            if (graph.nodes.Count == 0)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {"RootNode"}", (a) => CreateRootNode(type, mousePosition));
            }
            else
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, mousePosition));
            }
        }

        private void CreateRootNode(Type type, Vector2 position)
        {
            var node = graph.CreatRootNode(type, position);
            CreateRootNodeView(node);
        }
        private void CreateRootNodeView(Node node)
        {
            var path = AssetDatabase.GetAssetPath(nodeUxml);
            var nodeView = new NodeView(path, node, graph.nodeConnection.ConnectionTypes(), AddNewConnectedObj, true);
            nodeView.OnNodeSelected = OnNodeSelected;
            SetObjectFieldToNodeView(nodeView, true);
            SetPortColor(nodeView);
            AddElement(nodeView);
        }

        private void CreateNode(Type type, Vector2 position)
        {
            var node = graph.CreatNode(type, position);
            CreateNodeView(node);
        }

        private void AddNewConnectedObj(Node node, UnityEngine.Object targetObj)
        {
            Undo.RecordObject(node, "node connect obj change");
            node.ConnectedObj = targetObj;
        }

        private void SetPortColor(NodeView nodeView)
        {
            if (graph.nodeConnection != null)
            {
                foreach (var key in nodeView.ports.Keys)
                {
                    if (nodeView.ports[key].input != null)
                        nodeView.ports[key].input.portColor = graph.registedConnectionTypes[key];

                    if (nodeView.ports[key].output != null)
                        nodeView.ports[key].output.portColor = graph.registedConnectionTypes[key];
                }
            }
        }

        private void CreateNodeView(Node node)
        {
            var path = AssetDatabase.GetAssetPath(nodeUxml);
            var nodeView = new NodeView(path, node, graph.nodeConnection.ConnectionTypes(), AddNewConnectedObj);
            nodeView.OnNodeSelected = OnNodeSelected;
            SetObjectFieldToNodeView(nodeView, false);
            SetPortColor(nodeView);
            AddElement(nodeView);
        }

        private void SetObjectFieldToNodeView(NodeView view, bool isRoot)
        {
            try
            {
                var border = view.Q<VisualElement>("node-border");
                var field = border.Q<ObjectField>("ScriptableObjInput");
                if (isRoot)
                {
                    field.visible = false;
                    var color = ColorConveter.HexToColor("40FD10");
                    if (graph.root.Equals(view.node))
                    {
                        var titleLabel = border.Q<Label>("title-label");
                        titleLabel.style.backgroundColor = color;
                        titleLabel.style.color = Color.black;
                    }
                }
                else
                {
                    field.RegisterValueChangedCallback(evt => view.SetConnectionObj(evt));
                    field.RegisterCallback<MouseDownEvent>(evt => view.SetClickedObjectField(evt));
                    field.value = view.node.ConnectedObj;
                    var color = ColorConveter.HexToColor("FF7F00");
                    var titleLabel = border.Q<Label>("title-label");
                    titleLabel.style.backgroundColor = color;
                    titleLabel.style.color = Color.black;
                }
            }
            catch
            {
                Debug.Log("something wrong");
            }
        }
    }
}