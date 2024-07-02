using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace LumenCat92.SimpleGraphBuilder
{
    [CreateAssetMenu(fileName = nameof(TestConnectionType), menuName = "NodeGraph/ConnectionType/" + nameof(TestConnectionType))]
    public class TestConnectionType : NodeConnection
    {
        public enum ConnectionTypeList { Start, Done }
        public override List<string> ConnectionTypes()
        {
            var list = new List<string>();
            var enumSize = GetEnumSize<ConnectionTypeList>();
            for (int i = 0; i < enumSize; i++)
            {
                list.Add(GetEnumVal<ConnectionTypeList>(i).ToString());
            }

            return list;
        }
    }

    [CustomEditor(typeof(TestConnectionType))]
    public class TestConnectionTypeEditor : NodeConnectionEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}