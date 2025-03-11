using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TypeEnum;

public class SmallCardsPanel : MonoBehaviour
{
    //---------------------------以下为开发者自主调整区域------------------
    //表示面板要存放多少个卡牌
    public int cardsNumber;

    //是否开始相叠模式
    public bool isLapMode;

    //卡牌之间的间距
    public int cardInterval;

    //卡牌相叠概率
    public int lapCardsP;
    public int lapOneCardP;

    //卡牌面板方向
    public CardDirection panelDirection;

    //表示面板层级(层级越低，越在底层)
    public int level;  //默认为0

    //卡牌预制体
    public GameObject cardPrefab;

    //面板序号
    public int panelOrder;


    //--------------------------以下为非DIY对外调用区域----------------------------
    public List<GameObject> cards;

    //---------------------------以下为私有区域-------------------------------------

    //添加卡牌前数量
    private int addPreCount;

    //启动时候就进行构造
    private void Start()
    {
        Init();
    }
    private void Awake()
    {
        
    }
    //初始化
    private void Init()
    {
        if (panelOrder < 5)
        {
            if (isLapMode)
            {
                OverLapAddCard(cardsNumber);
            }
            else
            {
                AddCard(cardsNumber, true);
            }
           
        }
       
       

    }

    //添加卡牌
    public void AddCard(int addCount,bool isFirst)
    {
        if (!isFirst)
        {
            cardsNumber += addCount;

            foreach (var v in cards)
            {
                v.GetComponent<Card>().coveredNumber++;
            }
        }

        // Debug.Log($"测试添加固定面板元素*******************************************{cardsNumber}");
      
        for (int i = addPreCount; i < cardsNumber; i++)
        {
            GameObject g = Instantiate(cardPrefab,gameObject.transform);

           

            g.GetComponent<Card>().isBigCard = false;
            cards.Add(g);

            if (addPreCount != 0)
            {
                UpdateCoverMessage(cards[cards.Count - 2],i-1);
                
                
            }

            //进行单独的遮罩计算
            if (i + 1 != cardsNumber)
            {
                UpdateCoverMessage(g,i);
            }


        }
        
        addPreCount = cards.Count;

        
    }


    //重叠添加卡牌
    public void OverLapAddCard(int addCount)
    {


        // Debug.Log($"测试添加固定面板元素*******************************************{cardsNumber}");
        Vector3 vector = new Vector3();
        for (int i = 0; i < addCount; i++)
        {
 
            GameObject g = Instantiate(cardPrefab, gameObject.transform);

            g.transform.localPosition = vector;

           

            g.GetComponent<Card>().isBigCard = false;
            cards.Add(g);


            //进行单独的遮罩计算
            if (i + 1 != addCount)
            {
                UpdateCoverMessage(g, i);
            }


            //移动
            int r = Random.Range(0,lapOneCardP+lapCardsP);
            
            if (r < lapOneCardP)
            {
                continue;
            }
            

            switch (panelDirection)
            {
                case CardDirection.Left:
                    {
                        vector += new Vector3(cardInterval, 0);
                        break;
                    }

                case CardDirection.Right:
                    {
                        vector -= new Vector3(cardInterval, 0);
                        break;
                    }

                case CardDirection.Up:
                    {
                        vector += new Vector3(0, cardInterval);
                        break;
                    }

                case CardDirection.Down:
                    {
                        Debug.Log($"向下发牌:{vector}");
                        vector -= new Vector3(0, cardInterval);
                        break;
                    }

                default:
                    break;
            }


        }

    }

    //更新遮盖信息
    public void UpdateCoverMessage(GameObject g,int orderNumber)
    {
        //身上(溢出)覆盖的信息进行了更新
        g.GetComponent<Card>().coveredNumber = cardsNumber - orderNumber - 1;   //并非完全符合覆盖规则

        //进行颜色处理
        g.GetComponent<Card>().cardTypeImage.color = g.GetComponent<Card>().coverColor;

        //进行不可点击处理
        g.GetComponent<Button>().interactable = false;
    }


    //增加卡牌(重载)
    public void AddCard(GameObject card)
    {
        //1、先遍历增加数量和遮盖
        foreach (var v in cards)
        {
            if (v.GetComponent<Card>().coveredNumber == 0)
            {              
                //进行颜色处理
                v.GetComponent<Card>().cardTypeImage.color = v.GetComponent<Card>().coverColor;

                //进行不可点击处理
                v.GetComponent<Button>().interactable = false;
            }
           
            v.GetComponent<Card>().coveredNumber++;
        }

        //2、增加自己
        cards.Add(card);
    }

    public void RemoveCard(GameObject g)
    {
        Debug.Log($"删除物体：{g.name}");
        foreach (var v in  cards)
        {
            Debug.Log("移除中");
            if (v != g)
            {
                v.GetComponent<Card>().coveredNumber--;

                if (v.GetComponent<Card>().coveredNumber == 0)
                {
                    v.GetComponent<Button>().interactable = true;
                    v.GetComponent<Card>().cardTypeImage.color = v.GetComponent<Card>().oldClor;
                }
            }
                   
        }

       
        cards.Remove(g);


    }
}
