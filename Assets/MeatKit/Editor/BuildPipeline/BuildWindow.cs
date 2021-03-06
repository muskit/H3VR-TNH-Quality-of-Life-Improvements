using System;
using UnityEditor;
using UnityEngine;

namespace MeatKit
{
    public class BuildWindow : EditorWindow
    {
        private static BuildProfile _selectedProfileInternal;
        private static BuildProfileEditor _editor;

        public static BuildProfile SelectedProfile
        {
            get
            {
                if (!_selectedProfileInternal)
                    _selectedProfileInternal = MeatKitCache.LastSelectedProfile;
                return _selectedProfileInternal;
            }
            private set
            {
                if (_selectedProfileInternal != value)
                    _editor = null;
                _selectedProfileInternal = value;
            }
        }

        private static BuildProfileEditor EditorInstance
        {
            get
            {
                if (!_editor && SelectedProfile)
                    _editor = (BuildProfileEditor) Editor.CreateEditor(SelectedProfile, typeof(BuildProfileEditor));
                return _editor;
            }
        }

        [MenuItem("MeatKit/Build Window")]
        public static void Open()
        {
            GetWindow<BuildWindow>("MeatKit Build").Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Selected Build Profile", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            SelectedProfile = EditorGUILayout.ObjectField(SelectedProfile, typeof(BuildProfile), false) as BuildProfile;
            if (EditorGUI.EndChangeCheck())
            {
                MeatKitCache.LastSelectedProfile = SelectedProfile;
            }

            if (!SelectedProfile)
            {
                GUILayout.Label("Please select a profile");
                return;
            }

            EditorGUILayout.Space();

            EditorInstance.OnInspectorGUI();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Build!", GUILayout.Height(50)))
                MeatKit.DoBuild();

            if (MeatKitCache.LastBuildTime != default(DateTime))
                GUILayout.Label("Last build: " + MeatKitCache.LastBuildTime + " (" +
                                MeatKitCache.LastBuildDuration.GetReadableTimespan() + ")");
            else GUILayout.Label("Last build: Never");
        }
    }
}