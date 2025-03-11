using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameUIManager : MonoBehaviour
{

    //声音面板
    public GameObject audioSettingPanel;

    //音效调节
    public Slider audioClicpSlider;
    //背景音乐调节
    public Slider audioBGMSlider;


    //音效大小
    public Text audioClicpNumberText;
    //背景大小
    public Text audioBGMNumberText;

    //总音乐
    public Toggle musicToggle;

    //背景音乐源
    public AudioSource audioSource_BGM;

    //音效音乐源
    public AudioSource audioSource_AudioClicp;



    //颜色红调节
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    //显示的数字
    public Text redNumberText;
    public Text greenNumberText;
    public Text blueNumberText;

    public GameObject tipText;
    public Text modeNameText;

    public Toggle colorToggle;

    public Color defaultColor;
    public Color selectedColor;
    public Color noSelectedColor;

    public GameObject musicButton;
    public GameObject colorButton;
   
    //面板选择
    public Toggle menuMusicToggle;
    public Toggle menuColorToggle;

    public GameObject musicPanel;
    public GameObject colorPanel;

    //游戏时间
    public Text gameTimeText;


    private float spendTime;
    private int hour;
    private int minute;
    private int second;


    public static GameUIManager _instance;
    private void Awake()
    {
        _instance = this;
       
        TextChange();

       
    }

    private void Start()
    {
        //名字
        modeNameText.text = "关卡：" + ModeToString(CardManager._instance.currentMode.ToString());

    }

    private void Update()
    {
        UpdateGameTime();
    }

    //更新游戏时间
    public void UpdateGameTime()
    {
        spendTime += Time.deltaTime;  //将时间转化为时分秒毫秒

        hour = (int)spendTime / 3600;
        minute = (int)(spendTime - hour * 3600) / 60;
        second = (int)(spendTime - hour * 3600 - minute * 60);
   

        gameTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);

    }

    //背景渐变
    public void TextChange()
    {
        Tween tween = tipText.transform.DOMoveX(-2700,90f,false);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(tween);
        sequence.SetLoops(-1);
      
    }

    //返回到主菜单
    public void OnClickReturnMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    //打开声音设置

    public void OpenAudioSetting()
    {
        Time.timeScale = 0;
        audioSettingPanel.SetActive(true);
    }

    //关闭声音设置
    public void ClosedAudioSetting()
    {
        Time.timeScale = 1;
        audioSettingPanel.SetActive(false);
    }


    //调节音效
    public void AudioClicpChange()
    {
        if (audioClicpSlider.value == 0 && audioBGMSlider.value == 0)
        {
            musicToggle.isOn = false;
        }
        
        //Debug.Log("调节音效中");
        audioSource_AudioClicp.volume= audioClicpSlider.value;
        audioClicpNumberText.text = ((int)(audioClicpSlider.value * 100)).ToString();
    }

    //调节背景
    public void AudioBGMChange()
    {
        if (audioClicpSlider.value == 0 && audioBGMSlider.value == 0)
        {
            musicToggle.isOn = false;
        }
        

        audioSource_BGM.volume = audioBGMSlider.value;
        audioBGMNumberText.text = ((int)(audioBGMSlider.value * 100)).ToString();
    }

    //调节总音乐
    public void AudioChange()
    {
        if (musicToggle.isOn)
        {
            audioClicpSlider.enabled = true;
            audioBGMSlider.enabled = true;

            audioClicpSlider.value = 1;
            AudioClicpChange();

            audioBGMSlider.value = 1;
            AudioBGMChange();
        }
        else
        {
            audioClicpSlider.enabled = false;
            audioBGMSlider.enabled = false;

            audioClicpSlider.value = 0;
            AudioClicpChange();

            audioBGMSlider.value = 0;
            AudioBGMChange();
        }
    }

    public void RedSliderChange()
    {
        Color color = new Color(redSlider.value, Camera.main.backgroundColor.g, Camera.main.backgroundColor.b);
        Camera.main.backgroundColor= color ;
        redNumberText.text= ((int)(redSlider.value * 255)).ToString();
    }
    public void GreenSliderChange()
    {
        Color color = new Color(Camera.main.backgroundColor.r, greenSlider.value, Camera.main.backgroundColor.b);
        Camera.main.backgroundColor = color;
        greenNumberText.text = ((int)(greenSlider.value * 255)).ToString();
    }

    public void BlueSliderChange()
    {
        Color color = new Color(Camera.main.backgroundColor.r, Camera.main.backgroundColor.g, blueSlider.value);
        Camera.main.backgroundColor = color;
        blueNumberText.text = ((int)(blueSlider.value * 255)).ToString();
    }

    public void ColorToggleChange()
    {
        if (colorToggle.isOn)
        {
            Camera.main.backgroundColor = defaultColor;
            redSlider.value = defaultColor.r;
            greenSlider.value = defaultColor.g;
            blueSlider.value = defaultColor.b;
     
            redNumberText.text = ((int)(redSlider.value * 255)).ToString();
            greenNumberText.text = ((int)(greenSlider.value * 255)).ToString();
            blueNumberText.text = ((int)(blueSlider.value * 255)).ToString();

            redSlider.enabled=false;
            greenSlider.enabled=false;
            blueSlider.enabled=false;

        }
        else
        {
            redSlider.enabled = true;
            greenSlider.enabled = true;
            blueSlider.enabled = true;



        }
    }

    public void SelectSettingPanelChange()
    {
        if (menuMusicToggle.isOn)
        {
            musicButton.GetComponent<Image>().color = selectedColor;
            musicPanel.SetActive(true);

            colorButton.GetComponent<Image>().color = noSelectedColor;
            colorPanel.SetActive(false);
           
        }
        
        if(menuColorToggle.isOn)
        {
            colorButton.GetComponent<Image>().color = selectedColor;

            colorPanel.SetActive(true);

            musicButton.GetComponent<Image>().color = noSelectedColor;
            musicPanel.SetActive(false);
            
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


}
