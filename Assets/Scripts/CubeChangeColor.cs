using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CubeChangeColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointEnter(PointerEventData eventData)
    {
        
        transform.Rotate(new Vector3(0,0,10));
    }

    public void OnPointExit(PointerEventData eventData)
    {
       
        transform.Rotate(new Vector3(0, 0, -10));
    }
}
