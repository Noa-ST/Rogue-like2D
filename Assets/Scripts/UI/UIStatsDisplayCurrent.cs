using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UIStatsDisplayCurrent : MonoBehaviour
{
    private PlayerStat player; // Tự tìm
    [SerializeField] private bool updateInEditor = false;
    [SerializeField] private bool displayCurrentHealth = false;

    [SerializeField] private TextMeshProUGUI statNames; // Tên chỉ số
    [SerializeField] private TextMeshProUGUI statValues; // Giá trị chỉ số

    private bool hasSearchedForPlayer = false;

    private void Awake()
    {
        FindPlayer();
        if (player == null)
        {
            Debug.LogWarning("No PlayerStat found in scene for " + gameObject.name + " during Awake. Will retry in Start.");
        }
    }

    private void Start()
    {
        if (player == null && !hasSearchedForPlayer)
        {
            FindPlayer();
            hasSearchedForPlayer = true;
            if (player == null)
            {
                Debug.LogWarning("PlayerStat still not found for " + gameObject.name + " after Start.");
            }
        }
    }

    private void Update()
    {
        if (player == null && !hasSearchedForPlayer)
        {
            FindPlayer();
            hasSearchedForPlayer = true;
            if (player == null)
            {
                Debug.LogWarning("PlayerStat not found for " + gameObject.name + " in Update. Search stopped.");
            }
        }
    }

    private void OnEnable()
    {
        InitializeTextComponents();
        UpdateStatFields();
        if (player != null)
        {
            player.OnStatsChanged += UpdateStatFields;
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.OnStatsChanged -= UpdateStatFields;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (updateInEditor) UpdateStatFields();
    }

    private void InitializeTextComponents()
    {
        if (statNames == null)
        {
            statNames = transform.GetChild(0)?.GetComponent<TextMeshProUGUI>();
            if (statNames == null)
            {
                Debug.LogError("Cannot find statNames (child 0) on " + gameObject.name);
            }
        }
        if (statValues == null)
        {
            statValues = transform.GetChild(1)?.GetComponent<TextMeshProUGUI>();
            if (statValues == null)
            {
                Debug.LogError("Cannot find statValues (child 1) on " + gameObject.name);
            }
        }
    }

    private void FindPlayer()
    {
        player = FindObjectOfType<PlayerStat>();
        if (player == null)
        {
            // Thử tìm theo tag "Player" nếu có
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<PlayerStat>();
            }
        }
    }

    private void UpdateStatFields()
    {
        InitializeTextComponents();

        if (player == null || statNames == null || statValues == null)
        {
            Debug.LogWarning("Player or stat display texts not assigned/found on " + gameObject.name);
            return;
        }

        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        if (displayCurrentHealth)
        {
            names.AppendLine("Health");
            values.AppendLine(player.CurrentHealth.ToString("F1"));
        }

        CharacterData.Stats stats = player.Actual;
        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            names.AppendLine(ObjectNames.NicifyVariableName(field.Name));
            object val = field.GetValue(stats);
            float fval = val is int ? (float)(int)val : (float)val;

            if (field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fval * 100f - 100f);
                if (Mathf.Approximately(percentage, 0f))
                {
                    values.AppendLine("-");
                }
                else
                {
                    values.Append(percentage > 0 ? "+" : "-").Append(percentage).Append("%\n");
                }
            }
            else
            {
                values.AppendLine(fval.ToString("F1"));
            }
        }

        statNames.text = names.ToString();
        statValues.text = values.ToString();
    }

    private void Reset()
    {
        player = FindObjectOfType<PlayerStat>();
        if (statNames == null && transform.childCount > 0) statNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (statValues == null && transform.childCount > 1) statValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
}
