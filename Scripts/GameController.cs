using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public static GameController _instance;
    
    public int index=0;
    public GameObject SaveProfliePanel,LoadProfliePanel,SaveProfileLayout, LoadProfileLayout;//因为可能会隐藏，用find找不到，所以要拖过来

    //public bool treatmentAutoPlay = false;
    void Awake()
    {
        _instance = this;
        SaveProfliePanel = GameObject.Find("SaveProfliePanel");
        LoadProfliePanel = GameObject.Find("LoadProfliePanel");
        InitProfileData();//初始化存档和读档界面内容
       
    }
    

	// Use this for initialization
	void Start () {
        UIController._instance.SetRaycastTargetOff(SaveProfliePanel.transform);//初始化存档界面和读档界面的raycastTarget（设置为不可点击）
        UIController._instance.SetRaycastTargetOff(LoadProfliePanel.transform);
    }

    public void InitProfileData()//初始化存档和读档格，再每次打开存档界面或读档界面时都读取所有存档文件并就修改相应存档格的图片和文字
    {
        for(int i=0;i<SaveProfileLayout.transform.childCount;i++)
        {
            
            
            Profile profile = ReadSaveData(i.ToString());
            if(profile.profileName=="NotFound")
            {
                continue;
            }
            Sprite tempBgSPrite = (Sprite)Resources.Load(profile.BgPath, typeof(Sprite));
            Sprite tempPersonSPrite= (Sprite)Resources.Load(profile.personPicturePath, typeof(Sprite));


           GameObject SaveGrid =SaveProfileLayout. transform.GetChild(i).gameObject;//存档格

            SaveGrid.transform.Find("Bg").transform.Find("ProfilePicture").GetComponent<Image>().sprite = tempBgSPrite;//Bg
            SaveGrid.transform.Find("Bg").transform.Find("PersonPictureMask").transform.Find("PersonPicture").GetComponent<Image>().sprite = tempPersonSPrite;
            SaveGrid.transform.Find("Bg").transform.Find("Information").transform.Find("Time").GetComponent<Text>().text = profile.time;
            SaveGrid.transform.Find("Bg").transform.Find("Information").transform.Find("TreatmentName").GetComponent<Text>().text = profile.treatmentName;
            SaveGrid.transform.Find("Bg").transform.Find("Information").transform.Find("Treatment").GetComponent<Text>().text = profile.treatmentText;

            GameObject LoadGrid = LoadProfileLayout.transform.GetChild(i).gameObject;

            LoadGrid.transform.Find("Bg").transform.Find("PersonPictureMask").transform.Find("PersonPicture").GetComponent<Image>().sprite = tempPersonSPrite;//Bg
            LoadGrid.transform.Find("Bg").transform.Find("ProfilePicture").GetComponent<Image>().sprite = tempBgSPrite;
            LoadGrid.transform.Find("Bg").transform.Find("Information").transform.Find("Time").GetComponent<Text>().text = profile.time;
            LoadGrid.transform.Find("Bg").transform.Find("Information").transform.Find("TreatmentName").GetComponent<Text>().text = profile.treatmentName;
            LoadGrid.transform.Find("Bg").transform.Find("Information").transform.Find("Treatment").GetComponent<Text>().text = profile.treatmentText;



        }
    }
	
    public void SaveData(string profileName)//无后缀
    {

        index = TreatmentController._instance.index-1;//当前剧本播放行数
        
        string path = Application.streamingAssetsPath +"/"+profileName+".json";//存档路径
        Debug.Log("存档"+path);
        string time = DateTime.Now.ToString("yyyy-MM-dd   ") + DateTime.Now.ToShortTimeString();//当前时间
        string treatmentName = TreatmentController._instance.treatmentName;
        string treatmentText= "【" + TreatmentController._instance.currentPersonName + "】 " + TreatmentController._instance.currentTreatmentText;
        Debug.Log("Pictures/Bg/" + GameObject.Find("SceneBg").GetComponent<Image>().sprite.name);
        string BgPath = "Pictures/Bg/"+GameObject.Find("SceneBg").GetComponent<Image>().sprite.name;
        string currentPersonPicture = TreatmentController._instance.currentPersonPicturePath;
        //音频音量和文字速度
        double BGMVolume= GameObject.Find("BGMController").GetComponent<AudioSource>().volume;
        double VoiceVolume=  GameObject.Find("TreatmentController").GetComponent<AudioSource>().volume;
        double textSpeed = GameObject.Find("UIController").GetComponent<UIController>().showTextSpeed;

        Profile profile = new Profile(profileName, treatmentName,time,treatmentText,BgPath,currentPersonPicture,index, BGMVolume,VoiceVolume,textSpeed);//文件名，剧本名，存档时间，存档时对话文本，剧本行数，bgm音量，应该还有一个textSpeed；
        string saveDataJson = JsonMapper.ToJson(profile);
        
        StreamWriter sw = new StreamWriter(path);
        sw.Write(saveDataJson);
        sw.Close();
    }

    public Profile ReadSaveData(string profileName)//读取存档，返回profile类
    {
        string path = Application.streamingAssetsPath + "/" + profileName + ".json";
        if (!File.Exists(path))//没有找到文件时，也就是没有存档
        {
            Debug.Log(path + "路径不存在存档文件");
            Profile profile1 = new Profile("NotFound", "NotFound", "NotFound", "NotFound", "NotFound", "NotFound", 0, 1, 0.6f, 0.6f);
            return profile1;//返回文件名，剧本名等均为"NotFound"的profile类
        }
        StreamReader sr = new StreamReader(path);
        JsonData loadDataJson = sr.ReadToEnd();
        
        Profile profile = JsonMapper.ToObject<Profile>(loadDataJson.ToString());
        sr.Close();
        return profile;
 
    }

    public void LoadData(string profileName)//无后缀
    {
        //UIController._instance.SwitchPerson("All", "off");
        Profile profile=ReadSaveData(profileName);//读取存档
        if(profile.profileName=="NotFound")//如果存档不存在
        {
            return;
        }
        //否则设置相关选项
        GameObject.Find("BGMController").GetComponent<AudioSource>().volume = (float)profile.BGMVolume;
        GameObject.Find("TreatmentController").GetComponent<AudioSource>().volume = (float)profile.VoiceVolume;
        GameObject.Find("UIController").GetComponent<UIController>().showTextSpeed = (float)profile.textSpeed;

        string treatmentName = profile.treatmentName;
        TreatmentController._instance.treatmentName = treatmentName;
        //设置当前读取的剧本
        index = profile.index;//获取存档中的行数
        TreatmentController._instance.ReadTreatment(treatmentName, index);//从新读取treatmentName剧本并处理到index行再开始播放
        
    }
	
}
