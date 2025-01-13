using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI; // UI sýnýflarý için gerekli

public class CarDealer : MonoBehaviour
{
    public Car[] cars;             // ScriptableObject dizisi
    public Transform carDisplay;   // Arabalarýn gösterileceði boþluk (Transform tipi)
    public Transform carSellingTransform;
    public int currentCarIndex = 0; // Þu an gösterilen araba

    // Canvas UI elemanlarý
    public Text carNameText;       // Araba ismi yazan Text
    public Text carModelText;      // Araba modeli yazan Text
    public Text carPriceText;      // Araba fiyatý yazan Text
    public Text carConditionText;  // Araba kondisyonu yazan Text

    // Diðer UI elemanlarý
    public Button buyButton;       // Satýn al butonu
    public Button nextButton;      // Sonraki araba butonu
    public Button prevButton;      // Önceki araba butonu

    // Karakter kontrol ve kamera ayarlarý
    public GameObject player;      // Oyuncu objesi
    public Camera mainCamera;      // Ana kamera
    public Camera dealerCamera;    // Dealer kamera (farklý kamera)

    private PlayerController playerController;  // Oyuncunun kontrol script'i
    private Car currentCar;  // Þu an seçili olan araba
    private GameObject currentCarModel;  // Þu an gösterilen araba modelinin referansý

    // UI etkileþim kontrolü
    public GameObject carDealerUI; // Dealer UI'sini temsil eden obje

    private bool isInDealerRange = false; // Dealer alanýna girildi mi?

    private void Start()
    {
        // Butonlara týklama iþlevi atayýn
        nextButton.onClick.AddListener(ShowNextCar);
        prevButton.onClick.AddListener(ShowPrevCar);
        buyButton.onClick.AddListener(BuyCar);

        // Oyuncunun kontrol script'ini al
        playerController = player.GetComponent<PlayerController>();

        // Baþlangýçta UI'yi gizle
        carDealerUI.SetActive(false);
    }

    private void Update()
    {
        // Eðer dealer alanýndaysak ve E tuþuna basýlýyorsa
        if (isInDealerRange && Input.GetKeyDown(KeyCode.E))
        {
            EnterCarDealerUI();
        }
    }

    // Trigger alanýna girildiðinde çaðrýlacak fonksiyon
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInDealerRange = true;
        }
    }

    // Trigger alanýndan çýkýldýðýnda çaðrýlacak fonksiyon
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInDealerRange = false;
        }
    }

    // Dealer UI'yi açma, kamera deðiþtirip karakter kontrolünü devre dýþý býrakma
    private void EnterCarDealerUI()
    {
        // Kamera deðiþikliði
        mainCamera.gameObject.SetActive(false);   // Ana kamerayý devre dýþý býrak
        dealerCamera.gameObject.SetActive(true);  // Dealer kamerasýný aktif et

        // Karakter kontrolünü devre dýþý býrak
        playerController.enabled = false;

        // UI'yi aktif et
        carDealerUI.SetActive(true);

        // Ýlk arabayý göster
        UpdateCarUI();
    }

    // Araba bilgilerini UI'de güncelleme
    public void UpdateCarUI()
    {
        currentCar = cars[currentCarIndex];

        // UI elemanlarýný güncelle
        carNameText.text = currentCar.carName;
        carModelText.text = "Model: " + currentCar.model;
        carPriceText.text = "Fiyat: " + currentCar.price.ToString() + " TL";
        carConditionText.text = "Kondisyon: " + currentCar.condition.ToString() + "%";

        // Önceki araba modelini sil
        if (currentCarModel != null)
        {
            Destroy(currentCarModel);
        }

        // Yeni araba modelini göster (3D model)
        currentCarModel = Instantiate(currentCar.carModel, carDisplay.position, Quaternion.identity);
        //currentCarModel.transform.SetParent(carDisplay, false); // Transform'a ekle

        // Araba içeri binmeye çalýþýlmasýn diye sürüþ iþlevini devre dýþý býrak
        if (currentCarModel.GetComponent<RCC_CarControllerV3>() != null)
        {
            currentCarModel.GetComponent<RCC_CarControllerV3>().enabled = false;
        }
    }

    // Sonraki arabayý göster
    public void ShowNextCar()
    {
        currentCarIndex++;
        if (currentCarIndex >= cars.Length) currentCarIndex = 0;  // Listeyi döngüye sokar

        UpdateCarUI();
    }

    // Önceki arabayý göster
    public void ShowPrevCar()
    {
        currentCarIndex--;
        if (currentCarIndex < 0) currentCarIndex = cars.Length - 1; // Listeyi döngüye sokar

        UpdateCarUI();
    }

    // Araba satýn alma iþlemi
    // Araba satýn alma iþlemi
    public void BuyCar()
    {
        Debug.Log($"Satýn alýndý: {currentCar.carName}, Fiyat: {currentCar.price} TL, Kondisyon: {currentCar.condition}%");

        // Araba satýn alýndýktan sonra, 'sellingCar' modelini spawn et
        if (currentCar.sellingCar != null)
        {
            // 'sellingCar' modelini 'carDisplay' konumunda spawn et
            Instantiate(currentCar.sellingCar, carSellingTransform.position, Quaternion.identity);
        }

        // Araba alýndýktan sonra sürüþe geçme iþlemi
        EnterCarForDriving();
    }


    // Araba satýn alýndýðýnda, araba kullanýlabilir hale gelir
    private void EnterCarForDriving()
    {
        // Dealer UI'yi kapat
        carDealerUI.SetActive(false);

        // Dealer kamerayý devre dýþý býrak
        dealerCamera.gameObject.SetActive(false);

        // Ana kamerayý aktif et
        mainCamera.gameObject.SetActive(true);

        // Karakter kontrolünü tekrar aç
        playerController.enabled = true;

        // Þimdi arabaya binilebilir, sürüþ iþlevselliðini etkinleþtir
        if (currentCarModel.GetComponent<RCC_CarControllerV3>() != null)
        {
            currentCarModel.GetComponent<RCC_CarControllerV3>().enabled = true;
        }
    }

    // Satýn almayý tamamladýktan sonra UI'yi kapatma ve kamerayý eski haline getirme
    public void CloseCarDealerUI()
    {
        // UI'yi kapat
        carDealerUI.SetActive(false);

        // Kamerayý eski haline getirme
        dealerCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        // Karakter kontrolünü tekrar aç
        playerController.enabled = true;
    }
}
