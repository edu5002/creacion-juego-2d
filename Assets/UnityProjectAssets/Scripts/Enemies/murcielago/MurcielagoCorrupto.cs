using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MurcielagoCorrupto : MonoBehaviour
{
    private enum EstadoMurcielago
    {
        Patrulla = 0,
        Aviso = 1,
        Picada = 2,
        Aturdido = 3,
        Subida = 4
    }

    private static readonly int ParametroEstado = Animator.StringToHash("Estado");

    [Header("Patrulla")]
    [SerializeField] private Transform puntoVueloA;
    [SerializeField] private Transform puntoVueloB;
    [SerializeField, Min(0.1f)] private float velocidadPatrulla = 2f;
    [SerializeField, Min(0f)] private float esperaAlGirar = 0.2f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Animator animador;

    [Header("Detección")]
    [SerializeField] private PlayerVida jugador;
    [SerializeField] private LayerMask capasObstaculos;
    [SerializeField, Min(0.1f)] private float rangoHorizontal = 6f;
    [SerializeField, Min(0.1f)] private float rangoVertical = 5f;
    [SerializeField, Min(0.1f)] private float tiempoAviso = 0.7f;
    [SerializeField, Min(0f)] private float cooldownAviso = 2f;

    [Header("Picada y recuperación")]
    [SerializeField, Min(0.1f)] private float velocidadPicada = 11f;
    [SerializeField, Min(0.1f)] private float tiempoMaximoPicada = 2f;
    [SerializeField, Min(0.1f)] private float tiempoAturdido = 1.2f;
    [SerializeField, Min(0.1f)] private float velocidadSubida = 3f;
    [SerializeField] private GameObject hitboxDanio;


    private Vector2 origenPicada;
    private Vector2 objetivoPicada;
    private Vector2 direccionPicada;
    private float tiempoRestanteEstado;


    private Rigidbody2D cuerpo;
    private EnemyVida vida;
    private EstadoMurcielago estado;
    private bool vaHaciaB = true;
    private float esperaRestante;
    private float avisoRestante;
    private float proximoAviso;

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        vida = GetComponent<EnemyVida>();
        if (vida != null)
            vida.AlMorir += PrepararMuerte;
        CambiarEstado(EstadoMurcielago.Patrulla);
    }

    private void FixedUpdate()
    {
        switch (estado)
        {
            case EstadoMurcielago.Patrulla:
                if (PuedeAvisar(out Vector2 objetivo))
                {
                    origenPicada = cuerpo.position;
                    objetivoPicada = objetivo;
                    cuerpo.linearVelocity = Vector2.zero;
                    tiempoRestanteEstado = tiempoAviso;
                    Mirar(objetivo.x - cuerpo.position.x);
                    CambiarEstado(EstadoMurcielago.Aviso);
                }
                else
                {
                    Patrullar();
                }

                break;

            case EstadoMurcielago.Aviso:
                cuerpo.linearVelocity = Vector2.zero;
                tiempoRestanteEstado -= Time.fixedDeltaTime;

                if (jugador == null || jugador.EstaMuerto)
                    IniciarSubida();
                else if (tiempoRestanteEstado <= 0f)
                    IniciarPicada();

                break;

            case EstadoMurcielago.Picada:
                if (jugador == null || jugador.EstaMuerto)
                    ActivarHitbox(false);

                tiempoRestanteEstado -= Time.fixedDeltaTime;

                if (tiempoRestanteEstado <= 0f)
                    IniciarSubida();
                else
                    cuerpo.linearVelocity = direccionPicada * velocidadPicada;

                break;

            case EstadoMurcielago.Aturdido:
                cuerpo.linearVelocity = Vector2.zero;
                tiempoRestanteEstado -= Time.fixedDeltaTime;

                if (tiempoRestanteEstado <= 0f)
                    IniciarSubida();

                break;

            case EstadoMurcielago.Subida:
                ActualizarSubida();
                break;
        }
    }

    private void IniciarPicada()
    {
        Vector2 diferencia = objetivoPicada - cuerpo.position;

        if (diferencia.y >= 0f)
        {
            IniciarSubida();
            return;
        }

        direccionPicada = diferencia.normalized;
        tiempoRestanteEstado = tiempoMaximoPicada;
        Mirar(direccionPicada.x);
        CambiarEstado(EstadoMurcielago.Picada);

        cuerpo.linearVelocity = direccionPicada * velocidadPicada;
    }

    private void ActivarHitbox(bool activar)
    {
        if (hitboxDanio != null && hitboxDanio.activeSelf != activar)
            hitboxDanio.SetActive(activar);
    }

    public void ConfirmarImpactoPicada()
    {
        if (!isActiveAndEnabled || estado != EstadoMurcielago.Picada)
            return;

        IniciarSubida();
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        RevisarImpactoPicada(colision);
    }

    private void OnCollisionStay2D(Collision2D colision)
    {
        RevisarImpactoPicada(colision);
    }

    private void RevisarImpactoPicada(Collision2D colision)
    {
        if (!isActiveAndEnabled || estado != EstadoMurcielago.Picada)
            return;

        int capa = 1 << colision.gameObject.layer;

        if ((capasObstaculos.value & capa) == 0)
            return;

        bool golpeoObstaculo = false;

        for (int i = 0; i < colision.contactCount; i++)
        {
            Vector2 normal = colision.GetContact(i).normal;

            if (Vector2.Dot(direccionPicada, normal) >= 0f)
                continue;

            golpeoObstaculo = true;

            if (normal.y >= 0.5f)
            {
                cuerpo.linearVelocity = Vector2.zero;
                tiempoRestanteEstado = tiempoAturdido;
                CambiarEstado(EstadoMurcielago.Aturdido);
                return;
            }
        }

        if (golpeoObstaculo)
            IniciarSubida();
    }

    private void IniciarSubida()
    {
        cuerpo.linearVelocity = Vector2.zero;
        CambiarEstado(EstadoMurcielago.Subida);
    }

    private void ActualizarSubida()
    {
        Vector2 regreso = origenPicada - cuerpo.position;
        float distancia = regreso.magnitude;

        if (distancia <= 0.01f)
        {
            cuerpo.linearVelocity = Vector2.zero;
            proximoAviso = Time.fixedTime + cooldownAviso;
            CambiarEstado(EstadoMurcielago.Patrulla);
            return;
        }

        float velocidad = Mathf.Min(
            velocidadSubida, distancia / Time.fixedDeltaTime
        );

        cuerpo.linearVelocity = regreso.normalized * velocidad;
        Mirar(regreso.x);
    }

    private bool PuedeAvisar(out Vector2 objetivo)
    {
        objetivo = default;

        if (jugador == null || jugador.EstaMuerto ||
            Time.fixedTime < proximoAviso)
            return false;

        objetivo = jugador.transform.position;
        Vector2 diferencia = objetivo - cuerpo.position;

        if (diferencia.y >= 0f ||
            -diferencia.y > rangoVertical ||
            Mathf.Abs(diferencia.x) > rangoHorizontal)
            return false;

        RaycastHit2D obstaculo = Physics2D.Linecast(
            cuerpo.position, objetivo, capasObstaculos
        );

        return obstaculo.collider == null;
    }

    private void Patrullar()
    {
        if (puntoVueloA == null || puntoVueloB == null)
            return;

        if (esperaRestante > 0f)
        {
            esperaRestante -= Time.fixedDeltaTime;
            cuerpo.linearVelocity = Vector2.zero;
            return;
        }

        Transform destino = vaHaciaB ? puntoVueloB : puntoVueloA;
        float distanciaX = destino.position.x - cuerpo.position.x;

        if (Mathf.Abs(distanciaX) <= 0.01f)
        {
            cuerpo.linearVelocity = Vector2.zero;
            vaHaciaB = !vaHaciaB;
            esperaRestante = esperaAlGirar;
            return;
        }

        float velocidadX = Mathf.Min(
            velocidadPatrulla,
            Mathf.Abs(distanciaX) / Time.fixedDeltaTime
        );

        cuerpo.linearVelocity = new Vector2(
            Mathf.Sign(distanciaX) * velocidadX, 0f
        );

        Mirar(distanciaX);
    }

    private void Mirar(float diferenciaX)
    {
        if (visual != null && Mathf.Abs(diferenciaX) > 0.01f)
            visual.flipX = diferenciaX < 0f;
    }

    private void CambiarEstado(EstadoMurcielago nuevoEstado)
    {
        estado = nuevoEstado;
        ActivarHitbox(nuevoEstado == EstadoMurcielago.Picada);
        if (animador != null)
        {
            int estadoVisual = nuevoEstado == EstadoMurcielago.Subida
                ? 0
                : (int)nuevoEstado;

            animador.SetInteger(ParametroEstado, estadoVisual);
        }
    }

    private void OnDisable()
    {
        ActivarHitbox(false);
        if (cuerpo == null || !gameObject.activeInHierarchy ||
            (vida != null && vida.EstaMuerto))
            return;

        if (estado == EstadoMurcielago.Aviso ||
            estado == EstadoMurcielago.Picada)
        {
            IniciarSubida();
        }
    }

    private void PrepararMuerte()
    {
        cuerpo.linearVelocity = Vector2.zero;

        foreach (Collider2D colisionador in GetComponentsInChildren<Collider2D>())
            colisionador.enabled = false;
    }

    private void OnDestroy()
    {
        if (vida != null)
            vida.AlMorir -= PrepararMuerte;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centro = transform.position;
        centro.y -= rangoVertical * 0.5f;

        Vector3 tamaño = new Vector3(
            rangoHorizontal * 2f,
            rangoVertical,
            0f
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(centro, tamaño);
    }
}