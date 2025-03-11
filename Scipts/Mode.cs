using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mode : MonoBehaviour
{

    //通知界面
    public GameObject noticePanel;

    public void EnterNormal()
    {
        SceneManager.LoadScene("Normal");
    }

    public void EnterLap()
    {
        SceneManager.LoadScene("Lap");
    }

    public void EnterGold()
    {
        SceneManager.LoadScene("Gold");
    }

    public void EnterHanBao()
    {
        SceneManager.LoadScene("HanBao");
    }
    public void EnterBlindBox()
    {
        SceneManager.LoadScene("BlindBox");
    }
    public void EnterBamboo()
    {
        SceneManager.LoadScene("Bamboo");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //关闭通知
    public void OnClickClosedNotice()
    {
        noticePanel.SetActive(false);
    }

    //打开通知
    public void OnClickOpendNotice()
    {
        noticePanel.SetActive(true);
    }
}
