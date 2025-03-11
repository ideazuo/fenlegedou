using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameUIManager : MonoBehaviour
{

    //�������
    public GameObject audioSettingPanel;

    //��Ч����
    public Slider audioClicpSlider;
    //�������ֵ���
    public Slider audioBGMSlider;


    //��Ч��С
    public Text audioClicpNumberText;
    //������С
    public Text audioBGMNumberText;

    //������
    public Toggle musicToggle;

    //��������Դ
    public AudioSource audioSource_BGM;

    //��Ч����Դ
    public AudioSource audioSource_AudioClicp;



    //��ɫ�����
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    //��ʾ������
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
   
    //���ѡ��
    public Toggle menuMusicToggle;
    public Toggle menuColorToggle;

    public GameObject musicPanel;
    public GameObject colorPanel;

    //��Ϸʱ��
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
        //����
        modeNameText.text = "�ؿ���" + ModeToString(CardManager._instance.currentMode.ToString());

    }

    private void Update()
    {
        UpdateGameTime();
    }

    //������Ϸʱ��
    public void UpdateGameTime()
    {
        spendTime += Time.deltaTime;  //��ʱ��ת��Ϊʱ�������

        hour = (int)spendTime / 3600;
        minute = (int)(spendTime - hour * 3600) / 60;
        second = (int)(spendTime - hour * 3600 - minute * 60);
   

        gameTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);

    }

    //��������
    public void TextChange()
    {
        Tween tween = tipText.transform.DOMoveX(-2700,90f,false);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(tween);
        sequence.SetLoops(-1);
      
    }

    //���ص����˵�
    public void OnClickReturnMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    //����������

    public void OpenAudioSetting()
    {
        Time.timeScale = 0;
        audioSettingPanel.SetActive(true);
    }

    //�ر���������
    public void ClosedAudioSetting()
    {
        Time.timeScale = 1;
        audioSettingPanel.SetActive(false);
    }


    //������Ч
    public void AudioClicpChange()
    {
        if (audioClicpSlider.value == 0 && audioBGMSlider.value == 0)
        {
            musicToggle.isOn = false;
        }
        
        //Debug.Log("������Ч��");
        audioSource_AudioClicp.volume= audioClicpSlider.value;
        audioClicpNumberText.text = ((int)(audioClicpSlider.value * 100)).ToString();
    }

    //���ڱ���
    public void AudioBGMChange()
    {
        if (audioClicpSlider.value == 0 && audioBGMSlider.value == 0)
        {
            musicToggle.isOn = false;
        }
        

        audioSource_BGM.volume = audioBGMSlider.value;
        audioBGMNumberText.text = ((int)(audioBGMSlider.value * 100)).ToString();
    }

    //����������
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


}
