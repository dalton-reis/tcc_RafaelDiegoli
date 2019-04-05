using UnityEngine;
using UnityEngine.SceneManagement;

public class AppTeste : MonoBehaviour
{
    const string CENA_TRADICIONAL = "Tradicional";
    const string CENA_VUFORIA = "Vuforia AR";
    const string CENA_VUFORIA_PLUS = CENA_VUFORIA + " Plus";

    public const string CENA_MENU_PRINCIPAL = "Menu Principal";
    public const string TAG_DISPLAY = "Display";
    
    GameObject displayGameObj;
    ListAR listAR;

    float GetAjusteEscala(string cena)
    {
        if (cena == CENA_TRADICIONAL)
            return 2;

        if (cena == CENA_VUFORIA)
            return 1;

        if (cena == CENA_VUFORIA_PLUS)
            return 5;

        return 0;
    }

    void Start()
    {
        float ajusteEscala = GetAjusteEscala(SceneManager.GetActiveScene().name);

        displayGameObj = GameObject.FindGameObjectWithTag(TAG_DISPLAY);
        Vector3 scale = displayGameObj.transform.localScale + new Vector3(ajusteEscala, ajusteEscala, ajusteEscala);
        displayGameObj.transform.localScale = Vector3.zero;

        listAR = displayGameObj.GetComponent<ListAR>();

        listAR.AddItem(ItemFactory.GetListItems(scale));
        listAR.ShowItem();
    }
	
	void Update()
    {
	}
}
