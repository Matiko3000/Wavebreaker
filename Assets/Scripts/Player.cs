using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;

    private Vector2 input;

    void Update()
    {
        
    }

    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
        Debug.Log(input);
    }
}
