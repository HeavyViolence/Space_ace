using SpaceAce.Main.ObjectPooling;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpaceAce.Editors
{
    public sealed class ObjectPoolEntryLookupTableEditor : EditorWindow
    {
        private const float ViewSelectedTableButtonWidth = 150f;
        private const float RemoveButtonWidth = 80f;

        private const float EntryNumberFieldWidth = 42f;
        private const float EntryIDFieldWidth = 200f;
        private const float SearchEntryByNameOutputFieldWidth = 360f;

        private const string SearchEntryByNameDefaultInput = "Enter entry anchor name to search here (case insensitive)";
        private const string SearchEntryByNameDefaultOutput = "Search output will show up here";

        private ObjectPoolEntryLookupTable _selectedTable = null;
        private string _selectedTableName = string.Empty;
        private IEnumerable<KeyValuePair<string, ObjectPoolEntry>> _selectedTableContents = null;

        private ObjectPoolEntry _entryToRemove = null;
        private bool _entryToRemoveRemoved = true;
        private readonly Stack<Action> _undoActions = new();

        private Vector2 _scrollPosition;
        private string _searchedEntryByNameInput = SearchEntryByNameDefaultInput;
        private string _searchEntryByNameOutput = SearchEntryByNameDefaultOutput;
        private bool _safetyHandleActive = true;

        private string SafetyHandleLabel => _safetyHandleActive ? "Safety handle (active)" : "Safety handle (inactive)";

        [MenuItem("Window/Custom editors/Object pool entry lookup table editor")]
        private static void OpenWindow()
        {
            GetWindow<ObjectPoolEntryLookupTableEditor>("Object pool entry lookup table editor");
        }

        private void OnGUI()
        {
            DisplayStatus();

            if (_selectedTable is null)
            {
                if (GUILayout.Button("View selected table", GUILayout.Width(ViewSelectedTableButtonWidth)))
                {
                    AssignSelectedTable();
                }
            }
            else
            {
                DisplayControlButtons();
                DisplaySearchByName();
                DisplaySelectedTableContents();
            }
        }

        private void DisplayStatus()
        {
            if (_selectedTable is null)
            {
                EditorGUILayout.LabelField("Table is not selected!");
            }
            else
            {
                EditorGUILayout.LabelField($"Vieved table: {_selectedTableName}.");
            }

            EditorGUILayout.Separator();
        }

        private void AssignSelectedTable()
        {
            if (Selection.activeObject is ObjectPoolEntryLookupTable value)
            {
                _selectedTable = value;
                _selectedTableName = Selection.activeObject.name;
                _selectedTableContents = value.GetContents();
            }
        }

        private void DisplaySearchByName()
        {
            if (_selectedTable is not null && _selectedTable.EntriesAmount > 0)
            {
                EditorGUILayout.BeginHorizontal();

                _searchedEntryByNameInput = EditorGUILayout.TextField(_searchedEntryByNameInput);
                EditorGUILayout.TextField(_searchEntryByNameOutput, GUILayout.Width(SearchEntryByNameOutputFieldWidth));

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DisplayControlButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (_selectedTable.EntriesAmount > 0 && GUILayout.Button(SafetyHandleLabel))
            {
                _safetyHandleActive = !_safetyHandleActive;
            }

            if (GUILayout.Button("Save and deselect"))
            {
                DeselectTable();
            }

            if (GUILayout.Button("Add entries"))
            {
                AddEntries();
            }

            if (_selectedTable is not null && _selectedTable.EntriesAmount > 0)
            {
                if (GUILayout.Button("Search by name"))
                {
                    _searchEntryByNameOutput = SearchEntryByName(_searchedEntryByNameInput);
                }

                if (GUILayout.Button("Clear search"))
                {
                    _searchedEntryByNameInput = SearchEntryByNameDefaultInput;
                    _searchEntryByNameOutput = SearchEntryByNameDefaultOutput;
                }

                if (_safetyHandleActive == false && GUILayout.Button("Clear table"))
                {
                    ClearTable();
                }
            }

            if (_undoActions.Count > 0)
            {
                if (GUILayout.Button($"Undo ({_undoActions.Count})"))
                {
                    Undo();
                }

                if (GUILayout.Button("Undo all"))
                {
                    UndoAll();
                }

                if (GUILayout.Button("Clear undo"))
                {
                    ClearUndo();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DisplaySelectedTableContents()
        {
            if (_selectedTable is not null)
            {
                if (_selectedTable.EntriesAmount == 0)
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Contents: empty.");
                    EditorGUILayout.Separator();
                }
                else
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField($"Contents ({_selectedTable.EntriesAmount} entries):");
                    EditorGUILayout.Separator();

                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                    int index = 1;

                    foreach (var entry in _selectedTableContents)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.TextField(index++.ToString(), GUILayout.Width(EntryNumberFieldWidth));
                        EditorGUILayout.TextField(entry.Key, GUILayout.Width(EntryIDFieldWidth));
                        EditorGUILayout.TextField(entry.Value.AnchorName);

                        if (_safetyHandleActive == false &&
                            GUILayout.Button("Remove", GUILayout.Width(RemoveButtonWidth)))
                        {
                            _entryToRemoveRemoved = false;
                            _entryToRemove = entry.Value;

                            _undoActions.Push(() => _selectedTable.AddSpecificEntry(entry.Key, entry.Value));
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    if (_entryToRemoveRemoved == false)
                    {
                        _selectedTable.RemoveEntry(_entryToRemove);
                        _entryToRemoveRemoved = true;
                        _selectedTableContents = _selectedTable.GetContents();
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
        }

        private void DeselectTable()
        {
            _selectedTable = null;
            _selectedTableName = string.Empty;
            _selectedTableContents = null;

            _searchedEntryByNameInput = SearchEntryByNameDefaultInput;
            _searchEntryByNameOutput = SearchEntryByNameDefaultOutput;

            _safetyHandleActive = true;

            ClearUndo();
        }

        private void AddEntries()
        {
            var selectedObjects = Selection.objects;
            List<ObjectPoolEntry> selectedEntities = new(selectedObjects.Length);

            foreach (var selectedObject in selectedObjects)
            {
                if (selectedObject is ObjectPoolEntry value)
                {
                    var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(value);

                    if (prefab is not null)
                    {
                        selectedEntities.Add(prefab);

                        _undoActions.Push(() => _selectedTable.RemoveEntry(prefab));
                    }
                }
            }

            _selectedTable.AddEntries(selectedEntities);
            _selectedTableContents = _selectedTable.GetContents();
        }

        private void ClearTable()
        {
            foreach (var entry in _selectedTableContents)
            {
                _undoActions.Push(() => _selectedTable.AddSpecificEntry(entry.Key, entry.Value));
            }

            _selectedTable.Clear();
            _selectedTableContents = null;
        }

        private string SearchEntryByName(string name)
        {
            int index = 0;

            foreach (var entry in _selectedTableContents)
            {
                index++;

                if (entry.Value.AnchorName.ToLower() == name.ToLower())
                {
                    return $"#{index}, ID = {entry.Key}";
                }
            }

            return "Entry not found";
        }

        private void Undo()
        {
            _undoActions.Pop()();
            _selectedTableContents = _selectedTable.GetContents();
        }

        private void UndoAll()
        {
            while (_undoActions.Count > 0)
            {
                _undoActions.Pop()();
            }

            _selectedTableContents = _selectedTable.GetContents();
        }

        private void ClearUndo() => _undoActions.Clear();
    }
}