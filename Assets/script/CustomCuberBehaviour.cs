using UnityEngine;

public class CustomCuberBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("CustomCuberBehaviour Start");
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log("CustomCuberBehaviour Update");
        transform.position += new Vector3(0.1f, 0, 0); // Move the cube to the right
    }
}
