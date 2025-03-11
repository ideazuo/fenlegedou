using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TypeEnum;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //ϴ�ư�������ʱ�� 
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

    //��Ϸʤ�����
    public GameObject gameWinPanel;

    //ʧ�����
    public GameObject gameLosePanel;

    //��Ϸ���������
    public Text bigScore;

    //��Ϸ�ؿ�
    public Text titleText;
    
    //��Ϸ����
    public Text scoreText;
    //��Ϸʱ��
    public Text timeText;
    //��Ϸ�Ƴ�����
    public Text removeCountText;
    //��Ϸ��������
    public Text returnCountText;
    //��Ϸϴ������
    public Text refreshCountText;

    //������
    public GameObject noClickPanel;



    public static UIManager _instance;
    private void Awake()
    {
        _instance = this;
        Time.timeScale = 1;
    }

    //�û����ϴ�ƹ���
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
        //ִ��ϴ�ƵĶ���
        foreach (var v in CardManager._instance.currentCards)
        {
            v.GetComponent<Card>().BeginRefresh();
        }

        yield return new WaitForSeconds(refreshIntervel/2);

        MusicManager._instance.OnPlayRefreshCard();

        //��ʼϴ�Ƹ�ͼ
        CardManager._instance.RefreshCard();

        yield return new WaitForSeconds(refreshIntervel);

        refreshButton.interactable = true;
    }

    //�û�������ذ�ť
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
            //������ڴ󼯺��еĿ���
            cards = CardManager._instance.returnCardParent.GetComponent<BigCardsPanel>().currentCards;

            foreach (var v in CardManager._instance.returnCardCovers)
            {
                if (v.GetComponent<Card>().coveredNumber == 0)
                {
                    v.GetComponent<Button>().interactable = false;
                    //������ɫ����
                    v.GetComponent<Card>().cardTypeImage.color = v.GetComponent<Card>().coverColor;
                }
                v.GetComponent<Card>().coveredNumber++;

            }
            

            CardManager._instance.CoverToOthers.Add(card, CardManager._instance.returnCardCovers);
            cards.Add(card);
        }
        else
        {
            //�������С�����еĿ���
            cards = CardManager._instance.returnCardParent.GetComponent<SmallCardsPanel>().cards;

            foreach (var v in cards)
            {
                
                if (v.GetComponent<Card>().coveredNumber==0)
                {
                    v.GetComponent<Button>().interactable = false;
                    //������ɫ����
                    v.GetComponent<Card>().cardTypeImage.color = v.GetComponent<Card>().coverColor;
                }
                v.GetComponent<Card>().coveredNumber++;
            }

            //��ȡ������

            //��ӵ�������
            cards.Add(card);
        }

       

        //���׸���
        card.transform.parent = cardParent.transform;

        //��Ÿ���
        card.transform.SetSiblingIndex(cardOrder);

        bool isAdd = true;
        //�ж��Ƿ����Բ��ɸı�����
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
            //����������
            CardManager._instance.currentCards.Add(card);
        }
       

        //��ť������
        card.GetComponent<Button>().enabled = true;

        //�ƶ�
        card.transform.DOMove(cardPos,0.2f);

        int findIndex = CardManager._instance.pencilBoxCards.LastIndexOf(card);
        if (findIndex != CardManager._instance.pencilBoxCards.Count-1)
        {
            //�ƶ���������

            for (int i = findIndex+1; i < CardManager._instance.pencilBoxCards.Count; i++)
            {
                GameObject g = CardManager._instance.pencilBoxCards[i];
                Sequence seq = DOTween.Sequence();
                seq.Append(g.transform.DOMove(CardManager._instance.pencilBoxPoss[i - 1].position, 0.2f));


            }
        }

        //����Ǧ�ʺ��е�����
        CardManager._instance.pencilBoxCards.Remove(card);
        CardManager._instance.currentPencilBoxPos--;
        //��������
        Dictionary<CardTypeEnum, List<GameObject>> cardSlots = CardManager._instance.cardSlots;
        if (cardSlots.Count > 0)
        {
            cardSlots[card.GetComponent<Card>().cardType].Remove(card);
        }
        else
        {
            cardSlots.Remove(card.GetComponent<Card>().cardType);
        }

        //���ð�ť
        returnButton.interactable = false;
    }


    //�û�����Ƴ�
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

        //�����ƶ�����
        for (int i = 0; i < pencilBoxCards.Count; i++)
        {
            cards.Add(pencilBoxCards[i]);

            if (cards.Count == 3)
            {
                break;
            }
        }

        //�����ƶ�λ��
        foreach (var v in cards)
        {
            v.transform.DOMove(CardManager._instance.moveCardPanels[CardManager._instance.pointerMove].transform.position, 0.2f);
            CardManager._instance.pointerMove++;
            if (CardManager._instance.pointerMove >= 3)
            {
                CardManager._instance.pointerMove %= 3;
            }
        }
        
        //��ȫ���

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

            //����small
            cards[i].GetComponent<Card>().isBigCard = false;
           
            //��������



            //���ĺ���ָ��λ��
            CardManager._instance.currentPencilBoxPos--;
                      
        }
        
      

        //�ƶ��Ƴ���ʣ�������
        if (cards.Count != pencilBoxCards.Count)
        {
            //������ߵ�����������Ǧ�ʺ��е�������ô�Ϳ�ʼ�����ƶ�

            for (int i = cards.Count; i < pencilBoxCards.Count; i++)
            {
                GameObject g = pencilBoxCards[i];

                g.transform.DOMove(pencilBoxPoss[i - cards.Count].position, 0.2f);
            }
        }
        else
        {
            //�û������Ե���Ƴ�
            moveButton.interactable = false;

            //�û������Գ���
            returnButton.interactable = false;
        }

        //������Ϣ
        foreach (var v in cards)
        {
            CardTypeEnum cardType = v.GetComponent<Card>().cardType;
            if (CardManager._instance.cardSlots.ContainsKey(cardType))
            {
                //����ͳ����Ϣ
                CardManager._instance.cardSlots[cardType].Remove(v);

                if (CardManager._instance.cardSlots[cardType].Count == 0)
                {
                    CardManager._instance.cardSlots.Remove(cardType);
                }

                //���º�����Ϣ
                CardManager._instance.pencilBoxCards.Remove(v);

                //����������Ϣ
                v.GetComponent<Button>().enabled = true;
            }
            
        }
    }

    //���¿�ʼ��Ϸ

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
        removeCountText.text ="�Ƴ���" +moveText.text;
        returnCountText.text = "���أ�"+returnText.text;
        refreshCountText.text = "ϴ�ƣ�"+refreshText.text;

        gameLosePanel.SetActive(true);

    }

    public void UpdateScore()
    {
        bigScore.text = ((CardManager._instance.GetAllCardNumber()+CardManager._instance.GetPartCardNumber())/3).ToString();
    }
}
