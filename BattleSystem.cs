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

    #region �G�̏��ϐ�
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

    #region �v���C���[�̏��ϐ�
    string playername;
    float playerHP;
    float playerMP;
    float playerSP;
    float playerMSL;
    float playerMDEF;
    float playerATK;
    float playerDEF;
    #endregion

    #region �K�v��MP�܂����̖��@�̍U����
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

        #region �v���C���[�̏��擾
        playername = playerdata.Data.PlayerName;
        Debug.Log("�v���C���[:" + playername);
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

        #region�@�G����v���C���[�֕����_���[�W
        if (Input.GetKeyDown(KeyCode.K) && playerHP >= 0 && !PlayerDeath)
        {
            EnemyPhysicalAttacks();
        }
        else if (playerHP <= 0 && PlayerDeath == true)
        {
            Debug.Log("�v���C���[���S");
            PlayerDeath = false;
            GameOver();
        }
        #endregion

        #region �v���C���[����G�֕����_���[�W
        if (Input.GetKeyDown(KeyCode.L) && enemyHP >= 0 && !EnemyDeath)
        {
            PlayerPhysicalAttacks();
            PlayerTurn = false;  // �v���C���[�̃^�[�����I��
            EnemyTurn = true;    // �G�̃^�[�����J�n
            Enemybrain();        // �G�̍s��������
            CommandBattleTrue(); // �^�[���̐i�s��\��
        }
        else if (enemyHP <= 0 && EnemyDeath == true)
        {
            Debug.Log("�G���S");
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
            Debug.Log("\n" + "Player�̃X�s�[�h:" + playerSP + "\n" + "Enemy�̃X�s�[�h:" + enemySP);
            Debug.Log("�v���C���[���ŏ��ɍs�����Ƃ�܂�");
            PlayerTurn = true;
            EnemyTurn = false;
        }
        else if (playerSP == enemySP)
        {
            Debug.Log("\n" + "Player�̃X�s�[�h:" + playerSP + "\n" + "Enemy�̃X�s�[�h:" + enemySP);
            Debug.Log("�����_���ɍŏ��̍s���҂����߂܂�");
            int which = Random.Range(0, 10);
            if (which >= 5)
            {
                Debug.Log("���I�̌��ʃv���C���[���ŏ��ɍs�����Ƃ�܂�");
                PlayerTurn = true;
                EnemyTurn = false;
            }
            else
            {
                Debug.Log("���I�̌��ʓG���ŏ��ɍs�����Ƃ�܂�");
                PlayerTurn = false;
                EnemyTurn = true;
            }
        }
        else
        {
            Debug.Log("\n" + "Player�̃X�s�[�h:" + playerSP + "\n" + "Enemy�̃X�s�[�h:" + enemySP);
            Debug.Log("�G���ŏ��ɍs�����Ƃ�܂�");
            PlayerTurn = false;
            EnemyTurn = true;
        }
    }

    void EncounterEnemy()
    {
        #region �G�̏��擾
        enemyname = enemylist.Data[RandomEnemy].EnemyName;
        Debug.Log(enemyname + "���������d�|���Ă���");
        enemyHP = enemylist.Data[RandomEnemy].HitPoint;
        enemySP = enemylist.Data[RandomEnemy].Speed;
        enemyATK = enemylist.Data[RandomEnemy].Attack;
        enemyDEF = enemylist.Data[RandomEnemy].Defense;

        #region�@���@�Ɋւ�����
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
            Debug.Log("�v���C���[�͎���ł��܂�");
        }
        else
        {
            Debug.Log("����");
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
            EnemyTurn = true;  // �G�̃^�[�����J�n
        }
        else if (enemyHP <= 0)
        {
            Debug.Log("�G�͎���ł��܂�");
        }
        else
        {
            Debug.Log("����");
            Debug.Log(Damage);
            PlayerTurn = false;
            EnemyTurn = true;  // �G�̃^�[�����J�n
        }
    }
    //���񂱂��Ȃ�-----------------------------------------------------------------
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
            Debug.Log("�v���C���[�̃^�[���ł�");
            turn_number.Add(1);
            // PlayerTurn = false;

        }

        if (EnemyTurn)
        {
            Debug.Log("�G�l�~�[�̃^�[���ł�");
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
        Time.timeScale = 0f; // �Q�[���̎��Ԃ��~
        gameEnd = true;
    }

    void ReloadScene()
    {
        gameEnd = false;
        Time.timeScale = 1f; // �Q�[���̎��Ԃ��ĊJ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���݂̃V�[�����ă��[�h
    }


    //�o�߃^�[����\�����邾��
    private void LogTurnHistory()
    {
        string logMessage = "�o�߃^�[��: ";
        foreach (int turn in turn_number)
        {
            if (turn == 1)
            {
                logMessage += "�v���C���[, ";
            }
            else if (turn == 2)
            {
                logMessage += "�G�l�~�[, ";
            }
        }
        logMessage = logMessage.TrimEnd(',', ' ');
        int totalTurns = turn_number.Count; // ���v�^�[����
        logMessage += "\n���v�^�[����: " + totalTurns;
        Debug.Log(logMessage);

    }

    public void PlayerAttack()
    {
        PlayerPhysicalAttacks();
        //���ʂɊ֐����g�p���Ă���?�̂Ō�X���P
    }
    public void PlayerSkill()
    {
        //�G���O��U�����Ă���Ƃ������������o���Ă��Ȃ��̂Ŏ��̓G�̍U����80%�J�b�g�ɂ���
        IsPlayerUsingSkill = true;
    }

    public void PlayerItem()
    {
        if (playerHP == CheckPlayerHp)
        {
            Debug.Log("�v���C���[��HP��Max�ł�");
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
            Debug.Log("�̗͂��񕜂��܂���");
            EnemyTurn = false;
        }
        else if (Damage <= 0)
        {
            playerHP += Damage;
            Debug.Log("�_���[�W���󂯂܂���");
            Debug.Log(playerHP);
            EnemyTurn = false;
        }
        else
        {
            Debug.Log("�����ł�");
            EnemyTurn = false;
        }
    }


    public void GameManager()
    {
        if (PlayerTurn)
        {
            Debug.Log("�v���C���[�̃^�[�����i�s���ł�");
            PlayerPhysicalAttacks();
        }
        if (EnemyTurn)
        {
            Debug.Log("�G�̃^�[�����i�s���ł�");
            Enemybrain();
        }
        CommandBattleTrue();
    }

}
