using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordSelectionManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private List<WordData> selectedWords = new List<WordData>();
    private LineRenderer lineRenderer;
    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.sortingLayerName = "UI";
        lineRenderer.sortingOrder = 10;
        lineRenderer.useWorldSpace = true;

        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        selectedWords.Clear();
        lineRenderer.positionCount = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        foreach (WordData word in selectedWords)
        {
            word.DeselectChar();
        }
        selectedWords.Clear();
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && graphicRaycaster != null && eventSystem != null)
        {
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            // Raycast to detect UI elements
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                WordData wordData = result.gameObject.GetComponent<WordData>();
                if (wordData != null && !wordData.isSelected)
                {
                    selectedWords.Add(wordData);
                    wordData.SelectChar();

                    // Update line renderer
                    lineRenderer.positionCount = selectedWords.Count;
                    lineRenderer.SetPosition(selectedWords.Count - 1, wordData.transform.position);
                }
            }
        }
    }
    public void ResetSelections()
    {
        foreach (WordData word in selectedWords)
        {
            word.DeselectChar();
        }
        selectedWords.Clear();
        lineRenderer.positionCount = 0;
    }
}
