using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Player player;
    public List<Interactable> interactableList;

    private Interactable closestInteractable;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        player.Controls.Character.Interaction.performed += _ => InteractWithClosest();
    }

    private void InteractWithClosest()
    {
        if (!closestInteractable) return;

        closestInteractable.Interaction();
    }

    public void UpdateClosestInteractable()
    {
        if (closestInteractable) closestInteractable.HighlightActive(false);

        closestInteractable = null;

        float closestDistance = float.MaxValue;

        foreach (Interactable interactable in interactableList)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (!(distance < closestDistance)) continue;

            closestDistance = distance;
            closestInteractable = interactable;
        }

        if (closestInteractable) closestInteractable.HighlightActive(true);
    }
}