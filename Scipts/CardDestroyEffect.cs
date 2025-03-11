using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDestroyEffect : MonoBehaviour
{
    //--------------------------DIY����-------------------
    //���Ŷ�������
    public List<Sprite> sprites;

    //����ʱ��
    public float playTime;


    //--------------------------���ɱ༭����--------------------
    //ͼƬ���
    private float intervalTime;
    //��ǰ����ͼƬ
    private Image image;
    void Start()
    {
        intervalTime = playTime / sprites.Count;
        image = GetComponent<Image>();

        //��ʼЭ�̶���

        StartCoroutine(PlayAnimation());
    }

    public IEnumerator PlayAnimation()
    {
        Debug.Log($"Э�̶�����ʼ");
        foreach (var g in sprites)
        {
            image.sprite = g;
            yield return new WaitForSeconds(intervalTime);
        }
        Debug.Log($"Э�̶�������");

        Destroy(gameObject);
    }  
   
}
