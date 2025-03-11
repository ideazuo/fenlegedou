using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mode : MonoBehaviour
{

    //֪ͨ����
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

    //�ر�֪ͨ
    public void OnClickClosedNotice()
    {
        noticePanel.SetActive(false);
    }

    //��֪ͨ
    public void OnClickOpendNotice()
    {
        noticePanel.SetActive(true);
    }
}
