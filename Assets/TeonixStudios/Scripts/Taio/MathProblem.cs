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
    private float resultValue;        // Resultado final de la fórmula (0 - 100)

    private int operand;
    private int operandResult;
    private OperatorType opType;
    [SerializeField] private AudioClip bounceClip, deathClip;
    [SerializeField] private TextMeshProUGUI textComp;
    public delegate void OnEnemyDeath();
    public OnEnemyDeath notifyScore;
    private Animator animator;
    private bool isDead = false;

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
        GenerateFormula();
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
        else
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
        if (textComp != null)
            textComp.text = $"{operand} {opSymbol} X = {operandResult}";
    }
    public void CheckMathResult(float value)
    {
        if (value == correctNumber && !isDead)
            Death();
    }
    #endregion
}
