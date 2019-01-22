using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    public Mesh[] meshes;
    public Material material;
    public int MaxDepth = 5;
    private int _currentDepth;
    public float ChildScale = 0.5f;

    private static Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] childOrientations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
    };
    private Material[,] _materials;
    public Color BaseColor = Color.white, EndColorOne = Color.yellow , EndColorTwo = Color.cyan , CapColorOne = Color.magenta, CapColorTwo = Color.red;
    private void InitializeMaterials()
    {
        _materials = new Material[MaxDepth + 1, 2];
        for (int i = 0; i <= MaxDepth; i++)
        {
            float t = i / (MaxDepth - 1f);
            t *= t;
            _materials[i, 0] = new Material(material);
            _materials[i, 0].color =
                Color.Lerp(BaseColor, EndColorOne, t);

            _materials[i, 1] = new Material(material);
            _materials[i, 1].color = Color.Lerp(BaseColor, EndColorTwo, t);
        }
        _materials[MaxDepth, 0].color = CapColorOne;
        _materials[MaxDepth, 1].color = CapColorTwo;
    }
    // Use this for initialization
    void Start()
    {
        if (_materials == null)
        {
            InitializeMaterials();
        }
        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = _materials[_currentDepth, Random.Range(0, 2)];
        if (_currentDepth < MaxDepth)
        {
            StartCoroutine(CreateChildren());
        }
    }
    private void Initialize(Fractal parent, int childIndex)
    {
        _materials = parent._materials;
        meshes = parent.meshes;
        material = parent.material;
        MaxDepth = parent.MaxDepth;
        _currentDepth = parent._currentDepth + 1;
        transform.parent = parent.transform;
        ChildScale = parent.ChildScale;
        transform.localScale = Vector3.one * ChildScale;
        transform.localPosition =
             childDirections[childIndex] * (0.5f + 0.5f * ChildScale);
        transform.localRotation = childOrientations[childIndex];
    }
    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childDirections.Length; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            new GameObject("Fractal Child").AddComponent<Fractal>().
                Initialize(this, i);
        }
    }
}
