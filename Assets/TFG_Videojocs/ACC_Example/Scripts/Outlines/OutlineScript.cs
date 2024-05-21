using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class OutlineScript : MonoBehaviour {
  
  private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

  [SerializeField]
  private Material outlineMask;
  [SerializeField]
  private Material outlineFill;
  [SerializeField]
  private Color outlineColor;
  [SerializeField, Range(0f, 10f)]
  private float outlineWidth = 2f;
  
  private Renderer[] renderers;
  private Material outlineMaskMaterial;
  private Material outlineFillMaterial;
  
  private bool fadingIn = false;
  private bool fadingOut = false;
  
  private float fadeTimer = 0.0f;
  private bool isFading = false;
  public float fadeDuration = 1.0f;
  
  [SerializeField, HideInInspector]
  private List<Mesh> bakeKeys = new List<Mesh>();

  [SerializeField, HideInInspector]
  private List<ListVector3> bakeValues = new List<ListVector3>();
  
  [Serializable]
  private class ListVector3 {
    public List<Vector3> data;
  }
  
  private bool precomputeOutline;
  private bool needsUpdate;
  void Awake() {
    renderers = GetComponentsInChildren<Renderer>();
    outlineMaskMaterial = Instantiate(outlineMask);
    outlineFillMaterial = Instantiate(outlineFill);
    LoadSmoothNormals();
    precomputeOutline = true;
    needsUpdate = true;
  }
  
  void OnValidate() {
    needsUpdate = true;
    if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count) {
      bakeKeys.Clear();
      bakeValues.Clear();
    }
    if (precomputeOutline && bakeKeys.Count == 0) {
      Bake();
    }
  }
  void Update() {
    if (needsUpdate) {
      needsUpdate = false;

      UpdateMaterialProperties();
    }
  }

  void OnEnable() {
    foreach (var renderer in renderers) {
      var materials = renderer.sharedMaterials.ToList();
      materials.Add(outlineMaskMaterial);
      materials.Add(outlineFillMaterial);
      renderer.materials = materials.ToArray();
    }
    //StartCoroutine(fadeTransitionIn());
    outlineColor.a = 1;
    UpdateMaterialProperties();
  }
  
  public IEnumerator fadeTransitionIn()
  {
    Color tmp = outlineColor;
    while (tmp.a < 1)
    {
      tmp.a += 0.02f;
      outlineColor = tmp;
      UpdateMaterialProperties();
      yield return new WaitForSeconds(0.01f);
    }
  }

  void OnDisable() {
    //StartCoroutine(fadeTransitionOut());
    outlineColor.a = 0;
    UpdateMaterialProperties();
    foreach (var renderer in renderers) {
      var materials = renderer.sharedMaterials.ToList();
      materials.Remove(outlineMaskMaterial);
      materials.Remove(outlineFillMaterial);
      renderer.materials = materials.ToArray();
    }
  }
  
  public IEnumerator fadeTransitionOut()
  {
    Color tmp = outlineColor;
    while (tmp.a > 0)
    {
      tmp.a -= 0.02f;
      outlineColor = tmp;
      UpdateMaterialProperties();
      yield return new WaitForSeconds(0.01f);
    }
    foreach (var renderer in renderers) {
      var materials = renderer.sharedMaterials.ToList();
      materials.Remove(outlineMaskMaterial);
      materials.Remove(outlineFillMaterial);
      renderer.materials = materials.ToArray();
    }
  }

  void OnDestroy() {
    Destroy(outlineMaskMaterial);
    Destroy(outlineFillMaterial);
  }

  void UpdateMaterialProperties() {
    outlineFillMaterial.SetColor("_OutlineColor", outlineColor);
    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
    outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
  }
  
  void Bake() {
    var bakedMeshes = new HashSet<Mesh>();
    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>()) {
      if (!bakedMeshes.Add(meshFilter.sharedMesh)) {
        continue;
      }
      var smoothNormals = SmoothNormals(meshFilter.sharedMesh);
      bakeKeys.Add(meshFilter.sharedMesh);
      bakeValues.Add(new ListVector3() { data = smoothNormals });
    }
  }

  void LoadSmoothNormals()
  {
    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
    {
      if (!registeredMeshes.Add(meshFilter.sharedMesh))
      {
        continue;
      }
      var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
      var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);
      meshFilter.sharedMesh.SetUVs(3, smoothNormals);
      var renderer = meshFilter.GetComponent<Renderer>();
      if (renderer != null)
      {
        CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
      }
    }
    foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>()) {
      if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh)) {
        continue;
      }
      skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
      CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
    }
  }

  List<Vector3> SmoothNormals(Mesh mesh)
    {
      var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index))
        .GroupBy(pair => pair.Key);
      var smoothNormals = new List<Vector3>(mesh.normals);
      foreach (var group in groups)
      {
        if (group.Count() == 1)
        {
          continue;
        }
        var smoothNormal = Vector3.zero;
        foreach (var pair in group)
        {
          smoothNormal += smoothNormals[pair.Value];
        }
        smoothNormal.Normalize();
        foreach (var pair in group)
        {
          smoothNormals[pair.Value] = smoothNormal;
        }
      }
      return smoothNormals;
    }

    void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
      if (mesh.subMeshCount == 1)
      {
        return;
      }
      if (mesh.subMeshCount > materials.Length)
      {
        return;
      }
      mesh.subMeshCount++;
      mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }
}
