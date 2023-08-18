using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject cylinderPrefab;
    public Material validMaterial; 
    public Material invalidMaterial; 
    public float dragScale = 0.7f; 
    public float distanceFromCamera = 10f; 

    private GameObject currentCylinder;
    private Renderer lastHexRenderer; 
    private Material originalHexMaterial; 

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 mouseScreenPosition = new Vector3(eventData.position.x, eventData.position.y, distanceFromCamera);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        currentCylinder = Instantiate(cylinderPrefab, mouseWorldPosition, Quaternion.identity);
        currentCylinder.transform.localScale *= dragScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mouseScreenPosition = new Vector3(eventData.position.x, eventData.position.y, distanceFromCamera);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        currentCylinder.transform.position = mouseWorldPosition;

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (lastHexRenderer != null)
            {
                lastHexRenderer.material = originalHexMaterial; 
            }

            if (hit.collider != null && hit.collider.gameObject.tag == "Unit")
            {
                Renderer hexRenderer = hit.collider.GetComponent<Renderer>();
                if (hexRenderer != null)
                {
                   
                    if (hit.collider.transform.childCount > 0)
                    {
                        lastHexRenderer = null; 
                    }
                    else
                    {
                        lastHexRenderer = hexRenderer;
                        originalHexMaterial = hexRenderer.material; 
                        lastHexRenderer.material = validMaterial;
                    }
                }
            }
            else
            {
                lastHexRenderer = null;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        currentCylinder.transform.localScale /= dragScale;

        if (lastHexRenderer != null)
        {
            currentCylinder.transform.SetParent(lastHexRenderer.transform); 
            currentCylinder.transform.position = lastHexRenderer.transform.position + new Vector3(0, currentCylinder.transform.localScale.y / 2, 0);
            lastHexRenderer.material = originalHexMaterial; 
        }
        else
        {
            Destroy(currentCylinder);
        }

        lastHexRenderer = null;
    }
}
