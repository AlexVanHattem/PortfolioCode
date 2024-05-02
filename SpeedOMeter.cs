using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedOMeter : MonoBehaviour
{
    [SerializeField] private GameObject car;

    [SerializeField] private Text speedLabel;

    private float speed = 0.0f;
    private float changeToKMH = 2.36f;

    void Update()
    {
        speed = car.GetComponent<Rigidbody>().velocity.magnitude * changeToKMH;

        if (speedLabel != null)
        {
            speedLabel.text = ((int)speed) + "";
        }
    }
}
