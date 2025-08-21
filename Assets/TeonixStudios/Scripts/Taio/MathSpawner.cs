using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathSpawner : MonoBehaviour
{
    [Header("Spawn Positions For Enemies")]
    [SerializeField] private List<Transform> _basicSpawnPoints;        //Una lista para guardar todos los spawn points para spawnear un enemigo

    [Header("Spawn Positions For Group of Enemies")]
    [SerializeField] private List<GameObject> _prefabSpawnPoints;        //Una lista para guardar objetos que formar grupos de spawn points para spawnear grupos de enemigos

    private bool hasToSpawnAGroup = true;
    float _spawnTimerForProblem; //timer para determinar cuando spawnea el siguiente enemigo

    float _spawnTimerForGroup; //timer para determinar cuando spawnea el siguiente grupo
    [SerializeField] private MathProblemPool _pool;

    public float _spawnIntervalForProblem;    //El intervalo en el que se spawnean nuevos problemas
    public float _spawnIntervalForGroup;    //El intervalo en el que se spawnean nuevos grupos de problemas

    void Start()
    {
        SpawnGroupOfEnemies();
    }
    void Update()
    {

        _spawnTimerForProblem += Time.deltaTime;
        _spawnTimerForGroup += Time.deltaTime;
        //Chequea si es tiempo para spawnear un siguiente enemigo grupo
        if (_spawnTimerForGroup >= _spawnIntervalForGroup)
        {
            //Si el timer supera el cooldown de un spawnGroup y la cuota de spawneo de grupos todavia no se cumplio
            hasToSpawnAGroup = true;
            _spawnTimerForGroup = 0f;
            SpawnGroupOfEnemies();      //Spawnea un grupo de enemigos
        }
        //Chequea si es tiempo para spawnear un siguiente enemigo
        if (_spawnTimerForProblem >= _spawnIntervalForProblem && !hasToSpawnAGroup)  //Si no tiene que spawnear un grupo y el 
        //timer supero el del intervalo entre spawn de enemigos
        {
            _spawnTimerForProblem = 0f;
            SpawnSingleEnemies();     //Spawnea un enemigo

        }
    }
    void SpawnSingleEnemies()//Spawnea un solo enemigo aleatorio
    {
        //Chequea si ya se supero la cuota de enemigos
        GameObject enemy = CreateMathProblem();
        enemy.transform.position = _basicSpawnPoints[Random.Range(0, _basicSpawnPoints.Count)].position;
    }

    void SpawnGroupOfEnemies()      //Spawnea un grupo de enemigos en alguna formacion al azar entre la lista de formaciones
    {
        int groupIndex = Random.Range(0, _prefabSpawnPoints.Count);
        List<Transform> spawnPoints = new List<Transform>();
        _prefabSpawnPoints[groupIndex].GetComponentsInChildren<Transform>(false, spawnPoints);

        //Spawnea un grupo de enemigos spawneando uno en cada spawnpoint de la lista
        foreach (var spawn in spawnPoints)
        {
            GameObject enemy = CreateMathProblem();  //Crea un enemigo
            enemy.transform.position = spawn.position; //Lo coloca en la posicion del spawn point
        }
        _spawnTimerForProblem = 0f;
    }

    private GameObject CreateMathProblem()
    {  //Creo un enemigo
        GameObject problem = _pool.RequestMathProblem(transform);   //Spawnea un enemigo con ese indice
        problem.transform.rotation =  Quaternion.Euler(0f, 0f, 90);    //Setea su rotacion
        return problem;
    }

}
