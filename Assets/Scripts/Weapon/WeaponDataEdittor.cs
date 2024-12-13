using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEdittor : Editor
{
    WeaponData _weaponData;
    string[] _weaponSubtypes;
    int _selectedWeaponSubtype;

    void OnEnable()
    {
        _weaponData = (WeaponData)target;

        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();

        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        _weaponSubtypes = subTypesString.ToArray();

        _selectedWeaponSubtype = Mathf.Max(0, Array.IndexOf(_weaponSubtypes, _weaponData.behavior));
    }

    public override void OnInspectorGUI()
    {
        _selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, _selectedWeaponSubtype), _weaponSubtypes);

        if (_selectedWeaponSubtype > 0)
        {
            _weaponData.behavior = _weaponSubtypes[_selectedWeaponSubtype].ToString();
            EditorUtility.SetDirty(_weaponData);
            DrawDefaultInspector();
        }
    }
}
