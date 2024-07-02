using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
namespace LumenCat92.SimpleGraphBuilder
{
    //using this sentance to child of this ScriptableObject 
    //[CreateAssetMenu(fileName = "[Class Name]", menuName = "NodeTree/[Class Name]]")]
    public abstract class NodeConnection : ScriptableObject
    {
        [HideInInspector] public List<Color> colors = new List<Color>();
        public abstract List<String> ConnectionTypes();
        public Dictionary<string, Color> GetConnectionTypeWithColorList()
        {
            var dic = new Dictionary<string, Color>();
            var connectionTypeList = ConnectionTypes();
            for (int i = 0; i < connectionTypeList.Count; i++)
            {
                Color color;
                try
                {
                    color = colors[i];
                }
                catch
                {
                    colors.Add(Color.black);
                    color = colors[i];
                }
                dic.Add(connectionTypeList[i], color);
            }

            return dic;
        }

        protected T? GetEnumVal<T>(int index) where T : struct, Enum
        {
            if (Enum.IsDefined(typeof(T), index))
            {
                return (T)Enum.ToObject(typeof(T), index);
            }

            return null;
        }

        protected int GetEnumSize<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }

    [CustomEditor(typeof(NodeConnection))]
    public class NodeConnectionEditor : Editor
    {
        List<Color> colors = new List<Color>();
        NodeConnection nodeConnection { get { return target as NodeConnection; } }
        private void OnEnable()
        {
            if (nodeConnection != null)
            {
                colors.Clear();
                var dic = nodeConnection.GetConnectionTypeWithColorList();
                if (dic.Values.Count != 0)
                    colors.AddRange(dic.Values);
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (nodeConnection == null) return;
            var dic = nodeConnection.GetConnectionTypeWithColorList();
            if (dic.Keys.Count == 0)
            {
                EditorGUILayout.LabelField("list is empty");
            }
            else
            {
                int index = 0;
                foreach (var key in dic.Keys)
                {
                    var lastIndex = index;
                    var color = EditorGUILayout.ColorField(key, colors[index++]);
                    if (color != colors[lastIndex])
                    {
                        nodeConnection.colors[lastIndex] = color;
                        colors[lastIndex] = color;

                        EditorUtility.SetDirty(target);
                        //AssetDatabase.SaveAssets();
                    }
                }
            }
        }
    }
}