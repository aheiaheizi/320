using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private int bpm;
    public GameObject player;
    public TMP_Text bpmUI;
    ArduinoReceiver sensorData;

    // Start is called before the first frame update
    void Start()
    {
        sensorData = player.GetComponent<ArduinoReceiver>();
    }

    // Update is called once per frame
    void Update()
    {
        bpm = (int)sensorData.heartbeatBPM;
        bpmUI.text = bpm.ToString();
    }
}
