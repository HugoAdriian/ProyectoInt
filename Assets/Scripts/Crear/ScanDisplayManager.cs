using UnityEngine;
using TMPro;

public class ScanDisplayManager : MonoBehaviour
{
    public Transform listaObjetos;
    public GameObject textoPrefab;


    public void AgregarObjeto(string nombre)
    {
        GameObject nuevo =
            Instantiate(textoPrefab, listaObjetos);


        nuevo.GetComponent<TextMeshProUGUI>().text = nombre;
    }
}