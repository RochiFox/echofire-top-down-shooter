using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected MeshRenderer mesh;
    protected Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;

    private void Start()
    {
        if (!mesh)
        {
            mesh = GetComponentInChildren<MeshRenderer>();
        }

        defaultMaterial = mesh.sharedMaterial;
    }

    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        mesh = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    public virtual void Interaction()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

    public void HighlightActive(bool active)
    {
        mesh.material = active ? highlightMaterial : defaultMaterial;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (!playerInteraction) return;

        playerInteraction.interactableList.Add(this);
        playerInteraction.UpdateClosestInteractable();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (!playerInteraction) return;

        HighlightActive(false);

        playerInteraction.interactableList.Remove(this);
        playerInteraction.UpdateClosestInteractable();
    }
}