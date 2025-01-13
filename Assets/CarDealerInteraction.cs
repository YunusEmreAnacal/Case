using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class CarDealer : MonoBehaviour
{
    public PlayerCarInventory playerCarInventory;

    public Car[] cars;             
    public Transform carDisplay;   
    public Transform carSellingTransform;
    public int currentCarIndex = 0; 

    
    public Text carNameText;   
    public Text carModelText;   
    public Text carPriceText;      
    public Text carConditionText;  


    public Button buyButton;       
    public Button nextButton;      
    public Button prevButton;      

   
    public GameObject player;     
    public Camera mainCamera;    
    public Camera dealerCamera;    

    private PlayerController playerController;  
    private Car currentCar;  
    private GameObject currentCarModel;  

  
    public GameObject carDealerUI; 

    private bool isInDealerRange = false; // Dealer alanýna girildi mi?

    private void Start()
    {
        
        nextButton.onClick.AddListener(ShowNextCar);
        prevButton.onClick.AddListener(ShowPrevCar);
        buyButton.onClick.AddListener(BuyCar);

        
        playerController = player.GetComponent<PlayerController>();

        
        carDealerUI.SetActive(false);
    }

    private void Update()
    {
        // Eðer dealer alanýndaysak ve E tuþuna basýlýyorsa
        if (isInDealerRange && Input.GetKeyDown(KeyCode.E))
        {
            EnterCarDealerUI();
        }

        // 1 tuþuna basýldýðýnda envanteri göster
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerCarInventory.UpdateInventoryUI();
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
        mainCamera.gameObject.SetActive(false);   
        dealerCamera.gameObject.SetActive(true); 

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


    public void BuyCar()
    {
        Debug.Log($"Satýn alýndý: {currentCar.carName}, Fiyat: {currentCar.price} TL, Kondisyon: {currentCar.condition}%");

        if (playerCarInventory != null)
        {
            playerCarInventory.AddCar(currentCar);
        }

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

        carDealerUI.SetActive(false);

  
        dealerCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

     
        playerController.enabled = true;
    }
}
