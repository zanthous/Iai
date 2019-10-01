using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetAnimator : MonoBehaviour
{
    [SerializeField] private float amount = 2.0f;
    [SerializeField] private string stateName = "run";
    [SerializeField] private AnimationClip clip;

    private Animator anim;
    private GameData data;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;
        data = GameData.instance;
    }

    private void Update()
    {
        if(data.Timer >= 2.0f)
        {
            anim.enabled = true;
            //If we went over resynch by starting animation at that time
            anim.Play(stateName, -1, data.Timer - 2.0f / clip.length);
            Destroy(this);
        }
    }
}
