using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankDamage : MonoBehaviour
{
    private Text text;
    private void Start()
    {
        text = GetComponent<Text>();
        Destroy(this.gameObject, 0.5f);
    }
    private void LateUpdate()
    {
        this.transform.position += Vector3.up * Time.deltaTime * 10;
    }
}
