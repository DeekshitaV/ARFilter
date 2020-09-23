using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject[] filters;


    // Start is called before the first frame update
    void Start()
    {
        filters[0].SetActive(true);
    }

    public void onButtonPressed( GameObject filter )
    {
        foreach( var f in filters)
        {
            f.SetActive(false);
        }
        filter.SetActive(true);
    }
    // Update is called once per frame
    
        
    
}
