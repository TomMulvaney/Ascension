using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AscMove))]
public class LevelScriptEditor : Editor 
{
    public override void OnInspectorGUI() {
        AscMove myTarget = (AscMove)target;
        
        EditorGUILayout.LabelField ("Run Speed");
        myTarget.runSpeed = EditorGUILayout.Slider (myTarget.runSpeed, 0f, 30f);
        
        EditorGUILayout.LabelField ("Use Single Damping");
        myTarget.useSingleDamping = EditorGUILayout.Toggle (myTarget.useSingleDamping);
        
        if (myTarget.useSingleDamping) {
            EditorGUILayout.LabelField ("Damping");
            myTarget.groundDamping = EditorGUILayout.Slider (myTarget.groundDamping, 0f, 30f);
        } else {
            EditorGUILayout.LabelField ("Ground Damping");
            myTarget.groundDamping = EditorGUILayout.Slider (myTarget.groundDamping, 0f, 30f);

            EditorGUILayout.LabelField ("In-Air Damping");
            myTarget.inAirDamping = EditorGUILayout.Slider (myTarget.inAirDamping, 0f, 30f);
        }
        
        EditorGUILayout.LabelField ("Jump Height");
        myTarget.jumpHeight = EditorGUILayout.Slider (myTarget.jumpHeight, 0f, 10f);
        
        EditorGUILayout.LabelField ("Air Jumps");
        myTarget.maxAirJumps = EditorGUILayout.IntSlider (myTarget.maxAirJumps, 0, 5);
        
        EditorGUILayout.LabelField ("Hookshot Speed");
        myTarget.hookshotSpeed = EditorGUILayout.Slider (myTarget.hookshotSpeed, 0f, 50f);
        
        EditorGUILayout.LabelField ("Wall Slide Speed");
        myTarget.wallSlideSpeed = EditorGUILayout.Slider (myTarget.wallSlideSpeed, 0f, 10f);
//        
        EditorGUILayout.LabelField ("Gravity");
        myTarget.gravity = EditorGUILayout.Slider (myTarget.gravity, 0f, 50f);
    }
}