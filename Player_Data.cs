using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObject/PlayerData")]

public class Player_Data : ScriptableObject
{
    public PlayerData Data;
    [System.Serializable]
    public class PlayerData
    {
        public string PlayerName;
        public float HitPoint;
        public float Speed;
        public float MagicPoint;
        public float MagicSkillLevel;
        public float MagicDefense;
        public float INT;
        public float Attack;
        public float Defense;
    }
}
