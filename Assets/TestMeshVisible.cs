using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeshVisible : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            foreach (Material mat in mesh.materials)
            {
                Color c = mat.color;
                c.r = 1;
                c.a = 0f;
                mat.color = c;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("setAlpha");
    }

    IEnumerator setAlpha()
    {
        yield return new WaitForSeconds(2f);
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            foreach (Material mat in mesh.materials)
            {
                Color c = mat.color;
                c.r = 1;
                c.a = 1f;
                mat.color = c;
            }
        }
    }
}
