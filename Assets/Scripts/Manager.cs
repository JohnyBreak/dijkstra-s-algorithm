using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{ 
    [SerializeField] private GameObject _edgePrefab;
    [SerializeField] private List<GameObject> _vertexList, _edgeList;
    [SerializeField] private List<string> _edgeListByName;
    private GameObject startPoint = null;
    private GameObject finishPoint = null;
    private GameObject currentVertex, prevVertex;
    private List<GameObject> _pathVertexList, _pathEdgeList;
    [SerializeField] private bool _manual;
    private bool _reseted = false;
    void Start()
    {
        _vertexList.Reverse();
        CreateEdge();
        _pathVertexList = new List<GameObject>();
        _pathEdgeList = new List<GameObject>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _reseted)
        {
            _reseted = false;
            FindPath();
        }
        if (Input.GetMouseButtonDown(0))
        {
            CastRay(0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            CastRay(1);
        }
    }
    void CastRay(int mb)
    {
        RaycastHit2D hit;
        Vector2 origin = Vector2.zero;
        Vector2 dir = Vector2.zero;
        origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(origin, dir);
        if (hit)
        {
            if (hit.transform.GetComponent<Vertex>() != null)
            {
                hit.transform.GetComponent<Vertex>().ChangeStatus(mb);
                Reset();
            }
        }
    }

    public void UnSetPoints(int mb)
    {
        if (mb == 0)
        {
            startPoint = null;
        }
        if (mb == 1)
        {
            finishPoint = null;
        }
    }

    public void SetStartAndFinishPoints(int mb, GameObject vertex)
    {
        switch (mb)
        {
            case 0:
                if (vertex != finishPoint)
                {
                    startPoint = vertex;
                }
                
                break;
            case 1:
                if (vertex != startPoint)
                {
                    finishPoint = vertex;
                }
                break;
            default:
                Debug.Log("invalid data");
                break;
        }
    }
    void FindPath()
    {
        bool start = false, finish = false;
        foreach (var vertex in _vertexList)
        {
            vertex.GetComponent<Vertex>().Cost = float.MaxValue;
            vertex.GetComponent<Vertex>().Visited = false;
        }
        foreach (var vertex in _vertexList)
        {
            if (vertex.GetComponent<Vertex>().VertexStatus == Vertex.Status.Start)
            {
                startPoint = vertex;
                start = true;
                break;
            }
        }
        foreach (var vertex in _vertexList)
        {
            if (vertex.GetComponent<Vertex>().VertexStatus == Vertex.Status.Finish)
            {
                finishPoint = vertex;
                finish = true;
                break;
            }
        }
        if (start && finish)
        {
            startPoint.GetComponent<Vertex>().Cost = 0;
            DijkstraAlgorithm(startPoint);
        }
        else
            Debug.Log("no points");

    }
    void DijkstraAlgorithm(GameObject start)
    {
        currentVertex = start;
        float costOfPath = 0;
        GameObject currentEdge;
        if (currentVertex.GetComponent<Vertex>().Visited)
        {
            List<GameObject> notVisitedVertexes = new List<GameObject>();
            foreach (var v in _vertexList)
            {
                if (v.GetComponent<Vertex>().Visited)
                {
                    foreach (var notVisitedNeighbour in v.GetComponent<Vertex>().GetNeighbourList())
                    {
                        if (!notVisitedNeighbour.GetComponent<Vertex>().Visited)
                        {
                            notVisitedVertexes.Add(notVisitedNeighbour);
                        }
                    }
                }
            }
            float Cost = float.MaxValue;
            
            foreach (var minCostVertex in notVisitedVertexes)
            {
                if (minCostVertex.GetComponent<Vertex>().Cost < Cost)
                {
                    Cost = minCostVertex.GetComponent<Vertex>().Cost;
                    currentVertex = minCostVertex;
                }
            }
        }
        List<GameObject> currentVertexNeighbourList = currentVertex.GetComponent<Vertex>().GetNeighbourList();
        List<GameObject> currentVertexEdgeList = currentVertex.GetComponent<Vertex>().GetEdgeList();
        foreach (var currentVertexNeighbour in currentVertexNeighbourList)
        {
            List<GameObject> neighbourEdgeList = currentVertexNeighbour.GetComponent<Vertex>().GetEdgeList();
            foreach (var currentVertexEdge in currentVertexEdgeList)
            {
                foreach (var neighbourEdge in neighbourEdgeList)
                {
                    if (currentVertexEdge == neighbourEdge)
                    {
                        currentEdge = currentVertexEdge;

                        if (_manual)
                            costOfPath = currentEdge.GetComponent<Edge>().GetManualCost();
                        else
                            costOfPath = (float)currentEdge.GetComponent<Edge>().GetCost();
                    }
                }
            }
            float tempCost = costOfPath + currentVertex.GetComponent<Vertex>().Cost;
            if (currentVertexNeighbour.GetComponent<Vertex>().Cost > tempCost)
            {
                currentVertexNeighbour.GetComponent<Vertex>().Cost = tempCost;
                currentVertexNeighbour.GetComponent<Vertex>().Prev = currentVertex;
            }
        }
        prevVertex = currentVertex;
        float tmpCost = float.MaxValue;
        currentVertex.GetComponent<Vertex>().Visited = true;
        //Debug.Log(currentVertex.name);
        if (currentVertex == finishPoint)
        {
            EvaluatePath();
            return;
        }
        foreach (var nextCurrentPoint in currentVertexNeighbourList)
        {
            if (!nextCurrentPoint.GetComponent<Vertex>().Visited)
            {
                if (nextCurrentPoint.GetComponent<Vertex>().Cost < tmpCost)
                {
                    tmpCost = nextCurrentPoint.GetComponent<Vertex>().Cost;
                    currentVertex = nextCurrentPoint;
                }
            }
        }
        if (currentVertex != finishPoint)
        {
            DijkstraAlgorithm(currentVertex);
            return;
        }
        else
        {
            EvaluatePath();
        }
    }
    void EvaluatePath()
    {
        Debug.Log("cost of path = "+finishPoint.GetComponent<Vertex>().Cost);
        _pathVertexList.Add(finishPoint);
        GameObject tmpPoint = finishPoint;
        while (tmpPoint != startPoint)
        {
            tmpPoint = tmpPoint.GetComponent<Vertex>().Prev;
            _pathVertexList.Add(tmpPoint);
        }
        for (int i = 0; i < _pathVertexList.Count; i++)
        {
            if (i != _pathVertexList.Count - 1)
            {
                _pathVertexList[i].GetComponent<Vertex>().ChangeColor();
                GameObject newEdgeGen = Instantiate(_edgePrefab);
                newEdgeGen.GetComponent<Edge>().SetPoints(_pathVertexList[i], _pathVertexList[i + 1], true);
                _pathEdgeList.Add(newEdgeGen);
            }
        }
        _pathVertexList.Reverse();
        foreach (var edge in _edgeList)
        {
            edge.SetActive(false);
        }
    }

    public List<GameObject> GetPath()
    {
        return _pathVertexList;
    }

    void Reset()
    {

        _reseted = true;
        foreach (var edge in _edgeList)
        {
            edge.SetActive(true);
        }
        foreach (var pathEdge in _pathEdgeList)
        {
            Destroy(pathEdge.gameObject);
        }
        foreach (var vertex in _vertexList)
        {
            vertex.GetComponent<Vertex>().Cost = float.MaxValue;
            vertex.GetComponent<Vertex>().Visited = false;
        }
        _pathEdgeList.Clear();
        _pathVertexList.Clear();
    }
    public List<GameObject> GetVertexList()
    {
        return _vertexList;
    }
    void CreateEdge()
    {
        foreach (var vertex in _vertexList)
        {
            List<GameObject> vertexNeighbourList = vertex.GetComponent<Vertex>().GetNeighbourList();
            foreach (var tempVertex in vertexNeighbourList)
            {
                bool canCreateEdge = true;
                foreach (var edgePair in _edgeListByName)
                {
                    if ((edgePair == tempVertex.ToString() + vertex.ToString()) || (edgePair == vertex.ToString() + tempVertex.ToString()))
                    {
                        canCreateEdge = false;
                        break;
                    }
                }
                if (canCreateEdge)
                {
                    GameObject newEdgeGen = Instantiate(_edgePrefab);
                    newEdgeGen.GetComponent<Edge>().SetPoints(vertex, tempVertex, false);
                    vertex.GetComponent<Vertex>().AddEdge(newEdgeGen);
                    tempVertex.GetComponent<Vertex>().AddEdge(newEdgeGen);
                    _edgeListByName.Add(vertex.ToString() + tempVertex.ToString());
                    _edgeList.Add(newEdgeGen);
                }
            }
        }
    }
}
