using System.Collections;
using UnityEngine;

/// <summary>
/// Clase Nexus maneja proyectiles ("números"), interactúa con el cursor para posicionar y lanzar.
/// Implementa Singleton para fácil acceso.
/// </summary>
public class Nexus : MonoBehaviour
{
    public static Nexus Instance { get; private set; }
    #region Properties
    [Header("Configuración y referencias")]
    [SerializeField] private int index;             // Índice del misil a crear
    [SerializeField] private GameObject mouseOverMissile;
    [SerializeField] private GameObject missilePrefab;
    private Missile missileComp;
    [SerializeField] private CircleCollider2D collider1;
    [SerializeField] private AudioClip throwClip;
    [SerializeField] private bool pauseState = true;
    [SerializeField] private bool haveMissile = false;

    private Camera cam;
    private Vector2 force;

    private Vector2 minPower = new Vector2(-2f, -2f);
    private Vector2 maxPower = new Vector2(2f, 2f);

    private Vector3 startPoint;
    private Vector3 endPoint;

    private bool isDragging = false;  // Indica si el mouse está arrastrando el misil
    #endregion
    #region Initialization
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        cam = Camera.main;
    }

    private void OnEnable()
    {
        pauseState = true;
        haveMissile = false;
        index = 0;

        if (mouseOverMissile != null)
            mouseOverMissile.SetActive(false);

        //StartState();
    }
    
    private void OnDisable()
    {
        DisableNexus();
    }

    #endregion

    #region ShootMechanic

    private void Update()
    {
        if (pauseState) return;

        if (isDragging)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 currentPoint = cam.ScreenToWorldPoint(Input.mousePosition);
                currentPoint.z = 0;

                Vector3 direction = currentPoint - startPoint;
                // Limitar la distancia máxima del mouse al startPoint
                if (direction.magnitude > 2f)
                {
                    direction = direction.normalized * 2f;
                    currentPoint = startPoint + direction;
                }

                MouseHoldBehaviour(currentPoint);
                ShowFeedback();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                endPoint = missilePrefab.transform.position;
                endPoint.z = 0;

                ShootMissile();

                NexusRestore();
                haveMissile = false;

                StartCoroutine(DelayForSpawn());
            }
        }
    }

    /// <summary>
    /// Actualiza la posición del misil y cursor según la posición del mouse, calcula fuerza y velocidad.
    /// </summary>
    private void MouseHoldBehaviour(Vector3 currentPoint)
    {
        missilePrefab.transform.position = currentPoint;

        if (mouseOverMissile != null)
            mouseOverMissile.transform.position = currentPoint;

        collider1.radius = 1f;

        // Calcula fuerza vectorial, limitada a min/max
        force = new Vector2(
            Mathf.Clamp(startPoint.x - currentPoint.x, minPower.x, maxPower.x),
            Mathf.Clamp(startPoint.y - currentPoint.y, minPower.y, maxPower.y));

    }

    /// <summary>
    /// Aplica la fuerza del disparo al misil y activa su collider.
    /// </summary>
    private void ShootMissile()
    {
        if (missilePrefab == null) return;

        missileComp = missilePrefab.GetComponent<Missile>();
        if (missileComp != null)
            missileComp.ShootBehaviour();
        force = new Vector2(
            Mathf.Clamp(startPoint.x - endPoint.x, minPower.x, maxPower.x),
            Mathf.Clamp(startPoint.y - endPoint.y, minPower.y, maxPower.y));

        Rigidbody2D rb = missilePrefab.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.AddForce(force * 5f, ForceMode2D.Impulse);

        Collider2D col = missilePrefab.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        AudioManager.Instance.PlaySoundEffect(throwClip);
    }

    /// <summary>
    /// Restablece valores iniciales del nexus luego de disparar.
    /// </summary>
    private void NexusRestore()
    {
        collider1.radius = 0.2f;

        if (mouseOverMissile != null)
            mouseOverMissile.transform.position = transform.position;

        HideFeedback();

        isDragging = false;
    }
    #endregion
    #region Corrutines
    /// <summary>
    /// Corrutina para retrasar el spawn de un nuevo misil.
    /// </summary>
    private IEnumerator DelayForSpawn()
    {
        yield return new WaitForSeconds(2f);
        missilePrefab = MissilePool.Instance.RequestMissile(transform);
        if (missilePrefab != null)
        {
            missilePrefab.GetComponent<Collider2D>().enabled = false;
            missileComp = missilePrefab.GetComponent<Missile>();
            missileComp.SetNumberValue(index);
            haveMissile = true;
        }
    }
    #endregion
    #region MouseEvents
    private void OnMouseOver()
    {
        if (haveMissile && !pauseState)
        {
            ShowFeedback();

            if (Input.GetMouseButtonDown(0))
            {
                startPoint = transform.position;
                startPoint.z = 0f;
                isDragging = true;
            }
        }
    }

    private void OnMouseExit()
    {
        if (!isDragging)
            HideFeedback();
    }
    #endregion
    #region Feedback
    private void ShowFeedback()
    {
        if (mouseOverMissile != null)
            mouseOverMissile.SetActive(true);
    }

    private void HideFeedback()
    {
        if (mouseOverMissile != null)
            mouseOverMissile.SetActive(false);
    }

    /// <summary>
    /// Deshabilita la funcionalidad y objetos visuales del Nexus.
    /// </summary>
    private void DisableNexus()
    {
        if (missilePrefab != null) missilePrefab.SetActive(false);
        if (mouseOverMissile != null) mouseOverMissile.SetActive(false);
        gameObject.SetActive(false);
    }
    #endregion
    #region Public Methods
    /// <summary>
    /// Inicia el estado activo para que el Nexus pueda lanzar misiles.
    /// </summary>
    public void StartState()
    {
        pauseState = false;

        if (mouseOverMissile != null)
            mouseOverMissile.transform.position = transform.position;

        StartCoroutine(DelayForSpawn());
    }
    public void StopState()
    {
        pauseState = true;
        if(haveMissile)
            missilePrefab.SetActive(false);
    }

    public void SetMissileIndex(int newIndex)
    {  //Recive el indice del NexusStats y modifica el indice local
        index = newIndex;
        if(haveMissile && missileComp != null)
            missileComp.SetNumberValue(index);
    }
    #endregion
}
