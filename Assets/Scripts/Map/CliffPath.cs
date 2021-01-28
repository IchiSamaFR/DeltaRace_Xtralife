using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffPath : MonoBehaviour
{
    [System.Serializable]
    public class pathPossible
    {
        public List<Transform> p = new List<Transform>();
    }

    [SerializeField]
    public List<pathPossible> paths = new List<pathPossible>();

    public List<Transform> GetPath()
    {
        if(paths.Count > 0)
        {
            int i = Random.Range(0, paths.Count);
            return paths[i].p;
        }
        return null;
    }
}
