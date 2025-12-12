// --- File: NPCSpawner.cs (SpawnPoint.cs) ---
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPoint : MonoBehaviour {
    [Tooltip("Prefab de NPCs (visual) para la fila")]
    public GameObject npcUIPrefab;

    public Transform queueParent;

    public List<NPC> npcPool = new List<NPC>();
    public int queueSize = 8;

    private Queue<NPC> queue = new Queue<NPC>();
    private NPCFactory factory;

    void Start() {
        factory = FindObjectOfType<NPCFactory>();
        BuildInitialQueue();
        RenderQueueUI();
    }

    public void BuildInitialQueue() {
        queue.Clear();
        if (npcPool.Count < queueSize && factory != null) {
            for (int i = npcPool.Count; i < queueSize; i++) {
                npcPool.Add(factory.GenerateNPC(UnityEngine.Random.value < 0.5f));
            }
        }

        var indices = new List<int>();
        for (int i = 0; i < npcPool.Count; i++) indices.Add(i);
        indices.Sort((a, b) => UnityEngine.Random.Range(-1, 2));

        for (int i = 0; i < queueSize && i < indices.Count; i++) {
            queue.Enqueue(npcPool[indices[i]]);
        }
    }

    public NPC PeekNext() {
        return queue.Count > 0 ? queue.Peek() : null;
    }

    public NPC DequeueNext() {
        var npc = queue.Count > 0 ? queue.Dequeue() : null;

        if (queue.Count < queueSize && factory != null) {
            queue.Enqueue(factory.GenerateNPC(UnityEngine.Random.value < 0.5f));
        }

        RenderQueueUI();
        return npc;
    }

    void RenderQueueUI() {
        for (int i = queueParent.childCount - 1; i >= 0; i--) {
            Destroy(queueParent.GetChild(i).gameObject);
        }

        foreach (var n in queue) {
            var go = Instantiate(npcUIPrefab, queueParent);
            var img = go.GetComponentInChildren<Image>(); // ¡AHORA FUNCIONA!
            if (img != null) img.sprite = n.appearanceSprite;
        }
    }
}