using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Test003 : MonoBehaviour
{
    GameObject snap12, snap16, snap24, snap32, snap48, snap64;
    TMP_Dropdown dropdown;

    public void OnValueChenged()
    {
        if(dropdown.value%2==0)
        {
            if(dropdown.value >= 4)
            {
                snap48.SetActive(true);
            }
            else
            {
                snap48.SetActive(false);
            }
            if(dropdown.value >= 2)
            {
                snap24.SetActive(true);
            }
            else
            {
                snap24.SetActive(false);
            }
            snap12.SetActive(true);
            snap16.SetActive(false);
            snap32.SetActive(false);
            snap64.SetActive(false);
        }
        else
        {
            if(dropdown.value >= 5)
            {
                snap64.SetActive(true);
            }
            else
            {
                snap64.SetActive(false);
            }
            if(dropdown.value >= 3)
            {
                snap32.SetActive(true);
            }
            else
            {
                snap32.SetActive(false);
            }
            snap16.SetActive(true);
            snap12.SetActive(false);
            snap24.SetActive(false);
            snap48.SetActive(false);
        }
    }

    void Start()
    {
        snap12 = GameObject.Find("Snap12");
        snap16 = GameObject.Find("Snap16");
        snap24 = GameObject.Find("Snap24");
        snap32 = GameObject.Find("Snap32");
        snap48 = GameObject.Find("Snap48");
        snap64 = GameObject.Find("Snap64");
        dropdown = this.GetComponent<TMP_Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
