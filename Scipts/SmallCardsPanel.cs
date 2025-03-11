using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TypeEnum;

public class SmallCardsPanel : MonoBehaviour
{
    //---------------------------����Ϊ������������������------------------
    //��ʾ���Ҫ��Ŷ��ٸ�����
    public int cardsNumber;

    //�Ƿ�ʼ���ģʽ
    public bool isLapMode;

    //����֮��ļ��
    public int cardInterval;

    //�����������
    public int lapCardsP;
    public int lapOneCardP;

    //������巽��
    public CardDirection panelDirection;

    //��ʾ���㼶(�㼶Խ�ͣ�Խ�ڵײ�)
    public int level;  //Ĭ��Ϊ0

    //����Ԥ����
    public GameObject cardPrefab;

    //������
    public int panelOrder;


    //--------------------------����Ϊ��DIY�����������----------------------------
    public List<GameObject> cards;

    //---------------------------����Ϊ˽������-------------------------------------

    //��ӿ���ǰ����
    private int addPreCount;

    //����ʱ��ͽ��й���
    private void Start()
    {
        Init();
    }
    private void Awake()
    {
        
    }
    //��ʼ��
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

    //��ӿ���
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

        // Debug.Log($"������ӹ̶����Ԫ��*******************************************{cardsNumber}");
      
        for (int i = addPreCount; i < cardsNumber; i++)
        {
            GameObject g = Instantiate(cardPrefab,gameObject.transform);

           

            g.GetComponent<Card>().isBigCard = false;
            cards.Add(g);

            if (addPreCount != 0)
            {
                UpdateCoverMessage(cards[cards.Count - 2],i-1);
                
                
            }

            //���е��������ּ���
            if (i + 1 != cardsNumber)
            {
                UpdateCoverMessage(g,i);
            }


        }
        
        addPreCount = cards.Count;

        
    }


    //�ص���ӿ���
    public void OverLapAddCard(int addCount)
    {


        // Debug.Log($"������ӹ̶����Ԫ��*******************************************{cardsNumber}");
        Vector3 vector = new Vector3();
        for (int i = 0; i < addCount; i++)
        {
 
            GameObject g = Instantiate(cardPrefab, gameObject.transform);

            g.transform.localPosition = vector;

           

            g.GetComponent<Card>().isBigCard = false;
            cards.Add(g);


            //���е��������ּ���
            if (i + 1 != addCount)
            {
                UpdateCoverMessage(g, i);
            }


            //�ƶ�
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
                        Debug.Log($"���·���:{vector}");
                        vector -= new Vector3(0, cardInterval);
                        break;
                    }

                default:
                    break;
            }


        }

    }

    //�����ڸ���Ϣ
    public void UpdateCoverMessage(GameObject g,int orderNumber)
    {
        //����(���)���ǵ���Ϣ�����˸���
        g.GetComponent<Card>().coveredNumber = cardsNumber - orderNumber - 1;   //������ȫ���ϸ��ǹ���

        //������ɫ����
        g.GetComponent<Card>().cardTypeImage.color = g.GetComponent<Card>().coverColor;

        //���в��ɵ������
        g.GetComponent<Button>().interactable = false;
    }


    //���ӿ���(����)
    public void AddCard(GameObject card)
    {
        //1���ȱ��������������ڸ�
        foreach (var v in cards)
        {
            if (v.GetComponent<Card>().coveredNumber == 0)
            {              
                //������ɫ����
                v.GetComponent<Card>().cardTypeImage.color = v.GetComponent<Card>().coverColor;

                //���в��ɵ������
                v.GetComponent<Button>().interactable = false;
            }
           
            v.GetComponent<Card>().coveredNumber++;
        }

        //2�������Լ�
        cards.Add(card);
    }

    public void RemoveCard(GameObject g)
    {
        Debug.Log($"ɾ�����壺{g.name}");
        foreach (var v in  cards)
        {
            Debug.Log("�Ƴ���");
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
