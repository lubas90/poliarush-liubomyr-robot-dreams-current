using System.Collections;
using System.Collections.Generic;
using StateMachineSystem.Locomotion;
using UnityEditor;
using UnityEngine;

namespace StateMachineSystem.Editor
{
    [CustomEditor(typeof(LocomotionController))]
    public class LocomotionControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LocomotionController locomotionController = (LocomotionController)target;
            EditorGUILayout.LabelField("Current State:", locomotionController.CurrentState.ToString());
        }
    }
}