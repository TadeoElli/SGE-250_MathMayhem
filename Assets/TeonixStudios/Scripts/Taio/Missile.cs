using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Esta clase manejara los numeros que se lanzan como proyectiles
/// </summary>
public class Missile : MonoBehaviour
{
    int numerator;
    int denominator;
    Fraction fractionValue;
    [SerializeField] private TextMeshProUGUI textComp;
    [SerializeField] private int bounces = 5;
    [SerializeField] private AudioClip bounceClip;
    private bool isFraction = false;
 
    private void OnEnable()
    {   //Declaro las estadisticas
        numerator = 0;
        denominator = 0;
        bounces = 5;
        isFraction = false;
        textComp.text = numerator.ToString();
    }

    public void SetNumberValue(int newNumber)
    {
        if (isFraction && newNumber == 0)
            return;
        else if(isFraction)
        {
            denominator = newNumber;
            fractionValue = new Fraction(numerator,denominator);
            textComp.text = numerator.ToString() + "/" + denominator.ToString();
        }
        else
        {
            numerator = newNumber;
            textComp.text = numerator.ToString();
        }
    }
    public void ChangeValueToFraction()
    {
        if(numerator == 0)
            return ;
        isFraction = true;
        textComp.text = numerator.ToString() + "/";
    }

    //Comportamiento cuando collisiona con un objeto
    private void OnCollisionEnter2D(Collision2D other)
    {
        MathProblem problem = other.gameObject.GetComponent<MathProblem>();
        if (problem != null)
        {
            if(isFraction)
                problem.CheckMathResult(fractionValue);
            else
                problem.CheckMathResult(numerator);
        }
        bounces--;
        if (bounces <= 0)
            Death();
        else
            AudioManager.Instance.PlaySoundEffect(bounceClip);
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
