using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MissilePool : MonoBehaviour
{
    private static MissilePool instance;
    public static MissilePool Instance { get { return instance; } }
    [SerializeField] private GameObject missilePrefab;  // Prefab único de misil
    [SerializeField] private int poolSize = 5;                // Tamaño inicial de la pool

    private List<GameObject> missilePool = new List<GameObject>();
    Stopwatch stopwatch = new Stopwatch();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(AddMissilesToPoolCoroutine());
    }

    private IEnumerator AddMissilesToPoolCoroutine()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateMissile();
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
    private void CreateMissile()
    {
        GameObject missile = Instantiate(missilePrefab);
        missile.SetActive(false);
        missile.transform.parent = transform;
        missilePool.Add(missile);
    }

    /// <summary>
    /// Solicita un misil de la pool. Si no hay inactivos, crea uno nuevo.
    /// </summary>
    public GameObject RequestMissile()
    {
        // Buscar uno inactivo
        GameObject missileToReturn = missilePool.Find(m => !m.activeSelf);

        if (missileToReturn == null)
        {
            CreateMissile();
            missileToReturn = missilePool[missilePool.Count - 1];
        }

        missileToReturn.SetActive(true);
        return missileToReturn;
    }
}
