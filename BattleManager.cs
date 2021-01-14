using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GenerateUI gUI;
    [SerializeField] private UIManager uIM;
    [SerializeField] private MassageManager mM;
    [SerializeField] private TweenMG tMG;
    

    //通常のデータベース([0])と、定数のように扱えて最後にリセットする用のデータベース([1])を用意する
    public DataBase[] partyDataBase = new DataBase[2];
    public DataBase[] enemyDataBase = new DataBase[2];

    //今回のバトルに登場するキャラを格納するリスト
    private List<Data> thisParty = new List<Data>();  
    private List<Data> thisEnemy = new List<Data>();

    private List<Data> thisParty_tmp = new List<Data>();
    private List<Data> thisEnemy_tmp = new List<Data>();

    //private Dictionary<int, Data> agilityOrder = new Dictionary<int, Data>();   //<素早さ情報,キャラデータベース>
    private List<Data> agilityOrder = new List<Data>();

    private delegate IEnumerator SomeAttack(int order, int target); //attack系の関数のデリゲート
    
    private Queue<SomeAttack> commandQueue = new Queue<SomeAttack>();

    private int partyCount = 4;
    private int enemyCount;
    private int[] enemyIDRange = { 0, 3 };
    private int turn = 0;
    private int leadPartyOrder;
    public int LeadPartyOrder
    {
        get { return this.leadPartyOrder; }
    }

    private bool orderIsDead = false;
    private bool battle_flag = true;
    public static bool battleStart = false;
    private bool[] commandDone = { false, false, false, false };
    private bool[] commandNow = { true, false, false, false };

    [SerializeField]private GameObject enemyPanel;

    [SerializeField]private GameObject targetPanel;

    [SerializeField] private Transform camera;

    private RectTransform rectTransform;
    

    public List<Data> ThisParty
    {
        set { this.thisParty = value; }
        get { return this.thisParty; }
    }

    public List<Data> ThisEnemy
    {
        set { this.thisEnemy = value; }
        get { return this.thisEnemy; }
    }

    public bool[] CommandDone
    {
        set { this.commandDone = value; }
        get { return this.commandDone; }
    }

    public bool[] CommandNow
    {
        set { this.commandNow = value; }
        get { return this.commandNow; }
    }


    //---------------------------------------------------------------------EventArea--------------------------------------------------------------------//

    private void Awake()
    {
        enemyCount = Random.Range(1, 5);
        partyCount = 4;
        SetThisParty();
        SetThisEnemy();
        SetAgilityOrder();
    }

    public void Start()
    {
        
    }

    private void Update()
    {
        if (battleStart == true)
        {
            SetBattleQueue();
            StartBattleDeQueue();
            battleStart = false;
        }

        if (battle_flag == false)
        {
            BattleEnd();
            battle_flag = true;
        }
    }


    //--------------------------------------------------------------------FunctionArea------------------------------------------------------------------//


    private void SetThisParty()
    {
        for (int i = 0; i < partyCount; i++)
        {
            thisParty.Add(partyDataBase[0].DataLists[i]);
            thisParty[i].Order = i;
            thisParty_tmp.Add(partyDataBase[1].DataLists[i]);   //MaxData
        }
        Debug.Log("味方の数は"+thisParty.Count);
    }

    private void SetRandomEnemy(int count)
    {
        for (int i = 0; i < count; i++)
        {           
            int iD = Random.Range(enemyIDRange[0], enemyIDRange[1]);    //ここの乱数の範囲はステージ、状況などによって変化する敵テーブルかなんか作って対処

            if (thisEnemy.Contains(enemyDataBase[0].DataLists[iD])) 
            {
                Data data = new Data();
                Data.SetMultipleData(data, enemyDataBase[0].DataLists[iD]);
                string tmpName = data.Name;
                data.Name = data.Name + "-" + 2;

                for (int j = 0; j < i; j++) 
                {
                    if (data.Name == thisEnemy[j].Name)
                    {
                        data.Name = tmpName + "-" + 3;
                    }
                }

                thisEnemy.Add(data);
                thisEnemy_tmp.Add(data);            
            }
            else
            {
                thisEnemy.Add(enemyDataBase[0].DataLists[iD]);
                thisEnemy_tmp.Add(enemyDataBase[1].DataLists[iD]);  //MaxData
            }
            thisEnemy[i].Order = i;
        }
    }

    private void SetThisEnemy()
    {
        //enemyCount = Random.Range(1, 5);    //敵の数をランダムで決定

        SetRandomEnemy(enemyCount);       //敵の数だけランダムなIDのてきをThisEnemyに代入

        
        Debug.Log("敵の数は" + thisEnemy.Count);
    }

    private void SetAgilityOrder()
    {
        for (int i = 0; i < thisParty.Count; i++)
        {
            agilityOrder.Add(thisParty[i]);
        }
        for (int j = 0; j < thisEnemy.Count; j++)
        {
            agilityOrder.Add(thisEnemy[j]);
        }
        agilityOrder.Sort((a, b) => b.Agi - a.Agi);     //素早さ降順
        //IOrderedEnumerable<KeyValuePair<int, Data>> sorted = agilityOrder.OrderByDescending(pair => pair.Key);  //intを降順で並び替え
    }





    private IEnumerator AttackToEnemy(int order,int target)
    {
        if (thisParty[order].Hp != 0)   //死んだ敵が
        {
            if (target < 20 && thisEnemy[target].Hp != 0)
            {
                thisEnemy[target].Hp -= Damage(thisParty[order].Atk, thisEnemy[target].Def);   //とりあえず簡単な計算で
               
                Debug.Log(order + "Pは威力" + thisParty[order].Atk + "で敵" + target + "に攻撃");
                Debug.Log("敵" + target + "は防御力" + thisEnemy[target].Def + "でガード");
                Debug.Log("敵" + target + "のHpは残り" + thisEnemy[target].Hp + "となりました");
                JudgeEnemyDead(target);

                yield return StartCoroutine(mM.AttackToEnemyLog(order, target));                
            }
            else if (target >= 20) 
            {
                for (int i = 0; i < thisEnemy.Count; i++)
                {
                    thisEnemy[i].Hp -= Damage(thisParty[order].Atk, thisEnemy[i].Def);  //全体攻撃
                }
                yield break;
            }
            else if (thisEnemy[target].Hp == 0)
            {
                for (int n = 0; n < thisEnemy.Count; n++)
                {
                    if (thisEnemy[n].Hp != 0)
                    {
                        int changedTarget = thisEnemy[n].Order;
                        thisEnemy[changedTarget].Hp -= Damage(thisParty[order].Atk, thisEnemy[changedTarget].Def);
                        
                        Debug.Log(order + "Pは威力" + thisParty[order].Atk + "で敵" + changedTarget + "に攻撃");
                        Debug.Log("敵" + changedTarget + "は防御力" + thisEnemy[changedTarget].Def + "でガード");
                        Debug.Log("敵" + changedTarget + "のHpは残り" + thisEnemy[changedTarget].Hp + "となりました");
                        JudgeEnemyDead(changedTarget);

                        yield return StartCoroutine(mM.AttackToEnemyLog(order, changedTarget));
                        break;
                    }
                }
            }
        }
        else
        {
            orderIsDead = true;
        }
    }

    private IEnumerator AttackToParty(int order,int target)
    {
        if (thisEnemy[order].Hp != 0)   //死んだ敵が攻撃してくるのを防ぐ
        {
            if (thisParty[target].Hp != 0)
            {
                thisParty[target].Hp -= Damage(thisEnemy[order].Atk, thisParty[target].Def);   //とりあえず
                
                JudgePartyDead(target);
                
                Debug.Log("敵" + order + "は威力" + thisEnemy[order].Atk + "で" + target + "Pに攻撃");
                Debug.Log(target + "Pは防御力" + thisParty[target].Def + "でガード");
                Debug.Log(target + "PのHpは残り" + thisParty[target].Hp + "となりました");

                yield return StartCoroutine(mM.AttackToPartyLog(order, target));
            }
            else if (thisParty[target].Hp == 0)
            {
                for (int n = 0; n < thisParty.Count; n++)
                {
                    if (thisParty[n].Hp != 0)
                    {
                        int changedTarget = thisParty[n].Order;
                        thisParty[changedTarget].Hp -= Damage(thisEnemy[order].Atk, thisParty[changedTarget].Def);
                        JudgePartyDead(changedTarget);

                        Debug.Log("敵" + order + "は威力" + thisEnemy[order].Atk + "で" + changedTarget + "Pに攻撃");
                        Debug.Log(changedTarget + "Pは防御力" + thisParty[changedTarget].Def + "でガード");
                        Debug.Log(changedTarget + "PのHpは残り" + thisParty[changedTarget].Hp + "となりました");

                        yield return StartCoroutine(mM.AttackToEnemyLog(order, changedTarget));
                        break;
                    }
                }
            }          
        }
        else
        {
            orderIsDead = true;
        }
    }

    public int Damage(int atk, int def)
    {       
        if (atk - def >= 0)
        {
            return atk - def;
        }
        else
        {
            return 0;
        }
    }

    public void SetRandomTarget()
    {
        for (int i = 0; i < thisEnemy.Count; i++)
        {
            int changedTarget = Random.Range(0, thisParty.Count);
            while (true)
            {
                if (thisParty[changedTarget].Hp != 0)
                {
                    break;
                }
                else if (thisParty[changedTarget].Hp == 0)
                {
                    changedTarget = Random.Range(0, thisParty.Count);
                }
            }
            thisEnemy[i].Target = changedTarget;
        }
    }

    private void EnemyDead(int target)
    {
        Debug.Log("敵" + target + "を倒した！");
        
        gUI.TargetPanel.transform.GetChild(target).gameObject.SetActive(false);
        
        Debug.Log("Deadエネミーをターゲティングするボタンを非表示");
    }

    private void JudgePartyDead(int target)
    {
        if (thisParty[target].Hp <= 0)
        {
            thisParty[target].Hp = 0;
        }
        else
        {
            return;
        }
    }

    private void JudgeEnemyDead(int target)
    {
        if (thisEnemy[target].Hp <= 0)
        {
            thisEnemy[target].Hp = 0;
            EnemyDead(target);
        }
        else
        {
            return;
        }
    }

    private IEnumerator DeQueueCoroutine()
    {
        Debug.Log("バトルコルーチンを開始します");
        while (commandQueue.Count != 0)
        {
            Debug.Log("------------------------------------------------------------------------------------------");
            yield return StartCoroutine(commandQueue.Dequeue()(agilityOrder[turn].Order, agilityOrder[turn].Target));  //仮の見方一体のみの行動（吐き出したQueueに引数を渡して実行）
            
            if (CheckBattleEnd() == true) 
            {
                yield return new WaitForSeconds(2f);

                yield return StartCoroutine(mM.FinishBattleLog());

                battle_flag = false;
                Debug.Log("敵が全滅したのでコルーチンを終了し、バトルフラグをfalseにしました");
                yield break;
            }

            turn++;
            if (orderIsDead == false)
            {
                yield return new WaitForSeconds(2f);
            }
            else if (orderIsDead == true)
            {
                orderIsDead = false;
            }
            
        }
        Debug.Log("------------------------------------------------------------------------------------------");
        Debug.Log("今回のターンは終了しました");
        ResetTurn();
    }

    private bool CheckBattleEnd()
    {
        if (thisEnemy.All(data => data.Hp == 0) == true || thisParty.All(data => data.Hp == 0) == true)    //もし敵のHpがすべて0なら
        {
            return true;
        }
        return false;
    }

    private void BattleEnd()
    {
        for (int m = 0; m < thisEnemy.Count; m++)
        {
            thisEnemy[m].Hp = thisEnemy[m].MaxHp;
        }
        /*for (int n = 0; n < thisParty.Count; n++) 
        {
            thisParty[n] = thisParty_tmp[n];
            Debug.Log(thisParty[n]);
        }*/
        //敵データを戻す
        //コピーしたDataを戻す
        SceneManager.LoadScene("BattleScene");
        Debug.Log("バトル終了");
    }

    public void ResetTurn()
    {
        turn = 0;
        for (int i = 0; i < thisParty.Count; i++)
        {
            if (thisParty[i].Hp != 0)
            {
                commandDone[i] = false;
            }
            else if (thisParty[i].Hp == 0)
            {
                commandDone[i] = true;
            }
            
            commandNow[i] = false;
            uIM.CommandText[i].text = null;
        }
        for (int j = 0; j < thisParty.Count; j++)
        {
            if (thisParty[j].Hp != 0)
            {
                leadPartyOrder = j;
                if (leadPartyOrder != 0)
                {
                    uIM.SetGoBackButton(leadPartyOrder, false);
                }               

                gUI.CommandArea[j].SetActive(true);
                commandNow[j] = true;
                break;
            }
        }
        
        mM.BattleMassage.text = null;
        Debug.Log("command情報をリセットしました");
    }


    //----------------------------------------------------------------ButtonFunctionArea----------------------------------------------------------------//


    public void SetBattleQueue()
    {
        //予めデリゲートに関数を登録
        SomeAttack attackToEnemy = new SomeAttack(AttackToEnemy);
        SomeAttack attackToParty = new SomeAttack(AttackToParty);

        foreach (Data Value in agilityOrder)             //DictionaryのValue値をもとに攻撃デリゲートがQueueされていく（Magic関数などもいる）
        {
            if (Value.KindOfChar != Data.Kind_Char.ENEMY)
            {
                if (Value.KindOfCommand == Data.Kind_Command.ATTACK)
                {
                    commandQueue.Enqueue(attackToEnemy);            //DataにEnum型のATTACK、MAGIC...を用意してif文でEnqueueするSomeAttack関数を変える
                }
                else if (Value.KindOfCommand == Data.Kind_Command.ATTACKMAGIC)
                {
                    ;
                }
            }
            else if (Value.KindOfChar == Data.Kind_Char.ENEMY)
            {
                if (Value.KindOfCommand == Data.Kind_Command.ATTACK)
                {
                    commandQueue.Enqueue(attackToParty);           //DataにEnum型のATTACK、MAGIC...を用意してif文でEnqueueするSomeAttack関数を変える
                }
                else if (Value.KindOfCommand == Data.Kind_Command.ATTACKMAGIC)
                {
                    ;
                }             
            }
        }                    
    }

    public void StartBattleDeQueue()
    {
        StartCoroutine("DeQueueCoroutine");       
    }
}
