using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected PlayerWeaponController WeaponController;
    private MeshRenderer mesh;

    private Material defaultMaterial;
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
        if (!WeaponController)
            WeaponController = other.GetComponent<PlayerWeaponController>();

        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (!playerInteraction) return;

        playerInteraction.GetInteractableList().Add(this);
        playerInteraction.UpdateClosestInteractable();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (!playerInteraction) return;

        HighlightActive(false);

        playerInteraction.GetInteractableList().Remove(this);
        playerInteraction.UpdateClosestInteractable();
    }
}