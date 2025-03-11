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

    //存放当前物体
    public List<GameObject> currentCards;

    //是否对称
    public bool IsRect;

    //是否是最后一个
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
