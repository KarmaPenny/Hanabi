﻿#if !NCRUNCH
using System.Collections.Generic;
using System.Linq;
using Dissonance.Config;
using UnityEditor;
using UnityEngine;

namespace Dissonance.Editor
{
    [CustomEditor(typeof (ChatRoomSettings))]
    public class ChatRoomSettingsEditor : UnityEditor.Editor
    {
        private readonly List<int> _roomsToRemove = new List<int>();

        private Texture2D _logo;

        public void Awake()
        {
            _logo = Resources.Load<Texture2D>("dissonance_logo");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label(_logo);

            var settings = (ChatRoomSettings)target;
            for (var i = 0; i < settings.Names.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                settings.Names[i] = EditorGUILayout.TextField((i+1).ToString(), settings.Names[i]);
                if (GUILayout.Button("Delete", GUILayout.MaxWidth(50)))
                    _roomsToRemove.Add(i);

                EditorGUILayout.EndHorizontal();
            }

            _roomsToRemove.Reverse();
            foreach (var room in _roomsToRemove)
                settings.Names.RemoveAt(room);

            _roomsToRemove.Clear();

            if (GUILayout.Button("Add Room"))
                settings.Names.Add("New Room");

            if (GUI.changed)
                EditorUtility.SetDirty(settings);

            var duplicates = settings
                .Names
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();

            if (duplicates.Any())
            {
                const string message = "Duplicate room names found:\n{0}\n\nPlease choose different channel names.";

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(string.Format(message, string.Join(", ", duplicates)), MessageType.Error);
            }

            var collisions = settings
                .Names
                .Where(n => !duplicates.Contains(n))
                .Select(n => new {Name = n, Id = n.ToRoomId()})
                .GroupBy(x => x.Id)
                .Where(g => g.Count() > 1)
                .ToList();

            if (collisions.Any())
            {
                var collisionLists = collisions.Select(g => "{ " + string.Join(", ", g.Select(r => r.Name).ToArray()) + " }").ToArray();
                var message = "Channel ID hash collisions between the following channels:\n{0}\n\nPlease choose different channel names.";

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(string.Format(message, string.Join("\n", collisionLists)), MessageType.Error);
            }
        }

        #region static helpers
        [MenuItem("Window/Dissonance/Room Settings"), UsedImplicitly]
        private static void Show()
        {
            GoToSettings();
        }

        public static void GoToSettings()
        {
            var roomSettings = LoadRoomSettings();
            EditorApplication.delayCall += () =>
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = roomSettings;
            };
        }

        private static ChatRoomSettings LoadRoomSettings()
        {
            var asset = AssetDatabase.LoadAssetAtPath<ChatRoomSettings>(ChatRoomSettings.SettingsFilePath);
            if (asset == null)
            {
                asset = CreateInstance<ChatRoomSettings>();
                AssetDatabase.CreateAsset(asset, ChatRoomSettings.SettingsFilePath);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
        #endregion
    }
}
#endif