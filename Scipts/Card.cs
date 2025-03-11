using System.Collections;
using System.Collections.Generic;
using TypeEnum;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class Card : MonoBehaviour
{
    //���Ʊ�ը��Ч
    public GameObject cardBoomEffect;

    //�����ϸ���������
    public int coveredNumber;

    //���Ƶ�����
    public CardTypeEnum cardType;

    //������ɫ
    public Color coverColor;

    //ԭ����ɫ
    public Color oldClor;

    //����ͼƬ
    public Image cardTypeImage;

    //�������
    public RectTransform cardArea;

    //���Ʋ㼶
    private int hierarchy;

    //����
    private Transform owner;

    //�Ƿ�Ϊ����
    public bool isBigCard;

    private RectTransform rectTransform;
    //�ƶ��ļ��е�
    public Vector3 center;
    //�ƶ���ʱ��
    public float moveToCenterTime;

   



    private void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        
        
        

        oldClor = GetComponent<Image>().color;
        owner = gameObject.transform.parent;
        //Debug.Log("Start��ʼִ��");
        gameObject.GetComponent<Button>().onClick.AddListener(OnClick);

        Tween tween = rectTransform.DOMove(center,moveToCenterTime);
        tween.SetAutoKill(false);
        tween.Pause();

    }

    //��ҵ��
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

        //���Ǳ������

        gameObject.GetComponent<Button>().enabled = false;

        //��¼��������λ��
        CardManager._instance.returnCardPos = gameObject.transform.position;
        //��¼���صĸ���
        CardManager._instance.returnCardParent = gameObject.transform.parent.gameObject;

       // Debug.Log($"**********************************���صĸ����ǣ�{gameObject.transform.parent.gameObject.name}");
        //��¼������������
        CardManager._instance.retunCardOrder = gameObject.transform.hierarchyCount;

        //�Ƴ�������
        CardManager._instance.currentCards.Remove(gameObject);

        //�ƶ�����
        StartCoroutine(MoveCardToPencilBox());

        

    }

    public IEnumerator MoveCardToPencilBox()
    {
        CardManager cardManager = CardManager._instance;

        //�ж�����
        bool isMove = false;

        //����λ��
        int insert = 0;

        gameObject.transform.parent= CardManager._instance.pencilBox.transform;

        for (int i = cardManager.pencilBoxCards.Count - 1 ; i >= 0; i--)
        {
            if (cardManager.pencilBoxCards[i].GetComponent<Card>().cardType == this.cardType)
            {
                gameObject.transform.DOMove(cardManager.pencilBoxPoss[i + 1].position, 0.2f);
               
                yield return new WaitForSeconds(0.01f);
               
                insert = i + 1;
                //�ƶ���������
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

    //ͳ�ƿ�������
    public void ClassifyCard(GameObject card,int insertPos)
    {
      

        Dictionary<CardTypeEnum, List<GameObject>> cardSlots= CardManager._instance.cardSlots;
        if (cardSlots.ContainsKey(card.GetComponent<Card>().cardType))
        {
            
            cardSlots[card.GetComponent<Card>().cardType].Add(card);
           

            //�����������������
            if (cardSlots[card.GetComponent<Card>().cardType].Count >= 3)
            {

                MusicManager._instance.OnPlayDestroyMusic();

                //�������ƶ�
                Debug.Log("������������");
                int tempPos = CardManager._instance.currentPencilBoxPos;

                foreach (var v in cardSlots[card.GetComponent<Card>().cardType])
                {
                    //CardManager._instance.pencilBoxCards.Remove(v);
                    CardManager._instance.currentPencilBoxPos--;

                    Debug.Log($"����ɾ��Ԫ��{v.name}");
                    //�������ٶ���
                    Instantiate(v.GetComponent<Card>().cardBoomEffect, v.transform.position, Quaternion.identity, v.transform.parent);
                    Destroy(v);
                }
               

                //�ȴ����ٶ���1��


                //---------------------------------------�ƶ�������(����ԭ����ɾ��layout�����Ԫ��ʱ����и���)

                Debug.Log($"�Ƿ������ insertPos:{insertPos}  tempPos:{tempPos}");
                //����ǲ�����ô���ƶ�
                if (insertPos != tempPos - 1)
                {
                    Debug.Log($"�ǲ��� insertPos:{insertPos}  tempPos:{tempPos}");
                    int pos = insertPos + 1;
                    for (int i = pos; i < CardManager._instance.pencilBoxCards.Count; i++)
                    {
                        GameObject g = CardManager._instance.pencilBoxCards[i];
                        Debug.Log($"����������Ҫ�ƶ�, ������{g.name}�ƶ�,�ƶ�����Ϊ:{i - 3}");
                        Sequence seq = DOTween.Sequence();
                        seq.AppendInterval(0.3f);
                        seq.Append(g.transform.DOMove(CardManager._instance.pencilBoxPoss[i - 3].position, 0.2f));


                    }
                }
                else
                {
                    //��ʧ�˵��ǲ���Ҫ�ƶ�

                    if (insertPos == 2)
                    {
                        //��ʧ���ƶ�����û������
                        UIManager._instance.moveButton.interactable = false;
                    }
                }

                //������Ϣ
                foreach (var v in cardSlots[card.GetComponent<Card>().cardType])
                {
                    CardManager._instance.pencilBoxCards.Remove(v);

                }

                //�Ƴ��������ľߺ���
                cardSlots.Remove(card.GetComponent<Card>().cardType);

                //�����˾ͱ��رյ�
                UIManager._instance.returnButton.interactable = false;

                //�ж���Ϸ�Ƿ�ʤ��
                CardManager._instance.GameResult();
            }
            else
            {
                //���ڵ���û��������
                CardManager._instance.returnCard = card; //��¼���Գ��ض���
                //���ذ�ť���Ա����
                UIManager._instance.returnButton.interactable = true;
            }

            
        }
        else
        {
            //�����������µ���

            Debug.Log($"û�п�����:{card.GetComponent<Card>().cardType}");
            List<GameObject> gameObjects = new List<GameObject>();
            gameObjects.Add(gameObject);
            cardSlots.Add(card.GetComponent<Card>().cardType,gameObjects);

            CardManager._instance.returnCard = card;//��¼���Գ��ض���
            //���ذ�ť���Ա����
            UIManager._instance.returnButton.interactable = true;
            //�Ƴ���ť�������������������������ǰ������У�
            UIManager._instance.moveButton.interactable = true;

        }

        UIManager._instance.UpdateScore();
        UIManager._instance.noClickPanel.SetActive(false);

        //���¿�ʼ��Ϸ
        if (CardManager._instance.pencilBoxCards.Count >= 7)
        {

            Debug.Log("��Ϸ���¿�ʼ");
            UIManager._instance.ShowLoseGame();
           
        }
    }


    

    //��Ұ���
    public void OnClickDown()
    {
        if (!gameObject.GetComponent<Button>().interactable ||!gameObject.GetComponent<Button>().enabled )
        {
            return;
        }

        Debug.Log("��Ұ���");
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

    //���̧��
    public void OnClickUp()
    {
        if (!gameObject.GetComponent<Button>().interactable || !gameObject.GetComponent<Button>().enabled)
        {
            return;
        }

        Debug.Log("���̧��");

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

    
    //������ʱ
    private void OnDestroy()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
    }



    public void BeginRefresh()
    {
        StartCoroutine(MoveToCenter());
    }

    //���򲥷�
    public IEnumerator MoveToCenter()
    {

        rectTransform.DOPlayForward();

        yield return new WaitForSeconds(moveToCenterTime);

        rectTransform.DOPlayBackwards();

    }

    


}
