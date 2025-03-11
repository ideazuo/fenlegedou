using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCardsPanel : MonoBehaviour
{
    public int row;
    public int line;

    public int level;

    public Card[] cards;

    [SerializeField]
    public GameObject[,] cardArrays;

    //��ŵ�ǰ����
    public List<GameObject> currentCards;

    //�Ƿ�Գ�
    public bool IsRect;

    //�Ƿ������һ��
    public bool IsEnd;



    void Awake()
    {
        cards = gameObject.GetComponentsInChildren<Card>();
    }

    private void Start()
    {
        currentCards = new List<GameObject>();
        
    }

    public void UpdateCardArray(int max)
    {
        cardArrays = new GameObject[max,max];

        int count = 0;
        for (int i = 0; i < max; i++)
        {
            for (int j = 0; j < max; j++)
            {
              
                cardArrays[i, j] = cards[count].gameObject;

               

                
                count++;
            }
        }

    }

    

}
