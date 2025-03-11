using System;
using System.Collections;
using System.Collections.Generic;
using TypeEnum;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;





public class CardManager : MonoBehaviour
{
    public LevelMode currentMode;//��ǰ�ؿ�ģʽ
    public int[] GoldControl;//���ƽ������Ѷ� 
    public int tempLevel;//��ʱ�Ĳ㼶
    public List<Transform> hanBaoPanels;//�������������
    public int currentCardNumber;//�ƾ��Ͽ��Ƶ�����
    public Transform panel;
    public Transform mask;
    public GameObject cardPrefab; //���Ƶ�Ԥ����
    public List<Sprite> cardTypes; //��������ͼƬ�ļ���
    public int groupNumber;        //�����������
    public int rectNumber;  //���������
    public int rect_1_Number;//1*1��������
    public int rect_2_Number;//2*2��������
    public int rect_3_Number;//3*3��������
    public int rect_4_Number;//4*4��������
    public GameObject bigCardsPanelPrefab;//�����Ԥ����
    public List<GameObject> bigCardsPanels;//����弯��
    public List<GameObject> smallCardsPanesls;//С��弯��
    public List<GameObject> currentCards;  //ͬ�������Ϸ�ϵ����п���
    public List<RectTransform> pencilBoxPoss;//��ŵ�λ��
    public List<GameObject> pencilBoxCards;//��ŵĿ���
    public GameObject pencilBox;//����
    public Transform slotsPanel;
    public GameObject cardItem;

    public float cardBoundX;
    public float cardBoundY;

    private GameObject currentPanel;//��ǰ�������

    public int currentPencilBoxPos;//Ҫ��ŵĿ��Ƶ�λ��
   
    private List<GameObject> panelCardSlots;

    public List<GameObject> moveCardPanels;  //�Ƴ����
    public int pointerMove;//�Ƴ�ָ��

    int currentRow;
    int currentLine;



    public Dictionary<GameObject, List<GameObject>> CoverToOthers; //�����������帲������������ֵ�

    public Dictionary<CardTypeEnum, List<GameObject>> cardSlots;   //ͳ��ÿ���������͸���

    //���������λ��
    public Vector3 returnCardPos;
    //��������ĸ�����
    public GameObject returnCardParent;
    //�������������
    public int retunCardOrder;
    //��������ĸ�����
    public List<GameObject> returnCardCovers;

    //��������
    public GameObject returnCard;


    public static CardManager _instance;


    private void Awake()
    {
        //��ʼ����Ϣ

        _instance = this;

        Init();

       

        switch (currentMode)
        {
            case LevelMode.Normal:
                {
                    SendCardPanel();
                    break;
                }  
            case LevelMode.Gold:
                {
                    groupNumber = GetPartGoldPanel(GoldControl.Length);
                    SendCardPanelGold();
                    break;
                }
                
            case LevelMode.HanBao:
                {
                    SendCardPanelHanBao();
                    break;
                }
            case LevelMode.Bamboo:
                {
                    
                    break;
                }
            case LevelMode.BlindBox:
                {
                    
                    break;
                }
            case LevelMode.Lap:
                {

                    break;
                }
            default:
                break;
        }

      
               
        

        //���ö���,���ҽ����˶Գ��㷨��������ʣ������

        Debug.Log($"���ò���Э��ǰ--------------------------------------------------");
        StartCoroutine("WaitEnabled");  //������������ӳٵ���
        Debug.Log($"���ò���Э�̺�--------------------------------------------------");
       
    }

    //��ͬ�ķ��ƻ���
    public void SendCardPanelGold()
    {
     

        for (int i = 0; i < groupNumber; i++)
        {
           
            CreateBigCardsPanelGold(i, GetClassLevel(i));        

        }
    }

    //�������ƻ���
    public void SendCardPanelHanBao()
    {
        for (int j = 0; j < hanBaoPanels.Count; j++)
        {
            for (int i = 0; i < hanBaoPanels[j].gameObject.GetComponent<HanBao>().number; i++)
            {

                int one = hanBaoPanels[j].gameObject.GetComponent<HanBao>().oneNub;
                int two = hanBaoPanels[j].gameObject.GetComponent<HanBao>().twoNum;

                int r = UnityEngine.Random.Range(0,one+two);

                int cRect = 0;

                if (i + 1 == hanBaoPanels[j].gameObject.GetComponent<HanBao>().number)
                {
                    if (hanBaoPanels[j].gameObject.GetComponent<HanBao>().isEndOne)
                    {
                        cRect = 6;
                    }
                    else
                    {
                        cRect = 5;
                    }
                }
                else
                {

                    if (i == 0)
                    {
                        cRect = 5;
                    }
                    else
                    {
                        if (r < one)
                        {
                            cRect = 6;
                        }
                        else
                        {
                            cRect = 5;
                        }
                    }
                 
                }

                CreateBigCardsPanelHanBao(i, cRect, hanBaoPanels[j]);

            }
        }      
    }

    public void SendCardPanel()
    {
        for (int i = 0; i < groupNumber; i++)
        {

            CreateBigCardsPanel(i);

        }
    }

    //��ȡ�ü��Ĳ���
    public int GetClassLevel(int num)
    {
        int sum = 0;
        int count = 0;
        foreach (int n in GoldControl)
        {
            sum += n;

            if (num < sum)
            {
                Debug.Log($"num:{num}    sum:{sum}  {GoldControl.Length/2}");
                if (num < GetPartGoldPanel(GoldControl.Length/2))
                {
                    Debug.Log($"ǰ18��:{6 - count}");
                    return 6 - count;
                }
                else
                {
                    return count%7;
                }                          
            }

            count++;
        }

        return 0;
    }

    //��ȡ��岿�ֺ�
    public int GetPartGoldPanel(int end)
    {
        int sum = 0;

        for (int i = 0; i < end; i++)
        {
            sum += GoldControl[i];
        }

        Debug.Log($"�õ�һ��ĺ�Ϊ��{sum}");
        return sum;
    }

    
    public void Start()
    {
        StartCoroutine(EnterGame());
        
    }

    public IEnumerator EnterGame()
    {
        
        panel.transform.DOLocalMoveY(-mask.localPosition.y+80, 2f, true);
        yield return new WaitForSecondsRealtime(2f);
        CloseAllCardButton(false);
    }

    void CloseLayouts()
    {
        foreach (var g in bigCardsPanels)
        {
            g.GetComponent<GridLayoutGroup>().enabled = false;

        }      
    }


    /// <summary>
    /// ��ʼ����Ϸ��ʼʱ��Ϣ
    /// </summary>
    public void Init()
    {
       
        currentCards = new List<GameObject>();
        CoverToOthers = new Dictionary<GameObject, List<GameObject>>();
        cardSlots = new Dictionary<CardTypeEnum, List<GameObject>>();
        bigCardsPanels = new List<GameObject>();
        pencilBoxCards = new List<GameObject>();
             
    }


    public void CreateBigCardsPanelGold(int level,int levelClass)
    {
        Debug.Log("���������");
        Vector3 vector = bigCardsPanelPrefab.transform.position;

        GameObject g = Instantiate(bigCardsPanelPrefab, panel, false);

        g.name = "BigCardsPanel_" + level.ToString();


        //��������ģʽ
        g.GetComponent<BigCardsPanel>().IsRect = true;

        //�ü�

        int spx = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.x;
        int spy = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.y;
        int sizeX = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.x + spx;
        int sizeY = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.y + spy;

        //����ط��޸�
        g.GetComponent<RectTransform>().sizeDelta -= new Vector2(sizeX * levelClass, sizeY * levelClass);

        //����row
        currentLine = currentRow = 7 - levelClass;
        g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 7 - levelClass;



        bigCardsPanels.Add(g);


        //2���ü�   ɾ�������������
        ClipPanelSize(g, (currentLine * currentRow));


        //3������ڶ�ά������һ�ݣ����ڶԳ��ϣ�
        g.GetComponent<BigCardsPanel>().UpdateCardArray(currentRow);  //������⣬��Ž�ȥ��




        g.GetComponent<BigCardsPanel>().level = level;

        currentPanel = g;

    }

    public void CreateBigCardsPanelHanBao(int level, int levelClass,Transform parentPanel)
    {
        Debug.Log("���������");

        GameObject g = Instantiate(bigCardsPanelPrefab, parentPanel, false);

        g.name = "BigCardsPanel_" + level.ToString();


        //��������ģʽ
        g.GetComponent<BigCardsPanel>().IsRect = true;

        //�ü�

        int spx = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.x;
        int spy = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.y;
        int sizeX = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.x + spx;
        int sizeY = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.y + spy;

        //����ط��޸�
        g.GetComponent<RectTransform>().sizeDelta -= new Vector2(sizeX * levelClass, sizeY * levelClass);

        //����row
        currentLine = currentRow = 7 - levelClass;
        g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 7 - levelClass;



        bigCardsPanels.Add(g);


        //2���ü�   ɾ�������������
        ClipPanelSize(g, (currentLine * currentRow));


        //3������ڶ�ά������һ�ݣ����ڶԳ��ϣ�
        g.GetComponent<BigCardsPanel>().UpdateCardArray(currentRow);  //������⣬��Ž�ȥ��




        g.GetComponent<BigCardsPanel>().level = level;

        currentPanel = g;

    }

    public void CreateBigCardsPanel(int level)
    {
        Debug.Log("���������");
        Vector3 vector = bigCardsPanelPrefab.transform.position;

        GameObject g = Instantiate(bigCardsPanelPrefab, panel, false);

        g.name = "BigCardsPanel_" + level.ToString();


        //��ʶ���һ��
        if (level + 1 == this.groupNumber)
        {
            g.GetComponent<BigCardsPanel>().IsEnd = true;
        }

        Debug.Log(vector);

        //���и��ʿ���


        if (level < rectNumber)
        {
            g.GetComponent<BigCardsPanel>().IsRect = true;

            int r = UnityEngine.Random.Range(0, rectNumber);

            if (r < rect_1_Number)
            {
                //����1*1����

                g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 1;

            }
            else if (r < rect_1_Number + rect_2_Number)
            {
                //����2*2����
                g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 2;

            }
            else if (r < rect_1_Number + rect_2_Number + rect_3_Number)
            {
                //����3*3����
                g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 3;
            }
            else
            {
                //����4*4����
                g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 4;
            }

        }




        //1���������λ�ã��;�����״��1.���Ұڶ���2.��߲��ֲü� 3.curRow-lIne��ֵ��
        vector = NewRandomPanelPos(g);
        g.transform.localPosition = vector;

        bigCardsPanels.Add(g);

       
        //2���ü�   ɾ�������������
        ClipPanelSize(g, (currentLine * currentRow));


        //3������ڶ�ά������һ�ݣ����ڶԳ��ϣ�
        g.GetComponent<BigCardsPanel>().UpdateCardArray(currentRow);  //������⣬��Ž�ȥ��
        
       


        g.GetComponent<BigCardsPanel>().level = level;

        currentPanel = g;

    }

    public IEnumerator WaitEnabled()//Э��
    {     
        //�ȴ�֡������Ⱦ����
        yield return new WaitForEndOfFrame();
        Debug.Log($"------------------���ڽ��ò������------------");

        //�رղ���(�����)
        CloseLayouts();

        //�ȴ�֡������Ⱦ����
        yield return new WaitForEndOfFrame();

        //���жԳ�(�����ͨģʽ,�������Rect��ִ��)
        AllLeftAndRight(bigCardsPanels);

        //���������ڵ���ϵ(������Ǵ���弯�ϵ�����)
        OrderCoverLap();

        //ͳ�Ƶ�ǰ�������(������С���)
        currentCardNumber = GetAllCardNumber();

        //�޲���������(С��������ٴ���һ��)
        CorrectCardNumber(currentCardNumber);

        //��ʼ���д���(������С���,card��ӵ�ǰ������)
        ReCombine();

        //����button(��ǰ������)
        CloseAllCardButton(true);

        //��ʼ���з�����ͼ(��ǰ������)
        AddCardTypeImage();

        yield return new WaitForEndOfFrame();
        //�رգ����Ǧ�ʺеģ�
        CloseSmallLayout();

        UIManager._instance.UpdateScore();
    }

    

    //�ر�С���ֺ����򲼾�
    public void CloseSmallLayout()
    {
        foreach (var v in smallCardsPanesls)
        {
            if (v.GetComponent<HorizontalLayoutGroup>())              
            {
                v.GetComponent<HorizontalLayoutGroup>().enabled = false;
                continue;
            }

            if (v.GetComponent<VerticalLayoutGroup>())
            {
                v.GetComponent<VerticalLayoutGroup>().enabled = false;
            }
        }

        pencilBox.GetComponent<HorizontalLayoutGroup>().enabled=false;
    }

    //ƥ���ڵ�����
    public void OrderCoverLap()
    {
        //Debug.Log($"���ڽ����ڵ�����*********************��ʱ�������{bigCardsPanels.Count}");

        for (int i = 1; i < bigCardsPanels.Count; i++)
        {


            foreach (var v in bigCardsPanels[i].GetComponent<BigCardsPanel>().currentCards)
            {
                List<GameObject> allCards = new List<GameObject>();
                for (int j = 0; j < i; j++)
                {
                    allCards.AddRange(bigCardsPanels[j].GetComponent<BigCardsPanel>().currentCards);
                }


                OverLap(v, allCards);
            }

        }
    }

    //ͳ�Ƶ�ǰ�������
    public int GetAllCardNumber()
    {
        int sum = 0;

        //������������濨������
        foreach (var v in bigCardsPanels)
        {
            sum+= v.GetComponent<BigCardsPanel>().currentCards.Count;
        }

        foreach (var v in smallCardsPanesls)
        {
            sum += v.GetComponent<SmallCardsPanel>().cards.Count;
        }

        return sum;
    }

    public int GetPartCardNumber()
    {
        int sum = 0;

        //������������濨������
        foreach (var v in moveCardPanels)
        {
            sum += v.GetComponent<SmallCardsPanel>().cards.Count;
        }

        Debug.Log($"sumû�м���Ǧ�ʺе�ʱ��:{sum}");

        sum += pencilBoxCards.Count;

        Debug.Log($"sum����Ǧ�ʺе�ʱ��:{sum}");
        return sum;
    }

    //�޲���������
    public void CorrectCardNumber(int currentNumber)
    {
        Debug.Log($"���ڿ���{currentCardNumber}");
        int n = currentCardNumber % 3;
        if (n == 0)
        {
            return;
        }
        else if (n == 2)
        {
     
            //����һ��
            
            Debug.Log($"���ڲ���1������");
            smallCardsPanesls[0].GetComponent<SmallCardsPanel>().AddCard(1, false);
       
            
            currentCardNumber += 1;
        }
        else
        {
            //��������
            Debug.Log($"���ڲ���2������");
            if (smallCardsPanesls.Count == 0)
            {
                Debug.LogError("������С��壬�޷���������");
            }
            else if (smallCardsPanesls.Count == 1)
            {
                smallCardsPanesls[0].GetComponent<SmallCardsPanel>().AddCard(2, false);
            }
            else 
            {
                smallCardsPanesls[0].GetComponent<SmallCardsPanel>().AddCard(1, false);
                smallCardsPanesls[1].GetComponent<SmallCardsPanel>().AddCard(1, false);
            }
           
            currentCardNumber += 2;
        }
    }

    //��ʼ���в�����
    public void ReCombine()
    {
        List<GameObject> tempGroups = new List<GameObject>();

        foreach (var v in bigCardsPanels)
        {
            tempGroups.AddRange(v.GetComponent<BigCardsPanel>().currentCards);
        }

        foreach (var v in smallCardsPanesls)
        {
            tempGroups.AddRange(v.GetComponent<SmallCardsPanel>().cards);
        }

        while (tempGroups.Count != 0)
        {
            int r = UnityEngine.Random.Range(0,tempGroups.Count);

            currentCards.Add(tempGroups[r]);
            tempGroups.RemoveAt(r);
        }

        Debug.Log($"���Һ���Ϸ���Ƶ��ܹ�����Ϊ:{currentCards.Count}");
    }

    //��ӿ�������ͼƬ
    public void AddCardTypeImage()
    {
        int count = 0;
        int randomType=0;
        foreach (var g in currentCards)
        {
            if (count % 3 == 0)
            {
                randomType = UnityEngine.Random.Range(0, cardTypes.Count);
                count = 0;
            }

            g.GetComponent<Card>().cardTypeImage.sprite = cardTypes[randomType];
            g.GetComponent<Card>().cardType = (CardTypeEnum)Enum.Parse(typeof(CardTypeEnum), cardTypes[randomType].name);

            count++;       
        }      
    }

    //����button
    public void CloseAllCardButton(bool isClose)
    {
      

        UIManager._instance.noClickPanel.SetActive(isClose);
    }
  
    //���жԳ�
    public void AllLeftAndRight(List<GameObject> panels)
    {
        foreach (var v in panels)
        {

            if (v.GetComponent<BigCardsPanel>().IsRect)
            {
                SaveCurrentRectCard(v);
                continue;
            }
           

            int r = UnityEngine.Random.Range(0,2);

            if (r == 0)
            {
                //�����Գ�
                LeftAndRight(v);
            }
            else
            {
                //�����Գ�
                UpAndDown(v);
            }
           
        }
    }

    //�����ɺõľ�����б���
    public void SaveCurrentRectCard(GameObject g)
    {
        foreach (var v in g.GetComponent<BigCardsPanel>().cardArrays)
        {
            g.GetComponent<BigCardsPanel>().currentCards.Add(v);
        }
    }

    public void LeftAndRight(GameObject g)
    {
        int maxX = g.GetComponent<BigCardsPanel>().cardArrays.GetLength(0);
        int maxY = g.GetComponent<BigCardsPanel>().cardArrays.GetLength(1);
        Debug.Log($"----------------------�������ҶԳ�ɾ����--------------------Row:{maxY} Line{maxY}");
        for (int i = 0; i < maxY; i++)
        {
            for (int j = 0; j < (maxX+1)/2; j++)
            {             
                int r = UnityEngine.Random.Range(0,2);

                if (r == 0)
                {
                    
                    Destroy(g.GetComponent<BigCardsPanel>().cardArrays[i, j]);
                    if (j != (maxX - 1 - j))
                    {
                        Destroy(g.GetComponent<BigCardsPanel>().cardArrays[i, maxX - 1 - j]);
                    }

                }
                else
                {
                    g.GetComponent<BigCardsPanel>().currentCards.Add(g.GetComponent<BigCardsPanel>().cardArrays[i, j]);
                    if (j != (maxX - 1 - j))
                    {
                        g.GetComponent<BigCardsPanel>().currentCards.Add(g.GetComponent<BigCardsPanel>().cardArrays[i, maxX - 1 - j]);
                    }                                      
                }
            }
        }

        Debug.Log("---------------------------���ҶԳ�ɾ����----------------");
        foreach (var v in g.GetComponent<BigCardsPanel>().cardArrays)
        {
            if (v.activeInHierarchy==true)
            {
                
            }           
        }
    }

    public void UpAndDown(GameObject g)
    {
        int maxX = g.GetComponent<BigCardsPanel>().cardArrays.GetLength(0);
        int maxY = g.GetComponent<BigCardsPanel>().cardArrays.GetLength(1);
        Debug.Log($"----------------------���ڡ����¡��Գ�ɾ����--------------------Row:{maxY} Line{maxY}");
        for (int i = 0; i < (maxY+1)/2; i++)
        {
            for (int j = 0; j < maxX ; j++)
            {
                int r = UnityEngine.Random.Range(0, 2);
                if (r == 0)
                {
                    Destroy(g.GetComponent<BigCardsPanel>().cardArrays[i, j]);
                    if (i != (maxY - 1 - i))
                    {
                        Destroy(g.GetComponent<BigCardsPanel>().cardArrays[maxY - 1 - i, j]);
                    }
                }
                else
                {
                    g.GetComponent<BigCardsPanel>().currentCards.Add(g.GetComponent<BigCardsPanel>().cardArrays[i, j]);
                    if (i != (maxY - 1 - i))
                    {
                        g.GetComponent<BigCardsPanel>().currentCards.Add(g.GetComponent<BigCardsPanel>().cardArrays[maxY - 1 - i, j]);
                    }
                }
            }
        }

        Debug.Log("---------------------------���¶Գ�ɾ����----------------");
        foreach (var v in g.GetComponent<BigCardsPanel>().cardArrays)
        {
            if (v.activeInHierarchy == true)
            {
             
            }
        }
    }

    //���������ξ���
    public Vector3 NewRandomPanelPos(GameObject cardPanel)
    {
        int spx = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.x;
        int spy = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.y;
        int sizeX = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.x + spx;
        int sizeY = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.y + spy;
        int x = (int)bigCardsPanelPrefab.transform.localPosition.x;
        int y = (int)bigCardsPanelPrefab.transform.localPosition.y;
        int intervalX = sizeX / 2;
        int intervalY = sizeY / 2;

        Debug.Log($"intervalX:��{intervalX}�� intervalY:��{intervalY}��");

        //�Ż������   6,7�����м�

        int rowNumber;
        //��������ڶ�
        int ranMoveX = 0;
        int ranMoveY = 0;
        if (cardPanel.GetComponent<BigCardsPanel>().IsRect)
        {
            // 4*4�����ƶ���ʱ���������ȫ������
            if (cardPanel.GetComponent<BigCardsPanel>().row < 4)
            {
                //����ڶ�
                ranMoveX = UnityEngine.Random.Range(-2, 2);
                ranMoveY = UnityEngine.Random.Range(-2, 2);
            }
           

            rowNumber = 7- cardPanel.GetComponent<BigCardsPanel>().row;
        }
        else
        {
            //�ڶ�������С�����
            if (cardPanel.GetComponent<BigCardsPanel>().IsEnd)
            {
                rowNumber = 0;
            }
            else
            {
                rowNumber = UnityEngine.Random.Range(1, 6);    //6-----------1��

                if (rowNumber == 0)
                {
                    ranMoveX = 0;
                    ranMoveY = UnityEngine.Random.Range(0, 2);
                }
                else
                {
                    //����ڶ�
                    ranMoveX = UnityEngine.Random.Range(-2, 2);
                    ranMoveY = UnityEngine.Random.Range(-2, 2);
                }

            }


            cardPanel.GetComponent<BigCardsPanel>().row = cardPanel.GetComponent<BigCardsPanel>().line = 7-rowNumber ;
        }
       
        int lineNumber = rowNumber + 1;              //������ͬ

        //���ܳ����߽�
        int borderX = rowNumber + ranMoveX;
        int borderY = lineNumber + ranMoveY;

        if (borderX > 5)
        {
            borderX = 5;
        }
        else if (borderX < 0)
        {
            borderX = 0;
        }

        if (borderY > 6)
        {
            borderY = 6;
        }
        else if (borderY < 1)
        {
            borderY = 1;
        }

        int nx = x + intervalX * borderX;
        int ny = y + intervalY * borderY;

        cardPanel.GetComponent<RectTransform>().sizeDelta -=new Vector2(2*intervalX*rowNumber,2*intervalY*lineNumber) ;
        currentRow = currentLine = 8 - lineNumber;

        return new Vector3(nx, ny, 0);
    }

    //�̶���Сn*n
    public void ClipPanelSize(GameObject gameObject,int sum)
    {
      
        Card[] cards  = gameObject.GetComponent<BigCardsPanel>().cards;

        for (int i = sum; i < cards.Length; i++)
        {

            //Debug.Log("����ɾ����");
            DestroyImmediate(cards[i].gameObject);

        }

    }

   /// <summary>
   /// ���������ڵ�����
   /// </summary>
   /// <param name="target">������</param>
   /// <param name="targetCards">������ƥ��ļ���</param>
    public void OverLap(GameObject targetObeject, List<GameObject> targetCards)
    {
        RectTransform target = targetObeject.GetComponent<Card>().cardArea;
            
        List<GameObject> laps = new List<GameObject>();
        foreach (var bObeject in targetCards)
        {       
            RectTransform b = bObeject.GetComponent<Card>().cardArea;

            if (target.position.x + target.rect.xMax <= b.position.x + b.rect.xMin ||
                target.position.x + target.rect.xMin >= b.position.x + b.rect.xMax ||
                target.position.y + target.rect.yMax <= b.position.y + b.rect.yMin ||
                target.position.y + target.rect.yMin >= b.position.y + b.rect.yMax
                 )
            {
                continue;
            }
            else
            {
              

                if (bObeject.GetComponent<Button>().interactable)
                {
                    bObeject.GetComponent<Button>().interactable = false;
                    //������ɫ����
                    bObeject.GetComponent<Card>().cardTypeImage.color = bObeject.GetComponent<Card>().coverColor;

                }
                
                bObeject.GetComponent<Card>().coveredNumber++;
                //���������ڸǵļ�����
                laps.Add(bObeject);
               
            }

        }
        //���ڸǱ���
        CoverToOthers.Add(targetObeject, laps);

    }


    /// <summary>
    /// �����������λ�ã�������ֵ����Ϊ����
    /// </summary>
    /// <param name="x">��ʼ��λ��x</param>
    /// <param name="y">��ʼ��λ��y</param>
    /// <param name="width">�������</param>
    /// <param name="height">�����߶�</param>
    /// <param name="size">����߳�</param>
    /// <returns></returns>
    public Vector3 RandomPanelPos(GameObject gameObject)
    {
        int sizeX = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.x + (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.x;
        int sizeY = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.y + (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.y;
        int width = (int)gameObject.GetComponent<RectTransform>().rect.width;
        int height = (int)gameObject.GetComponent<RectTransform>().rect.height;
        int x = (int)bigCardsPanelPrefab.transform.localPosition.x;
        int y = (int)bigCardsPanelPrefab.transform.localPosition.y;


        Debug.Log($"w:{width};h:{height}");

        int intervalX = sizeX / 2;
        int intervalY = sizeY / 2;

        Debug.Log($"intervalX:{intervalX};intervalY:{intervalY}");

        int rowMax = width / intervalX;
        int lineMax = height / intervalY;

        //������������
        int rowNumber = UnityEngine.Random.Range(5, rowMax - 6);
        int lineNumber = UnityEngine.Random.Range(5, lineMax - 6);


        Debug.Log($"�Ż����row��{rowMax / 2} �Ż����line��{lineMax / 2}");

        int nx = x + intervalX * rowNumber;
        int ny = y + intervalY * lineNumber;

        Debug.Log($"   X:{nx};Y:{ny}   intervalX:{ intervalX}   intervalY:{intervalY}   rowNumber:{rowNumber}   LineNumber:{lineNumber}  x:{x} y:{y}");
        int realRow;
        int realLine;

        if (rowNumber % 2 == 1)
        {
            realRow = rowNumber + 1;
        }
        else
        {
            realRow = rowNumber;
        }

        if (lineNumber % 2 == 0)
        {
            realLine = lineNumber - 1;
        }
        else
        {
            realLine = lineNumber;
        }

        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((rowMax - realRow) * intervalX - (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.x,
            (lineMax - realLine) * intervalY - (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.y);

        ClipPanelSize(gameObject, (rowMax - realRow) * (lineMax - realLine));

        int a = (rowMax - realRow + 1) / 2;
        int b = (lineMax - realLine + 1) / 2;
        Debug.Log($"��ǰ����{gameObject.name} һ���� {a}�У�{b}��");
        currentRow = a;
        currentLine = b;
        gameObject.GetComponent<BigCardsPanel>().row = currentRow;
        gameObject.GetComponent<BigCardsPanel>().line = currentLine;

        return new Vector3(nx, ny, 0);
    }

    //�Ƴ�����
    public void RemoveCard(GameObject g)
    {
        if (CoverToOthers.ContainsKey(g))
        {
            returnCardCovers = CoverToOthers[g];
            foreach (var v in CoverToOthers[g])
            {
                v.GetComponent<Card>().coveredNumber--;

                if (v.GetComponent<Card>().coveredNumber == 0)
                {
                    v.GetComponent<Button>().interactable = true;
                    v.GetComponent<Card>().cardTypeImage.color = v.GetComponent<Card>().oldClor;                  
                }
            }        
        }
        //�ֵ����Ƴ�
        CoverToOthers.Remove(g);

        //��ǰ�������Ƴ�
        g.transform.parent.GetComponent<BigCardsPanel>().currentCards.Remove(g);
    }

    //��Ϸʤ���ж�
    public void GameResult()
    {
        //Ǧ�ʺ����Ƿ�Ϊ��
        if (pencilBoxCards.Count != 0)
        {
            return;
        }

        //��Ϸ��ˢ�¼����Ƿ�Ϊ��
        if (currentCards.Count != 0)
        {
            return;
        }

        //��Ϸ���������Ƿ�Ϊ��
        foreach (var v in moveCardPanels)
        {
            if (v.GetComponent<SmallCardsPanel>().cards.Count != 0)
            {
                return;
            }
        }

        //��Ϸʤ��
        UIManager._instance.gameWinPanel.SetActive(true);

    }



    //****************��Ϸ����****************************
    public void RefreshCard()
    {

        for (int i = 0; i < currentCards.Count / 2; i++)
        {
            Sprite tempSprite = currentCards[i].GetComponent<Card>().cardTypeImage.sprite;
            CardTypeEnum tempCardType = currentCards[i].GetComponent<Card>().cardType;

            currentCards[i].GetComponent<Card>().cardTypeImage.sprite = currentCards[currentCards.Count-1-i].GetComponent<Card>().cardTypeImage.sprite;
            currentCards[i].GetComponent<Card>().cardType = currentCards[currentCards.Count - 1 - i].GetComponent<Card>().cardType;

            currentCards[currentCards.Count - 1 - i].GetComponent<Card>().cardTypeImage.sprite = tempSprite;
            currentCards[currentCards.Count - 1 - i].GetComponent<Card>().cardType = tempCardType;


        }
    }

}
