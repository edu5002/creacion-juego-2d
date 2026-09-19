using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float velocidadX = input.GetAxis("Horizontal");

        vector3 posicion = transform.position;

        transform.position = new Vector3(velocidadX + posicion.x, posicion.y, posicion.z);
        
    }
}
