using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 2f;
    public Transform cameraTransform; // Oyuncunun kamerasý
    public float interactDistance = 2f;

    [Header("Vehicle Interaction")]
    public LayerMask vehicleLayer;
    private GameObject currentVehicle;
    private bool isDriving = false;

    private CharacterController characterController;
    private Camera originalCamera; // Oyuncunun kamerasý
    private Camera vehicleCamera;  // Araç kamerasý
    private RCC_CarControllerV3 carController; // Araç kontrolü

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        originalCamera = cameraTransform.GetComponent<Camera>();
        vehicleCamera = null; // Araç kamerasý baþlangýçta null olacak
        //DisableVehicleControl();
        originalCamera.enabled = true; // Baþlangýçta oyuncu kamerasý aktif
    }

    void Update()
    {
        if (!isDriving)
        {
            // Oyuncu yürürken hareket kontrolü
            PlayerMovement();
            CheckForVehicleInteraction();
        }
        else
        {
            // Araçta hareket etmek ve kamerayý kontrol etmek
            if (Input.GetKeyDown(KeyCode.F))
            {
                ExitVehicle();
            }
        }
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        characterController.Move(Vector3.down * 9.81f * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * turnSpeed;
        transform.Rotate(0, mouseX, 0);
    }

    void CheckForVehicleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, vehicleLayer))
            {
                Debug.LogError("Ray ile vurulan obje: " + hit.collider.name);

                // Ray ile vurulan nesnenin üst parentlarýnda RCC_CarControllerV3 bileþenini arýyoruz
                currentVehicle = hit.collider.gameObject;

                // Body'nin üst parentlarýnda bileþeni arýyoruz
                carController = currentVehicle.GetComponentInParent<RCC_CarControllerV3>();

                if (carController != null)
                {
                    Debug.LogError("Araba kontrolü bulundu!");
                    EnterVehicle();
                }
                else
                {
                    Debug.LogError("Araba kontrolü bulunamadý: " + currentVehicle.name);
                }
            }
            else
            {
                Debug.LogError("Araç bulunamadý.");
            }
        }
    }

    void EnterVehicle()
    {
        if (currentVehicle == null) return;

        isDriving = true;

        // Araç kontrolünü etkinleþtir
        carController.canControl = true;
        carController.engineRunning = true; // Motoru çalýþtýr

        // Oyuncunun hareketini devre dýþý býrak
        characterController.enabled = false;

        // Oyuncuyu aracýn çocuðu yap
        characterController.transform.SetParent(currentVehicle.transform);
        characterController.transform.localPosition = Vector3.zero; // Aracýn içinde doðru pozisyonda olsun
        characterController.transform.localRotation = Quaternion.identity; // Aracýn rotasýyla hizalanacak

        // Araçtaki kamerayý bul
        Camera vehicleCam = currentVehicle.GetComponentInChildren<Camera>(); // Araçtaki kamera, çocuklarda aranýr

        if (vehicleCam != null)
        {
            vehicleCamera = vehicleCam; // Dinamik olarak aracýn kamerasýna eþitleniyor
            vehicleCamera.enabled = true; // Araç kamerasýný etkinleþtir
            originalCamera.enabled = false; // Oyuncu kamerasýný devre dýþý býrak
        }
        else
        {
            Debug.LogError("Araçta kamera bulunamadý!");
        }
    }

    void ExitVehicle()
    {
        if (currentVehicle == null) return;

        isDriving = false;

        // Araç kontrolünü devre dýþý býrak
        carController.canControl = false;
        carController.engineRunning = false; // Motoru durdur

        // Oyuncunun hareketini tekrar etkinleþtir
        characterController.enabled = true;

        // Oyuncuyu araçtan çýkar
        characterController.transform.SetParent(null);

        // Oyuncuyu araçtan uzak bir pozisyona yerleþtir
        Vector3 exitPosition = currentVehicle.transform.position + currentVehicle.transform.forward * 3f; // Araçtan 3 birim uzaklýk
        characterController.transform.position = exitPosition;

        // Kamera geçiþi: Oyuncu kamerasýný tekrar etkinleþtir, araç kamerasýný devre dýþý býrak
        if (vehicleCamera != null)
        {
            vehicleCamera.enabled = false; // Araç kamerasýný devre dýþý býrak
        }

        originalCamera.enabled = true; // Oyuncu kamerasýný tekrar etkinleþtir
        currentVehicle = null;
    }

    void DisableVehicleControl()
    {
        // Sahnedeki tüm araçlarý bul ve kontrolünü devre dýþý býrak
        RCC_CarControllerV3[] vehicles = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (RCC_CarControllerV3 vehicle in vehicles)
        {
            vehicle.canControl = false; // Kontrolü devre dýþý býrak
            vehicle.engineRunning = false; // Motoru durdur
        }
    }
}
