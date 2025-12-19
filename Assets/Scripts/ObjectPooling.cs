using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _initialPoolSize = 10;

    private Queue<GameObject> _pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(_prefab);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        _pool.Enqueue(obj);
        return obj;
    }

    public GameObject GetObject()
    {
        if (_pool.Count == 0)
        {
            CreateNewObject();
        }

        GameObject obj = _pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        _pool.Enqueue(obj);
    }
}