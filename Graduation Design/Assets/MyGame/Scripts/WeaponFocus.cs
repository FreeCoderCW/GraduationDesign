using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFocus : MonoBehaviour
{

    public GameObject FocusBar;

    // Update is called once per frame
    void Update()
    {
            if (Input.GetMouseButtonDown(0))
            {
                FocusBar.SetActive(true);
            }
            
            if (Input.GetMouseButtonUp(0))
            {  
                FocusBar.SetActive(false);

            }
    }
}
