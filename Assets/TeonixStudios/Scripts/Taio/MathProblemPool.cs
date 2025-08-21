using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class MathProblemPool : MonoBehaviour
{
    private static MathProblemPool instance;
    public static MathProblemPool Instance { get { return instance; } }
    [SerializeField] private GameObject problemPrefab;  
    [SerializeField] private int poolSize = 5;                // Tamaño inicial de la pool

    private List<GameObject> problemPool = new List<GameObject>();
    Stopwatch stopwatch = new Stopwatch();

    [SerializeField] private TextMeshProUGUI scoreText;
    private int cantOfResults = 0;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(AddMathProblemsToPoolCoroutine());
    }

    private IEnumerator AddMathProblemsToPoolCoroutine()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateMathProblem();
            // Espera un frame antes de continuar con la siguiente instancia
            if (stopwatch.ElapsedMilliseconds > 1f / 60f)
            {
                yield return new WaitForEndOfFrame();
                stopwatch.Restart();
                //UnityEngine.Debug.Log("Spawnie misiles en un frame");
            }
        }
    }

    /// <summary>
    /// Crea un misil, lo añade a la pool y lo desactiva.
    /// </summary>
    private void CreateMathProblem()
    {
        GameObject mathProblem = Instantiate(problemPrefab);
        mathProblem.SetActive(false);
        mathProblem.transform.SetParent(transform);
        mathProblem.GetComponent<MathProblem>().notifyScore = IncreaseAmount;
        problemPool.Add(mathProblem);
    }

    private void IncreaseAmount()
    {
        cantOfResults++;
        scoreText.text = cantOfResults.ToString();
    }
    /// <summary>
    /// Solicita un misil de la pool. Si no hay inactivos, crea uno nuevo.
    /// </summary>
    public GameObject RequestMathProblem(Transform origin)
    {
        // Buscar uno inactivo
        GameObject problemToReturn = problemPool.Find(m => !m.activeSelf);

        if (problemToReturn == null)
        {
            CreateMathProblem();
            problemToReturn = problemPool[problemPool.Count - 1];
        }

        problemToReturn.SetActive(true);
        problemToReturn.transform.position = origin.position;
        return problemToReturn;
    }
}
