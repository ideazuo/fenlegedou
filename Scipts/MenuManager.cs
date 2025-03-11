using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    //消息
    public GameObject noticePanel;



    //模式面板
    public GameObject modePanel;

    //开始面板
    public GameObject beginPanel;


    //点击消息通知
    public void OnClickNotice()
    {
        noticePanel.SetActive(true);
    }

    //点击消息关闭
    public void OnClickClosedNotice()
    {
        noticePanel.SetActive(false);
    }

    //点击游戏开始
    public void OnClickBeginGame()
    {
        beginPanel.SetActive(false);
        modePanel.SetActive(true);
    }

    //返回开始界面
    public void OnClickReturnBeginMenu()
    {
        modePanel.SetActive(false);
        beginPanel.SetActive(true);
       
    }
   
}
