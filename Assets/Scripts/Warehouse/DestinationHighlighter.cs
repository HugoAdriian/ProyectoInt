using UnityEngine;

public class DestinationHighlighter : MonoBehaviour
{
    #region REFERENCES

    [Header("Referencias")]
    [Tooltip("Objeto visual que marca la ubicación.")]
    [SerializeField] private GameObject markerObject;

    [Tooltip("Renderer del objeto visual.")]
    [SerializeField] private Renderer markerRenderer;

    #endregion

    #region SETTINGS

    [Header("Animación")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minimumScaleMultiplier = 0.92f;
    [SerializeField] private float maximumScaleMultiplier = 1.08f;

    [Header("Colores")]
    [SerializeField]
    private Color pendingColor =
        new Color(1f, 0.75f, 0f, 0.3f);

    [SerializeField]
    private Color validatedColor =
        new Color(0f, 1f, 0.25f, 0.35f);

    #endregion

    #region STATE

    private Vector3 originalScale;
    private Material markerMaterial;
    private bool highlighting;
    private bool destinationValidated;

    #endregion

    #region UNITY

    private void Awake()
    {
        if (markerObject == null)
            markerObject = gameObject;

        if (markerRenderer == null)
            markerRenderer =
                markerObject.GetComponent<Renderer>();

        originalScale = markerObject.transform.localScale;

        if (markerRenderer != null)
        {
            // Crea una instancia independiente del material.
            markerMaterial = markerRenderer.material;
        }

        Hide();
    }

    private void Update()
    {
        if (!highlighting || markerObject == null)
            return;

        AnimatePulse();
    }

    #endregion

    #region PUBLIC METHODS

    public void ShowPending()
    {
        destinationValidated = false;
        highlighting = true;

        markerObject.SetActive(true);
        markerObject.transform.localScale = originalScale;

        ApplyColor(pendingColor);
    }

    public void ShowValidated()
    {
        destinationValidated = true;
        highlighting = true;

        markerObject.SetActive(true);
        markerObject.transform.localScale = originalScale;

        ApplyColor(validatedColor);
    }

    public void Hide()
    {
        highlighting = false;
        destinationValidated = false;

        if (markerObject != null)
        {
            markerObject.transform.localScale = originalScale;
            markerObject.SetActive(false);
        }
    }

    #endregion

    #region ANIMATION

    private void AnimatePulse()
    {
        float normalizedPulse =
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        float scaleMultiplier =
            Mathf.Lerp(
                minimumScaleMultiplier,
                maximumScaleMultiplier,
                normalizedPulse);

        markerObject.transform.localScale =
            originalScale * scaleMultiplier;
    }

    #endregion

    #region MATERIAL

    private void ApplyColor(Color color)
    {
        if (markerMaterial == null)
            return;

        if (markerMaterial.HasProperty("_BaseColor"))
        {
            markerMaterial.SetColor(
                "_BaseColor",
                color);
        }

        if (markerMaterial.HasProperty("_Color"))
        {
            markerMaterial.SetColor(
                "_Color",
                color);
        }

        if (markerMaterial.HasProperty("_EmissionColor"))
        {
            Color emissionColor =
                new Color(
                    color.r,
                    color.g,
                    color.b,
                    1f) * 1.5f;

            markerMaterial.SetColor(
                "_EmissionColor",
                emissionColor);
        }
    }

    #endregion
}