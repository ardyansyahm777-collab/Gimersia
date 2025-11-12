using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseMenu : MonoBehaviour
{
    public GameObject menuPanel;
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void ButtonOpen()
    {
        menuPanel.SetActive(true);
    }

    public void ButtonClose()
    {
        menuPanel.SetActive(false);
    }   
}
