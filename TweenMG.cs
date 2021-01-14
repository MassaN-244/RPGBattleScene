using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TweenMG : MonoBehaviour
{
    [SerializeField] private BattleManager bM;
    [SerializeField] private GenerateUI gUI;
    [SerializeField] private MassageManager mM;
    [SerializeField] private UIManager uIM;

    private float duration = 0.5f;

    private Sequence sequence;




    public void DamageSequence(int order, int target)
    {
        GameObject damageObject = gUI.EnemyPanel.transform.GetChild(target).GetChild(1).gameObject;
        GameObject damageText= gUI.EnemyPanel.transform.GetChild(target).GetChild(1).GetChild(0).gameObject;

        Vector3 pos = damageObject.GetComponent<RectTransform>().transform.position;
        Vector3 scale = damageObject.GetComponent<RectTransform>().transform.localScale;
        Vector3 textScale = damageText.GetComponent<RectTransform>().transform.localScale;
        Color textColor = damageObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color;
        Color imageColor = damageObject.GetComponent<Image>().color;

        damageObject.SetActive(true);
        damageObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text= bM.Damage(bM.ThisParty[order].Atk, bM.ThisEnemy[target].Def).ToString();

        sequence = DOTween.Sequence()
            .Join(damageObject.GetComponent<RectTransform>().DOScale(scale*1.3f, 0.3f))
            .Join(damageText.GetComponent<RectTransform>().DOScale(textScale * 3f, 0.2f))
            //.Append(damageText.GetComponent<RectTransform>().DOScale(textScale, duration + 0.2f))
            .Append(damageObject.GetComponent<RectTransform>().DOLocalMove(new Vector3(0, 1.0f, 0), duration).SetRelative())
            .Join(damageText.GetComponent<RectTransform>().DOScale(textScale, duration))
            .Join(damageObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0, duration))
            .Join(damageObject.GetComponent<Image>().DOFade(0, duration))
            .OnComplete(() =>
            {
                damageObject.GetComponent<RectTransform>().transform.position = pos;
                damageObject.GetComponent<RectTransform>().transform.localScale = scale;
                damageText.GetComponent<RectTransform>().transform.localScale = textScale;
                damageObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = textColor;
                damageObject.GetComponent<Image>().color = imageColor;

                damageObject.SetActive(false);
            });
        sequence.Play();
    }

    public void DamageToEnemyTween(int target)
    {
        gUI.EnemyPanel.transform.GetChild(target).GetComponent<SpriteRenderer>().DOFade(0, 0.1f).SetLoops(3)
            .OnComplete(() =>
            {
                Color color = gUI.EnemyPanel.transform.GetChild(target).GetComponent<SpriteRenderer>().color;
                color.a = 1;
                gUI.EnemyPanel.transform.GetChild(target).GetComponent<SpriteRenderer>().color = color;
            });
    }

    public void KillEnemyTween(int target)
    {
        gUI.EnemyPanel.transform.GetChild(target).GetComponent<SpriteRenderer>().DOFade(0, 0.1f).SetLoops(3);
    }

    public void SliderTween(Slider slider, int endValue)
    {
        slider.DOValue(endValue, 0.5f);
    }

    public void ShakeByDamage(GameObject gameObject)
    {
        gameObject.transform.DOShakePosition(0.5f, 20f, 50);
    }
}
