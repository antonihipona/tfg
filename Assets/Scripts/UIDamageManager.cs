using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageManager : MonoBehaviour
{
    public static UIDamageManager instance;

    public GameObject damageText;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance);
        else
            instance = this;
    }
    
    public void InstantiateDamage(float damage, Vector3 position)
    {
        GameObject text = Instantiate(damageText, GameObject.Find("Canvas").GetComponent<Transform>());
        text.transform.position = Camera.main.WorldToScreenPoint(position);
        text.GetComponentInChildren<Text>().text = "-" + damage.ToString();
    }
}
