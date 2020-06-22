using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileController : MonoBehaviour {
    public static FileController _instance;
    public string DPath;

    // Use this for initialization
    private void Awake()
    {
        _instance = this;
        DPath = Application.streamingAssetsPath;//默认路径，加上文件名为完整文件路径
    }
    void Start () {
       

        
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public string[] ReadFile(string fileName)//读取文件（剧本），返回获得的所有文本的字符串，每行为一个元素
    {

        //注意此游戏不能导出到安卓，因为无法读取streamingAssests里的文件，需要异步加载，暂时不会
        string Path = DPath;
        Path = DPath+"/" +fileName;
       
        Debug.Log("读取文件(Dpath=)" +Path);
        string[] FileText = File.ReadAllLines(Path);
        return FileText;
    }
   
   
  

    
}
