using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Esta clase manejara los numeros que se lanzan como proyectiles
/// </summary>
public class Missile : MonoBehaviour
{
    float numberValue;
    [SerializeField] private TextMeshProUGUI textComp;
    [SerializeField] private int bounces = 5;
 
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
        MathProblem problem = other.gameObject.GetComponent<MathProblem>();
        if (problem != null) 
            problem.CheckMathResult(numberValue);
        bounces--;
        if (bounces <= 0)
            Death();
    }

    public void ShootBehaviour()
    {
        StartCoroutine(DestroyAfterDelay(7f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (bounces >= 0)
        {
            bounces = 0;
            Death();
        }
    }
    private void Death()
    {
        gameObject.SetActive(false);
    }

}
