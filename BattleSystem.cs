using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] public Enemy_List enemylist;
    [SerializeField] public Player_Data playerdata;
    int RandomEnemy;
    bool PlayerDeath = true;
    bool EnemyDeath = true;
    bool PlayerTurn;
    bool EnemyTurn;

    private TextSelector textSelector;
    private TextActivator textActivator;

    public TextMeshProUGUI gameOver_text;
    public TextMeshProUGUI gameWin_text;
    public bool gameEnd;
    public float CheckPlayerHp;
    public bool IsPlayerUsingSkill;
    public List<int> turn_number = new List<int>();

    #region 敵の情報変数
    string enemyname;
    float enemyHP;
    float enemyMP;
    float enemySP;
    float enemyMSL;
    float enemyMDEF;
    float enemyINT;
    float enemyATK;
    float enemyDEF;
    #endregion

    #region プレイヤーの情報変数
    string playername;
    float playerHP;
    float playerMP;
    float playerSP;
    float playerMSL;
    float playerMDEF;
    float playerATK;
    float playerDEF;
    #endregion

    #region 必要なMPまたその魔法の攻撃力
    float Test = 20;
    float TestPower = 100;
    #endregion


    void Start()
    {
        int enemyCount = enemylist.Data.Count;
        RandomEnemy = Random.Range(0, enemyCount);
        EncounterEnemy();
        textSelector = GameObject.Find("GameSystem").GetComponent<TextSelector>();
        gameOver_text.gameObject.SetActive(false);
        gameWin_text.gameObject.SetActive(false);
        gameEnd = false;
        CheckPlayerHp = playerHP;

        #region プレイヤーの情報取得
        playername = playerdata.Data.PlayerName;
        Debug.Log("プレイヤー:" + playername);
        playerHP = playerdata.Data.HitPoint;
        playerMP = playerdata.Data.MagicPoint;
        playerSP = playerdata.Data.Speed;
        playerMSL = playerdata.Data.MagicSkillLevel;
        playerMDEF = playerdata.Data.MagicDefense;
        playerATK = playerdata.Data.Attack;
        playerDEF = playerdata.Data.Defense;
        Debug.Log("HP: " + playerHP + ", MP: " + playerMP + "MSL:" + playerMSL + ",MPDEF" + playerMDEF + ", Attack: " + playerATK + ", Defense: " + playerDEF);
        #endregion

        ProgressOfTheBattle();
        CommandBattleTrue();

    }

    private void Update()
    {
        GameManager();

        #region　敵からプレイヤーへ物理ダメージ
        if (Input.GetKeyDown(KeyCode.K) && playerHP >= 0 && !PlayerDeath)
        {
            EnemyPhysicalAttacks();
        }
        else if (playerHP <= 0 && PlayerDeath == true)
        {
            Debug.Log("プレイヤー死亡");
            PlayerDeath = false;
            GameOver();
        }
        #endregion

        #region プレイヤーから敵へ物理ダメージ
        if (Input.GetKeyDown(KeyCode.L) && enemyHP >= 0 && !EnemyDeath)
        {
            PlayerPhysicalAttacks();
            PlayerTurn = false;  // プレイヤーのターンを終了
            EnemyTurn = true;    // 敵のターンを開始
            Enemybrain();        // 敵の行動を処理
            CommandBattleTrue(); // ターンの進行を表示
        }
        else if (enemyHP <= 0 && EnemyDeath == true)
        {
            Debug.Log("敵死亡");
            EnemyDeath = false;
            GameWin();
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.R) && gameEnd)
        {
            ReloadScene();
        }
    }

    void ProgressOfTheBattle()
    {

        if (playerSP > enemySP)
        {
            Debug.Log("\n" + "Playerのスピード:" + playerSP + "\n" + "Enemyのスピード:" + enemySP);
            Debug.Log("プレイヤーが最初に行動をとります");
            PlayerTurn = true;
            EnemyTurn = false;
        }
        else if (playerSP == enemySP)
        {
            Debug.Log("\n" + "Playerのスピード:" + playerSP + "\n" + "Enemyのスピード:" + enemySP);
            Debug.Log("ランダムに最初の行動者を決めます");
            int which = Random.Range(0, 10);
            if (which >= 5)
            {
                Debug.Log("抽選の結果プレイヤーが最初に行動をとります");
                PlayerTurn = true;
                EnemyTurn = false;
            }
            else
            {
                Debug.Log("抽選の結果敵が最初に行動をとります");
                PlayerTurn = false;
                EnemyTurn = true;
            }
        }
        else
        {
            Debug.Log("\n" + "Playerのスピード:" + playerSP + "\n" + "Enemyのスピード:" + enemySP);
            Debug.Log("敵が最初に行動をとります");
            PlayerTurn = false;
            EnemyTurn = true;
        }
    }

    void EncounterEnemy()
    {
        #region 敵の情報取得
        enemyname = enemylist.Data[RandomEnemy].EnemyName;
        Debug.Log(enemyname + "が勝負を仕掛けてきた");
        enemyHP = enemylist.Data[RandomEnemy].HitPoint;
        enemySP = enemylist.Data[RandomEnemy].Speed;
        enemyATK = enemylist.Data[RandomEnemy].Attack;
        enemyDEF = enemylist.Data[RandomEnemy].Defense;

        #region　魔法に関する情報
        enemyMP = enemylist.Data[RandomEnemy].MagicPoint;
        enemyMSL = enemylist.Data[RandomEnemy].MagicSkillLevel;
        enemyMDEF = enemylist.Data[RandomEnemy].MagicDefense;
        enemyINT = enemylist.Data[RandomEnemy].INT;
        #endregion

        Debug.Log("HP: " + enemyHP + ", MP: " + enemyMP + "MSL:" + enemyMSL + "INT:" + enemyINT +
            ",MPDEF" + enemyMDEF + ", Attack: " + enemyATK + ", Defense: " + enemyDEF);
        #endregion
    }

    void EnemyPhysicalAttacks()
    {
        float Damage = playerDEF - enemyATK;
        if (IsPlayerUsingSkill)
        {
            float n = Damage;
            Damage = n * 0.8f;
            IsPlayerUsingSkill = false;
        }

        if (Damage <= 0)
        {
            playerHP += Damage;
            Debug.Log(playerHP);
        }
        else if (playerHP <= 0)
        {
            Debug.Log("プレイヤーは死んでいます");
        }
        else
        {
            Debug.Log("無傷");
            Debug.Log(Damage);
        }
    }

    void PlayerPhysicalAttacks()
    {
        float Damage = enemyDEF - playerATK;
        if (Damage <= 0)
        {
            enemyHP += Damage;
            Debug.Log(enemyHP);
            PlayerTurn = false;
            EnemyTurn = true;  // 敵のターンを開始
        }
        else if (enemyHP <= 0)
        {
            Debug.Log("敵は死んでいます");
        }
        else
        {
            Debug.Log("無傷");
            Debug.Log(Damage);
            PlayerTurn = false;
            EnemyTurn = true;  // 敵のターンを開始
        }
    }
    //今回これらない-----------------------------------------------------------------
    void EnemyMagicAttacks(float NeedMp, float MagicPower, float MagicSkillLevel)
    {
        float Damage = playerMDEF - MagicPower * MagicSkillLevel;
        Debug.Log("Magic Attack Damage: " + Damage);
    }
    //--------------------------------------------------------------------------------

    void CommandBattleTrue()
    {

        if (PlayerTurn)
        {
            Debug.Log("プレイヤーのターンです");
            turn_number.Add(1);
            // PlayerTurn = false;

        }

        if (EnemyTurn)
        {
            Debug.Log("エネミーのターンです");
            turn_number.Add(2);
            //EnemyTurn = false;
            //Enemybrain();
        }
    }

    public void GameOver()
    {
        gameOver_text.gameObject.SetActive(true);
        LogTurnHistory();
        Time.timeScale = 0f;
        gameEnd = true;
    }

    public void GameWin()
    {
        gameWin_text.gameObject.SetActive(true);
        LogTurnHistory();
        Time.timeScale = 0f; // ゲームの時間を停止
        gameEnd = true;
    }

    void ReloadScene()
    {
        gameEnd = false;
        Time.timeScale = 1f; // ゲームの時間を再開
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 現在のシーンを再ロード
    }


    //経過ターンを表示するだけ
    private void LogTurnHistory()
    {
        string logMessage = "経過ターン: ";
        foreach (int turn in turn_number)
        {
            if (turn == 1)
            {
                logMessage += "プレイヤー, ";
            }
            else if (turn == 2)
            {
                logMessage += "エネミー, ";
            }
        }
        logMessage = logMessage.TrimEnd(',', ' ');
        int totalTurns = turn_number.Count; // 合計ターン数
        logMessage += "\n合計ターン数: " + totalTurns;
        Debug.Log(logMessage);

    }

    public void PlayerAttack()
    {
        PlayerPhysicalAttacks();
        //無駄に関数を使用している?ので後々改善
    }
    public void PlayerSkill()
    {
        //敵が三回攻撃してくるといった実装が出来ていないので次の敵の攻撃を80%カットにする
        IsPlayerUsingSkill = true;
    }

    public void PlayerItem()
    {
        if (playerHP == CheckPlayerHp)
        {
            Debug.Log("プレイヤーのHPはMaxです");
        }
        else
        {
            playerHP += 20;
            if (playerHP >= CheckPlayerHp)
            {
                playerHP = CheckPlayerHp;
            }
            Debug.Log(playerHP);
        }

        textSelector.RemoveNextItem();
    }

    void Enemybrain()
    {
        float Damage = playerDEF - enemyATK;
        if (enemyHP / 2 >= enemyHP)
        {
            enemyHP += 20;
            Debug.Log("体力を回復しました");
            EnemyTurn = false;
        }
        else if (Damage <= 0)
        {
            playerHP += Damage;
            Debug.Log("ダメージを受けました");
            Debug.Log(playerHP);
            EnemyTurn = false;
        }
        else
        {
            Debug.Log("無傷です");
            EnemyTurn = false;
        }
    }


    public void GameManager()
    {
        if (PlayerTurn)
        {
            Debug.Log("プレイヤーのターンが進行中です");
            PlayerPhysicalAttacks();
        }
        if (EnemyTurn)
        {
            Debug.Log("敵のターンが進行中です");
            Enemybrain();
        }
        CommandBattleTrue();
    }

}
