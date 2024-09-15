using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System;
using static UnityEditor.MaterialProperty;

[CustomEditor(typeof(PieceData))]
[CanEditMultipleObjects]
[Serializable]
public class PieceDataEditor : Editor
{
    private PieceData data => target as PieceData;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.Space(10);

        ShowInputSliders();
        EditorGUILayout.Space(10);

        if(data.PieceMap != null && data.Columns > 0 && data.Rows > 0)
        {
            ShowMap();
        } 
        
        EditorGUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();
        
       

        if(GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }
    }


    private void ShowInputSliders()
    {
        int tmprows = data.Rows;
        int tmpcols = data.Columns;

        data.Rows = EditorGUILayout.IntSlider("Rows" , data.Rows, 1, 10);
        data.Columns = EditorGUILayout.IntSlider("Columns", data.Columns, 1, 10);

        if ((data.Rows != tmprows || data.Columns != tmpcols) && data.Rows > 0 && data.Columns > 0)
        {
            data.CreateNewMap();
        }

    }

    private void ShowMap()
    {
        for (int i = 0; i < data.Rows; i++)
        { 
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < data.Columns; j++)
            {
                var tmpdata = EditorGUILayout.ToggleLeft(".", data.PieceMap[i].Column[j], GUILayout.Width(10), GUILayout.Height(10));
                data.PieceMap[i].Column[j] = tmpdata;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}