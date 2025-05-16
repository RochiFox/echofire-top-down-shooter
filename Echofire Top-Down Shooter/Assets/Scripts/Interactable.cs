using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Material highlightMaterial;
    private Material defaultMaterial;

    private void Start()
    {
        if (!mesh)
        {
            mesh = GetComponentInChildren<MeshRenderer>();
        }

        defaultMaterial = mesh.material;
    }

    public void HighlightActive(bool active)
    {
        mesh.material = active ? highlightMaterial : defaultMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (!playerInteraction) return;

        playerInteraction.interactableList.Add(this);
        playerInteraction.UpdateClosestInteractable();
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (!playerInteraction) return;

        HighlightActive(false);

        playerInteraction.interactableList.Remove(this);
        playerInteraction.UpdateClosestInteractable();
    }
}