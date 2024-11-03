using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ResourceModule;

public class TestResource : MonoBehaviour
{
    public Texture2D texture;

    void Start()
    {
        texture = ResourceManager.Instance.LoadResource<Texture2D>("Textures/Icon/Unity_2021.jpg");

        Instantiate(ResourceManager.Instance.LoadResource<GameObject>("Prefabs/Objects/Sphere.prefab"));

    }

    void Update()
    {
        
    }
}
