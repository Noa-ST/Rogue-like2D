using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPostition;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject _latestChunk;
    public float maxOpDist;
    float _opDist;
    float _optimizerCooldown;
    public float optimizerCooldownDur;

    void Start()
    {
        playerLastPostition = player.transform.position;
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimzer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPostition;
        playerLastPostition = player.transform.position;
        string dirName = GetDirectionName(moveDir);

        CheckAndSpawnChunk(dirName);
        if (dirName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }
        if (dirName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }
        if (dirName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }
        if (dirName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }
    }

    void CheckAndSpawnChunk(string dir)
    {
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(dir).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(dir).position);
        }
    }

    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.y > 0.5f)
            {
                return direction.x > 0 ? "Right Up" : "Left Up";
            }
            else if (direction.y < -0.5f)
            {
                return direction.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            if (direction.x > 0.5f)
            {
                return direction.y > 0 ? "Right Up" : "Right Down";
            }
            else if (direction.x < -0.5f)
            {
                return direction.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpawnChunk(Vector3 spawnPostition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        _latestChunk = Instantiate(terrainChunks[rand], spawnPostition, Quaternion.identity);
        spawnedChunks.Add(_latestChunk);
        currentChunk = _latestChunk;
    }

    void ChunkOptimzer()
    {
        _optimizerCooldown -= Time.deltaTime;

        if (_optimizerCooldown <= 0f)
        {
            _optimizerCooldown = optimizerCooldownDur;   //Check every 1 second to save cost, change this value to lower to check more times
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in spawnedChunks)
        {
            _opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (_opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}