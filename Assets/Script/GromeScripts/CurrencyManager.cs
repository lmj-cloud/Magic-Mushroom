using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    // 버섯 종류도 수십가지고 가격도 수십가지가 되면 이 코드는 못 씀.  
    //버섯 종류를 일일이 데이터로 입력하던가 아니면 버섯이 자신의 가격을 데이터로 보내주는 방식이 되어야 할듯.
    [SerializeField] private TextMeshProUGUI MoneyText;
    [SerializeField] private int mushroomValue = 100;

    public int money;

    void Start()
    {
        UpdateMoneyText();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryCollectMushroom(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryCollectMushroom(other.gameObject);
    }

    private void TryCollectMushroom(GameObject hit)
    {
        if (!hit.CompareTag("Pickup"))
            return;

        money += mushroomValue;
        UpdateMoneyText();
        Destroy(hit);
    }

    private void UpdateMoneyText()
    {
        if (MoneyText == null)
            return;

        MoneyText.text = "Don: " + money.ToString();
    }
}
