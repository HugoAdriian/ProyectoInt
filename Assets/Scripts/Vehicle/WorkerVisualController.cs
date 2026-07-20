using UnityEngine;

public class WorkerVisualController : MonoBehaviour
{
    [Header("Modelo visual del operador")]
    [Tooltip("Asigna el objeto Worker. Sus renderers se buscarán automáticamente.")]
    [SerializeField] private GameObject worker;

    private Renderer[] workerRenderers;

    private void Awake()
    {
        if (worker == null)
        {
            Debug.LogError(
                "WorkerVisualController: no se asignó el objeto Worker.",
                this);

            return;
        }

        workerRenderers = worker.GetComponentsInChildren<Renderer>(true);
    }

    /// <summary>
    /// Oculta únicamente las mallas del operador.
    /// No desactiva el objeto Worker ni su cámara.
    /// </summary>
    public void HideWorker()
    {
        SetWorkerVisible(false);
    }

    /// <summary>
    /// Vuelve a mostrar las mallas del operador.
    /// </summary>
    public void ShowWorker()
    {
        SetWorkerVisible(true);
    }

    private void SetWorkerVisible(bool visible)
    {
        if (workerRenderers == null)
            return;

        foreach (Renderer workerRenderer in workerRenderers)
        {
            if (workerRenderer != null)
                workerRenderer.enabled = visible;
        }
    }
}