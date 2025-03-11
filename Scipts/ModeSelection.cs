using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TypeEnum;

public class ModeSelection : MonoBehaviour
{


    public LevelMode currentMode;
    public Color oldColor;
    public Color seletedColor;
    public Image backGround;
    public Toggle currentToggle;


    private void Start()
    {
        currentToggle.onValueChanged.AddListener((bool value) => OnToggleChange(currentToggle, value));
    }

    //��ѡ�е�ʱ��
    public void OnToggleChange(Toggle toggle, bool isSelected)
    {
        if (isSelected)
        {
            //������Ϣ

            //�ı���ɫ
            backGround.color = seletedColor;

            Debug.Log($"onti: {currentMode.ToString()}");

            ModePanelManager._instance.CurrentModeName=currentMode.ToString();

            
        }
        else
        {
            //�ı���ɫ
            backGround.color = oldColor;
        }
    }

    
  
}
