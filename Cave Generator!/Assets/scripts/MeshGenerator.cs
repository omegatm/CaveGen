using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//The marching squares algorithm is very complicated and I took most of the implmentation from https://www.youtube.com/watch?v=2gIxh8CX3Hk
public class MeshGenerator : MonoBehaviour
{
    public SquareGrid Grid;
    List<Vector3> vertices;
    public MeshFilter walls;
    List<int> triangles;
    Dictionary<int, List<Triangle>> trianglesMap = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();
    public void GenerateMesh(int[,] map, float size)
    {
        trianglesMap.Clear();
        outlines.Clear();
        checkedVertices.Clear();
        Grid = new SquareGrid(map, size);
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for (int x = 0; x < Grid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < Grid.squares.GetLength(1); y++)
            {
                TriangulateSquare(Grid.squares[x, y]);
            }
        }
        Mesh caveMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = caveMesh;
        caveMesh.vertices = vertices.ToArray();
        caveMesh.triangles = triangles.ToArray();
        caveMesh.RecalculateNormals();
        int tileAmt = 10;
        Vector2[] uvs = new Vector2[vertices.Count];
        for(int i = 0; i < vertices.Count; i++)
        {
            float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * size, map.GetLength(0) / 2 * size, vertices[i].x)*tileAmt;
            float percentY = Mathf.InverseLerp(-map.GetLength(0) / 2 * size, map.GetLength(0) / 2 * size, vertices[i].z)*tileAmt;
            uvs[i] = new Vector2(percentX, percentY);
        }
        caveMesh.uv = uvs;

        CreateWalls();
    }
    void CreateWalls()
    {
        CalculateOutlines();
        List<Vector3> WallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 10f;
        foreach (List<int> Outline in outlines)
        {
            for (int i = 0; i < Outline.Count - 1; i++)
            {
                int start = WallVertices.Count;
                WallVertices.Add(vertices[Outline[i]]);
                WallVertices.Add(vertices[Outline[i + 1]]);
                WallVertices.Add(vertices[Outline[i]] - Vector3.up * wallHeight);
                WallVertices.Add(vertices[Outline[i + 1]] - Vector3.up * wallHeight);

                wallTriangles.Add(start);
                wallTriangles.Add(start + 2);
                wallTriangles.Add(start + 3);

                wallTriangles.Add(start + 3);
                wallTriangles.Add(start + 1);
                wallTriangles.Add(start);
            }
        }
        wallMesh.vertices = WallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        walls.mesh = wallMesh;
        MeshCollider wallCollider = walls.gameObject.GetComponent<MeshCollider>();
        if (wallCollider == null)
        {
            wallCollider = walls.gameObject.AddComponent<MeshCollider>();
        }
        wallCollider.sharedMesh = null;
        wallCollider.sharedMesh = wallMesh;

    }


    public class SquareGrid
    {
        public Square[,] squares;
        public SquareGrid(int[,] map, float size)
        {
            int nodecountx = map.GetLength(0);
            int nodecounty = map.GetLength(1);
            float width = nodecountx * size;
            float height = nodecounty * size;
            ControlNode[,] nodes = new ControlNode[nodecountx, nodecounty];
            for (int x = 0; x < nodecountx; x++)
            {
                for (int y = 0; y < nodecounty; y++)
                {
                    Vector3 pos = new Vector3(-width / 2 + x * size + size / 2, 0, -height + y * size + size / 2);
                    nodes[x, y] = new ControlNode(pos, map[x, y] == 1, size);
                }
            }
            squares = new Square[nodecountx - 1, nodecounty - 1];
            for (int x = 0; x < nodecountx - 1; x++)
            {
                for (int y = 0; y < nodecounty - 1; y++)
                {
                    squares[x, y] = new Square(nodes[x, y + 1], nodes[x + 1, y + 1], nodes[x + 1, y], nodes[x, y]);
                }
            }
        }

    }
    void TriangulateSquare(Square square)
    {
        switch (square.config)
        {
            case 0:
                break;

            case 1:
                MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomleft);
                break;
            case 2:
                MeshFromPoints(square.bottomright, square.centerBottom, square.centerRight);
                break;
            case 4:
                MeshFromPoints(square.topright, square.centerRight, square.centerTop);
                break;
            case 8:
                MeshFromPoints(square.topleft, square.centerTop, square.centerLeft);
                break;

            case 3:
                MeshFromPoints(square.centerRight, square.bottomright, square.bottomleft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topright, square.bottomright, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topleft, square.centerTop, square.centerBottom, square.bottomleft);
                break;
            case 12:
                MeshFromPoints(square.topleft, square.topright, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topright, square.centerRight, square.centerBottom, square.bottomleft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topleft, square.centerTop, square.centerRight, square.bottomright, square.centerBottom, square.centerLeft);
                break;

            case 7:
                MeshFromPoints(square.centerTop, square.topright, square.bottomright, square.bottomleft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topleft, square.centerTop, square.centerRight, square.bottomright, square.bottomleft);
                break;
            case 13:
                MeshFromPoints(square.topleft, square.topright, square.centerRight, square.centerBottom, square.bottomleft);
                break;
            case 14:
                MeshFromPoints(square.topleft, square.topright, square.bottomright, square.centerBottom, square.centerLeft);
                break;

            case 15:
                MeshFromPoints(square.topleft, square.topright, square.bottomright, square.bottomleft);
                checkedVertices.Add(square.topleft.vertexIndex);
                checkedVertices.Add(square.topright.vertexIndex);
                checkedVertices.Add(square.bottomright.vertexIndex);
                checkedVertices.Add(square.bottomleft.vertexIndex);
                break;
        }
    }
    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);
        if (points.Length >= 3)
        {
            CreateTriangle(points[0], points[1], points[2]);
        }
        if (points.Length >= 4)
        {
            CreateTriangle(points[0], points[2], points[3]);
        }
        if (points.Length >= 5)
        {
            CreateTriangle(points[0], points[3], points[4]);
        }
        if (points.Length >= 6)
        {
            CreateTriangle(points[0], points[4], points[5]);
        }
    }
    void AssignVertices(Node[] nodes)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].vertexIndex == -1)
            {
                nodes[i].vertexIndex = vertices.Count;
                vertices.Add(nodes[i].position);
            }
        }
    }
    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);
        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }
    void AddTriangleToDictionary(int key, Triangle triangle)
    {
        if (trianglesMap.ContainsKey(key))
        {
            trianglesMap[key].Add(triangle);
        }
        else
        {
            List<Triangle> list = new List<Triangle>();
            list.Add(triangle);
            trianglesMap.Add(key, list);
        }
    }
    void CalculateOutlines()
    {
        for (int vertex = 0; vertex < vertices.Count; vertex++)
        {
            if (!checkedVertices.Contains(vertex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertex);
                if (newOutlineVertex != -1)
                {
                    checkedVertices.Add(vertex);
                    List<int> newOutline = new List<int>();
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertex);
                }
            }
        }
    }
    void FollowOutline(int vertex, int outlineIndex)
    {
        outlines[outlineIndex].Add(vertex);
        checkedVertices.Add(vertex);
        int nextVertex = GetConnectedOutlineVertex(vertex);
        if (nextVertex != -1)
        {
            FollowOutline(nextVertex, outlineIndex);
        }
    }
    int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> triangleVertex = trianglesMap[vertexIndex];
        for (int i = 0; i < triangleVertex.Count; i++)
        {
            Triangle triangle = triangleVertex[i];
            for (int j = 0; j < 3; j++)
            {
                int vertexx = triangle[j];
                if (vertexx != vertexIndex && !checkedVertices.Contains(vertexx))
                {
                    if (IsEdge(vertexIndex, vertexx))
                    {
                        return vertexx;
                    }
                }
            }
        }
        return -1;
    }
    bool IsEdge(int vertexA, int vertexB)
    {
        List<Triangle> vertexa = trianglesMap[vertexA];
        int sharedtriangles = 0;
        for (int i = 0; i < vertexa.Count; i++)
        {
            if (vertexa[i].contains(vertexB))
            {
                sharedtriangles++;
                if (sharedtriangles > 1)
                    break;
            }
        }
        return sharedtriangles == 1;
    }
    struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertexArr;

        public Triangle(int a, int b, int c)
        {
            vertexIndexA = a; vertexIndexB = b; vertexIndexC = c;
            vertexArr = new int[3];
            vertexArr[0] = vertexIndexA;
            vertexArr[1] = vertexIndexB;
            vertexArr[2] = vertexIndexC;
        }
        public int this[int i]
        {
            get { return vertexArr[i]; }
        }
        public bool contains(int vertex)
        {
            return vertex == vertexIndexA || vertex == vertexIndexB || vertex == vertexIndexC;
        }
    }

    public class Square
    {
        public ControlNode topleft, topright, bottomright, bottomleft;
        public Node centerTop, centerRight, centerBottom, centerLeft;
        public int config;
        public Square(ControlNode topleft, ControlNode topright, ControlNode bottomright, ControlNode bottomleft)
        {
            this.topleft = topleft;
            this.topright = topright;
            this.bottomright = bottomright;
            this.bottomleft = bottomleft;

            centerTop = this.topleft.right;
            centerRight = this.bottomright.above;
            centerBottom = this.bottomleft.right;
            centerLeft = this.bottomleft.above;
            if (this.topleft.alive)
            {
                config += 8;
            }
            if (this.bottomright.alive)
                config += 2;
            if (this.bottomleft.alive)
            {
                config += 1;
            }
            if (this.topright.alive)
            {
                config += 4;
            }
        }
    }
    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos)
        {
            position = _pos;
        }

    }
    public class ControlNode : Node
    {
        public bool alive;
        public Node above, right;
        public ControlNode(Vector3 _pos, bool alive, float squareSize) : base(_pos)
        {
            this.alive = alive;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}
