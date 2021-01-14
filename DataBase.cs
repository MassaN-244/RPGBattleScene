using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBase", menuName = "CreateDataBase")]
public class DataBase : ScriptableObject
{
    [SerializeField]
    private List<Data> dataLists = new List<Data>();

    public List<Data> DataLists
    {
        set { this.dataLists = value; }
        get { return dataLists; }
    }
}
