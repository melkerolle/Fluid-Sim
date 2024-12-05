using UnityEngine;
using Unity.Mathematics;
using System.Security.Cryptography;
using System;

public class ParticleSpawner : MonoBehaviour
{
    public int particleCount;

    public Vector2 initialVelocity;
    public Vector2 spawnCentre;
    public Vector2 spawnSize;
    public float jitterStr;
    public bool showSpawnBoundsGizmos;

    public ParticleSpawnData GetSpawnData()
    {
        ParticleSpawnData data = new ParticleSpawnData(particleCount);
        var rng = new Unity.Mathematics.Random(Convert.ToUInt32(UnityEngine.Random.Range(0, 10000)));
        float2 s = spawnSize;
        int numX = Mathf.CeilToInt(Mathf.Sqrt(s.x / s.y * particleCount + (s.x - s.y) * (s.x - s.y) / (4 * s.y * s.y)) - (s.x - s.y) / (2 * s.y));
        int numY = Mathf.CeilToInt(particleCount / (float)numX);
        int i = 0;

        for (int y = 0; y < numY; y++)
        {
            for (int x = 0; x < numX; x++)
            {
                if (i >= particleCount) break;



                float tx = numX <= 1 ? 0.5f : x / (numX - 1f);
                float ty = numY <= 1 ? 0.5f : y / (numY - 1f);

                float angle = (float)rng.NextDouble() * 3.14f * 2;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 jitter = dir * jitterStr * ((float)rng.NextDouble() - 0.5f);
                Vector2 pos = new Vector2((tx - 0.5f) * spawnSize.x, (ty - 0.5f) * spawnSize.y) + jitter + spawnCentre;
                Vector2 dif = pos - spawnCentre;
                if (dif.magnitude > spawnSize.x/2) continue;
                data.positions[i] = pos;
                data.velocities[i] = initialVelocity;
                i++;
            }
        }

        ParticleSpawnData newData = new ParticleSpawnData(i);
        newData.velocities = data.velocities[0..i];
        newData.positions = data.positions[0..i];
        return newData;
    }

    public struct ParticleSpawnData
    {
        public float2[] positions;
        public float2[] velocities;

        public ParticleSpawnData(int num)
        {
            positions = new float2[num];
            velocities = new float2[num];
        }
    }

    void OnDrawGizmos()
    {
        if (showSpawnBoundsGizmos && !Application.isPlaying)
        {
            Gizmos.color = new Color(1, 1, 0, 0.5f);
            Gizmos.DrawWireCube(spawnCentre, Vector2.one * spawnSize);
        }
    }
}
