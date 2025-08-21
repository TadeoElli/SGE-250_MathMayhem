using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Esta clase manejara los numeros que se lanzan como proyectiles
/// </summary>
public class Missile : MonoBehaviour
{
    float numberValue;
    [SerializeField] private TextMeshProUGUI textComp;

    private void OnEnable()
    {   //Declaro las estadisticas
        numberValue = 0;
        textComp.text = numberValue.ToString();
    }

    public void SetNumberValue(float newNumber)
    {
        numberValue = newNumber;
        textComp.text = numberValue.ToString();
    }

    //Comportamiento cuando collisiona con un objeto
    private void OnCollisionEnter2D(Collision2D other)
    {
       // CollisionBehaviour(other.gameObject);
    }

}
