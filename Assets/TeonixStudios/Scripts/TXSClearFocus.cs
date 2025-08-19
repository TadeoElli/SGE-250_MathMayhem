using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TXSClearFocus : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (Input.GetMouseButtonDown(0)) // Detecta el clic izquierdo del mouse
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }


    }
}
