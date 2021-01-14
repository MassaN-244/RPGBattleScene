using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private BattleManager bM;
    [SerializeField] private GenerateUI gUI;
    [SerializeField] private TweenMG tMG;
    


    [SerializeField] private Sprite[] charSprite;
    [SerializeField] private GameObject[] charImage;
    [SerializeField] private Slider[] hpSlider;
    [SerializeField] private Slider[] mpSlider;
    [SerializeField] private TextMeshProUGUI[] hpText;
    [SerializeField] private TextMeshProUGUI[] mpText;
    [SerializeField] private GameObject[] partyObjects;
    [SerializeField] private TextMeshProUGUI[] commandText;



    public Slider[] HpSlider
    {
        //set { this.hpSlider = value; }
        get { return this.hpSlider; }
    }

    public TextMeshProUGUI[] HpText
    {
        set { this.hpText = value; }
        get { return this.hpText; }
    }

    public TextMeshProUGUI[] CommandText
    {
        set { this.commandText = value; }
        get { return this.commandText; }
    }

    private void Start()
    {
        for (int i = 0; i < bM.ThisParty.Count; i++)
        {
            charImage[i].GetComponent<Image>().sprite = charSprite[i];  //commandAreaのイメージを設定

            hpSlider[i].maxValue = bM.ThisParty[i].MaxHp;
            hpSlider[i].value = bM.ThisParty[i].Hp;
            mpSlider[i].maxValue = bM.ThisParty[i].MaxMp;
            mpSlider[i].value = bM.ThisParty[i].Mp;

            hpText[i].text = "HP : " + bM.ThisParty[i].Hp + " / " + bM.ThisParty[i].MaxHp;
            mpText[i].text = "MP : " + bM.ThisParty[i].Mp + " / " + bM.ThisParty[i].MaxMp;
        }
    }

    public void GoBack()
    {
        for (int i = bM.LeadPartyOrder + 1; i < bM.ThisParty.Count; i++)  
        {
            if (gUI.CommandArea[i].activeSelf == true)
            {
                for (int j = i - 1; j >= bM.LeadPartyOrder; j--)
                {
                    if (bM.ThisParty[j].Hp != 0)
                    {
                        gUI.CommandArea[i].SetActive(false);
                        bM.CommandNow[i] = false;
                        gUI.CommandArea[j].SetActive(true);
                        bM.CommandNow[j] = true;
                        break;
                    }
                }
            }
        }

        if (gUI.TargetPanel.activeSelf == true)
        {
            for (int i = 0; i < bM.ThisParty.Count; i++)
            {
                if (bM.CommandNow[i] == true)
                {
                    gUI.TargetPanel.SetActive(false);
                    gUI.CommandArea[i].SetActive(true);
                    gUI.ReturnButton.SetActive(false);
                    break;
                }
            }
        }
    }

    public void SetGoBackButton(int order, bool active)
    {
        if (active == true)
        {
            gUI.CommandArea[order].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
        else if (active == false)
        {
            gUI.CommandArea[order].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }

    public void ChangeHpInfo(int target)
    {
        tMG.SliderTween(hpSlider[target], bM.ThisParty[target].Hp);
        tMG.ShakeByDamage(partyObjects[target]);
        hpText[target].text = "HP : " + bM.ThisParty[target].Hp + " / " + bM.ThisParty[target].MaxHp;
    }

    public void SetCommandText(int order, int target)
    {
        string kind;
        string to = bM.ThisEnemy[target].Name;

        if (bM.ThisParty[order].KindOfCommand == Data.Kind_Command.ATTACK)
        {
            kind = "こうげき";
            commandText[order].text = kind + " > " + to;
        }
    }
}
