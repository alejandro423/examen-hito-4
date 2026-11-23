
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 7f;

    [Header("Dash")]
    [SerializeField] private float fuerzaDash = 15f;
    [SerializeField] private float duracionDash = 0.15f;
    [SerializeField] private float delayDash = 0.5f;

    [Header("Gravedad")]
    [SerializeField] private float gravedad = 3f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float radioGroundCheck = 0.2f;

    private Rigidbody2D rb;

    private bool enSuelo = false;
    private bool haciendoDash = false;
    private bool esperandoDash = false;

    private int cargaDash = 0;
    private int cargaDobleSalto = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravedad;
    }

    private void Update()
    {
        VerificarSuelo();
        Mover();
        Saltar();
        Dash();
    }

    private void VerificarSuelo()
    {
        enSuelo = Physics2D.OverlapCircle(
            groundCheck.position,
            radioGroundCheck,
            groundLayer
        );

        if (enSuelo)
        {
            cargaDobleSalto = 1;

            if (!esperandoDash)
                cargaDash = 1;
        }
    }

    private void Mover()
    {
        float direccion = 0f;

        if (Keyboard.current.spaceKey.isPressed)
            direccion = 1f;

        if (Keyboard.current.upArrowKey.isPressed)
            direccion = -1f;

        if (!haciendoDash)
        {
            rb.linearVelocity = new Vector2(
                direccion * velocidad,
                rb.linearVelocity.y
            );
        }
    }

    private void Saltar()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame && enSuelo)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                fuerzaSalto
            );
        }
        else if (Keyboard.current.pKey.wasPressedThisFrame
                 && !enSuelo
                 && cargaDobleSalto == 1)
        {
            cargaDobleSalto = 0;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                fuerzaSalto
            );
        }
    }

    private void Dash()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame
            && cargaDash == 1
            && !haciendoDash
            && !esperandoDash)
        {
            float direccion = 0f;

            if (Keyboard.current.spaceKey.isPressed)
                direccion = 1f;
            else if (Keyboard.current.upArrowKey.isPressed)
                direccion = -1f;

            if (direccion != 0f)
            {
                cargaDash = 0;
                StartCoroutine(EjecutarDash(direccion));
            }
        }
    }

    private IEnumerator EjecutarDash(float direccion)
    {
        haciendoDash = true;
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(
            direccion * fuerzaDash,
            0f
        );

        yield return new WaitForSeconds(duracionDash);

        rb.gravityScale = gravedad;
        haciendoDash = false;

        StartCoroutine(RecargarDash());
    }

    private IEnumerator RecargarDash()
    {
        esperandoDash = true;

        yield return new WaitForSeconds(delayDash);

        esperandoDash = false;

        if (enSuelo)
            cargaDash = 1;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            radioGroundCheck
        );
    }
}
