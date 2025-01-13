using UnityEngine;

[CreateAssetMenu(fileName = "New Car", menuName = "Car/Car")]
public class Car : ScriptableObject
{
    public string carName;       // Araba ismi
    public string model;         // Araba modeli
    public float price;          // Fiyat
    public float topSpeed;       // Maksimum hýz
    public int condition;        // Kondisyon (0-100 arasý bir deðer, %100 yeni)
    public GameObject carModel;  // Araba modelinin GameObject'i (3D model)

    public GameObject sellingCar;
}
