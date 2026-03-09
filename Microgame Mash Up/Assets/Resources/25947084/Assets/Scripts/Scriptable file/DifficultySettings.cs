using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDifficultySettings", menuName = "Game/Difficulty Settings")]
public class DifficultySettings : ScriptableObject
{
    [Header("Difficulty Info")]
    public GameMaster.Difficulty difficulty;
    public string difficultyName;

    [Header("Hit Settings")]
    public int maxHits = 1;
    public bool allowHits = true;

    [Header("Obstacle Speed Settings")]
    public float minSpeed = 3f;
    public float maxSpeed = 5f;
    public bool randomizeSpeed = true;

    [Header("Obstacle Spawn Settings")]
    public float spawnInterval = 1.5f;
    public int maxObstaclesOnScreen = 10;

    [Header("Eagle Settings")]
    public bool enableEagle = false; // Only true for Normal and above
    public float eagleChance = 0.2f; // 20% chance to spawn eagle instead of regular obstacle
    public float eagleSpeed = 15f;
    public float eagleWarningTime = 1.5f;

    [Header("Obstacle Size Settings")]
    public float minHeight = 0.8f;
    public float maxHeight = 1.2f;
    public float minWidth = 0.8f;
    public float maxWidth = 1.2f;
    public bool randomizeSize = true;

    [Header("Special Settings")]
    public bool enablePowerups = false;
    public float powerupChance = 0.1f;

    [Header("Scoring")]
    public int pointsPerSecond = 10;

    public float GetRandomSpeed()
    {
        return randomizeSpeed ? Random.Range(minSpeed, maxSpeed) : (minSpeed + maxSpeed) / 2f;
    }

    public float GetRandomHeight()
    {
        return randomizeSize ? Random.Range(minHeight, maxHeight) : (minHeight + maxHeight) / 2f;
    }

    public float GetRandomWidth()
    {
        return randomizeSize ? Random.Range(minWidth, maxWidth) : (minWidth + maxWidth) / 2f;
    }
}