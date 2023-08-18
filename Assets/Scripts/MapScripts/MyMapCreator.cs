using System.Collections;
using UnityEngine;

public class MyMapCreator : MonoBehaviour
{
    public GameObject hexPrefab;
    public GameObject castlePrefab;
    public Material spawnMaterial;
    public Material baseMaterial;
    public Material pathMaterial;
    public Material unitMaterial;
    public Material damagedCastleMaterial;
    public Material destroyedCastleMaterial;
    public int width = 25;
    public int height = 25;
    public int castleLife = 100;
    public Transform boundary;
    private GameObject castle;
    private float xOffset = 1.732f;
    private float zOffset = 1.5f;
    private GameObject[,] hexGrid;
   void Start()
{
    hexGrid = new GameObject[width, height];
    GenerateMap();
    Vector2Int startPos = new Vector2Int(Random.Range(0, width), 0); 
    StartCoroutine(GeneratePathWithEffect(startPos));
}

    void GenerateMap()
    {
        Quaternion rotation = Quaternion.Euler(-88, 0, 0);
        float totalWidth = (width - 1) * xOffset;
        Vector3 startPos = new Vector3(-totalWidth / 2, 0, 0);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float xPos = startPos.x + x * xOffset;
                if (z % 2 == 0)
                {
                    xPos += xOffset / 2f;
                }

                float zPos = startPos.z + z * zOffset;
                GameObject hex = Instantiate(hexPrefab, new Vector3(xPos, 0, zPos), rotation);
                hexGrid[x, z] = hex;
            }
        }

        
    }


    public void ReduceCastleLife(int damage)
    {
        castleLife -= damage;
        if (castleLife <= 0)
        {
            castle.GetComponent<Renderer>().material = destroyedCastleMaterial;
        }
        else
        {
            castle.GetComponent<Renderer>().material = damagedCastleMaterial;
            StartCoroutine(ShakeEffect(castle, 0.1f, 0.2f));
        }
    }

    IEnumerator ShakeEffect(GameObject target, float magnitude, float duration)
    {
        Vector3 originalPosition = target.transform.position;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float x = originalPosition.x + Random.Range(-magnitude, magnitude);
            float y = originalPosition.y + Random.Range(-magnitude, magnitude);

            target.transform.position = new Vector3(x, originalPosition.y, y);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        target.transform.position = originalPosition;
    }
IEnumerator GeneratePathWithEffect(Vector2Int startPos)
{
    Vector2Int currentPos = startPos;
    Vector2Int direction = Random.Range(0, 2) == 0 ? new Vector2Int(0, 1) : new Vector2Int(1, 0); // Dirección inicial aleatoria
    int minSegmentLength = 3;
    int maxSegmentLength = 5;
    float turnProbability = 0.4f;
    int pathHexagonCount = 0;
    GameObject firstPathHex = null; 

    while (pathHexagonCount < 25)
    {
        int segmentLength = Random.Range(minSegmentLength, maxSegmentLength + 1);

        for (int step = 0; step < segmentLength; step++)
        {
            Vector2Int nextPos = currentPos + direction;
            if (!IsValid(nextPos) || (nextPos.y == height - 1 && direction.y == 1))
            {
                break;
            }

            GameObject nextHex = hexGrid[nextPos.x, nextPos.y];
            if (nextHex.tag == "Path" || HasAdjacentHexagon(nextPos, direction))
            {
                break;
            }

            GameObject pathHex = hexGrid[currentPos.x, currentPos.y];
            if (firstPathHex == null) firstPathHex = pathHex; // Guardamos el primer hexágono de la ruta

            StartCoroutine(BoingEffect(pathHex, 0.5f, 0.2f)); // Efecto Boing más pronunciado
            pathHex.GetComponent<Renderer>().material = pathMaterial;
            pathHex.tag = "Path";
            CreateUnitHexagons(currentPos);
            pathHexagonCount++;

            currentPos += direction;
        }

        if (Random.value < turnProbability)
        {
            direction = Random.value < 0.5f ? new Vector2Int(-direction.y, direction.x) : new Vector2Int(direction.y, -direction.x);
        }

        yield return new WaitForSeconds(0.5f);
    }

    GameObject baseHex = hexGrid[currentPos.x, currentPos.y];
    baseHex.GetComponent<Renderer>().material = baseMaterial;
    baseHex.tag = "Base";

    Vector3 castlePosition = baseHex.transform.position;
    castlePosition.y = 1; 
    castle = Instantiate(castlePrefab, castlePosition, Quaternion.identity);
    firstPathHex.GetComponent<Renderer>().material = spawnMaterial;
    firstPathHex.tag = "Spawn";
}

    bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    bool HasAdjacentHexagon(Vector2Int pos, Vector2Int direction)
    {
        Vector2Int checkPos = pos + direction;

        if (!IsValid(checkPos))
        {
            return false;
        }

        GameObject adjacentHex = hexGrid[checkPos.x, checkPos.y];

        if (adjacentHex.tag == "Path")
        {
            return true;
        }

        return false;
    }
void CreateUnitHexagons(Vector2Int pos)
{
    Vector2Int[] neighbors = {
        new Vector2Int(pos.x + 1, pos.y), new Vector2Int(pos.x - 1, pos.y),
        new Vector2Int(pos.x, pos.y + 1), new Vector2Int(pos.x, pos.y - 1),
        new Vector2Int(pos.x + 1, pos.y - 1), new Vector2Int(pos.x - 1, pos.y + 1)
    };

    foreach (Vector2Int neighbor in neighbors)
    {
        if (IsValid(neighbor) && hexGrid[neighbor.x, neighbor.y].tag != "Path")
        {
            GameObject unitHex = hexGrid[neighbor.x, neighbor.y];
            unitHex.GetComponent<Renderer>().material = unitMaterial;
            unitHex.tag = "Unit";
        }
    }
}

IEnumerator BoingEffect(GameObject hex, float duration, float bounceHeight)
{
    float timeElapsed = 0;
    Vector3 originalPosition = hex.transform.position;

    while (timeElapsed < duration)
    {
        float bounceAmount = Mathf.Sin(timeElapsed / duration * Mathf.PI) * bounceHeight; // Ajuste de altura
        hex.transform.position = originalPosition + new Vector3(0, bounceAmount, 0);
        timeElapsed += Time.deltaTime;
        yield return null;
    }

    hex.transform.position = originalPosition;
}


}
