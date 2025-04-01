using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

public class UIStatsDisplay : MonoBehaviour
{
    public PlayerStat player;
    public CharacterData character;
    public bool updateInEditor = false;
    public bool displayCurrentHealth = false;
    TextMeshProUGUI _statNames, _statsValues;

    private void OnEnable()
    {
        UpdateStatFields();
    }

    private void OnDrawGizmosSelected()
    {
        if (updateInEditor) UpdateStatFields();
    }

    public CharacterData.Stats GetDisplayedStats()
    {
        if (player) return player.Stats;
        else if (character) return character.stats;
        return new CharacterData.Stats();
    }

    public void UpdateStatFields()
    {
        if (!player) return;

        if (!_statNames) _statNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!_statsValues) _statsValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        if (displayCurrentHealth)
        {
            names.AppendLine("Heath");
            values.AppendLine(player.CurrentHealth.ToString());
        }

        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            names.AppendLine(field.Name);

            object val = field.GetValue(player.Stats);
            float fval = val is int ? (int)val : (float)val;

            PropertyAttribute atribute = (PropertyAttribute)PropertyAttribute.GetCustomAttribute(field, typeof(PropertyAttribute));

            if (atribute != null && field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fval * 100 - 100);

                if (Mathf.Approximately(percentage, 0))
                {
                    values.Append('-').Append('\n');
                }
                else
                {
                    if (percentage > 0)
                        values.Append('+');
                    else
                        values.Append('-');
                    values.Append(percentage).Append('%').Append('\n');
                }
            }
            else
            {
                values.Append(fval).Append('\n');
            }

            _statNames.text = PrettifyNames(names);
            _statsValues.text = values.ToString();
        }
    }

    public static string PrettifyNames(StringBuilder input)
    {
        if (input.Length <= 0) return string.Empty;

        StringBuilder result = new StringBuilder();
        char last = '\0';
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (last == '\0' || char.IsWhiteSpace(last))
            {
                c = char.ToUpper(c);
            }
            else if (char.IsUpper(c))
            {
                result.Append(c);
            }
            result.Append(c);

            last = c;
        }

        return result.ToString();

    }

    private void Reset()
    {
        player = FindObjectOfType<PlayerStat>();
    }

}





