using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundScrolling : MonoBehaviour
{
    private float lenght, startpos;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        startpos = transform.position.x;
        // Ambil lebar SpriteRenderer
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        // 1. Hitung titik reset (posisi kamera relatif terhadap background startpos, dikurangi efek paralaks)
        // Jika kamera bergerak melewati startpos + lenght, background harus di-reset.
        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        // 2. Hitung jarak pergerakan paralaks
        float dist = (cam.transform.position.x * parallaxEffect);

        // 3. Terapkan posisi paralaks
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // 4. Logika Looping (Reset Posisi)
        // Jika kamera telah bergerak melewati titik start + panjang background (ke kanan)
        if (temp > startpos + lenght)
        {
            // Reset startpos ke kanan sejauh panjang background
            startpos += lenght;
        }
        // Jika kamera telah bergerak melewati titik start - panjang background (ke kiri)
        else if (temp < startpos - lenght)
        {
            // Reset startpos ke kiri sejauh panjang background
            startpos -= lenght;
        }
    }
}