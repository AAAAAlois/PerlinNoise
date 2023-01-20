using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor (typeof (MidpointCustomTerrian))]
//[CanEditMultipleObjects]
//public class MidpointCustomTerrainEditor : Editor
//{
//    SerializedProperty resetTerrain;
//    SerializedProperty MPDheightMin;
//    SerializedProperty MPDheightMax;
//    SerializedProperty MPDheightDampenerPower;
//    SerializedProperty MPDroughness;
//    bool showMPD = false;
//    private void OnEnable()
//    {
//        resetTerrain = serializedObject.FindProperty("resetTerrain");
//        MPDheightMin = serializedObject.FindProperty("MPDheightMin");
//        MPDheightMax = serializedObject.FindProperty("MPDheightMax");
//        MPDheightDampenerPower = serializedObject.FindProperty("MPDheightDampenerPower");
//        MPDroughness = serializedObject.FindProperty("MPDroughness");
//    }
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        MidpointCustomTerrian terrian = (MidpointCustomTerrian)target;
//        EditorGUILayout.PropertyField(resetTerrain);
//        #region 
//        showMPD = EditorGUILayout.Foldout(showMPD, "Midpoint Displacement");
        
//        if (showMPD)
//        {
//        EditorGUILayout.PropertyField(MPDheightMin);
//        EditorGUILayout.PropertyField(MPDheightMax);
//        EditorGUILayout.PropertyField(MPDheightDampenerPower);
//        EditorGUILayout.PropertyField(MPDroughness);
//            if(GUILayout .Button("MPD"))
//            {
//                terrian.MidPointDisplacement();
//            }
//        }
//        #endregion 
//        serializedObject.ApplyModifiedProperties();
//    }
//}
