using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDestroyEffect : MonoBehaviour
{
    //--------------------------DIY区域-------------------
    //播放动画序列
    public List<Sprite> sprites;

    //播放时间
    public float playTime;


    //--------------------------不可编辑区域--------------------
    //图片间隔
    private float intervalTime;
    //当前物体图片
    private Image image;
    void Start()
    {
        intervalTime = playTime / sprites.Count;
        image = GetComponent<Image>();

        //开始协程动画

        StartCoroutine(PlayAnimation());
    }

    public IEnumerator PlayAnimation()
    {
        Debug.Log($"协程动画开始");
        foreach (var g in sprites)
        {
            image.sprite = g;
            yield return new WaitForSeconds(intervalTime);
        }
        Debug.Log($"协程动画结束");

        Destroy(gameObject);
    }  
   
}
