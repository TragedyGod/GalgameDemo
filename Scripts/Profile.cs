using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Profile  {

    public string profileName;//存档名
    public string treatmentName;//剧本名
    public string time;//存档时间
    public string treatmentText;//存档对话内容
    public string BgPath,personPicturePath;//背景图片地址，人物图片地址
    public int index;//当前剧本播放的行数
    public double BGMVolume;//背景音乐音量
    public double VoiceVolume;//语音音量
    public double textSpeed;//文字速度

    public Profile()//使用json保存存档数据时并且用类保存时必须有一个默认的构造方法，并且数据成员变量不能为float，float会报错
    {
    }

    public Profile(string profileName, string treatmentName, string time, string treatmentText, string bgPath, string personPicturePath, int index, double bGMVolume, double voiceVolume, double textSpeed)
    {
        this.profileName = profileName;
        this.treatmentName = treatmentName;
        this.time = time;
        this.treatmentText = treatmentText;
        BgPath = bgPath;
        this.personPicturePath = personPicturePath;
        this.index = index;
        BGMVolume = bGMVolume;
        VoiceVolume = voiceVolume;
        this.textSpeed = textSpeed;
    }











    // Use this for initialization

}
