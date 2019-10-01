using System.Collections;
using TMPro;
using UnityEngine;

public class TimeMultiplier : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = (Settings.GlobalTimeScale.ToString() + "x");
    }
}
