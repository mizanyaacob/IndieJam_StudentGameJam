using UnityEngine;

public class SwingBerpusing : MonoBehaviour
{
    public float rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}