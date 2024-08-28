using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="ScriptableObject/EnemyList")]

public class Enemy_List : ScriptableObject
{
    public List<EnemyList> Data;
    [System.Serializable]
    public class EnemyList
    {
        public string EnemyName;
        public float HitPoint;
        public float Speed;
        public float MagicPoint;
        public float MagicSkillLevel;
        public float MagicDefense;
        public float INT;
        public float Attack;
        public float Defense;

    }
    public int GetEnemyCount()
    {
        return Data.Count;
    }

}
