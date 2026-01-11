using UnityEngine;


public class SafeAreaView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public float Radius = 1f;
    [SerializeField] private SpriteRenderer Visual;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Visual == null)
        {
            Visual = GetComponent<SpriteRenderer>();
        }
        
        Visual.transform.localScale = Vector3.one * Radius;
    }
    
}
