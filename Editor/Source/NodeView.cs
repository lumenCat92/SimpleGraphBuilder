using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Search;
using System.Reflection;
namespace LumenCat92.SimpleGraphBuilder
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Node node;
        public Dictionary<string, PortsPair> ports = new Dictionary<string, PortsPair>();
        public Action<NodeView> OnNodeSelected;
        public UnityEngine.Object connectedObj;
        bool isSelected = false;
        Action<Node, UnityEngine.Object> onChangeConnectedVal { set; get; }
        public NodeView(string path, Node node, List<string> connectionType, Action<Node, UnityEngine.Object> onChangeConnectedVal, bool isRoot = false) : base(path)
        {
            this.onChangeConnectedVal = onChangeConnectedVal;
            var types = typeof(NodeView);

            this.node = node;
            this.title = isRoot ? "Root" : "Node";
            this.viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            node.SetSyncPortsConnection(connectionType);
            if (!isRoot)
                CreateInputPorts(connectionType);
            CreateOutputPorts(connectionType);

            this.RegisterCallback<MouseOverEvent>(OnMouseOver);
            this.RegisterCallback<MouseOutEvent>(OnMouseOut);

            var border = this.Q<VisualElement>("node-border");
            var field = border.Q<ObjectField>("ScriptableObjInput");

            connectedObj = node.ConnectedObj;
        }

        public void SetConnectionObj(ChangeEvent<UnityEngine.Object> evt)
        {
            connectedObj = evt.newValue;
            onChangeConnectedVal?.Invoke(node, evt.newValue);
        }

        private void OnMouseOut(MouseOutEvent evt)
        {
            if (!isSelected)
                SetSelectedColor(false);
        }

        private void OnMouseOver(MouseOverEvent evt)
        {
            SetSelectedColor(true);
        }

        private void CreateOutputPorts(List<string> connectionTypeList)
        {
            foreach (var connectionType in connectionTypeList)
            {
                if (!ports.ContainsKey(connectionType)) ports.Add(connectionType, new PortsPair());
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                if (port != null)
                {
                    port.portName = connectionType;
                    outputContainer.Add(port);
                    ports[connectionType].output = port;
                }
            }
        }

        private void CreateInputPorts(List<string> connectionTypeList)
        {
            foreach (var connectionType in connectionTypeList)
            {
                if (!ports.ContainsKey(connectionType)) ports.Add(connectionType, new PortsPair());
                var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
                if (port != null)
                {
                    port.portName = connectionType;
                    inputContainer.Add(port);
                    ports[connectionType].input = port;
                }
            }
        }

        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(node, "Moving NodeView Position");
            base.SetPosition(newPos);
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
        }

        public override void OnSelected()
        {
            base.OnSelected();

            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }

            isSelected = true;
            SetSelectedColor(true);
        }
        public override void OnUnselected()
        {
            base.OnUnselected();
            isSelected = false;
            SetSelectedColor(false);
        }

        void SetSelectedColor(bool isSelected)
        {
            var hexColor = isSelected ? "00C9FF" : "383838";
            this.style.borderTopColor = ColorConveter.HexToColor(hexColor);
            this.style.borderBottomColor = ColorConveter.HexToColor(hexColor);
            this.style.borderLeftColor = ColorConveter.HexToColor(hexColor);
            this.style.borderRightColor = ColorConveter.HexToColor(hexColor);
        }

        internal void SetClickedObjectField(MouseDownEvent evt)
        {
            if (connectedObj == null)
                Selection.activeObject = null;

            else
                Selection.activeObject = connectedObj;
        }

        [Serializable]
        public class PortsPair
        {
            public Port input;
            public Port output;
        }
    }
}