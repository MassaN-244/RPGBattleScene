using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class MassageManager : MonoBehaviour
{
    [SerializeField] private BattleManager bM;
    [SerializeField] private GenerateUI gUI;
    [SerializeField] private TweenMG tMG;
    [SerializeField] private AnimationMG aMG;
    [SerializeField] private UIManager uIM;

    [SerializeField] private TextMeshProUGUI battleMassage;




    private void Start()
    {
        StartCoroutine("StartBattleLog", bM.ThisEnemy);
    }

    public TextMeshProUGUI BattleMassage
    {
        set { this.battleMassage = value; }
        get { return this.battleMassage; }
    }


    public void BattleLog(string log)
    {
        battleMassage.text = log;
    }

    public IEnumerator AttackToEnemyLog(int order, int target)
    {
        BattleLog(bM.ThisParty[order].Name + "の　こうげき！");
        Vector3 enemyPos= gUI.EnemyPanel.transform.GetChild(target).GetComponent<RectTransform>().position;
        
        aMG.EffectObject.GetComponent<RectTransform>().position = enemyPos;
        aMG.Animator.SetTrigger("SlashAnim");

        yield return new WaitForSeconds(2f);

        if (bM.ThisEnemy[target].Hp == 0)
        {
            BattleLog(bM.ThisEnemy[target].Name + "に　" + bM.Damage(bM.ThisParty[order].Atk, bM.ThisEnemy[target].Def) + "の　ダメージ！"
                      + "\n" + bM.ThisEnemy[target].Name + "を　たおした！");
            tMG.DamageSequence(order, target);
            tMG.KillEnemyTween(target);
        }
        else
        {
            BattleLog(bM.ThisEnemy[target].Name + "に　" + bM.Damage(bM.ThisParty[order].Atk, bM.ThisEnemy[target].Def) + "の　ダメージ！");
            tMG.DamageToEnemyTween(target);
            tMG.DamageSequence(order,target);
        }
    }

    public IEnumerator AttackToPartyLog(int order, int target)
    {
        BattleLog(bM.ThisEnemy[order].Name + "の　こうげき！");

        yield return new WaitForSeconds(2f);

        if (bM.ThisParty[target].Hp == 0)
        {
            BattleLog(bM.ThisParty[target].Name + "は　" + bM.Damage(bM.ThisEnemy[order].Atk, bM.ThisParty[target].Def) + "の　ダメージを　うけた！"
                      + "\n" + bM.ThisParty[target].Name + "は　しんでしまった！");
            uIM.ChangeHpInfo(target);
        }
        else
        {
            BattleLog(bM.ThisParty[target].Name + "は　" + bM.Damage(bM.ThisEnemy[order].Atk, bM.ThisParty[target].Def) + "の　ダメージを　うけた！");
            uIM.ChangeHpInfo(target);
        }
    }

    public IEnumerator StartBattleLog(List<Data> thisEnemy)
    {
        yield return new WaitForSeconds(1f);    //シーンのフェードイン待ち

        string log = null;
        for (int i = 0; i < bM.ThisEnemy.Count; i++)
        {
            if (log == null)
            {
                log = bM.ThisEnemy[i].Name + "が　あらわれた！";
            }
            else
            {
                log+= "\n"+ bM.ThisEnemy[i].Name + "が　あらわれた！";
            }
            
            BattleLog(log);

            yield return new WaitForSeconds(0.8f);           
        }
        BattleLog("");
        bM.ResetTurn();
    }

    public IEnumerator FinishBattleLog()
    {
        BattleLog("まものたちを　たおした！");
        yield return new WaitForSeconds(2f);
        //将来的にはタッチ検知待ち
        BattleLog(bM.ThisParty[0].Name + "たちは　それぞれ" + "\n" + 999 + "の　けいけんちを　かくとくした！");
        yield return new WaitForSeconds(2f);
        //将来的にはタッチ検知待ち
        BattleLog(999 + "ゴールドを　手に入れた！");

        yield return new WaitForSeconds(2f);
    }
}
