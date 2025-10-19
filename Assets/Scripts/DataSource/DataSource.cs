using UnityEngine;

namespace DataSource
{
    public abstract class DataSource<T> : ScriptableObject
    {
        public T DataInstance { get; set; }
    }
}