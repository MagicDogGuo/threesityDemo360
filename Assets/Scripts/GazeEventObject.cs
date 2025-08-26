using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//焦点进入事件
//焦点退出事件

public class GazeEventObject : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public delegate void GazeEnter();
    public static GazeEnter OnGazeEnter;

    public delegate void GazeExit();
    public static GazeExit OnGazeExit;


    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("焦点进入");
        if(OnGazeEnter != null)
        {
            OnGazeEnter();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("焦点退出");
        if(OnGazeExit != null)
        {
            OnGazeExit();
        }
    }

   
}
