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

    //当选中的时候
    public void OnToggleChange(Toggle toggle, bool isSelected)
    {
        if (isSelected)
        {
            //传递信息

            //改变颜色
            backGround.color = seletedColor;

            Debug.Log($"onti: {currentMode.ToString()}");

            ModePanelManager._instance.CurrentModeName=currentMode.ToString();

            
        }
        else
        {
            //改变颜色
            backGround.color = oldColor;
        }
    }

    
  
}
