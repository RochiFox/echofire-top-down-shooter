using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public List<Interactable> interactableList;

    private Interactable closestInteractable;

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