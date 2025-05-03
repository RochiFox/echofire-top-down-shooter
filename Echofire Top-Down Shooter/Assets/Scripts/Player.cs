using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControls controls;
    public PlayerAim aim { get; private set; }

    private void Awake()
    {
        controls = new PlayerControls();
        aim = GetComponent<PlayerAim>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
