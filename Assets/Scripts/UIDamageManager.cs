using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageManager : MonoBehaviour
{
    // We need to make this a singleton in order to fake a static prefab reference
    public static UIDamageManager Instance;

    public GameObject damageText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }
    
    public void InstantiateDamage(float damage, Vector3 position)
    {
        GameObject text = Instantiate(damageText, GameObject.Find("Canvas").GetComponent<Transform>());
        text.transform.position = Camera.main.WorldToScreenPoint(position);
        text.GetComponentInChildren<Text>().text = "-" + damage.ToString();
    }
}
