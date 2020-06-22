using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRead : MonoBehaviour {
    public int startIndex = 0;
	// Use this for initialization
	void Awake () {
        
        TreatmentController._instance.ReadTreatment("02",0);//无需后缀
        
	}
	//进行其他控制
	// Update is called once per frame
	void Update () {

		
      
	}
}
