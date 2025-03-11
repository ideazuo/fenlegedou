using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TypeEnum;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //洗牌按键禁用时间 
    public float refreshIntervel;

    public Button moveButton;
    public Button returnButton;
    public Button refreshButton;

    public Text moveText;
    public Text returnText;
    public Text refreshText;

    public int moveCount;
    public int returnCount;
    public int refreshCount;

    //游戏胜利面板
    public GameObject gameWinPanel;

    //失败面板
    public GameObject gameLosePanel;

    //游戏分数大面板
    public Text bigScore;

    //游戏关卡
    public Text titleText;
    
    //游戏分数
    public Text scoreText;
    //游戏时间
    public Text timeText;
    //游戏移除数量
    public Text removeCountText;
    //游戏撤回数量
    public Text returnCountText;
    //游戏洗牌数量
    public Text refreshCountText;

    //禁用区
    public GameObject noClickPanel;



    public static UIManager _instance;
    private void Awake()
    {
        _instance = this;
        Time.timeScale = 1;
    }

    //用户点击洗牌功能
    public void OnClickRefreshCard()
    {
        refreshCount++;
        if (refreshCount > 99)
        {
            refreshCount = 99;
        }
        refreshText.text = refreshCount.ToString();
       
        StartCoroutine(RefreshCard());
    }

    public IEnumerator RefreshCard()
    {
        refreshButton.interactable = false;

        MusicManager._instance.OnPlayRefreshCard();
        //执行洗牌的动画
        foreach (var v in CardManager._instance.currentCards)
        {
            v.GetComponent<Card>().BeginRefresh();
        }

        yield return new WaitForSeconds(refreshIntervel/2);

        MusicManager._instance.OnPlayRefreshCard();

        //开始洗牌附图
        CardManager._instance.RefreshCard();

        yield return new WaitForSeconds(refreshIntervel);

        refreshButton.interactable = true;
    }

    //用户点击撤回按钮
    public void OnClickRerunCard()
    {
        returnCount++;
        if (returnCount > 99)
        {
            returnCount = 99;
        }
        returnText.text = returnCount.ToString();

        MusicManager._instance.OnPlayClickMusic();

        GameObject card = CardManager._instance.returnCard;
        GameObject cardParent = CardManager._instance.returnCardParent;
        int cardOrder = CardManager._instance.retunCardOrder;
        Vector3 cardPos = CardManager._instance.returnCardPos;
        List<GameObject> cards = new List<GameObject>();
        if (card.GetComponent<Card>().isBigCard)
        {
            //如果是在大集合中的卡牌
            cards = CardManager._instance.returnCardParent.GetComponent<BigCardsPanel>().currentCards;

            foreach (var v in CardManager._instance.returnCardCovers)
            {
                if (v.GetComponent<Card>().coveredNumber == 0)
                {
                    v.GetComponent<Button>().interactable = false;
                    //进行颜色处理
                    v.GetComponent<Card>().cardTypeImage.color = v.GetComponent<Card>().coverColor;
                }
                v.GetComponent<Card>().coveredNumber++;

            }
            

            CardManager._instance.CoverToOthers.Add(card, CardManager._instance.returnCardCovers);
            cards.Add(card);
        }
        else
        {
            //如果是在小集合中的卡牌
            cards = CardManager._instance.returnCardParent.GetComponent<SmallCardsPanel>().cards;

            foreach (var v in cards)
            {
                
                if (v.GetComponent<Card>().coveredNumber==0)
                {
                    v.GetComponent<Button>().interactable = false;
                    //进行颜色处理
                    v.GetComponent<Card>().cardTypeImage.color = v.GetComponent<Card>().coverColor;
                }
                v.GetComponent<Card>().coveredNumber++;
            }

            //获取到集合

            //添加到集合中
            cards.Add(card);
        }

       

        //父亲更换
        card.transform.parent = cardParent.transform;

        //序号更换
        card.transform.SetSiblingIndex(cardOrder);

        bool isAdd = true;
        //判断是否来自不可改变区域
        foreach(var v in CardManager._instance.moveCardPanels)
        {
            if (v.name == card.transform.parent.name)
            {
                isAdd = false;
               
                break;
            }
        }

        if (isAdd)
        {
            //总体中增加
            CardManager._instance.currentCards.Add(card);
        }
       

        //按钮被激活
        card.GetComponent<Button>().enabled = true;

        //移动
        card.transform.DOMove(cardPos,0.2f);

        int findIndex = CardManager._instance.pencilBoxCards.LastIndexOf(card);
        if (findIndex != CardManager._instance.pencilBoxCards.Count-1)
        {
            //移动其他物体

            for (int i = findIndex+1; i < CardManager._instance.pencilBoxCards.Count; i++)
            {
                GameObject g = CardManager._instance.pencilBoxCards[i];
                Sequence seq = DOTween.Sequence();
                seq.Append(g.transform.DOMove(CardManager._instance.pencilBoxPoss[i - 1].position, 0.2f));


            }
        }

        //清理铅笔盒中的物体
        CardManager._instance.pencilBoxCards.Remove(card);
        CardManager._instance.currentPencilBoxPos--;
        //清理类型
        Dictionary<CardTypeEnum, List<GameObject>> cardSlots = CardManager._instance.cardSlots;
        if (cardSlots.Count > 0)
        {
            cardSlots[card.GetComponent<Card>().cardType].Remove(card);
        }
        else
        {
            cardSlots.Remove(card.GetComponent<Card>().cardType);
        }

        //禁用按钮
        returnButton.interactable = false;
    }


    //用户点击移除
    public void OnClickMoveCard()
    {
        moveCount++;
        if (moveCount > 99)
        {
            moveCount = 99;
        }
        moveText.text = moveCount.ToString();

        MusicManager._instance.OnPlayClickMusic();

        StartCoroutine(MoveCard());
    }


    public IEnumerator MoveCard()
    {
        List<GameObject> cards = new List<GameObject>();

        List<GameObject> pencilBoxCards = CardManager._instance.pencilBoxCards;

        List<RectTransform> pencilBoxPoss = CardManager._instance.pencilBoxPoss;

        //加载移动对象
        for (int i = 0; i < pencilBoxCards.Count; i++)
        {
            cards.Add(pencilBoxCards[i]);

            if (cards.Count == 3)
            {
                break;
            }
        }

        //分配移动位置
        foreach (var v in cards)
        {
            v.transform.DOMove(CardManager._instance.moveCardPanels[CardManager._instance.pointerMove].transform.position, 0.2f);
            CardManager._instance.pointerMove++;
            if (CardManager._instance.pointerMove >= 3)
            {
                CardManager._instance.pointerMove %= 3;
            }
        }
        
        //完全添加

        yield return new WaitForSeconds(0.05f);

        int teamPoint = CardManager._instance.pointerMove;
        
        for (int i = cards.Count-1; i >=0; i--)
        {
            teamPoint--;
            if (teamPoint < 0)
            {
                teamPoint = 2;
            }

            CardManager._instance.moveCardPanels[teamPoint].GetComponent<SmallCardsPanel>().AddCard(cards[i]);
            
            cards[i].transform.parent=CardManager._instance.moveCardPanels[teamPoint].transform;

            //设置small
            cards[i].GetComponent<Card>().isBigCard = false;
           
            //开启布局



            //更改盒子指针位置
            CardManager._instance.currentPencilBoxPos--;
                      
        }
        
      

        //移动移除后剩余的物体
        if (cards.Count != pencilBoxCards.Count)
        {
            //如果拿走的数量不等于铅笔盒中的数量那么就开始进行移动

            for (int i = cards.Count; i < pencilBoxCards.Count; i++)
            {
                GameObject g = pencilBoxCards[i];

                g.transform.DOMove(pencilBoxPoss[i - cards.Count].position, 0.2f);
            }
        }
        else
        {
            //用户不可以点击移除
            moveButton.interactable = false;

            //用户不可以撤回
            returnButton.interactable = false;
        }

        //更新信息
        foreach (var v in cards)
        {
            CardTypeEnum cardType = v.GetComponent<Card>().cardType;
            if (CardManager._instance.cardSlots.ContainsKey(cardType))
            {
                //更新统计信息
                CardManager._instance.cardSlots[cardType].Remove(v);

                if (CardManager._instance.cardSlots[cardType].Count == 0)
                {
                    CardManager._instance.cardSlots.Remove(cardType);
                }

                //更新盒子信息
                CardManager._instance.pencilBoxCards.Remove(v);

                //更新自身信息
                v.GetComponent<Button>().enabled = true;
            }
            
        }
    }

    //重新开始游戏

    public void OnClickAgainGame()
    {
        SceneManager.LoadScene(CardManager._instance.currentMode.ToString());
    }

    public void OnClickRetunMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ShowLoseGame()
    {
        Time.timeScale = 0;

        titleText.text =GameUIManager._instance.modeNameText.text;
        scoreText.text =bigScore.text;
        timeText.text = GameUIManager._instance.gameTimeText.text;
        removeCountText.text ="移除：" +moveText.text;
        returnCountText.text = "撤回："+returnText.text;
        refreshCountText.text = "洗牌："+refreshText.text;

        gameLosePanel.SetActive(true);

    }

    public void UpdateScore()
    {
        bigScore.text = ((CardManager._instance.GetAllCardNumber()+CardManager._instance.GetPartCardNumber())/3).ToString();
    }
}
