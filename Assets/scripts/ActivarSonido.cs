
using UnityEngine;
using TMPro;

public class ActivarSonido : MonoBehaviour
{
    [Header("Configuración del sonido")]
    [SerializeField] private AudioClip sonido;
    [SerializeField] private float volumen = 1f;

    [Header("Configuración del mensaje")]
    [SerializeField] private string mensaje = "¡Cuidado!";
    [SerializeField] private float duracionMensaje = 3f;
    [SerializeField] private float alturaMensaje = 1.5f;
    [SerializeField] private float tamañoTexto = 3f;
    [SerializeField] private Color colorTexto = Color.white;

    private AudioSource audioSource;

    private GameObject mensajeActual;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.volume = volumen;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Reproducir sonido
            if (sonido != null)
            {
                audioSource.PlayOneShot(sonido);
            }

            // Mostrar mensaje
            MostrarMensaje(collision.transform);
        }
    }

    private void MostrarMensaje(Transform jugador)
    {
        // Eliminar mensaje anterior si existe
        if (mensajeActual != null)
        {
            Destroy(mensajeActual);
        }

        // Crear objeto de texto
        mensajeActual = new GameObject("MensajeJugador");

        // Hacer que siga al jugador
        mensajeActual.transform.SetParent(jugador);

        mensajeActual.transform.localPosition = new Vector3(
            0f,
            alturaMensaje,
            0f
        );

        // Agregar TextMeshPro
        TextMeshPro texto = mensajeActual.AddComponent<TextMeshPro>();

        texto.text = mensaje;
        texto.fontSize = tamañoTexto;
        texto.color = colorTexto;
        texto.alignment = TextAlignmentOptions.Center;
        texto.fontStyle = FontStyles.Bold;

        texto.sortingOrder = 10;

        // Eliminar después del tiempo configurado
        Destroy(mensajeActual, duracionMensaje);
    }
private void LateUpdate()
{
    if (mensajeActual != null)
    {
        mensajeActual.transform.rotation = Quaternion.identity;
    }
}
}
