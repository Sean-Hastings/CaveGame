using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGeneratorThreeD : MonoBehaviour
{

    public SquareGrid squareGrid;
    public MeshFilter walls;

    float squareSize;

    List<Vector3> vertices;
    List<int> triangles;

    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();

    public void GenerateMesh(int[,,] map, float square_Size)
    {
        squareSize = square_Size;

        triangleDictionary.Clear();
        outlines.Clear();
        checkedVertices.Clear();

        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < squareGrid.cubes.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.cubes.GetLength(1); y++)
            {
                for (int z = 0; z < squareGrid.cubes.GetLength(2); z++)
                {
                    TriangulateCube(squareGrid.cubes[x, y, z]);
                }
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    void TriangulateCube(Cube square)
    {
        switch (square.configuration)
        {
        case 0:
            break;

        // 1 points:
        case 1:
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
            break;
        case 2:
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		case 3:
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 4:
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		case 5:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
			/*
		case 6:
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack, square.centerRightFront);
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerRightFront, square.centerLeftBack);
			break;
			*/
		case 8:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
			/*
		case 9:
			square.rotateRightHoriz();
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack, square.centerRightFront);
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerRightFront, square.centerLeftBack);
			break;
			*/
		case 10:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 12:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 15:
			MeshFromPoints(square.centerRightFront, square.centerRightBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 16:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		case 32:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		case 64:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		case 128:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;

			/*
            centerTopFront
            centerRightFront
            centerBottomFront
            centerLeftFront
            topLeftCenter
            topRightCenter
            bottomRightCenter
            bottomLeftCenter
            centerTopBack
            centerRightBack
            centerBottomBack
            centerLeftBack
			*/
        }
    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 6)
			CreateTriangle(points[5], points[4], points[0]);
		if (points.Length >= 5)
			CreateTriangle(points[4], points[3], points[0]);
		if (points.Length >= 4)
			CreateTriangle(points[3], points[2], points[0]);
		if (points.Length >= 3)
			CreateTriangle(points[2], points[1], points[0]);

    }

    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
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

    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
    {
        if (triangleDictionary.ContainsKey(vertexIndexKey))
        {
            triangleDictionary[vertexIndexKey].Add(triangle);
        }
        else
        {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triangleDictionary.Add(vertexIndexKey, triangleList);
        }
    }

    void CalculateMeshOutlines()
    {

        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
        {
            if (!checkedVertices.Contains(vertexIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if (newOutlineVertex != -1)
                {
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }

    void FollowOutline(int vertexIndex, int outlineIndex)
    {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex != -1)
        {
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }

    int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];

            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];
                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
                {
                    if (IsOutlineEdge(vertexIndex, vertexB))
                    {
                        return vertexB;
                    }
                }
            }
        }

        return -1;
    }

    bool IsOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
        int sharedTriangleCount = 0;

        for (int i = 0; i < trianglesContainingVertexA.Count; i++)
        {
            if (trianglesContainingVertexA[i].Contains(vertexB))
            {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1)
                {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }

    struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertices;

        public Triangle(int a, int b, int c)
        {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        public int this[int i]
        {
            get
            {
                return vertices[i];
            }
        }


        public bool Contains(int vertexIndex)
        {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }

    public class SquareGrid
    {
        public Cube[,,] cubes;

        public SquareGrid(int[,,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            int nodeCountZ = map.GetLength(2);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;
            float mapDepth = nodeCountZ * squareSize;

            ControlNode[,,] controlNodes = new ControlNode[nodeCountX, nodeCountY, nodeCountZ];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    for (int z = 0; z < nodeCountZ; z++)
                    {
                        Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, -mapDepth / 2 + z * squareSize + squareSize / 2, -mapHeight / 2 + y * squareSize + squareSize / 2);
                        controlNodes[x, y, z] = new ControlNode(pos, map[x, y, z] == 1, squareSize);
                    }
                }
            }

            cubes = new Cube[nodeCountX - 1, nodeCountY - 1, nodeCountZ - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    for (int z = 0; z < nodeCountZ - 1; z++)
                    {
                        cubes[x, y, z] = new Cube(
                            controlNodes[x, y + 1, z], 
                            controlNodes[x + 1, y + 1, z], 
                            controlNodes[x + 1, y, z], 
                            controlNodes[x, y, z],
                            controlNodes[x, y + 1, z + 1],
                            controlNodes[x + 1, y + 1, z + 1],
                            controlNodes[x + 1, y, z + 1],
                            controlNodes[x, y, z + 1]);
                    }
                }
            }
        }
    }

    public class Cube
    {

        public ControlNode 
            topLeftFront, 
            topRightFront, 
            bottomRightFront, 
            bottomLeftFront, 
            topLeftBack, 
            topRightBack, 
            bottomRightBack, 
            bottomLeftBack;
        public Node 
            centerTopFront, 
            centerRightFront, 
            centerBottomFront, 
            centerLeftFront, 
            topLeftCenter, 
            topRightCenter, 
            bottomRightCenter, 
            bottomLeftCenter, 
            centerTopBack, 
            centerRightBack, 
            centerBottomBack, 
            centerLeftBack;
        public int configuration;

        public Cube(
            ControlNode _topLeftFront, 
            ControlNode _topRightFront, 
            ControlNode _bottomRightFront, 
            ControlNode _bottomLeftFront, 
            ControlNode _topLeftBack, 
            ControlNode _topRightBack, 
            ControlNode _bottomRightBack, 
            ControlNode _bottomLeftBack)
        {
            topLeftFront = _topLeftFront;
            topRightFront = _topRightFront;
            bottomRightFront = _bottomRightFront;
            bottomLeftFront = _bottomLeftFront;
            topLeftBack = _topLeftBack;
            topRightBack = _topRightBack;
            bottomRightBack = _bottomRightBack;
            bottomLeftBack = _bottomLeftBack;

            centerTopFront = topLeftFront.right;
            centerRightFront = bottomRightFront.above;
            centerBottomFront = bottomRightFront.right;
            centerLeftFront = bottomLeftFront.above;
			topLeftCenter = topLeftBack.behind;
			topRightCenter = topRightBack.behind;
			bottomRightCenter = bottomRightBack.behind;
			bottomLeftCenter = bottomLeftBack.behind;
            centerTopBack = topLeftBack.right;
            centerRightBack = bottomRightBack.above;
            centerBottomBack = bottomLeftBack.right;
            centerLeftBack = bottomLeftBack.above;

            if (topLeftFront.active)
				configuration += 1;
			if (topLeftBack.active)
				configuration += 2;
            if (topRightFront.active)
                configuration += 4;
            if (topRightBack.active)
				configuration += 8;
			if (bottomRightFront.active)
				configuration += 16;
            if (bottomRightBack.active)
				configuration += 32;
			if (bottomLeftFront.active)
				configuration += 64;
            if (bottomLeftBack.active)
                configuration += 128;
        }

		public void rotateRightVert()
		{
			ControlNode tempBLF = bottomLeftFront,
			tempBLB = bottomLeftBack;

			Node tempBLC = bottomLeftCenter,
			tempCLF = centerLeftFront,
			tempCLB = centerLeftBack;
			
			bottomLeftFront = bottomRightFront;
			bottomLeftCenter = bottomRightCenter;
			bottomLeftBack = bottomRightBack;
			centerLeftFront = centerBottomFront;
			centerLeftBack = centerBottomBack;
			
			bottomRightFront = topRightFront;
			bottomRightCenter = topRightCenter;
			bottomRightBack = topRightBack;
			centerBottomFront = centerRightFront;
			centerBottomBack = centerRightBack;
			
			topRightFront = topLeftFront;
			topRightCenter = topLeftCenter;
			topRightBack = topLeftBack;
			centerRightFront = centerTopFront;
			centerRightBack = centerTopBack;
			
			topLeftFront = tempBLF;
			topLeftCenter = tempBLC;
			topLeftBack = tempBLB;
			centerTopFront = tempCLF;
			centerTopBack = tempCLB;
		}
		
		public void rotateRightHoriz()
		{
			ControlNode tempTLF = topLeftFront,
			tempBLF = bottomLeftFront;

			Node tempCLF = centerLeftFront,
			tempTLC = topLeftCenter,
			tempBLC = bottomLeftCenter;
			
			topLeftFront = topRightFront;
			centerLeftFront = centerRightFront;
			bottomLeftFront = bottomRightFront;
			topLeftCenter = centerTopFront;
			bottomLeftCenter = centerBottomFront;
			
			topRightFront = topRightBack;
			centerRightFront = centerRightBack;
			bottomRightFront = bottomRightBack;
			centerTopFront = topRightCenter;
			centerBottomFront = bottomRightCenter;
			
			topRightBack = topLeftBack;
			centerRightBack = centerLeftBack;
			bottomRightBack = bottomLeftBack;
			topRightCenter = centerTopBack;
			bottomRightCenter = centerBottomBack;
			
			topLeftBack = tempTLF;
			centerLeftBack = tempCLF;
			bottomLeftBack = tempBLF;
			centerTopBack = tempTLC;
			centerBottomBack = tempBLC;
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

        public bool active;
        public Node above, right, behind;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
        {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
            behind = new Node(position + Vector3.up * squareSize / 2f);
        }
    }
}