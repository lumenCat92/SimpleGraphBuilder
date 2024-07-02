using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

namespace LumenCat92.SimpleGraphBuilder
{
    [CreateAssetMenu(fileName = "NodeGraph", menuName = "NodeGraph/NodeGraph")]
    public class NodeGraph : ScriptableObject
    {
        public Node root;
        public List<Node> nodes = new List<Node>();
        public Dictionary<string, Color> registedConnectionTypes = new Dictionary<string, Color>();
        public bool shouldMatchConnectedPortName = true;
        [HideInInspector] public NodeConnection nodeConnection;
        public GameObject targetObject;
        public Node CreatRootNode(System.Type type, Vector2 position)
        {
            var node = CreateInstance(type) as Node;
            node.name = "Root";
            root = node;
            return SetNodeDefault(node, position);
        }
        public Node CreatNode(System.Type type, Vector2 position)
        {
            var node = CreateInstance(type) as Node;
            node.name = type.Name + nodes.Count;
            return SetNodeDefault(node, position);
        }
        private Node SetNodeDefault(Node node, Vector2 position)
        {
            node.guid = GUID.Generate().ToString();
            node.SetSyncPortsConnection(nodeConnection.ConnectionTypes());
            node.position = position;

            // Do Not Change This Recording Sequence
            AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "adding node to Tree");
            AssetDatabase.SaveAssets();

            Undo.RecordObject(this, "creating node");
            nodes.Add(node);
            return node;
        }
        public bool CheckConnectionType()
        {
            var connectionList = nodeConnection?.GetConnectionTypeWithColorList();
            if (connectionList == null)
            {
                Debug.LogError("ConnectionType dosent exist.");
                return false;
            }

            foreach (var key in registedConnectionTypes.Keys)
            {
                if (!connectionList.ContainsKey(key))
                {
                    Debug.LogError("Some of registered connectionType do not support in new ConnectionType. its Possible that connectionType might change on Script in new ConnectionType when New Connection was placed in Tree ScriptObject. plz Reconnect ConnectionType first");
                    return false;
                }
            }

            return true;
        }
        public void SetSyncConnectionTypeToAllNode()
        {
            if (nodeConnection == null)
            {
                registedConnectionTypes.Clear();
            }
            else
            {
                registedConnectionTypes = nodeConnection?.GetConnectionTypeWithColorList();
                nodes.ForEach(x => x.SetSyncPortsConnection(nodeConnection.ConnectionTypes()));
            }
        }
        public void DeleteNode(Node node)
        {
            // Do Not Change This Recording Sequence
            Undo.RecordObject(this, "deleting node");
            nodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddExit(Node parent, Node child, string inputPortName, string outputPortName)
        {
            // Do Not Change This Recording Sequence
            Undo.RecordObjects(new UnityEngine.Object[2] { parent, child }, "Adding Record");
            parent.AddConnectionNode(inputPortName, outputPortName, child, false);
            child.AddConnectionNode(inputPortName, outputPortName, parent, true);
            EditorUtility.SetDirty(parent);
            EditorUtility.SetDirty(child);
            EditorUtility.SetDirty(this);
        }
        public void RemoveExit(Node parent, Node child, string inputPortName, string outputPortName)
        {
            // Do Not Change This Recording Sequence
            Undo.RecordObjects(new UnityEngine.Object[2] { parent, child }, "Removing Record");
            parent.RemoveConnectionNode(inputPortName, outputPortName, child, false);
            child.RemoveConnectionNode(inputPortName, outputPortName, parent, true);
            EditorUtility.SetDirty(parent);
            EditorUtility.SetDirty(child);
            EditorUtility.SetDirty(this);
        }

        public Node GetNode(UnityEngine.Object connectedObj)
        {
            var obj = nodes.Find(x => x != root && x.ConnectedObj.Equals(connectedObj));
            if (obj != null) return obj;

            return null;
        }
        public void FindPathWithMixConnectionType(Node currentNode, List<string> findConnectionType, ref List<List<Node>> pathNodes, ref List<Node> path, ref bool isPathDone, bool shouldDoneWithFirstFinding)
        {
            path.Insert(0, currentNode);

            if (currentNode == root)
            {
                pathNodes.Add(path.ToList());
                isPathDone = true;
                return;
            }

            var connectionPairs = currentNode.GetConnectionPairs(true);
            foreach (var connectionPair in connectionPairs)
            {
                var parentNode = connectionPair.connectedNode;
                foreach (var connectionType in connectionPair.connectionTypes)
                {
                    foreach (var findType in findConnectionType)
                    {
                        if (connectionType == findType)
                        {
                            FindPathWithMixConnectionType(parentNode, findConnectionType, ref pathNodes, ref path, ref isPathDone, shouldDoneWithFirstFinding);
                            if (isPathDone)
                            {
                                if (shouldDoneWithFirstFinding)
                                {
                                    return;
                                }
                                else
                                {
                                    path.RemoveAt(0);
                                    isPathDone = false;
                                }
                            }
                        }
                    }
                }

                path.Remove(parentNode);
            }
        }
        public Dictionary<string, List<List<Node>>> FindPathWithEachConnectionType(Node currentNode, List<string> findConnectionType)
        {
            var eachAllPath = new Dictionary<string, List<List<Node>>>();
            var allPath = new List<List<Node>>();
            var path = new List<Node>();
            var didPathFind = false;
            findConnectionType.ForEach(x =>
            {
                FindPathWithMixConnectionType(currentNode, new List<string>() { x }, ref allPath, ref path, ref didPathFind, false);
                eachAllPath.Add(x, allPath.ToList());
                allPath.Clear();
                path.Clear();
                didPathFind = false;
            });

            return eachAllPath;
        }

        public class RecordingNode : MonoBehaviour
        {
            Node node;
            public RecordingNode(Node node)
            {
                this.node = node;
            }
        }
    }

    [CustomEditor(typeof(NodeGraph))]
    public class NodeTreeEditor : Editor
    {
        bool ShowConnectionListFromConnectionTypeObject = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var graph = target as NodeGraph;

            var getObject = EditorGUILayout.ObjectField("Set Connection", graph.nodeConnection, typeof(NodeConnection), true);
            if (getObject as NodeConnection == null)
            {
                graph.nodeConnection = null;
            }
            else
            {
                if (graph.nodeConnection != getObject as NodeConnection)
                {
                    string warningText = "If a Node has a ConnectionType that does not support the new ConnectionType, that connection will be lost.\nTo maintain safety, after you press the 'Confirm' button, the graph will be copied and saved with the same location but renamed as (Old).\nWill you proceed with this action?";
                    if (EditorUtility.DisplayDialog("Warning! U Can't UnDo This!", warningText, "Confirm", "Cancle"))
                    {
                        try
                        {
                            if (Selection.activeObject != null &&
                                Selection.activeObject is NodeGraph)
                            {
                                var selectedTree = Selection.activeObject as NodeGraph;
                                if (selectedTree.Equals(graph))
                                {
                                    var path = AssetDatabase.GetAssetPath(selectedTree);
                                    var pathPart = path.Split("/");
                                    if (pathPart.Length > 0)
                                    {
                                        var newTree = Instantiate(graph) as ScriptableObject;
                                        var lastWord = pathPart[pathPart.Length - 1];
                                        newTree.name = graph.name + "(Old)";
                                        lastWord = lastWord.Replace(graph.name, newTree.name);
                                        var newPath = "";
                                        for (int i = 0; i < pathPart.Length; i++)
                                        {
                                            if (i == pathPart.Length - 1)
                                            {
                                                newPath += lastWord;
                                                break;
                                            }
                                            newPath += pathPart[i] + "/";
                                        }
                                        AssetDatabase.CreateAsset(newTree, newPath);
                                        AssetDatabase.SaveAssets();

                                        graph.nodeConnection = getObject as NodeConnection;
                                        graph.SetSyncConnectionTypeToAllNode();
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Debug.Log("something wrong with processing");
                        }
                    }
                }
            }

            if (graph.nodeConnection != null)
            {
                ShowConnectionListFromConnectionTypeObject = EditorGUILayout.Toggle(nameof(ShowConnectionListFromConnectionTypeObject), ShowConnectionListFromConnectionTypeObject);
                if (ShowConnectionListFromConnectionTypeObject)
                {
                    var dic = graph.nodeConnection.GetConnectionTypeWithColorList();
                    foreach (var key in dic.Keys)
                    {
                        EditorGUILayout.LabelField(key);
                        EditorGUILayout.ColorField(dic[key]);
                    }
                }
            }

            // if (GUILayout.Button("Find Each Connection Path"))
            // {
            //     if (graph.nodeConnection is TestConnectionType)
            //     {
            //         var typeList = new List<string>() { TestConnectionType.ConnectionTypeList.Start.ToString(), TestConnectionType.ConnectionTypeList.Done.ToString() };
            //         var startNode = graph.GetNode(graph.targetObject);
            //         var allPath = new List<List<Node>>();
            //         var path = new List<Node>();
            //         var finded = graph.FindPathWithEachConnectionType(startNode, typeList);
            //         var index = 1;

            //         foreach (var key in finded.Keys)
            //         {
            //             Debug.Log("*find " + finded[key].Count + " path of " + key);
            //             finded[key].ForEach(x =>
            //             {
            //                 Debug.Log("path " + index++);
            //                 var path = "";
            //                 x.ForEach(j => path += j.ConnectedObj == null ? "=root-" : j.ConnectedObj.name + "-");
            //                 Debug.Log(path);
            //             });
            //             index = 1;
            //         }
            //     }
            // }
        }
    }
}