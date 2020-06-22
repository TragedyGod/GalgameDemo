using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;//正则表达式

public class TreatmentController : MonoBehaviour {
    public static TreatmentController _instance;

   
    //private Text TalkText;
    private Text LeftName;
    public bool AutoPlaying=false;//是否在执行自动播放协程
    private AudioSource voiceSource;


    public string[] Treatment;
    
    public GameObject LogScrollBar;
    public int startIndex = 0;//读档用
    public int index=0;
    public string[] info;
    public float TextSpeed;
    public string protagonist="将臣";//主人公名称
    public float turnBgTime = 1.5f;
    public GameObject Logs,LogInstance,LogParent;
    public bool End = false;
    public float waitAudioTime=1;//自动播放时间间隔
    public bool treatmentAutoPlay = false;//自动播放
    public float playTime=-1;//当前播放时间
    public string treatmentName = "Test";//无txt后缀
    public string currentTreatmentText, currentPersonName, currentPersonPicturePath;
    public string oldPersonPicture;//用于在无法获得人物图片路径时的旧的人物图片路径


    private void Awake()
    {
        _instance = this;
        
        LeftName = GameObject.Find("LeftName").GetComponent<Text>();
        voiceSource = GetComponent<AudioSource>();

        

                                                         
    }

   
    public void ReadTreatment(string treatmentName,int index)//读取至第某剧本第index行
    {
        Debug.Log("读取新剧本" + treatmentName);
        this.treatmentName = treatmentName;
        this.index = 0;
        Treatment = FileController._instance.ReadFile(treatmentName+".txt");
        UIController._instance.SwitchPerson("All", "off");
        ReadTreatmentToIndex(index);
        

    }
    public int FindIndexByText(string text)
    {
        for(int i=index-1;i>=0;i--)
        {
            if (Treatment[i].Split('|').Length<2)//如果不是对话行
            {
                continue;
            }
            string treatmentTextInIndex = Treatment[i].Split('|')[1];
            Debug.Log("寻找index，当前行文本为" + treatmentTextInIndex + "，index为" + i);
            if(text==treatmentTextInIndex)
            {
                return i;
            }
        }
        return 0;
    }

    public void ReadTreatmentToIndex(int index)//从第0行读到第index行位置的命令和立绘,这样读档时才能显示正确立绘，否则可能会缺少立绘
    {   
        for (int i = 0; i < index; i++)//执行一次所有的命令
        {
            Debug.Log(i);
            ReadTreatmentLine(i);//读取第几行剧本
            
        }
        if(index==0)
        {
            this.index = 0;
        }
        else
        {
            this.index = index;       
        }
       
        Debug.Log("读取到第" + this.index+"行");
        ReadTreatmentLine(this.index);
    }

 
    

    private void Update()
    {
        //自动播放算法说明，在每次读取剧本时计算出应该等待的时间，并且开始计时播放时间。如果已经开启自动播放，则启动协程，在到时间后自动读取下一行剧本
        //同时将AutoPlaying作为是否在使用协程自动播放的标志，如果当前剧本没有启动协程(AutoPlaying==false)，说明在读取此行时没有开启自动播放。
        //所以如果此时开启自动播放，应等待这行语音播放完或字显示完后再读取下一行剧本，也就是当(playTime>waitAudioTime)时。然而如果当正在播放某行剧本时关闭自动播放
        //name就应该关闭协程，并将AutoPlaying（是否在运行协程）设置为false;
        if (playTime>=0)
        {
            playTime += Time.deltaTime;
        }
        if(treatmentAutoPlay&&!AutoPlaying)
        {
            if(playTime>waitAudioTime)
            {
                playTime = -1;
                ReadTreatmentLine(index);

            }
        }
        if(!treatmentAutoPlay)//停止了自动播放
        {
            StopCoroutine("AutoPlay");
            AutoPlaying = false;
        }
        
    }
    

    public void ReadTreatmentLine(int line)
    {
        
        StopCoroutine("AutoPlay");
        playTime = 0;//播放时间计时

        string tempText = Treatment[line];//读取剧本行

        if (tempText[0] == '#')//读取到注释行，读取下一行
        {
            Debug.Log(tempText);
            ReadTreatmentLine(line + 1);
            index++;
            return;
        }
        if (tempText[0] == '@')//命令行
        {
            //命令
            string command = tempText.Substring(1);//获取命令
            string[] parameters = Regex.Split(command, "\\s+");
            Debug.Log("命令：" + parameters[0] );


            if (parameters[0] == "TurnBg")//切换背景
            {

                UIController._instance.TurnBackground(parameters[1], float.Parse(parameters[2]));

            }
            else if (parameters[0] == "SwitchPerson")
            {
                UIController._instance.SwitchPerson(parameters[1], parameters[2]);

            }
            else if (parameters[0] == "AddPerson")
            {
                UIController._instance.AddPerson(parameters[1], parameters[2], parameters[3]);
            }
            else if(parameters[0] == "End")
            {
                UIController._instance.TurnBackground("画面_黑", 2);
                UIController._instance.SwitchWindow("off");
                End = true;
                return;
            }
            ReadTreatmentLine(line + 1);
            index++;
            return;
        }

        info = tempText.Split('|');
        //Debug.Log("读取第" + line + "行,info[0]="+info[0]+",info[1]="+info[1]+ ",info[2]=" + info[2]);
        currentPersonName = info[0];
        currentTreatmentText = info[1];
        currentPersonPicturePath = info[2];//由于人物是“我”或者主人公名字是或者路径是"null"时不会获取到的人物图片路径是无法使用的
        string voicePath = info[3];
        string picturePosition = info[4];
        //@开头的行是命令行


        //图片路径处开头是@的话则使用自定义路径
        if (info[2][0] == '@')
        {
            //自定义图片路径
            currentPersonPicturePath = currentPersonPicturePath.Substring(currentPersonPicturePath.IndexOf('@') + 1);
            UIController._instance.LoadPicture(currentPersonPicturePath, picturePosition);
        }

        else if (currentPersonName != "我" && currentPersonPicturePath != "null" && currentPersonName != protagonist)//否则使用默认路径显示立绘
        {
            currentPersonPicturePath = "Pictures/Person/" + currentPersonName + "/" + currentPersonPicturePath;//根据名字查找表情
            oldPersonPicture = currentPersonPicturePath;
            UIController._instance.LoadPicture(currentPersonPicturePath, picturePosition);
        }
        else if (currentPersonPicturePath == "null" || currentPersonName == "我" || currentPersonName == protagonist)//如果名字是“我”或主人公或者路径处是空，则不对立绘做处理
        {
            currentPersonPicturePath = oldPersonPicture;
            //不处理
        }
        else if (currentPersonPicturePath == "hide")//如果路径处是hide，则隐藏立绘
        {
            currentPersonPicturePath = "None";//全透明图片，隐藏立绘
            UIController._instance.LoadPicture(currentPersonPicturePath, picturePosition);
        }

        //播放语音
        AudioClip voice = (AudioClip)Resources.Load("Voice/" + currentPersonName + "/" + voicePath, typeof(AudioClip));
        voiceSource.clip = voice;
        voiceSource.Play();

        //自动播放逻辑
        if(voicePath!=""&&voicePath!="null")//有语音路径
        {
        waitAudioTime = voiceSource.clip.length + 1;//获取语音长度，根据语音长度来决定自动播放时间间隔，开启计时协程，到时间自动播放下一条对话，注意协程必须在读取剧本时关闭。
       
        }
        else
        {
            waitAudioTime =Mathf.CeilToInt((currentTreatmentText.Length / UIController._instance.showTextSpeed))+2;
           
        }

        if (treatmentAutoPlay)
        {
            StartCoroutine("AutoPlay");
        }
   
        //名字显示
        string leftNameText = currentPersonName; 
        
        //对话内容
        if (currentPersonName != "我"&&currentPersonName!="")
        {   
            leftNameText= "【" + currentPersonName + "】";
            currentTreatmentText = "「" + currentTreatmentText + "」";
        }
        if(currentPersonName=="我")
        {
            leftNameText = "";
        }
        LeftName.text = leftNameText;
        UIController._instance.ShowTalkText(currentTreatmentText,UIController._instance.showTextSpeed);
        
        index++;
    }

    IEnumerator AutoPlay()//自动播放
    {
        AutoPlaying = true;
        yield return new WaitForSeconds(waitAudioTime);
        AutoPlaying = false;
        ReadTreatmentLine(index);
    }

    
}
