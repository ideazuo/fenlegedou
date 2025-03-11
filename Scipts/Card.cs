using System.Collections;
using System.Collections.Generic;
using TypeEnum;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class Card : MonoBehaviour
{
    //卡牌爆炸特效
    public GameObject cardBoomEffect;

    //卡牌上覆盖物数量
    public int coveredNumber;

    //卡牌的类型
    public CardTypeEnum cardType;

    //覆盖颜色
    public Color coverColor;

    //原来颜色
    public Color oldClor;

    //卡牌图片
    public Image cardTypeImage;

    //检测区域
    public RectTransform cardArea;

    //卡牌层级
    private int hierarchy;

    //主人
    private Transform owner;

    //是否为大卡牌
    public bool isBigCard;

    private RectTransform rectTransform;
    //移动的集中点
    public Vector3 center;
    //移动的时间
    public float moveToCenterTime;

   



    private void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        
        
        

        oldClor = GetComponent<Image>().color;
        owner = gameObject.transform.parent;
        //Debug.Log("Start开始执行");
        gameObject.GetComponent<Button>().onClick.AddListener(OnClick);

        Tween tween = rectTransform.DOMove(center,moveToCenterTime);
        tween.SetAutoKill(false);
        tween.Pause();

    }

    //玩家点击
    public void OnClick()
    {
        UIManager._instance.noClickPanel.SetActive(true);

        MusicManager._instance.OnPlayClickMusic();

        
        if (gameObject.transform.parent.GetComponent<SmallCardsPanel>())
        {
            gameObject.transform.parent.GetComponent<SmallCardsPanel>().RemoveCard(gameObject);

           

        }
        else
        {
            CardManager._instance.RemoveCard(gameObject);
        }

        //都是被点击了

        gameObject.GetComponent<Button>().enabled = false;

        //记录撤回物体位置
        CardManager._instance.returnCardPos = gameObject.transform.position;
        //记录撤回的父亲
        CardManager._instance.returnCardParent = gameObject.transform.parent.gameObject;

       // Debug.Log($"**********************************撤回的父亲是：{gameObject.transform.parent.gameObject.name}");
        //记录撤回物体的序号
        CardManager._instance.retunCardOrder = gameObject.transform.hierarchyCount;

        //移除总体中
        CardManager._instance.currentCards.Remove(gameObject);

        //移动卡牌
        StartCoroutine(MoveCardToPencilBox());

        

    }

    public IEnumerator MoveCardToPencilBox()
    {
        CardManager cardManager = CardManager._instance;

        //判断类型
        bool isMove = false;

        //插入位置
        int insert = 0;

        gameObject.transform.parent= CardManager._instance.pencilBox.transform;

        for (int i = cardManager.pencilBoxCards.Count - 1 ; i >= 0; i--)
        {
            if (cardManager.pencilBoxCards[i].GetComponent<Card>().cardType == this.cardType)
            {
                gameObject.transform.DOMove(cardManager.pencilBoxPoss[i + 1].position, 0.2f);
               
                yield return new WaitForSeconds(0.01f);
               
                insert = i + 1;
                //移动其他物体
                for (int j = cardManager.pencilBoxCards.Count-1; j > i; j--)
                {
                    isMove = true;
                    cardManager.pencilBoxCards[j].transform.DOMove(cardManager.pencilBoxPoss[j+1].position,0.15f);
                }
                break;
            }

        }
        if (!isMove)
        {
            //
            
            gameObject.transform.DOMove(cardManager.pencilBoxPoss[cardManager.currentPencilBoxPos].position, 0.2f);
            cardManager.pencilBoxCards.Add(gameObject);

            gameObject.transform.SetSiblingIndex(cardManager.currentPencilBoxPos);
        }
        else
        {
            cardManager.pencilBoxCards.Insert(insert,gameObject);

            gameObject.transform.SetSiblingIndex(insert);

            for (int i = insert + 1; i < cardManager.pencilBoxCards.Count; i++)
            {
                cardManager.pencilBoxCards[i].transform.SetSiblingIndex(i);
            }
        }
         
        cardManager.currentPencilBoxPos++;

        yield return new WaitForSeconds(0.2f);
       

        ClassifyCard(gameObject, insert);
    }

    //统计卡牌类型
    public void ClassifyCard(GameObject card,int insertPos)
    {
      

        Dictionary<CardTypeEnum, List<GameObject>> cardSlots= CardManager._instance.cardSlots;
        if (cardSlots.ContainsKey(card.GetComponent<Card>().cardType))
        {
            
            cardSlots[card.GetComponent<Card>().cardType].Add(card);
           

            //如果满足消除的条件
            if (cardSlots[card.GetComponent<Card>().cardType].Count >= 3)
            {

                MusicManager._instance.OnPlayDestroyMusic();

                //消除并移动
                Debug.Log("满足消除个数");
                int tempPos = CardManager._instance.currentPencilBoxPos;

                foreach (var v in cardSlots[card.GetComponent<Card>().cardType])
                {
                    //CardManager._instance.pencilBoxCards.Remove(v);
                    CardManager._instance.currentPencilBoxPos--;

                    Debug.Log($"正在删除元素{v.name}");
                    //播放销毁动画
                    Instantiate(v.GetComponent<Card>().cardBoomEffect, v.transform.position, Quaternion.identity, v.transform.parent);
                    Destroy(v);
                }
               

                //等待销毁动画1秒


                //---------------------------------------移动有问题(根本原因是删除layout里面的元素时会进行更新)

                Debug.Log($"是否插入检测 insertPos:{insertPos}  tempPos:{tempPos}");
                //如果是插入那么就移动
                if (insertPos != tempPos - 1)
                {
                    Debug.Log($"是插入 insertPos:{insertPos}  tempPos:{tempPos}");
                    int pos = insertPos + 1;
                    for (int i = pos; i < CardManager._instance.pencilBoxCards.Count; i++)
                    {
                        GameObject g = CardManager._instance.pencilBoxCards[i];
                        Debug.Log($"插入消除后要移动, 正在让{g.name}移动,移动坐标为:{i - 3}");
                        Sequence seq = DOTween.Sequence();
                        seq.AppendInterval(0.3f);
                        seq.Append(g.transform.DOMove(CardManager._instance.pencilBoxPoss[i - 3].position, 0.2f));


                    }
                }
                else
                {
                    //消失了但是不需要移动

                    if (insertPos == 2)
                    {
                        //消失不移动并且没有物体
                        UIManager._instance.moveButton.interactable = false;
                    }
                }

                //清理信息
                foreach (var v in cardSlots[card.GetComponent<Card>().cardType])
                {
                    CardManager._instance.pencilBoxCards.Remove(v);

                }

                //移除此类型文具盒中
                cardSlots.Remove(card.GetComponent<Card>().cardType);

                //消除了就被关闭掉
                UIManager._instance.returnButton.interactable = false;

                //判断游戏是否胜利
                CardManager._instance.GameResult();
            }
            else
            {
                //存在但是没有满足三
                CardManager._instance.returnCard = card; //记录可以撤回对象
                //撤回按钮可以被点击
                UIManager._instance.returnButton.interactable = true;
            }

            
        }
        else
        {
            //不存在增加新的类

            Debug.Log($"没有卡牌类:{card.GetComponent<Card>().cardType}");
            List<GameObject> gameObjects = new List<GameObject>();
            gameObjects.Add(gameObject);
            cardSlots.Add(card.GetComponent<Card>().cardType,gameObjects);

            CardManager._instance.returnCard = card;//记录可以撤回对象
            //撤回按钮可以被点击
            UIManager._instance.returnButton.interactable = true;
            //移除按钮被点亮（大于满足条件，但是包括其中）
            UIManager._instance.moveButton.interactable = true;

        }

        UIManager._instance.UpdateScore();
        UIManager._instance.noClickPanel.SetActive(false);

        //重新开始游戏
        if (CardManager._instance.pencilBoxCards.Count >= 7)
        {

            Debug.Log("游戏重新开始");
            UIManager._instance.ShowLoseGame();
           
        }
    }


    

    //玩家按下
    public void OnClickDown()
    {
        if (!gameObject.GetComponent<Button>().interactable ||!gameObject.GetComponent<Button>().enabled )
        {
            return;
        }

        Debug.Log("玩家按下");
        if (gameObject.transform.parent.GetComponent<BigCardsPanel>())
        {
        

            if (CardManager._instance.currentMode != LevelMode.HanBao)
            {
                hierarchy = gameObject.transform.hierarchyCount;

                int max = CardManager._instance.groupNumber - 1;
                gameObject.transform.parent = CardManager._instance.bigCardsPanels[max].transform;
                gameObject.transform.SetAsLastSibling();
            }
        
            
        }
        gameObject.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.2f);

    }

    //玩家抬起
    public void OnClickUp()
    {
        if (!gameObject.GetComponent<Button>().interactable || !gameObject.GetComponent<Button>().enabled)
        {
            return;
        }

        Debug.Log("玩家抬起");

        if (CardManager._instance.currentMode != LevelMode.HanBao)
        {
            if (gameObject.transform.parent.GetComponent<BigCardsPanel>())
            {
                gameObject.transform.parent = owner;
                gameObject.transform.SetSiblingIndex(hierarchy);
            }

        }

        gameObject.transform.DOScale(new Vector3(1f, 1f,1f), 0.2f);
       
    }

    
    //在销毁时
    private void OnDestroy()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
    }



    public void BeginRefresh()
    {
        StartCoroutine(MoveToCenter());
    }

    //正向播放
    public IEnumerator MoveToCenter()
    {

        rectTransform.DOPlayForward();

        yield return new WaitForSeconds(moveToCenterTime);

        rectTransform.DOPlayBackwards();

    }

    


}
