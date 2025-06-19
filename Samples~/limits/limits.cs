using UnityEngine;

public class Limits : MonoBehaviour
{
    [Header("Configuración")]
    public Material materialWithEffect;
    public MeshRenderer targetRenderer;
    public float maxDistanceToEdge = 0.8f;

    private Bounds blockBounds;

    void Start()
    {
        // Toma el Bounds del MeshRenderer (incluye rotación y escala)
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<MeshRenderer>();
        }
        blockBounds = targetRenderer.bounds;
    }

    void Update()
    {
        if (Player.Instance == null || targetRenderer == null) return;

        // Posición mundial del jugador
        Vector3 headWorld = Player.Instance.GetPositionHead().position;

        // Extremos en X y Z del bounds
        float minX = blockBounds.min.x;
        float maxX = blockBounds.max.x;
        float minZ = blockBounds.min.z;
        float maxZ = blockBounds.max.z;

        // Distancia al borde más cercano en X y Z
        float distX = Mathf.Min(
            Mathf.Abs(headWorld.x - minX),
            Mathf.Abs(headWorld.x - maxX)
        );
        float distZ = Mathf.Min(
            Mathf.Abs(headWorld.z - minZ),
            Mathf.Abs(headWorld.z - maxZ)
        );

        bool showEffect = distX <= maxDistanceToEdge || distZ <= maxDistanceToEdge;

        // Activa/desactiva renderer y actualiza shader solo si hace falta
        if (showEffect)
        {
            if (!targetRenderer.enabled)
                targetRenderer.enabled = true;

            targetRenderer.material = materialWithEffect;
            materialWithEffect.SetVector("_PlayerCylinderPosition", headWorld);
        }
        else if (targetRenderer.enabled)
        {
            targetRenderer.enabled = false;
        }

        // Solo para depuración:
        // Debug.Log($"distX={distX:F2}, distZ={distZ:F2}, showEffect={showEffect}");
    }
}
