using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathProblem : MonoBehaviour
{
    [Header("Behaviour")]
    [SerializeField]private float speed = 0.25f;
    private float direction;        //La direccion en la que se mueve
    private float rotationSpeed = 1;
    [SerializeField] public bool normalDir = true;        //si se va a mover en la direccion normal (hacia la izquierda) o no, se setea desde el spawner
    private float timer;

    private Collider2D col;
    private Rigidbody2D rb2D;

    [Header("Math")]
    [HideInInspector] public DifficultyLevel difficulty;
    private int correctNumber;        // Número entre 0-9 que completa la fórmula
    private Fraction correctFraction;

    private int operand;
    private int operandResult;
    private Fraction operandFractionResult;
    private Fraction operandFraction;
    private OperatorType opType;
    [SerializeField] private AudioClip bounceClip, deathClip;
    [SerializeField] private TextMeshProUGUI textComp;
    [SerializeField] private RectTransform textTransform, borderTransform;
    public delegate void OnEnemyDeath();
    public OnEnemyDeath notifyScore;
    private Animator animator;
    private bool isDead = false;
    private Coroutine lerpCoroutine;
    [SerializeField] private BoxCollider2D boxCollider;

    #region Initialization
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        col.enabled = false;
        isDead = false;
    }

    public void Initialize(DifficultyLevel newDifficulty)
    {
        difficulty = newDifficulty;
        if (difficulty == DifficultyLevel.Hard)
        {
            Debug.Log("SetHard");
            GenerateFractionFormula();
        }
        else
        {
            Debug.Log("Set" + difficulty.ToString());
            GenerateFormula();
        }
        UpdateFormulaText();
        StartCoroutine(DelayForActivateCollider());
    }

    IEnumerator DelayForActivateCollider()
    {     //Desactiva la colisision al spawnear y la activa desp de unos segundo para evitar choques al spawnear
        yield return new WaitForSeconds(2);
        col.enabled = true;
    }
    #endregion
    #region Movement
    void Update()
    {
        direction = normalDir ? 90 : 270;     //Establezco segun normalDir que direccion va a tener el enemigo
        // Aplica la estrategia de movimiento actual
        if (rb2D.velocity.magnitude < 0.2)
        {  //Si la velocidad del objeto es lo suficiente mente chica, procede a moverse, esto sirve para que 
            //cuando la velocidad aumente debido a una colision o un comportamiento, espere a que se detenga para retomar el movimiento, desp de un tiempo
            timer = timer + 1 * Time.deltaTime;
            if (timer > 1.5f)
            {
                Rotate();       //Rota el objeto hacia la direccion establecida
                if (transform.eulerAngles.z > (direction - 90) && transform.eulerAngles.z < (direction + 90))
                { //Si esta apuntando medianamente a esa dirreccion
                    MoveForward();      //Lo mueve hacia adelante
                }
            }
        }

    }
    private void MoveForward()
    {
        if (normalDir)
        {
            transform.Translate(transform.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(-transform.right * speed * Time.deltaTime);
        }
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, direction), rotationSpeed * Time.deltaTime);
    }
    #endregion

    #region Collision
    private void OnCollisionEnter2D(Collision2D other)
    {
        timer = 0;  //resetea el timer para que se vuelva a incorporar
        AudioManager.Instance.PlaySoundEffect(bounceClip);
    }
    private void Death()
    {       //Comportamiento de muerte
        notifyScore?.Invoke();
        isDead = true;
        animator.SetTrigger("Death");
    }
    public void DeathSound()
    {
        AudioManager.Instance.PlaySoundEffect(deathClip);
        // Iniciar lerp del ancho a 0 en 0.1 segundos
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);
        lerpCoroutine = StartCoroutine(LerpWidthToZero(0.1f));
    }
    private IEnumerator LerpWidthToZero(float duration)
    {
        float elapsed = 0f;
        float startWidth = textTransform.sizeDelta.x;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newWidth = Mathf.Lerp(startWidth, 0f, t);
            Vector2 size = textTransform.sizeDelta;
            size.x = newWidth;
            textTransform.sizeDelta = size;
            yield return null;
        }
        // Asegurar que quede a 0 exacto
        Vector2 finalSize = textTransform.sizeDelta;
        finalSize.x = 0f;
        textTransform.sizeDelta = finalSize;
    }

    private void Deactivate()
    {
        this.gameObject.SetActive(false);       //Desactiva este objeto
    }

    #endregion
    #region Math
    private void GenerateFormula()
    {
        // Elegir el número correcto (entre 0 y 9)
        correctNumber = Random.Range(0, 10);

        // Determinar operador según dificultad
        if (difficulty == DifficultyLevel.Easy)
        {
            // Solo suma o resta
            opType = (Random.value < 0.5f) ? OperatorType.Sum : OperatorType.Subtract;
        }
        else if(difficulty == DifficultyLevel.Normal)
        {
            // Puede ser cualquiera
            int op = Random.Range(0, 4);
            opType = (OperatorType)op;
        }

        // Generar operandos acorde a operador y resultado deseado
        switch (opType)
        {
            case OperatorType.Sum: //Para suma eligo un numero random entre 0 y 100 para que sea el resultado, y luego saco el operando restandole al resultado el numero correcto
                operandResult = Random.Range(0, 100);
                operand = operandResult - correctNumber;
                break;
            case OperatorType.Subtract: //Para la resta eligo un numero random entre 0 y 990 para que sea el resultado, y luego saco el operando sumandole al resultado el numero correcto
                operandResult = Random.Range(0, 90);
                operand = operandResult + correctNumber;
                break;
            case OperatorType.Multiply:
                // Para mult, elijo un operando entre 0 y 10 y el resultado sera el operando * el numero correcto
                operand = Random.Range(0, 11); // divisor no 0
                operandResult = correctNumber * operand;
                break;
            case OperatorType.Divide: // para div elijo un numero random entre 1 y 10 para el resultado y el operando sera ese numero multiplicado por el numero correcto
                operandResult = Random.Range(1, 10); // divisor no 0
                operand = operandResult * correctNumber; // dividendo para que dé resultado entero
                break;
        }
    }

    private void GenerateFractionFormula()
    {
        // Generar incógnita fraccionaria con denominador != 0
        int num = Random.Range(0, 10);
        int den;
        do
        {
            den = Random.Range(1, 10);
        } while (den == 0);

        correctFraction = new Fraction(num, den);

        // Puede ser cualquiera
        int op = Random.Range(0, 4);
        opType = (OperatorType)op;

        // Generar operandos acorde a operador y resultado deseado
        switch (opType)
        {
            case OperatorType.Sum: //Para suma eligo dos numeros random entre 1 y 9 y creo una fraccion con esos numeros para que sea el resultado, y luego saco el operando restandole al resultado la fraccion correcta
                operandFractionResult = new Fraction(Random.Range(1,9), Random.Range(1, 9));
                operandFraction = operandFractionResult - correctFraction;
                break;
            case OperatorType.Subtract: //Para la resta eligo dos numeros random entre 1 y 9 para que sea el resultado, y luego saco el operando sumandole al resultado la fraccion correcta
                operandFractionResult = new Fraction(Random.Range(1, 9), Random.Range(1, 9));
                operandFraction = operandFractionResult + correctFraction;
                break;
            case OperatorType.Multiply:
                // Para mult, elijo una fraccion random como operando y el resultado sera el operando * la fraccion correcto
                operandFraction = new Fraction(Random.Range(1, 9), Random.Range(1, 9)); 
                operandFractionResult = correctFraction * operandFraction;
                break;
            case OperatorType.Divide: // para div elijo un numero random entre 1 y 10 para el resultado y el operando sera ese numero multiplicado por el numero correcto
                operandFractionResult = new Fraction(Random.Range(1, 9), Random.Range(1, 9));
                operandFraction = operandFractionResult * correctFraction;
                break;
        }
    }


    /// <summary>
    /// Actualiza la UI de la fórmula (texto), ejemplo: "A + B"
    /// </summary>
    private void UpdateFormulaText()
    {
        string opSymbol = "+";
        switch (opType)
        {
            case OperatorType.Sum: opSymbol = "+"; break;
            case OperatorType.Subtract: opSymbol = "-"; break;
            case OperatorType.Multiply: opSymbol = "×"; break;
            case OperatorType.Divide: opSymbol = "÷"; break;
        }
        if (textComp == null) return;
        if (difficulty == DifficultyLevel.Hard)
        {
            textComp.text = $"{operandFraction.ToString()} {opSymbol} X = {operandFractionResult.ToString()}";
        }
        else
            textComp.text = $"{operand} {opSymbol} X = {operandResult}";
        Vector2 newSize = new Vector2(textComp.preferredWidth, textComp.preferredHeight);
        borderTransform.sizeDelta = newSize;
        textTransform.sizeDelta = newSize;
        boxCollider.size = new Vector2(textComp.preferredHeight,textComp.preferredWidth);
    }
    public void CheckMathResult(float value)
    {
        if (value == correctNumber && !isDead)
            Death();
    }
    public void CheckMathResult(Fraction value)
    {
        if (Fraction.Equals(value, correctFraction) && !isDead)
            Death();
    }
    #endregion
}
