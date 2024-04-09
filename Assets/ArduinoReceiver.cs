using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class ArduinoReceiver : MonoBehaviour
{
    SerialPort data_stream = new SerialPort("COM11", 115200); //Arduino connected to COM7 with 115200 baud rate
    public string receivedString;
    public int bodyTemp = 100;
    public int tempIncrease;
    public int tempDecrease = 3;
    public bool isTempZero;

    private float pitchFactor;

    public GameObject coldBreath;
    public AudioClip heartbeat;
    public AudioSource soundSource;
    public bool m_play;
    bool m_ToggleChange;

    GameObject myColdBreath;

    public string[] datas;

    // Start is called before the first frame update
    void Start()
    {
        data_stream.Open(); //Initialize the serial stream
        myColdBreath = Instantiate(coldBreath, new Vector3(0, 0, 0), Quaternion.identity);
        myColdBreath.SetActive(false);
        isTempZero = false;

        soundSource = GetComponent<AudioSource>();
        m_play = true;
    }

    // Update is called once per frame
    void Update()
    {
        receivedString = data_stream.ReadLine();

        string[] datas = receivedString.Split(',');
        Debug.Log("Heart rate" + float.Parse(datas[0]));
        Debug.Log("Normalized value" + float.Parse(datas[1]));

        CheckTemperature(float.Parse(datas[1]));
        if (!soundSource.isPlaying && (int)float.Parse(datas[0]) > 0)
        {
            pitchFactor = MapBPMToPitchFactor((int)float.Parse(datas[0]));
            soundSource.clip = heartbeat;
            soundSource.pitch = pitchFactor;
            soundSource.Play();
        }
    }
    
    
    void CheckTemperature(float data)
    {
        if (!isTempZero)
        {
            bodyTemp -= tempDecrease;

            if (bodyTemp < 70)
            {
                myColdBreath.SetActive(true);

                if (bodyTemp <= 1)
                {
                    isTempZero = true;
                    bodyTemp = 0;
                }
            }
            else
            {
                myColdBreath.SetActive(false);
            }
        }
        else
        {
            //reduce player movement
            if (bodyTemp >= 70)
            {
                myColdBreath.SetActive(false);
                isTempZero = false;
            }
        }

        WarmUp(data);
    }

    void WarmUp(float data)
    {
        if (data > 175)
        {
            tempIncrease = (int)data - 175;

            if (bodyTemp >= 100)
            {
                bodyTemp = 100;
            }
            else
            {
                bodyTemp += tempIncrease;
            }
        }
    }
    
    float MapBPMToPitchFactor(int bpm)
    {
        // Define the BPM range
        int minBPM = 30;  // Adjust as needed
        int maxBPM = 120; // Adjust as needed

        // Define the pitch factor range
        float minPitchFactor = 0.5f; // Half speed at min BPM
        float maxPitchFactor = 2.0f; // Double speed at max BPM

        // Clamp bpm within the defined range
        bpm = Mathf.Clamp(bpm, minBPM, maxBPM);

        // Map the bpm to a pitch factor
        float pitchFactor = minPitchFactor + ((float)(bpm - minBPM) / (maxBPM - minBPM)) * (maxPitchFactor - minPitchFactor);

        return pitchFactor;
    }
}