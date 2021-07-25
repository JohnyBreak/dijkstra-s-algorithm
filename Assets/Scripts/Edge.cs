using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Edge : MonoBehaviour
{
    private GameObject _firstPoint;
    private GameObject _secondPoint;
    [SerializeField] private float _cost;
    [SerializeField] private int _manualCost;
    private LineRenderer _lineRenderer;
    //[SerializeField] private TextMeshPro _tmp;
    [SerializeField] private Material _standartMat, _pathMat;
    private bool _path = false;
    void Start()
    {
        
           _lineRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        CreateEdge();
    }
    public float GetCost()
    {
            return _cost;
    }
    public float GetManualCost()
    {
        return _manualCost;
    }
    public void SetPoints(GameObject first, GameObject second, bool p)
    {
        _firstPoint = first;
        _secondPoint = second;
        _path = p;
    }

    public void CreateEdge()
    {
       
        _lineRenderer.SetPosition(0, _firstPoint.transform.position);
        _lineRenderer.SetPosition(1, _secondPoint.transform.position);
        _cost = Vector2.Distance(_firstPoint.transform.position, _secondPoint.transform.position);
        //_tmp.text = _cost.ToString();
        _manualCost = Mathf.FloorToInt(_cost);
        if (_path)
            _lineRenderer.material = _pathMat;
    }
}
