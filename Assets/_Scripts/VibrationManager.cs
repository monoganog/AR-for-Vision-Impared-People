using UnityEngine;
using UnityEngine.UI;

public class VibrationManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject needle;
    public Text debug;

    private float averageField = 0f;
    public float weight;

    void Start()
    {
        Vibration.VibratePredefined(3, false);
        Input.compass.enabled = true;
    }

    public void DoPattern(int index)
    {
        Vibration.VibratePredefined(index, true);
    }

    // Update is called once per frame
    void Update()
    {
        //Handheld.Vibrate();

        Vector3 magnet = Input.compass.rawVector;
        needle.transform.position = new Vector3(magnet.x, magnet.y, magnet.z);
        Debug.Log(magnet);
        //debug.text = "Vector " + magnet.ToString() + " Magnitude " + magnet.magnitude.ToString();
        float oldAverageField = averageField;
        averageField = weight * averageField + (1f - weight) * magnet.magnitude;


        debug.text = "Vector " + magnet.ToString() + " Magnitude " + magnet.magnitude.ToString() + " Average " + averageField.ToString("F3");


        float haptic = 10f * Mathf.Abs(averageField - oldAverageField);
        Vibration.Vibrate((int)haptic);


    }




}
