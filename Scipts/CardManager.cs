using System;
using System.Collections;
using System.Collections.Generic;
using TypeEnum;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;





public class CardManager : MonoBehaviour
{
    public LevelMode currentMode;//当前关卡模式
    public int[] GoldControl;//控制金字塔难度 
    public int tempLevel;//临时的层级
    public List<Transform> hanBaoPanels;//汉堡的所有面板
    public int currentCardNumber;//牌局上卡牌的数量
    public Transform panel;
    public Transform mask;
    public GameObject cardPrefab; //卡牌的预制体
    public List<Sprite> cardTypes; //卡牌类型图片的集合
    public int groupNumber;        //卡牌组合数量
    public int rectNumber;  //矩阵的数量
    public int rect_1_Number;//1*1矩阵数量
    public int rect_2_Number;//2*2矩阵数量
    public int rect_3_Number;//3*3矩阵数量
    public int rect_4_Number;//4*4矩阵数量
    public GameObject bigCardsPanelPrefab;//大面板预制体
    public List<GameObject> bigCardsPanels;//大面板集合
    public List<GameObject> smallCardsPanesls;//小面板集合
    public List<GameObject> currentCards;  //同来存放游戏上的所有卡牌
    public List<RectTransform> pencilBoxPoss;//存放的位置
    public List<GameObject> pencilBoxCards;//存放的卡牌
    public GameObject pencilBox;//盒子
    public Transform slotsPanel;
    public GameObject cardItem;

    public float cardBoundX;
    public float cardBoundY;

    private GameObject currentPanel;//当前卡牌面板

    public int currentPencilBoxPos;//要存放的卡牌的位置
   
    private List<GameObject> panelCardSlots;

    public List<GameObject> moveCardPanels;  //移除面板
    public int pointerMove;//移除指针

    int currentRow;
    int currentLine;



    public Dictionary<GameObject, List<GameObject>> CoverToOthers; //用来查找物体覆盖其他物体的字典

    public Dictionary<CardTypeEnum, List<GameObject>> cardSlots;   //统计每个卡牌类型个数

    //撤回物体的位置
    public Vector3 returnCardPos;
    //撤回物体的父物体
    public GameObject returnCardParent;
    //撤回物体的序列
    public int retunCardOrder;
    //撤回物体的覆盖物
    public List<GameObject> returnCardCovers;

    //撤回物体
    public GameObject returnCard;


    public static CardManager _instance;


    private void Awake()
    {
        //初始化信息

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

      
               
        

        //禁用对齐,并且进行了对称算法处理，保存剩余物体

        Debug.Log($"禁用布局协程前--------------------------------------------------");
        StartCoroutine("WaitEnabled");  //布局组件必须延迟调用
        Debug.Log($"禁用布局协程后--------------------------------------------------");
       
    }

    //不同的发牌机制
    public void SendCardPanelGold()
    {
     

        for (int i = 0; i < groupNumber; i++)
        {
           
            CreateBigCardsPanelGold(i, GetClassLevel(i));        

        }
    }

    //汉堡发牌机制
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

    //获取裁剪的层数
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
                    Debug.Log($"前18个:{6 - count}");
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

    //获取面板部分和
    public int GetPartGoldPanel(int end)
    {
        int sum = 0;

        for (int i = 0; i < end; i++)
        {
            sum += GoldControl[i];
        }

        Debug.Log($"得到一半的和为：{sum}");
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
    /// 初始化游戏开始时信息
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
        Debug.Log("创建面板中");
        Vector3 vector = bigCardsPanelPrefab.transform.position;

        GameObject g = Instantiate(bigCardsPanelPrefab, panel, false);

        g.name = "BigCardsPanel_" + level.ToString();


        //正金字塔模式
        g.GetComponent<BigCardsPanel>().IsRect = true;

        //裁剪

        int spx = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.x;
        int spy = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.y;
        int sizeX = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.x + spx;
        int sizeY = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.y + spy;

        //这个地方修改
        g.GetComponent<RectTransform>().sizeDelta -= new Vector2(sizeX * levelClass, sizeY * levelClass);

        //辅助row
        currentLine = currentRow = 7 - levelClass;
        g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 7 - levelClass;



        bigCardsPanels.Add(g);


        //2、裁剪   删除掉多余的物体
        ClipPanelSize(g, (currentLine * currentRow));


        //3、存放在二维数组中一份（用在对称上）
        g.GetComponent<BigCardsPanel>().UpdateCardArray(currentRow);  //经过检测，存放进去了




        g.GetComponent<BigCardsPanel>().level = level;

        currentPanel = g;

    }

    public void CreateBigCardsPanelHanBao(int level, int levelClass,Transform parentPanel)
    {
        Debug.Log("创建面板中");

        GameObject g = Instantiate(bigCardsPanelPrefab, parentPanel, false);

        g.name = "BigCardsPanel_" + level.ToString();


        //正金字塔模式
        g.GetComponent<BigCardsPanel>().IsRect = true;

        //裁剪

        int spx = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.x;
        int spy = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().spacing.y;
        int sizeX = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.x + spx;
        int sizeY = (int)bigCardsPanelPrefab.GetComponent<GridLayoutGroup>().cellSize.y + spy;

        //这个地方修改
        g.GetComponent<RectTransform>().sizeDelta -= new Vector2(sizeX * levelClass, sizeY * levelClass);

        //辅助row
        currentLine = currentRow = 7 - levelClass;
        g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 7 - levelClass;



        bigCardsPanels.Add(g);


        //2、裁剪   删除掉多余的物体
        ClipPanelSize(g, (currentLine * currentRow));


        //3、存放在二维数组中一份（用在对称上）
        g.GetComponent<BigCardsPanel>().UpdateCardArray(currentRow);  //经过检测，存放进去了




        g.GetComponent<BigCardsPanel>().level = level;

        currentPanel = g;

    }

    public void CreateBigCardsPanel(int level)
    {
        Debug.Log("创建面板中");
        Vector3 vector = bigCardsPanelPrefab.transform.position;

        GameObject g = Instantiate(bigCardsPanelPrefab, panel, false);

        g.name = "BigCardsPanel_" + level.ToString();


        //标识最后一层
        if (level + 1 == this.groupNumber)
        {
            g.GetComponent<BigCardsPanel>().IsEnd = true;
        }

        Debug.Log(vector);

        //进行概率控制


        if (level < rectNumber)
        {
            g.GetComponent<BigCardsPanel>().IsRect = true;

            int r = UnityEngine.Random.Range(0, rectNumber);

            if (r < rect_1_Number)
            {
                //生成1*1矩阵

                g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 1;

            }
            else if (r < rect_1_Number + rect_2_Number)
            {
                //生成2*2矩阵
                g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 2;

            }
            else if (r < rect_1_Number + rect_2_Number + rect_3_Number)
            {
                //生成3*3矩阵
                g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 3;
            }
            else
            {
                //生成4*4矩阵
                g.GetComponent<BigCardsPanel>().row = g.GetComponent<BigCardsPanel>().line = 4;
            }

        }




        //1、随机生成位置，和矩阵形状（1.左右摆动，2.宽高部分裁剪 3.curRow-lIne赋值）
        vector = NewRandomPanelPos(g);
        g.transform.localPosition = vector;

        bigCardsPanels.Add(g);

       
        //2、裁剪   删除掉多余的物体
        ClipPanelSize(g, (currentLine * currentRow));


        //3、存放在二维数组中一份（用在对称上）
        g.GetComponent<BigCardsPanel>().UpdateCardArray(currentRow);  //经过检测，存放进去了
        
       


        g.GetComponent<BigCardsPanel>().level = level;

        currentPanel = g;

    }

    public IEnumerator WaitEnabled()//协程
    {     
        //等待帧画面渲染结束
        yield return new WaitForEndOfFrame();
        Debug.Log($"------------------正在禁用布局组件------------");

        //关闭布局(大面板)
        CloseLayouts();

        //等待帧画面渲染结束
        yield return new WaitForEndOfFrame();

        //集中对称(针对普通模式,里面如果Rect则不执行)
        AllLeftAndRight(bigCardsPanels);

        //遍历所有遮挡关系(面向的是大面板集合的物体)
        OrderCoverLap();

        //统计当前物体个数(大面板和小面板)
        currentCardNumber = GetAllCardNumber();

        //修补卡牌数量(小面板中至少存在一个)
        CorrectCardNumber(currentCardNumber);

        //开始进行打乱(大面板和小面板,card添加当前集合中)
        ReCombine();

        //禁用button(当前集合中)
        CloseAllCardButton(true);

        //开始进行分租贴图(当前集合中)
        AddCardTypeImage();

        yield return new WaitForEndOfFrame();
        //关闭（外带铅笔盒的）
        CloseSmallLayout();

        UIManager._instance.UpdateScore();
    }

    

    //关闭小布局和区域布局
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

    //匹配遮挡计算
    public void OrderCoverLap()
    {
        //Debug.Log($"正在进行遮挡处理*********************此时面板数量{bigCardsPanels.Count}");

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

    //统计当前物体个数
    public int GetAllCardNumber()
    {
        int sum = 0;

        //遍历大面板里面卡牌数量
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

        //遍历大面板里面卡牌数量
        foreach (var v in moveCardPanels)
        {
            sum += v.GetComponent<SmallCardsPanel>().cards.Count;
        }

        Debug.Log($"sum没有加上铅笔盒的时候:{sum}");

        sum += pencilBoxCards.Count;

        Debug.Log($"sum加上铅笔盒的时候:{sum}");
        return sum;
    }

    //修补卡牌数量
    public void CorrectCardNumber(int currentNumber)
    {
        Debug.Log($"正在卡牌{currentCardNumber}");
        int n = currentCardNumber % 3;
        if (n == 0)
        {
            return;
        }
        else if (n == 2)
        {
     
            //补齐一个
            
            Debug.Log($"正在补齐1个卡牌");
            smallCardsPanesls[0].GetComponent<SmallCardsPanel>().AddCard(1, false);
       
            
            currentCardNumber += 1;
        }
        else
        {
            //补齐两个
            Debug.Log($"正在补齐2个卡牌");
            if (smallCardsPanesls.Count == 0)
            {
                Debug.LogError("不存在小面板，无法修正个数");
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

    //开始集中并打乱
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

        Debug.Log($"打乱后，游戏卡牌的总共个数为:{currentCards.Count}");
    }

    //添加卡牌类型图片
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

    //禁用button
    public void CloseAllCardButton(bool isClose)
    {
      

        UIManager._instance.noClickPanel.SetActive(isClose);
    }
  
    //集中对称
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
                //单个对称
                LeftAndRight(v);
            }
            else
            {
                //单个对称
                UpAndDown(v);
            }
           
        }
    }

    //将生成好的矩阵进行保存
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
        Debug.Log($"----------------------正在左右对称删除物--------------------Row:{maxY} Line{maxY}");
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

        Debug.Log("---------------------------左右对称删除后----------------");
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
        Debug.Log($"----------------------正在【上下】对称删除物--------------------Row:{maxY} Line{maxY}");
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

        Debug.Log("---------------------------上下对称删除后----------------");
        foreach (var v in g.GetComponent<BigCardsPanel>().cardArrays)
        {
            if (v.activeInHierarchy == true)
            {
             
            }
        }
    }

    //生成正方形矩阵
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

        Debug.Log($"intervalX:【{intervalX}】 intervalY:【{intervalY}】");

        //优化坐标点   6,7是正中间

        int rowNumber;
        //控制随机摆动
        int ranMoveX = 0;
        int ranMoveY = 0;
        if (cardPanel.GetComponent<BigCardsPanel>().IsRect)
        {
            // 4*4矩阵移动的时候容易造成全屏铺满
            if (cardPanel.GetComponent<BigCardsPanel>().row < 4)
            {
                //随机摆动
                ranMoveX = UnityEngine.Random.Range(-2, 2);
                ranMoveY = UnityEngine.Random.Range(-2, 2);
            }
           

            rowNumber = 7- cardPanel.GetComponent<BigCardsPanel>().row;
        }
        else
        {
            //摆动发生在小面板上
            if (cardPanel.GetComponent<BigCardsPanel>().IsEnd)
            {
                rowNumber = 0;
            }
            else
            {
                rowNumber = UnityEngine.Random.Range(1, 6);    //6-----------1层

                if (rowNumber == 0)
                {
                    ranMoveX = 0;
                    ranMoveY = UnityEngine.Random.Range(0, 2);
                }
                else
                {
                    //随机摆动
                    ranMoveX = UnityEngine.Random.Range(-2, 2);
                    ranMoveY = UnityEngine.Random.Range(-2, 2);
                }

            }


            cardPanel.GetComponent<BigCardsPanel>().row = cardPanel.GetComponent<BigCardsPanel>().line = 7-rowNumber ;
        }
       
        int lineNumber = rowNumber + 1;              //层数相同

        //不能超过边界
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

    //固定大小n*n
    public void ClipPanelSize(GameObject gameObject,int sum)
    {
      
        Card[] cards  = gameObject.GetComponent<BigCardsPanel>().cards;

        for (int i = sum; i < cards.Length; i++)
        {

            //Debug.Log("正在删除中");
            DestroyImmediate(cards[i].gameObject);

        }

    }

   /// <summary>
   /// 大面板进行遮挡处理
   /// </summary>
   /// <param name="target">检测对象</param>
   /// <param name="targetCards">被进行匹配的集合</param>
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
                    //进行颜色处理
                    bObeject.GetComponent<Card>().cardTypeImage.color = bObeject.GetComponent<Card>().coverColor;

                }
                
                bObeject.GetComponent<Card>().coveredNumber++;
                //加入物体遮盖的集合中
                laps.Add(bObeject);
               
            }

        }
        //我遮盖别人
        CoverToOthers.Add(targetObeject, laps);

    }


    /// <summary>
    /// 随机生成面板的位置，以下数值必须为整数
    /// </summary>
    /// <param name="x">初始化位置x</param>
    /// <param name="y">初始化位置y</param>
    /// <param name="width">大面板宽度</param>
    /// <param name="height">大面板高度</param>
    /// <param name="size">精灵边长</param>
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

        //随机生成坐标点
        int rowNumber = UnityEngine.Random.Range(5, rowMax - 6);
        int lineNumber = UnityEngine.Random.Range(5, lineMax - 6);


        Debug.Log($"优化后的row：{rowMax / 2} 优化后的line：{lineMax / 2}");

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
        Debug.Log($"当前物体{gameObject.name} 一共是 {a}行，{b}列");
        currentRow = a;
        currentLine = b;
        gameObject.GetComponent<BigCardsPanel>().row = currentRow;
        gameObject.GetComponent<BigCardsPanel>().line = currentLine;

        return new Vector3(nx, ny, 0);
    }

    //移除物体
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
        //字典中移除
        CoverToOthers.Remove(g);

        //当前物体中移除
        g.transform.parent.GetComponent<BigCardsPanel>().currentCards.Remove(g);
    }

    //游戏胜利判断
    public void GameResult()
    {
        //铅笔盒中是否为空
        if (pencilBoxCards.Count != 0)
        {
            return;
        }

        //游戏可刷新集合是否为空
        if (currentCards.Count != 0)
        {
            return;
        }

        //游戏叠加区域是否为空
        foreach (var v in moveCardPanels)
        {
            if (v.GetComponent<SmallCardsPanel>().cards.Count != 0)
            {
                return;
            }
        }

        //游戏胜利
        UIManager._instance.gameWinPanel.SetActive(true);

    }



    //****************游戏功能****************************
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
