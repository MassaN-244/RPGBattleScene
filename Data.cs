using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "CreateData")]
public class Data : ScriptableObject
{
    public enum Kind_Char
    {
        ENEMY,
        BRAVE,
        PRINCESS,
        GRAPPLER,
        WIZARD
    }

    public enum Kind_Enemy_Size
    {
        S,
        M,
        L
    }

    public enum Kind_Command
    {
        ATTACK,
        ATTACKMAGIC,
        SUPPORTMAGIC,
        SKILL,
        ITEM
    }

    [SerializeField]
    private Kind_Char kindOfChar;   //職業

    [SerializeField]
    private Kind_Enemy_Size kindOfSize;

    [SerializeField]
    private Kind_Command kindOfCommand;

    [SerializeField]
    private int num;    //ID

    [SerializeField]
    private Sprite sprite;      //アイコン画像


    [SerializeField]
    private string name;    //名前

    [SerializeField]
    private int maxHp;     //体力

    [SerializeField]
    private int hp;     //体力

    [SerializeField]
    private int maxMp;     //魔力

    [SerializeField]
    private int mp;     //魔力

    [SerializeField]
    private int atk;    //攻撃力

    [SerializeField]
    private int matk;   //魔法攻撃力

    [SerializeField]
    private int def;    //防御力

    [SerializeField]
    private int mdef;   //魔法防御力

    [SerializeField]
    private int agi;    //素早さ


    [SerializeField]
    private int expPoint;   //ENEMYの場合のみ、経験値もらえる基準

    [SerializeField]
    private int exp_Sum;    //合計経験値

    [SerializeField]
    private int exp_Next;   //次のレベルまでの経験値

    [SerializeField]
    private int lv;     //現在のレベル


    [SerializeField]
    private int order;

    [SerializeField]
    private int target;


    //セッター・ゲッター
    public Kind_Char KindOfChar
    {
        set { this.kindOfChar = value; }
        get { return this.kindOfChar; }
    }

    public Kind_Enemy_Size KindOfSize
    {
        set { this.kindOfSize = value; }
        get { return this.kindOfSize; }
    }

    public Kind_Command KindOfCommand
    {
        set { this.kindOfCommand = value; }
        get { return this.kindOfCommand; }
    }

    public int Num
    {
        set { this.num = value; }
        get { return this.num; }
    }

    public Sprite Sprite
    {
        set { this.sprite = value; }
        get { return this.sprite; }
    }

    public string Name
    {
        set { this.name = value; }
        get { return this.name; }
    }

    public int MaxHp
    {
        set { this.maxHp = value; }
        get { return this.maxHp; }
    }

    public int Hp
    {
        set { this.hp = value; }
        get { return this.hp; }
    }

    public int MaxMp
    {
        set { this.maxMp = value; }
        get { return this.maxMp; }
    }

    public int Mp
    {
        set { this.mp = value; }
        get { return this.mp; }
    }

    public int Atk
    {
        set { this.atk = value; }
        get { return this.atk; }
    }

    public int Matk
    {
        set { this.matk = value; }
        get { return this.matk; }
    }

    public int Def
    {
        set { this.def = value; }
        get { return this.def; }
    }

    public int Mdef
    {
        set { this.mdef = value; }
        get { return this.mdef; }
    }

    public int Agi
    {
        set { this.agi = value; }
        get { return this.agi; }
    }

    public int ExpPoint
    {
        get { return this.expPoint; }
    }

    public int Exp_Sum
    {
        set { this.exp_Sum = value; }
        get { return this.exp_Sum; }
    }

    public int Exp_Next
    {
        set { this.exp_Next = value; }
        get { return this.exp_Next; }
    }

    public int Lv
    {
        set { this.lv = value; }
        get { return this.lv; }
    }

    public int Order
    {
        set { this.order = value; }
        get { return this.order; }
    }

    public int Target
    {
        set { this.target = value; }
        get { return this.target; }
    }

    public static void SetMultipleData(Data multipleData, Data originalData)   //同じ種類の敵が出てきた際にData型の変数に同じ値を代入するための関数
    {
        multipleData.KindOfChar = originalData.KindOfChar;
        multipleData.KindOfSize = originalData.KindOfSize;
        multipleData.KindOfCommand = originalData.KindOfCommand;
        multipleData.Sprite = originalData.Sprite;
        multipleData.Name = originalData.Name;
        multipleData.MaxHp = originalData.MaxHp;
        multipleData.Hp = originalData.Hp;
        multipleData.MaxMp = originalData.MaxMp;
        multipleData.Mp = originalData.Mp;
        multipleData.Atk = originalData.Atk;
        multipleData.Matk = originalData.Matk;
        multipleData.Def = originalData.Def;
        multipleData.Mdef = originalData.Mdef;
        multipleData.Agi = originalData.Agi;
        multipleData.Order = originalData.Order;
        multipleData.Target = originalData.Target;
    }
}
