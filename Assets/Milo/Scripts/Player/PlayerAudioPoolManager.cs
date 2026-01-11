using System.Collections.Generic;
using UnityEngine;

public class AudioPoolManager : MonoBehaviour
{
    public static AudioPoolManager Instance { get; private set; }

    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] private int initialPoolSize = 10;

    private readonly Queue<AudioSource> _pool = new Queue<AudioSource>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < initialPoolSize; i++) { AddSourceToPool(); }
    }

    private AudioSource AddSourceToPool()
    {
        GameObject go = Instantiate(audioSourcePrefab, transform);
        go.SetActive(false);
        AudioSource source = go.GetComponent<AudioSource>();
        _pool.Enqueue(source);
        return source;
    }

    public AudioSource GetSource()
    {
        AudioSource source = _pool.Count > 0 ? _pool.Dequeue() : AddSourceToPool();
        source.gameObject.SetActive(true);
        return source;
    }

    public void ReturnToPool(AudioSource source)
    {
        source.gameObject.SetActive(false);
        _pool.Enqueue(source);
    }
}