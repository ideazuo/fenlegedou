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

            //�ؿ���Ϣ

            currentModeText.text = "��ѡ��Ĺؿ���<color=yellow>" + modeName + "</color>";
        }
        get 
        {
            return modeName; 
        }
        
    }

    private string ModeToString(string levelMode)
    {
        string endName = "";

        Debug.Log($"������Ϊ��{levelMode}");

        switch (levelMode)
        {
            case "Normal":
                endName = "ƽ��֮·";
                break;
            case "Gold":
                endName = "���ﳤ��";
                break;
            case "HanBao":
                endName = "��������";
                break;
            case "Bamboo":
                endName = "����ΪӪ";
                break;
            case "BlindBox":
                endName = "��������";
                break;
            case "Lap":
                endName = "�յ�����";
                break;
            default:
                break;
        }

        return endName;
    }


    //����ؿ�
    public void EnterMode()
    {
        SceneManager.LoadScene(currentLevelMode);
    }
}
