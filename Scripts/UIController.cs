using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public static UIController _instance;

    public Text TalkText;//对话文本框的文本组件，修改此处来更改对话
    private AudioSource printAudio;//打字音效，但未找到合适音源
    public GameObject Bg; //背景图片
    public GameObject LeftPerson, CenterPerson, RightPerson, NearPerson;//四个人物会出现的位置
    public Dictionary<string, GameObject> PositionToPersonPicture;//字典，通过汉字来获取对应的人物图片物体，如"中"对应centerPerson这个物体
    public GameObject TalkBg;//对话栏根物体
    public GameObject LeftName;
    public GameObject Logs,LogParent, LogInstance,LogScrollBar;
    public bool showTalkTexting = false;
    public GameObject BgmVolume, VoiceVolume, TextSpeed;//手动拖
    public GameObject SettingsPanel;

    //协程的公共变量，由于stopCorution无法停止带参数的协程，所以在获得参数后设置为公共变量再在协程中使用这些公共变量
    //也可用其他方法，但这是比较简单的一种
    public string text;
    public float showTextSpeed=8;//每秒显示的字的个数
    private string newBgPath;
    private float durationTime;

    //private string LoadPicturePosition;
    
    // Use this for initialization
    private void Awake()
    {
        _instance = this;
        LeftName = GameObject.Find("LeftName");
        Bg = GameObject.Find("SceneBg");
        TalkBg = GameObject.Find("TalkBg");
        printAudio = GetComponent<AudioSource>();
        TalkText = GameObject.Find("TalkText").GetComponent<Text>();
        LeftPerson = GameObject.Find("LeftPerson");
        CenterPerson = GameObject.Find("CenterPerson");
        RightPerson = GameObject.Find("RightPerson");
        NearPerson = GameObject.Find("NearPerson");
        PositionToPersonPicture = new Dictionary<string, GameObject> { { "左", LeftPerson }, { "中", CenterPerson }, { "右", RightPerson }, { "近", NearPerson } };
    }
    void Start () {
        
	}
    public void InitSettingsPanel()//初始化设置面板,在打开设置面板后将slider的值调整到正确的位置
    {
        BgmVolume.transform.Find("Slider").GetComponent<Slider>().value = GameObject.Find("BGMController").GetComponent<AudioSource>().volume;
        TextSpeed.transform.Find("Slider").GetComponent<Slider>().value = GameObject.Find("UIController").GetComponent<UIController>().showTextSpeed;
        VoiceVolume.transform.Find("Slider").GetComponent<Slider>().value = GameObject.Find("TreatmentController").GetComponent<AudioSource>().volume;

    }
    public void SetRaycastTargetOn(Transform t)//设置物体及其所有子物体（包括所有层级子物体的子物体）的image组件的raycastTarget为on
    {

        foreach (Transform child in t.GetComponentsInChildren<Transform>())
        {
            //Debug.Log("开启了" + child.name + "的RaycastTarget");
            if (child.GetComponent<Image>())
                child.GetComponent<Image>().raycastTarget = true;
        }
    }
    public void SetRaycastTargetOff(Transform t)
    {
        
        foreach (Transform child in t.GetComponentsInChildren<Transform>())
        {
            //Debug.Log("关闭了" + child.name + "的RaycastTarget");
            if (child.GetComponent<Image>())
                child.GetComponent<Image>().raycastTarget = false;
        }
    }

 
    public void LoadPicture( string path,string position)//根据剧本内容在相应位置显示立绘
    {
        
        Sprite tempsprite = (Sprite)Resources.Load(path,typeof(Sprite));
        
        Debug.Log("命令:加载图片---立绘路径:" + path+",位置："+PositionToPersonPicture[position]);
        GameObject PersonPicture = PositionToPersonPicture[position];
       
        PersonPicture.GetComponent<Image>().sprite = tempsprite;
       
        
        PersonPicture.GetComponent<Image>().color = new Color(1, 1, 1, 1);


    }
    //开关立绘
    public void SwitchPerson(string position,string type)
    {
        if(position=="All")
        {
            if(type=="on")
            {
                PositionToPersonPicture["左"].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                PositionToPersonPicture["中"].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                PositionToPersonPicture["右"].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                PositionToPersonPicture["近"].GetComponent<Image>().color = new Color(1, 1, 1, 1);

            }
            else
            {
                PositionToPersonPicture["左"].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                PositionToPersonPicture["中"].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                PositionToPersonPicture["右"].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                PositionToPersonPicture["近"].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            return;
        }
        if(type=="on")
        {
            PositionToPersonPicture[position].GetComponent<Image>().color = new Color(1, 1, 1, 1);

        }
        else
        {
            PositionToPersonPicture[position].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }//开关相应位置的立绘，这里是通过调整透明度值来隐藏或开启

    

    public void AddPerson(string name,string position,string picturePath)
    {

        Sprite tempsprite = (Sprite)Resources.Load("Pictures/Person/"+name+"/"+picturePath, typeof(Sprite));
        PositionToPersonPicture[position].GetComponent<Image>().sprite = tempsprite;
        PositionToPersonPicture[position].GetComponent<Image>().color = new Color(1, 1, 1, 1);

    }//在某处加载人物立绘而不加载剧本，也就是添加人物

    //切换背景图
    public void TurnBackground(string newBgPath,float durationTime)
    {
        StopCoroutine("TurnBgCoroutine");       //关闭上一个切换背景的协程
        this.newBgPath = newBgPath;
        this.durationTime = durationTime;
        StartCoroutine("TurnBgCoroutine");//用协程计时并缓慢切换背景图片
    }//更改背景图片

    public void ShowTalkText(string text,float speed)//一个一个字显示对话文本
    {
        StopCoroutine("ShowTalkTextCoroutine");
        
        this.text = text;
        this.showTextSpeed = speed;
       
        StartCoroutine("ShowTalkTextCoroutine");
    }
    IEnumerator ShowTalkTextCoroutine()
    {
        showTalkTexting = true;
        string tempText="";
        for(int i=0;i<text.Length;i++)
        {
            
            tempText += text[i];
            //Debug.Log(tempText);
            printAudio.Play();
            TalkText.text = tempText.Replace("\\n", "\n");//解析文本中换行符
            yield return new WaitForSeconds(1/showTextSpeed);
            
        }
        showTalkTexting = false;
    }
    IEnumerator TurnBgCoroutine()
    {
        Sprite newBg = (Sprite)Resources.Load("Pictures/Bg/" + newBgPath, typeof(Sprite));

        Bg.GetComponent<Image>().CrossFadeAlpha(0.4f, durationTime, true);
        yield return new WaitForSeconds(durationTime);
        Bg.GetComponent<Image>().sprite = newBg;
        
        Bg.GetComponent<Image>().CrossFadeAlpha(1, durationTime, true);
    }
    //IEnumerator ShowPictureSlowly()
    //{
    //    PositionToPersonPicture[LoadPicturePosition].GetComponent<Image>().CrossFadeAlpha(1, 1, true);
    //    yield return new WaitForSeconds(1);
    //}

    public void SwitchWindow(string type)
    {
        if(type=="on")
        {
            TalkBg.SetActive(true);
            LeftName.SetActive(true);
            
        }
        else
        {
            TalkBg.SetActive(false);
            LeftName.SetActive(false);
        }
    }//开关对话框


    public void OpenLogs()
    {
        Logs.SetActive(true);

        //清除Logs的所有子物体
        for (int i = 0; i < LogParent.transform.childCount; i++)
        {
            Destroy(LogParent.transform.GetChild(i).gameObject);
        }
        int LogNum = 0;
        int startLine = 0;//开始打印的行
        int index = TreatmentController._instance.index;
        string[] Treatment=TreatmentController._instance.Treatment;
        for (int i = index; i >= 0; i--)
        {
            if (Treatment[i][0] != '@' && Treatment[i][0] != '#')//不是命令行和注释行
            {

                LogNum++;
                if (LogNum == 30)
                {
                    startLine = i;
                    break;
                }

            }
            else
            {
                continue;
            }
        }
        UIController._instance.SwitchWindow("off");
        for (int i = startLine; i < index; i++)
        {
            if (Treatment[i][0] == '@' || Treatment[i][0] == '#')
            {
                continue;
            }

            GameObject tempInstance = Instantiate(LogInstance, new Vector3(0, 0, 0), transform.rotation, LogParent.transform);
            string[] treatmentText = Treatment[i].Split('|');
            string tempName = treatmentText[0];
            string tempText = treatmentText[1];
            tempInstance.transform.Find("LogBg").transform.Find("Text").GetComponent<Text>().text = "【" + tempName + "】\t[" + tempText + "]";
        }
        LogScrollBar.GetComponent<Scrollbar>().value = 0.01f;
    }//开关文本回放面板
    public void CloseLogs()
    {
        Logs.SetActive(false);
        UIController._instance.SwitchWindow("on");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//按下escape按钮是调出设置面板
        {
            
            SettingsPanel.SetActive(!SettingsPanel.activeInHierarchy);
        }
    }
}
