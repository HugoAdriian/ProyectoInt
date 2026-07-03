using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanDistance = 5f;

    private Pallet currentPallet;

    void Update()
    {
        DetectPallet();
    }

    void DetectPallet()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, scanDistance))
        {
            Pallet pallet = hit.collider.GetComponentInParent<Pallet>();

            if (pallet != null)
            {
                if (currentPallet != pallet)
                {
                    currentPallet = pallet;

                    Debug.Log("Pallet detectado:");
                    Debug.Log(currentPallet.productName);
                }

                return;
            }
        }

        currentPallet = null;
    }
}