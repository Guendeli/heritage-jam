using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    
    public Sprite[] sprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Shuffle<Sprite>(sprites);
        spriteRenderer.sprite = sprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static void Shuffle<T>(Sprite[] sprites)
    {
        System.Random Rng = new System.Random();
        int n = sprites.Length;
        while (n > 1)
        {
            n--;
            // Generate a random index k such that 0 <= k <= n (inclusive of n)
            int k = Rng.Next(n + 1);

            // Swap the elements at list[k] and list[n]
            Sprite value = sprites[k];
            sprites[k] = sprites[n];
            sprites[n] = value;

            
        }
    }
}
