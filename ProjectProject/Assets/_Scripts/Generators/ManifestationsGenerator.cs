using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManifestationsGenerator : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private bool canSpawn;

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("PlayerCorrupted", CanSpawn);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("PlayerCorrupted", CanSpawn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && canSpawn)
        {
            Vector2 spawnPos = new Vector2(player.transform.position.x + 35, player.transform.position.y);
            ManifestationsFactory.Instance.CreateObject("Runner", spawnPos);
        }
    }

    private void CanSpawn(bool value)
    {
        canSpawn = value;
    }


}
