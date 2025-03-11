using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    //��Ϣ
    public GameObject noticePanel;



    //ģʽ���
    public GameObject modePanel;

    //��ʼ���
    public GameObject beginPanel;


    //�����Ϣ֪ͨ
    public void OnClickNotice()
    {
        noticePanel.SetActive(true);
    }

    //�����Ϣ�ر�
    public void OnClickClosedNotice()
    {
        noticePanel.SetActive(false);
    }

    //�����Ϸ��ʼ
    public void OnClickBeginGame()
    {
        beginPanel.SetActive(false);
        modePanel.SetActive(true);
    }

    //���ؿ�ʼ����
    public void OnClickReturnBeginMenu()
    {
        modePanel.SetActive(false);
        beginPanel.SetActive(true);
       
    }
   
}
