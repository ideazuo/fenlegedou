using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameWin : MonoBehaviour
{
    public GameObject winMessage;
    //Àı∑≈¥Û–°
    public Vector3 EndScale;
    public Vector3 BeginScale;

    public float scaleTime;
    void Start()
    {
        MusicManager._instance.OnPlayWinGame();

        winMessage.transform.localScale = BeginScale;
        winMessage.transform.DOScale(EndScale,scaleTime);
    }

   

}
