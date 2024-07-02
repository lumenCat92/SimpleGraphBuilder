using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using PlasticGui.WorkspaceWindow;
namespace LumenCat92.SimpleGraphBuilder
{
    public class Node : ScriptableObject
    {
        public UnityEngine.Object ConnectedObj;
        public List<ConnectionPair> enterConnectionPairs = new List<ConnectionPair>();
        public List<ConnectionPair> exitConnectionPairs = new List<ConnectionPair>();
        public List<ConnectionPair> GetConnectionPairs(bool isEnter) => isEnter ? enterConnectionPairs : exitConnectionPairs;
        public string guid;
        public Vector2 position;
        public bool CanGetConnectedObj<T>(out T connectedObj) where T : UnityEngine.Object
        {
            connectedObj = default;
            var isSameType = ConnectedObj is T;
            if (isSameType)
                connectedObj = ConnectedObj as T;

            return isSameType;
        }
        public void SetConnection(UnityEngine.Object targetObj)
        {
            ConnectedObj = targetObj;
        }
        public void SetSyncPortsConnection(List<string> newConnectionTypeList)
        {
            for (int i = 0; i < 2; i++)
            {
                var targetConnectionPairs = GetConnectionPairs(i == 0);
                var removeList = new List<string>();
                var addList = new List<string>();
                if (targetConnectionPairs == null)
                    targetConnectionPairs = new List<ConnectionPair>();
                foreach (var connectionPair in targetConnectionPairs)
                {
                    var targetConnectionTypeList = connectionPair.connectionTypes;
                    foreach (var connectionTypeWithNode in targetConnectionTypeList)
                    {
                        if (!newConnectionTypeList.Contains(connectionTypeWithNode))
                        {
                            removeList.Add(connectionTypeWithNode);
                        }
                    }
                    removeList.ForEach(x => targetConnectionTypeList.Remove(x));
                    removeList.Clear();
                }
            }
        }

        public void AddConnectionNode(string inputPortName, string outPutPortName, Node targetNode, bool isEnter)
        {
            var targetList = isEnter ? enterConnectionPairs : exitConnectionPairs;
            var portName = isEnter ? inputPortName : outPutPortName;
            var findPair = targetList.Find(x => x.connectedNode == targetNode);
            if (findPair == null)
            {
                targetList.Add(new ConnectionPair(targetNode, new List<string>() { portName }));
            }
            else
            {
                if (findPair.connectionTypes.Contains(portName))
                {
                    Debug.LogError("port already exist");
                }
                else
                {
                    findPair.connectionTypes.Add(portName);
                }
            }
        }

        public void RemoveConnectionNode(string inputPortName, string outPutPortName, Node targetNode, bool isEnter)
        {
            var targetList = isEnter ? enterConnectionPairs : exitConnectionPairs;
            var portName = isEnter ? inputPortName : outPutPortName;
            var findPair = targetList.Find(x => x.connectedNode == targetNode);
            if (findPair == null)
            {
                Debug.Log("The targetNode : " + targetNode.name + "wanst connected with this Node : " + this.name);
            }
            else
            {
                if (findPair.connectionTypes.Contains(portName))
                {
                    findPair.connectionTypes.Remove(portName);
                    if (findPair.connectionTypes.Count == 0)
                    {
                        targetList.Remove(findPair);
                    }
                }
                else
                {
                    Debug.Log("The targetNode : " + targetNode.name + "wanst connected with port : " + portName);
                }
            }
        }
        [Serializable]
        public class ConnectionPair
        {
            public Node connectedNode;
            public List<string> connectionTypes;

            public ConnectionPair(Node targetNode, List<string> connectionTypeList)
            {
                connectedNode = targetNode;
                connectionTypes = connectionTypeList;
            }
        }
    }

}