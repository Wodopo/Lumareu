using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EdgeCollider2DSpriteSnaper))]
public class EdgeCollider2DSpriteSnaperEditor : Editor
{
    EdgeCollider2DSpriteSnaper snaper;
    bool canSnap = true;

    private void OnEnable()
    {
        snaper = target as EdgeCollider2DSpriteSnaper;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        canSnap = true;

        if (snaper.snapPoints != null)
            EditorGUILayout.HelpBox("Snap Point Count: " + snaper.snapPoints.Count, MessageType.None);
        else
            EditorGUILayout.HelpBox("Snap Point Count: 0", MessageType.None);

        if (snaper.spRenderer == null) {
            canSnap = false;
            EditorGUILayout.HelpBox("SpriteRenderer Missing!", MessageType.Warning);
        }
        else if (snaper.spRenderer.sprite == null)
        {
            canSnap = false;
            EditorGUILayout.HelpBox("SpriteRenderer's Sprite Missing!", MessageType.Warning);
        }

        if (snaper.edgeCollider2D == null) {
            canSnap = false;
            EditorGUILayout.HelpBox("EdgeCollider2D Missing!", MessageType.Warning);
        }

        if (canSnap && (snaper.snapPoints == null || snaper.snapPoints.Count == 0))
        {
            EditorGUILayout.HelpBox("No snap points", MessageType.Warning);
        }

        if (GUILayout.Button("Regenerate Snap Points"))
            snaper.GenerateSnapPoints();
        
        if (canSnap && !snaper.activeSnap)
            if (GUILayout.Button("Snap Points to Sprite Corners"))
                snaper.SnapToClosest();
    }
}
#endif

[ExecuteInEditMode]
public class EdgeCollider2DSpriteSnaper : MonoBehaviour
{
    public class Node
    {
        public int x, y;
        public List<Vector2> points;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            points = new List<Vector2>();
        }

        public void CalculateContributionPoints(Texture2D texture)
        {
            points.Clear();

            bool bottomLeft = IsTransparent(texture, x - 1, y - 1);
            bool left = IsTransparent(texture, x - 1, y);
            bool topLeft = IsTransparent(texture, x - 1, y + 1);

            bool bottomRight = IsTransparent(texture, x + 1, y - 1);
            bool right = IsTransparent(texture, x + 1, y);
            bool topRight = IsTransparent(texture, x + 1, y + 1);

            bool top = IsTransparent(texture, x, y + 1);
            bool bottom = IsTransparent(texture, x, y - 1);

            if (bottomLeft)
                if ((left && bottom) || (!left && !bottom))
                    points.Add(new Vector2(x - texture.width/2f, y - texture.height / 2f));

            if (bottomRight)
                if ((right && bottom) || (!right && !bottom))
                    points.Add(new Vector2(x - texture.width / 2f + 1f, y - texture.height / 2f));

            if (topLeft)
                if ((left && top) || (!left && !top))
                    points.Add(new Vector2(x - texture.width / 2f, y - texture.height / 2f +1f));

            if (topRight)
                if ((right && top) || (!right && !top))
                    points.Add(new Vector2(x - texture.width / 2f + 1f, y - texture.height / 2f +1f));
        }

        private bool IsTransparent(Texture2D texture, int x, int y)
        {
            if (x < 0 || x > texture.width ||
                y < 0 || y > texture.height)
                return false;

            Color pixel = texture.GetPixel(x, y);

            return pixel.a < 1.0f;
        }
    }

    public Color color;
    public float snapRadius;
    public bool activeSnap = false;

    [HideInInspector] public SpriteRenderer spRenderer;
    [HideInInspector] public EdgeCollider2D edgeCollider2D;
    [HideInInspector] public Texture2D texture;
    [HideInInspector] public List<Vector2> snapPoints;

    private void OnValidate()
    {
        if (spRenderer == null)
            spRenderer = GetComponent<SpriteRenderer>();

        if (edgeCollider2D == null)
            edgeCollider2D = GetComponent<EdgeCollider2D>();

        if (texture == null && spRenderer != null && spRenderer.sprite != null)
            texture = spRenderer.sprite.texture;
    }

    private void Update()
    {
        if (activeSnap && !Application.isPlaying)
        {
            if (edgeCollider2D == null)
                return;

            if (texture == null || snapPoints.Count == 0)
                GenerateSnapPoints();

            SnapToClosest();
        }
    }
    

    public void GenerateSnapPoints()
    {
        if (spRenderer.sprite == null)
            return;

        if (spRenderer.sprite.packed)
            return;
        
        texture = spRenderer.sprite.texture;
        List<Node> nodes = GetNodes(texture);
        snapPoints = GetCollisionPoints(nodes);

        SnapToClosest();
    }

    public void SnapToClosest()
    {
        if (edgeCollider2D == null)
        {
            Debug.Log("EdgeCollider2D is null");
            return;
        }

        List<Vector2> temp = new List<Vector2>();
        for (int i = 0; i < edgeCollider2D.pointCount; i++)
        {
            float distance = float.MaxValue;
            Vector2 positionCandidate = edgeCollider2D.points[i];
            for (int j = 0; j < snapPoints.Count; j++)
            {
                float tempDistance = Vector2.Distance(edgeCollider2D.points[i], snapPoints[j]);
                if (tempDistance < snapRadius && tempDistance < distance)
                {
                    distance = tempDistance;
                    positionCandidate = snapPoints[j];
                }
            }
            temp.Add(positionCandidate);
        }

        edgeCollider2D.points = temp.ToArray();
    }

    private List<Vector2> GetCollisionPoints(List<Node> nodes)
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < nodes.Count; i++)
            for (int j = 0; j < nodes[i].points.Count; j++)
                points.Add(nodes[i].points[j]);

        return points;
    }

    private List<Node> GetNodes(Texture2D texture)
    {
        List<Node> nodes = new List<Node>();
        for (int x = 0; x < texture.width; x++)
            for (int y = 0; y < texture.height; y++)
                if (IsOpaque(texture, x, y) && HasTransparentNeighbor(texture, x, y))
                {
                    Node node = new Node(x, y);
                    node.CalculateContributionPoints(texture);
                    nodes.Add(node);
                }
        return nodes;
    }

    private bool HasTransparentNeighbor(Texture2D texture, int x, int y)
    {
        bool bottomLeft = IsTransparent(texture, x - 1, y - 1);
        bool left = IsTransparent(texture, x - 1, y);
        bool topLeft = IsTransparent(texture, x - 1, y + 1);

        bool bottomRight = IsTransparent(texture, x + 1, y - 1);
        bool right = IsTransparent(texture, x + 1, y);
        bool topRight = IsTransparent(texture, x + 1, y + 1);

        bool top = IsTransparent(texture, x, y + 1);
        bool bottom = IsTransparent(texture, x, y - 1);

        if (bottomLeft || bottom || bottomRight || right || topRight || top || topLeft || left)
            return true;

        return false;
    }

    private bool IsOpaque(Texture2D texture, int x, int y)
    {
        if (x < 0 || x > texture.width ||
            y < 0 || y > texture.height)
            return false;

        Color pixel = texture.GetPixel(x, y);

        return pixel.a == 1.0f;
    }

    private bool IsTransparent(Texture2D texture, int x, int y)
    {
        if (x < 0 || x > texture.width ||
            y < 0 || y > texture.height)
            return false;

        Color pixel = texture.GetPixel(x, y);

        return pixel.a < 1.0f;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (texture == null)
            return;

        //Vector2 offset = new Vector2(transform.position.x - texture.width / 2 + 0.5f, transform.position.y - texture.height / 2 + 0.5f);
        Handles.color = color;
        foreach (var item in snapPoints)
            Handles.DrawWireDisc(item, Vector3.forward, snapRadius);
    }
    #endif
}
