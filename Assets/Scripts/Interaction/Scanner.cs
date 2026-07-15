using UnityEngine;
using UnityEngine.InputSystem;

public class Scanner : MonoBehaviour
{
    [Header("Configuración")]
    public float scanDistance = 5f;

    [Header("Interfaz")]
    public ScannerUI scannerUI;

    private Pallet currentPallet;

    void Update()
    {
        DetectPallet();

        if (currentPallet != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ScanPallet();
        }
    }

    void DetectPallet()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        Debug.DrawRay(transform.position, transform.forward * scanDistance, Color.green);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, scanDistance))
        {
            Pallet pallet = hit.collider.GetComponentInParent<Pallet>();

            if (pallet != null)
            {
                currentPallet = pallet;

                // Mostrar solamente el mensaje
                scannerUI.ShowAction();

                return;
            }
        }

        currentPallet = null;

        scannerUI.Hide();
    }

    void ScanPallet()
    {
        scannerUI.HideAction();

        scannerUI.Show(currentPallet);

        Debug.Log("Pallet escaneado: " + currentPallet.productName);
    }
}