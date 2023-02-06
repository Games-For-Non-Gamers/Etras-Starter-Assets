using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Treadmill_Interactable : MonoBehaviour
{
    public float treadmillForce = 1f;
    public float textureScrollSpeed = 0.5f;


    void Update()
    {
        float offsetY = Time.time * textureScrollSpeed;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0, offsetY);
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            EtraCharacterMainController.Instance.GetComponent<CharacterController>().Move(transform.forward * treadmillForce * Time.deltaTime);
        }

        if (other.GetComponent<Rigidbody>() != null)
        {
            other.GetComponent<Rigidbody>().AddForce(transform.forward * treadmillForce * Time.deltaTime);
        }
    }

}
