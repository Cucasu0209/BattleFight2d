using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Color32 a1;
    public Color32 a2;
    public Color32 a3;
    public Color32 a4;
    public Color32 a5;
    public Color32 a6;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Mathf.Sqrt(a1.b * a1.b + a1.r * a1.r + a1.g + a1.g));
        Debug.Log(Mathf.Sqrt(a2.b * a2.b + a2.r * a2.r + a2.g + a2.g));
        Debug.Log(Mathf.Sqrt(a3.b * a3.b + a3.r * a3.r + a3.g + a3.g));
        Debug.Log(Mathf.Sqrt(a4.b * a4.b + a4.r * a4.r + a4.g + a4.g));
        Debug.Log(Mathf.Sqrt(a5.b * a5.b + a5.r * a5.r + a5.g + a5.g));
        Debug.Log(Mathf.Sqrt(a6.b * a6.b + a6.r * a6.r + a6.g + a6.g));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
