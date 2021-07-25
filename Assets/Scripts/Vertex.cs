using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    [SerializeField] private List<GameObject> _neighbourList;
    [SerializeField] private List<GameObject> _edgeList;
    [SerializeField] private Manager _manager;
    public bool Visited = false;
    public float Cost = float.MaxValue;
    [SerializeField] private GameObject sprite;
    //private List<GameObject> _vertexList;
    public GameObject Prev;
    public enum Status
    {
        Start,
        Finish,
        Neutral
    }
    public Status VertexStatus;
    void Start()
    {
        //_vertexList = _manager.GetVertexList();
        VertexStatus = Status.Neutral;
        SetStatus();
    }
    void Update()
    {
        
    }

    public void ChangeStatus(int mb)
    {
        ResetColor();
        List<GameObject> _vertexList = _manager.GetVertexList();
        switch (mb)
        {
            case 0:
                if (VertexStatus != Status.Start)
                {
                    if (VertexStatus != Status.Finish)
                    {
                        foreach (var vertex in _vertexList)
                        {
                            if (vertex.gameObject != this.gameObject && vertex.GetComponent<Vertex>().VertexStatus == Status.Start)
                            {
                                vertex.GetComponent<Vertex>().ChangeStatus(-1);
                                _manager.UnSetPoints(0);
                            }
                        }
                        VertexStatus = Status.Start;
                    }
                }

                else
                    VertexStatus = Status.Neutral;
                _manager.SetStartAndFinishPoints(mb, this.gameObject);
                break;
            case 1:
                if (VertexStatus != Status.Finish)
                {
                    if (VertexStatus != Status.Start)
                    {
                        foreach (var vertex in _vertexList)
                        {
                            if (vertex.gameObject != this.gameObject && vertex.GetComponent<Vertex>().VertexStatus == Status.Finish)
                            {
                                vertex.GetComponent<Vertex>().ChangeStatus(-1);
                                _manager.UnSetPoints(1);
                            }
                        }
                        VertexStatus = Status.Finish;
                    }
                }
                else
                    VertexStatus = Status.Neutral;
                _manager.SetStartAndFinishPoints(mb, this.gameObject);
                break;
            case -1:
                VertexStatus = Status.Neutral;
                break;
        }
        
        SetStatus();
    }

    void SetStatus()
    {
        switch (VertexStatus)
        {
            case Status.Start:
                sprite.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case Status.Finish:
                sprite.GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case Status.Neutral:
                sprite.GetComponent<SpriteRenderer>().color = Color.gray;
                break;
        }
    }
    public List<GameObject> GetNeighbourList()
    {
        return _neighbourList;
    }
    public List<GameObject> GetEdgeList()
    {
        return _edgeList;
    }
    public void AddNeighbour(GameObject neighbour)
    {
        if(neighbour != this.gameObject)
            _neighbourList.Add(neighbour);
    }
    public void AddEdge(GameObject edge)
    {
        
            _edgeList.Add(edge);
    }

    public void ChangeColor()
    {
        if (VertexStatus == Status.Neutral)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
    void ResetColor()
    {
        foreach (var v in _manager.GetVertexList())
        {
            if (v.GetComponent<Vertex>().VertexStatus == Status.Neutral)
            {
                v.GetComponent<Vertex>().sprite.GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }

        
    }

}
