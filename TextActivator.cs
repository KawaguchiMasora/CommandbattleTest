using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextActivator : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI[] options;
    [SerializeField] public TextMeshProUGUI[] attackMethods;
    [SerializeField] public TextMeshProUGUI[] skillMethods;
    [SerializeField] public TextMeshProUGUI[] itemMethods;

    private void SetActiveState(TextMeshProUGUI[] texts, bool state)
    {
        Array.ForEach(texts, text => text.gameObject.SetActive(state));
    }

    public void ToggleOptions(bool active) => SetActiveState(options, active);
    public void ToggleAttackMethods(bool active) => SetActiveState(attackMethods, active);
    public void ToggleSkillMethods(bool active) => SetActiveState(skillMethods, active);
    public void ToggleItemMethods(bool active) => SetActiveState(itemMethods, active);
}
