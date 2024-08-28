using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextSelector : MonoBehaviour
{
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int currentIndex = 0;

    private bool Attack;
    private bool Skill;
    private bool Item;

    private int optionsIndex = 0;
    private int attackMethodsIndex = 0;
    private int skillMethodsIndex = 0;
    private int itemMethodsIndex = 0;

    private bool prevAttack;
    private bool prevSkill;
    private bool prevItem;

    private BattleSystem battleSystem;
    private TextActivator textActivator;


    void Start()
    {
        textActivator = GameObject.FindObjectOfType<TextActivator>(); // TextActivatorのインスタンスを取得

        UpdateTextColors();

        battleSystem = GameObject.Find("GameSystem").GetComponent<BattleSystem>();

        // 初期状態としてオプションを表示
        textActivator.ToggleOptions(true);
        textActivator.ToggleAttackMethods(false);
        textActivator.ToggleSkillMethods(false);
        textActivator.ToggleItemMethods(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && !battleSystem.gameEnd)
        {
            MoveSelectionUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !battleSystem.gameEnd)
        {
            MoveSelectionDown();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && !battleSystem.gameEnd)
        {
            SelectOption();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !battleSystem.gameEnd)
        {
            // Escキーが押されたとき前の状態に戻す
            RevertToPreviousState();
        }

        if (battleSystem.gameEnd)
        {
            textActivator.ToggleOptions(false);
            textActivator.ToggleAttackMethods(false);
            textActivator.ToggleSkillMethods(false);
            textActivator.ToggleItemMethods(false);
        }
    }

    void MoveSelectionUp()
    {
        if (Attack)
        {
            attackMethodsIndex--;
            if (attackMethodsIndex < 0)
            {
                attackMethodsIndex = textActivator.attackMethods.Length - 1;
            }
        }
        else if (Skill)
        {
            skillMethodsIndex--;
            if (skillMethodsIndex < 0)
            {
                skillMethodsIndex = textActivator.skillMethods.Length - 1;
            }
        }
        else if (Item)
        {
            itemMethodsIndex--;
            if (itemMethodsIndex < 0)
            {
                itemMethodsIndex = textActivator.itemMethods.Length - 1;
            }
        }
        else
        {
            optionsIndex--;
            if (optionsIndex < 0)
            {
                optionsIndex = textActivator.options.Length - 1;
            }
        }
        UpdateTextColors();
    }

    void MoveSelectionDown()
    {
        if (Attack)
        {
            attackMethodsIndex++;
            if (attackMethodsIndex >= textActivator.attackMethods.Length)
            {
                attackMethodsIndex = 0;
            }
        }
        else if (Skill)
        {
            skillMethodsIndex++;
            if (skillMethodsIndex >= textActivator.skillMethods.Length)
            {
                skillMethodsIndex = 0;
            }
        }
        else if (Item)
        {
            itemMethodsIndex++;
            if (itemMethodsIndex >= textActivator.itemMethods.Length)
            {
                itemMethodsIndex = 0;
            }
        }
        else
        {
            optionsIndex++;
            if (optionsIndex >= textActivator.options.Length)
            {
                optionsIndex = 0;
            }
        }
        UpdateTextColors();
    }

    void UpdateTextColors()
    {
        if (Attack)
        {
            for (int i = 0; i < textActivator.attackMethods.Length; i++)
            {
                textActivator.attackMethods[i].color = (i == attackMethodsIndex) ? selectedColor : normalColor;
            }
        }
        else if (Skill)
        {
            for (int i = 0; i < textActivator.skillMethods.Length; i++)
            {
                textActivator.skillMethods[i].color = (i == skillMethodsIndex) ? selectedColor : normalColor;
            }
        }
        else if (Item)
        {
            for (int i = 0; i < textActivator.itemMethods.Length; i++)
            {
                textActivator.itemMethods[i].color = (i == itemMethodsIndex) ? selectedColor : normalColor;
            }
        }
        else
        {
            for (int i = 0; i < textActivator.options.Length; i++)
            {
                textActivator.options[i].color = (i == optionsIndex) ? selectedColor : normalColor;
            }
        }
    }

    void SelectOption()
    {
        //種類を増やすのが面倒だったのですべて同じ攻撃、スキル、回復量です
        if (Attack)
        {
            Debug.Log("Selected attack method: " + textActivator.attackMethods[attackMethodsIndex].text);
            battleSystem.PlayerAttack();
        }
        else if (Skill)
        {
            Debug.Log("Selected skill method: " + textActivator.skillMethods[skillMethodsIndex].text);
            battleSystem.PlayerSkill();
        }
        else if (Item)
        {
            Debug.Log("Selected item method: " + textActivator.itemMethods[itemMethodsIndex].text);
            battleSystem.PlayerItem();
        }
        else
        {
            Debug.Log("Selected option: " + textActivator.options[optionsIndex].text);

            if (textActivator.options[optionsIndex].text == "Skill")
            {
                SaveCurrentState();
                Skill = true;
                Attack = false;
                Item = false;
                UpdateTextVisibility();
            }
            else if (textActivator.options[optionsIndex].text == "Attack")
            {
                SaveCurrentState();
                Attack = true;
                Skill = false;
                Item = false;
                UpdateTextVisibility();
            }
            else if (textActivator.options[optionsIndex].text == "Item")
            {
                SaveCurrentState();
                Item = true;
                Skill = false;
                Attack = false;
                UpdateTextVisibility();
            }
        }
    }

    void UpdateTextVisibility()
    {
        textActivator.ToggleOptions(!Attack && !Skill && !Item);
        textActivator.ToggleAttackMethods(Attack);
        textActivator.ToggleSkillMethods(Skill);
        textActivator.ToggleItemMethods(Item);
    }

    void SaveCurrentState()
    {
        prevAttack = Attack;
        prevSkill = Skill;
        prevItem = Item;
    }

    void RevertToPreviousState()
    {
        Attack = prevAttack;
        Skill = prevSkill;
        Item = prevItem;
        UpdateTextVisibility();
    }

    //BattleSystemで呼び出す-------------------------------------------------------------------------
    public void RemoveNextItem()
    {
        // 配列のサイズを把握
        int length = textActivator.itemMethods.Length;

        // 配列からアイテムを削除するためにゲームオブジェクトを破棄し、nullを設定
        if (itemMethodsIndex >= 0 && itemMethodsIndex < length)
        {
            TextMeshProUGUI itemToRemove = textActivator.itemMethods[itemMethodsIndex];
            if (itemToRemove != null)
            {
                Destroy(itemToRemove.gameObject);
                textActivator.itemMethods[itemMethodsIndex] = null;
            }
        }
    }
    //----------------------------------------------------------------------------------------------
}

