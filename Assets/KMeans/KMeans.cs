using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMeans : MonoBehaviour
{
    // Randomly generated data set
    public int Width = 30;
    public int Depth = 30;
    public int Points = 40;
    public int Centroids = 3;
    public GameObject Point;
    public GameObject Centroid;
    public Transform PointsHolder;
    public Transform CentroidsHolder;
    public GameObject DoneText;

    List<GameObject> points;
    List<GameObject> centroids;
    List<Color> colors; // New colours generated each time a data set is generated
    Dictionary<GameObject, List<GameObject>> clusters;
    List<Vector3> previousCentroids; // This calls for the centroids to stop if the new set are in the same position as the old set

    // Start is called before the first frame update
    void Start()
    {
        StartKMeansClustering();
    }

    public void StartKMeansClustering()
    {
        ClearData();

        // Initialization
        points = GenerateGameObjects(Point, Points, PointsHolder);
        centroids = GenerateGameObjects(Centroid, Centroids, CentroidsHolder);
        previousCentroids = GetCentroidsList();
        colors = GenerateColors();
        SetColorsToCentroids();

        // Start with an execution of the algorithm
        Cluster();
    }

    private List<Vector3> GetCentroidsList()
    {
        var result = new List<Vector3>();

        foreach (var item in centroids)
        {
            result.Add(item.transform.position);
        }

        return result;
    }

    public void Cluster()
    {
        // Construct clusters dictionary
        clusters = InitializeClusters();

        // Add points to clusters they belong
        AddPointsToClusters();

        // If there's a cluster with no points extract the closest point and add it to the empty cluster
        CheckForEmptyClusters();

        // Set colors to points from each cluster
        SetColorToClusterPoints();

        // Take the sum of all the positions in the cluster and divide by the number of points
        RecomputeCentroidPositions();

        // Check if no centroids changed their position
        CheckForEnd();

        // Update previous centroids to the positions of current
        UpdatePreviousCentroids();
    }

    private void ClearData()
    {
        DeleteChildren(PointsHolder);
        DeleteChildren(CentroidsHolder);
        DoneText.SetActive(false);
    }

    private void DeleteChildren(Transform parent)
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
    }

    private void UpdatePreviousCentroids()
    {
        for (int i = 0; i < centroids.Count; i++)
        {
            previousCentroids[i] = centroids[i].transform.position;
        }
    }

    private void CheckForEnd()
    {
        for (int i = 0; i < centroids.Count; i++)
        {
            if (centroids[i].transform.position != previousCentroids[i])
                return;
        }

        DoneText.SetActive(true);
    }

    private void RecomputeCentroidPositions()
    {
        var clusterCounter = 0;
        foreach (var cluster in clusters)
        {
            var sum = Vector3.zero;

            foreach (var point in cluster.Value)
            {
                sum += point.transform.position;
            }

            var average = sum / cluster.Value.Count;
            centroids[clusterCounter].transform.position = average;
            clusterCounter++;
        }
    }

    private void SetColorToClusterPoints()
    {
        var clusterCounter = 0;
        foreach (var cluster in clusters)
        {
            foreach (var point in cluster.Value)
            {
                point.GetComponent<MeshRenderer>().material.color = colors[clusterCounter];
            }
            clusterCounter++;
        }
    }

    private void CheckForEmptyClusters()
    {
        foreach (var cluster in clusters)
        {
            if (cluster.Value.Count == 0)
            {
                var closestPoint = ExtractClosestPointToCluster(cluster.Key.transform.position);
                cluster.Value.Add(closestPoint);
            }
        }
    }

    private GameObject ExtractClosestPointToCluster(Vector3 clusterPosition)
    {
        var closestPoint = points[0];
        GameObject clusterThePointBelongsTo = null;
        var minDistance = float.MaxValue;

        foreach (var cluster in clusters)
        {
            foreach (var point in cluster.Value)
            {
                var distance = Vector3.Distance(point.transform.position, clusterPosition);
                if (distance < minDistance && cluster.Value.Count > 1)
                {
                    closestPoint = point;
                    minDistance = distance;
                    clusterThePointBelongsTo = cluster.Key;
                }
            }
        }

        clusters[clusterThePointBelongsTo].Remove(closestPoint);
        return closestPoint;
    }

    private Dictionary<GameObject, List<GameObject>> InitializeClusters()
    {
        // At this point we will have the centroids already generated
        var result = new Dictionary<GameObject, List<GameObject>>();

        for (int i = 0; i < Centroids; i++)
        {
            result.Add(centroids[i], new List<GameObject>());
        }

        return result;
    }

    private void AddPointsToClusters()
    {
        for (int i = 0; i < Points; i++)
        {
            var pointPosition = points[i].transform.position;
            var minDistance = float.MaxValue;
            var closestCentroid = centroids[0]; // Randomly pick any centroid

            for (int j = 0; j < Centroids; j++)
            {
                var distance = Vector3.Distance(pointPosition, centroids[j].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCentroid = centroids[j];
                }
            }

            clusters[closestCentroid].Add(points[i]);
        }
    }

    private void SetColorsToCentroids()
    {
        for (int i = 0; i < centroids.Count; i++)
        {
            centroids[i].GetComponent<MeshRenderer>().material.color = colors[i];
        }
    }

    private List<Color> GenerateColors()
    {
        var result = new List<Color>();

        for (int i = 0; i < Centroids; i++)
        {
            var color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            result.Add(color);
        }

        return result;
    }

    private List<GameObject> GenerateGameObjects(GameObject prefab, int size, Transform parent)
    {
        var result = new List<GameObject>();

        for (int i = 0; i < size; i++)
        {
            var prefabXScale = prefab.transform.localScale.x;
            var positionX = UnityEngine.Random.Range(-Width / 2 + prefabXScale, Width / 2 - prefabXScale);

            var prefabZScale = prefab.transform.localScale.z;
            var positionZ = UnityEngine.Random.Range(-Depth / 2 + prefabZScale, Depth / 2 - prefabZScale);

            var newPosition = new Vector3(positionX, prefab.transform.position.y, positionZ);
            var newGameObject = Instantiate(prefab, newPosition, Quaternion.identity, parent);

            result.Add(newGameObject);
        }

        return result;
    }
}
