using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private GameObject winpanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (currencyManager.money >= 1000)
        {
            winpanel.SetActive(true);
        }
    }
}
