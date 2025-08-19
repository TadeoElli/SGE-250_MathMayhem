using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TXSDontClickBelow : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detecta clic izquierdo
        {
            if (EventSystem.current.IsPointerOverGameObject()) // Verifica si el clic fue sobre la UI
            {
                Debug.Log("Clic sobre la UI, ignorando interacción con objetos 3D.");
                return;
            }
        }
    }
}
