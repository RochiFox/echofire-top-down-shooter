using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    [Header("Color")] [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    public void SetupLook()
    {
        SetupRandomColor();
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);

        Material newMaterial = new Material(skinnedMeshRenderer.material)
        {
            mainTexture = colorTextures[randomIndex]
        };

        skinnedMeshRenderer.material = newMaterial;
    }
}