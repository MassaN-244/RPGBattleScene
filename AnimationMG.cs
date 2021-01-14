using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationMG : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject effectObject;

    public Animator Animator
    {
        set { this.animator = value; }
        get { return this.animator; }
    }

    public GameObject EffectObject
    {
        set { this.effectObject = value; }
        get { return this.effectObject; }
    }
}
