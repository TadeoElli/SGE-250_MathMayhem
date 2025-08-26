using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;
using UnityEngine.Events;

public class GoalLine : MonoBehaviour
{
    public UnityEvent OnFinish;
    //Cuando sale de la linea, si es un enemigo que no estaba en la lista, y la direccion en la que se dirigia es la correcta, reduce la cantidad de vidas
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Problem"))
        {
            GameOver();
        }
        
    }
    private void GameOver()
    {
        Debug.Log("Finish");
        OnFinish?.Invoke();
    }
}
