using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeEnum;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModePanelManager : MonoBehaviour
{
    public string currentLevelMode;
    public Text currentModeText;

    //[SerializeField]
    private string modeName;

    public static ModePanelManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    public string CurrentModeName
    {
        set
        {
      

            currentLevelMode = value;
            modeName = ModeToString(value);

            //关卡信息

            currentModeText.text = "你选择的关卡：<color=yellow>" + modeName + "</color>";
        }
        get 
        {
            return modeName; 
        }
        
    }

    private string ModeToString(string levelMode)
    {
        string endName = "";

        Debug.Log($"进来了为：{levelMode}");

        switch (levelMode)
        {
            case "Normal":
                endName = "平凡之路";
                break;
            case "Gold":
                endName = "万里长城";
                break;
            case "HanBao":
                endName = "坎坷人生";
                break;
            case "Bamboo":
                endName = "步步为营";
                break;
            case "BlindBox":
                endName = "迷雾重重";
                break;
            case "Lap":
                endName = "诱敌深入";
                break;
            default:
                break;
        }

        return endName;
    }


    //进入关卡
    public void EnterMode()
    {
        SceneManager.LoadScene(currentLevelMode);
    }
}
