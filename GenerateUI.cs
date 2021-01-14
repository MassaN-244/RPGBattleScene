using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using TMPro;

public class GenerateUI : MonoBehaviour
{
    [SerializeField] private BattleManager bM;
    [SerializeField] private UIManager uIM;
    [SerializeField] private TweenMG tMG;

    [SerializeField] private GameObject[] targetButtons;
    [SerializeField] private GameObject[] enemyObjects;
    [SerializeField] private GameObject[] commandArea;
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private GameObject enemyPanel;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private GameObject returnButton;


    public GameObject[] CommandArea
    {
        set { this.commandArea = value; }
        get { return this.commandArea; }
    }

    public GameObject TargetPanel
    {
        get { return this.targetPanel; }
    }

    public GameObject EnemyPanel
    {
        get { return this.enemyPanel; }
    }

    public GameObject ReturnButton
    {
        get { return this.returnButton; }
    }



    

//--------------------------------------------------------------------------------------------------------------------------------------------------// 

    private void Start()
    {        
        SetButtonText();
        GenerateEnemyTargetButton();
        fadePanel.SetActive(true);
        fadePanel.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(GenerateEnemyUI);
    }

    
    public void OnClickTargetButton(int target)
    {
        int n = 0;

        for (int j = 0; j < bM.ThisEnemy.Count; j++)
        {
            enemyPanel.transform.GetChild(j).GetChild(0).gameObject.SetActive(false);
        }

        for (int i = 0; i < bM.ThisParty.Count; i++)
        {
            if (bM.CommandNow[i] == true)
            {
                bM.ThisParty[i].Target = target;
                Debug.Log("パーティ" + i + "のターゲットが" + target + "に変更されました");
                uIM.SetCommandText(i, target);

                n = i;
                Debug.Log(n + "Pのコマンドが完了しました");
                bM.CommandDone[n] = true;
                bM.CommandNow[n] = false;
                targetPanel.SetActive(false);
                break;
            }
        }

        if (bM.CommandDone.All(value => value == true) == true)
        {
            BattleManager.battleStart = true;
            targetPanel.SetActive(false);
            bM.SetRandomTarget();
        }
        else if (bM.CommandDone.All(value => value == true) == false)
        {
            for (int k = n + 1; k < bM.ThisParty.Count; k++)
            {
                if (bM.CommandDone[k] != true)
                {
                    bM.CommandNow[k] = true;
                    commandArea[k].SetActive(true);
                    Debug.Log(k + "Pが" + bM.CommandNow[k] + "です");
                    break;
                }
            }
        }
        returnButton.SetActive(false);
    }
    
    public void OnClickAttackButton()
    {
        targetPanel.SetActive(true);

        for (int i = 0; i < bM.ThisParty.Count; i++)
        {
            if (commandArea[i].activeSelf == true)
            {
                bM.ThisParty[i].KindOfCommand = Data.Kind_Command.ATTACK;
                commandArea[i].SetActive(false);
                Debug.Log(commandArea[i].activeSelf);
                returnButton.SetActive(true);
            }
        }
        for (int j = 0; j < bM.ThisEnemy.Count; j++)
        {
            if (bM.ThisEnemy[j].Hp != 0)
            {
                enemyPanel.transform.GetChild(j).GetChild(0).gameObject.SetActive(true);    //名前表示
            }           
        }
    }





    public void SetButtonText()
    {
        for (int i = 0; i < bM.ThisEnemy.Count; i++) 
        {
            string name = bM.ThisEnemy[i].Name;
            
            targetButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = name;
        }
    }





    public void GenerateEnemyUI()
    {
        for (int i = 0; i < bM.ThisEnemy.Count; i++)
        {
            if (bM.ThisEnemy[i].KindOfSize == Data.Kind_Enemy_Size.S) 
            {
                enemyObjects[0].GetComponent<SpriteRenderer>().sprite = bM.ThisEnemy[i].Sprite;
                enemyObjects[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bM.ThisEnemy[i].Name;

                GameObject obj = (GameObject)Instantiate(enemyObjects[0], new Vector3(0, 0, 0), Quaternion.identity);
                obj.transform.parent = enemyPanel.transform;

            }
            else if(bM.ThisEnemy[i].KindOfSize == Data.Kind_Enemy_Size.M)
            {
                enemyObjects[1].GetComponent<SpriteRenderer>().sprite = bM.ThisEnemy[i].Sprite;                
                enemyObjects[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bM.ThisEnemy[i].Name;

                GameObject obj = (GameObject)Instantiate(enemyObjects[1], new Vector3(0, 0, 0), Quaternion.identity);
                obj.transform.parent = enemyPanel.transform;
            }
            else if(bM.ThisEnemy[i].KindOfSize == Data.Kind_Enemy_Size.L)
            {
                enemyObjects[2].GetComponent<SpriteRenderer>().sprite = bM.ThisEnemy[i].Sprite;               
                enemyObjects[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bM.ThisEnemy[i].Name;

                GameObject obj = (GameObject)Instantiate(enemyObjects[2], new Vector3(0, 0, 0), Quaternion.identity);
                obj.transform.parent = enemyPanel.transform;
            }
            enemyPanel.transform.GetChild(i).GetComponent<SpriteRenderer>().DOFade(1, 1);
        }
    }
    

    //-----------------------------------------------------------------------------------------------------------------------------------------------//

    private void GenerateEnemyTargetButton()
    {
        for (int i = 0; i < bM.ThisEnemy.Count; i++)
        {        
            GameObject obj = (GameObject)Instantiate(targetButtons[i], new Vector3(0, 0, 0), Quaternion.identity);
            
            obj.transform.parent = targetPanel.transform;
            
            Debug.Log("button" + i + "を生成");
        }
    }
}
