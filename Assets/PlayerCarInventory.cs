using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCarInventory : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI inventoryText; // Envanter UI Text'i

    [Header("Player Data")]
    public List<Car> ownedCars = new List<Car>(); // Sahip olunan arabalar listesi

    // Yeni bir araba envantere eklenir
    public void AddCar(Car car)
    {
        if (car != null)
        {
            ownedCars.Add(car);
            Debug.Log($"{car.carName} envantere eklendi!");
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning("Eklenecek araba bulunamadý!");
        }
    }

    // Envanteri günceller
    public void UpdateInventoryUI()
    {
        if (inventoryText == null)
        {
            Debug.LogWarning("Inventory UI Text atanmadý!");
            return;
        }

        if (ownedCars.Count == 0)
        {
            inventoryText.text = "Hiç araç yok.";
            return;
        }

        string inventoryContent = "Sahip Olduðunuz Araçlar:\n";
        foreach (Car car in ownedCars)
        {
            inventoryContent += $"- {car.carName} ({car.model})\n";
        }

        inventoryText.text = inventoryContent;
    }

    // Giriþ kontrolleri (1'e basýldýðýnda envanteri günceller)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateInventoryUI();
        }
    }
}
