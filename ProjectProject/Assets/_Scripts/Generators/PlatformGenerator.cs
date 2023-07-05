using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : Singleton<PlatformGenerator>
{

    [SerializeField]
    private List<Platform> platformsPrefabs = new List<Platform>();

    [SerializeField]
    private Player player;

    private GameObject platformContainer;

    public List<Platform> GeneratePlatforms(Transform parentTransform, bool initChunk, out GameObject _platformContainer)
    {
        float jumpTollerance = 0.7f;
        float h1 = player.jumpVelocity * 0.5f;
        float t = player.jumpVelocity / player.gravity;
        float h2 = player.jumpVelocity * t + (0.5f * (player.gravity * (t * t)));
        float maxJumpHeight = h1 + h2;
        //float maxY = Mathf.Ceil((player.transform.position.y + maxJumpHeight) * 0.7f);
        float minY = -6;

        platformContainer = new GameObject("Platforms");
        platformContainer.transform.position = parentTransform.position;
        platformContainer.transform.SetParent(parentTransform);

        _platformContainer = platformContainer;

        List<Platform> platforms = new List<Platform>();
        
        if(!initChunk)
        {
            for (int i = 0; i < 8; i++)
            {
                float posY = Mathf.Ceil(Random.Range(-10, (-7 + maxJumpHeight) * jumpTollerance));
                float posX = parentTransform.position.x - 30f;
                if (platforms.Count > 0)
                {
                    Platform previousPlatform = platforms[platforms.Count - 1];
                    float maxY = previousPlatform.transform.position.y + maxJumpHeight;
                    maxY *= jumpTollerance;
                    float actuallyY = Random.Range(minY, maxY);
                    posY = Mathf.Ceil(actuallyY);
                    posX = previousPlatform.transform.position.x + previousPlatform.GetComponent<BoxCollider2D>().size.x + 4f;
                }
                Platform platformPrefab = platformsPrefabs[Random.Range(0, platformsPrefabs.Count)];
                Vector2 position = new Vector2(posX, posY);
                Platform platform = Instantiate(platformPrefab, position, Quaternion.identity).GetComponent<Platform>();
                platform.transform.SetParent(platformContainer.transform);

                platforms.Add(platform);
            
            }
        }
        
        return platforms;
    }
}
