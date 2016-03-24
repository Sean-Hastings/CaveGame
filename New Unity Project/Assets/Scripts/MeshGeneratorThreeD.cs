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

		GetComponent<MeshCollider>().sharedMesh = mesh;
    }

	void OnDrawGizmos()
	{
		for (int x = 0; x < squareGrid.cubes.GetLength(0); x++)
		{
			for (int y = 0; y < squareGrid.cubes.GetLength(1); y++)
			{
				for (int z = 0; z < squareGrid.cubes.GetLength(2); z++)
				{
					Gizmos.color = new Color (.7f, .7f, .7f);
					foreach (var node in squareGrid.cubes[x, y, z].getNodes())
					{
						Gizmos.DrawSphere (node.position, .75f);
					}

					foreach (var node in squareGrid.cubes[x, y, z].getActiveControlNodes())
					{
						if (node.active)
							Gizmos.color = new Color (1f, .1f, .1f);
						else
							Gizmos.color = new Color (.1f, .1f, 1f);
						Gizmos.DrawSphere (node.position, 1.5f);
					}
				}
			}
		}
	}


    void TriangulateCube(Cube square)
    {
        switch (square.configuration)
        {
        case 0:
			break;
		case 255:
			break;

        // 1 points:
        case 1:
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		case 2:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		case 4:
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		case 8:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
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
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		case 128:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerLeftFront, square.centerTopFront);
			break;
		
		// 2 points
		case 3:
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 5:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 10:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 12:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 20:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 40:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 48:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 65:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 80:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 130:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 160:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 192:
			square.rotateRightVert();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerLeftBack, square.centerLeftFront);
			break;

		case 9:
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 6:
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 17:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 24:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 34:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 36:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 66:
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 68:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 96:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 129:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 136:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
		case 144:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront);
			MeshFromPoints(square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter);
			break;
			
		case 18:
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.topLeftCenter, square.centerTopBack, square.bottomRightCenter);
			MeshFromPoints(square.centerLeftBack, square.topLeftCenter, square.centerBottomFront, square.bottomRightCenter, square.centerTopBack);
			break;
		case 33:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.topLeftCenter, square.centerTopBack, square.bottomRightCenter);
			MeshFromPoints(square.centerLeftBack, square.topLeftCenter, square.centerBottomFront, square.bottomRightCenter, square.centerTopBack);
			break;
		case 72:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.topLeftCenter, square.centerTopBack, square.bottomRightCenter);
			MeshFromPoints(square.centerLeftBack, square.topLeftCenter, square.centerBottomFront, square.bottomRightCenter, square.centerTopBack);
			break;
		case 132:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.topLeftCenter, square.centerTopBack, square.bottomRightCenter);
			MeshFromPoints(square.centerLeftBack, square.topLeftCenter, square.centerBottomFront, square.bottomRightCenter, square.centerTopBack);
			break;

		// 3 points
		case 7:
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 11:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 13:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 14:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 21:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 28:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 42:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 44:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 52:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 56:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 67:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 69:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 81:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 84:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 112:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 131:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 138:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 162:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 168:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 176:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 193:
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 194:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 208:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 224:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;

		case 22:
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 19:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 26:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 35:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 37:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 41:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 49:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 50:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 73:
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 74:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 82:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 88:
			square.rotateRightVert();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 97:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 104:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 133:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 134:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 140:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 148:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 146:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 161:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 164:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 196:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 200:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;
		case 76:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints (square.topLeftCenter, square.centerTopBack, square.centerLeftBack);
			MeshFromPoints (square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.topRightCenter);
			break;

		case 25:
			MeshFromPoints(square.bottomRightCenter, square.centerRightFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerBottomFront, square.centerLeftFront);
			break;
		case 137:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.bottomRightCenter, square.centerRightFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerBottomFront, square.centerLeftFront);
			break;
		case 70:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.bottomRightCenter, square.centerRightFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerBottomFront, square.centerLeftFront);
			break;
		case 38:
			square.rotateRightHoriz();
			MeshFromPoints(square.bottomRightCenter, square.centerRightFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerBottomFront, square.centerLeftFront);
			break;
		case 145:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.bottomRightCenter, square.centerRightFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerBottomFront, square.centerLeftFront);
			break;
		case 152:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.bottomRightCenter, square.centerRightFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerBottomFront, square.centerLeftFront);
			break;
		case 98:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.bottomRightCenter, square.centerRightFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerBottomFront, square.centerLeftFront);
			break;
		case 100:
			square.rotateRightVert();
			MeshFromPoints(square.bottomRightCenter, square.centerRightFront, square.topRightCenter, square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerBottomFront, square.centerLeftFront);
			break;

		// 4 points
		case 15:
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.centerRightBack, square.centerLeftBack);
			break;
		case 60:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.centerRightBack, square.centerLeftBack);
			break;
		case 85:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.centerRightBack, square.centerLeftBack);
			break;
		case 195:
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.centerRightBack, square.centerLeftBack);
			break;
		case 240:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.centerRightBack, square.centerLeftBack);
			break;
		case 170:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerRightFront, square.centerRightBack, square.centerLeftBack);
			break;
			
		case 51:
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerRightBack, square.centerRightFront);
			MeshFromPoints(square.centerBottomFront, square.centerBottomBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 90:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerRightBack, square.centerRightFront);
			MeshFromPoints(square.centerBottomFront, square.centerBottomBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 165:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerRightBack, square.centerRightFront);
			MeshFromPoints(square.centerBottomFront, square.centerBottomBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 204:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerRightBack, square.centerRightFront);
			MeshFromPoints(square.centerBottomFront, square.centerBottomBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 105:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerRightBack, square.centerRightFront);
			MeshFromPoints(square.centerBottomFront, square.centerBottomBack, square.centerLeftBack, square.centerLeftFront);
			break;
		case 150:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerTopBack, square.centerRightBack, square.centerRightFront);
			MeshFromPoints(square.centerBottomFront, square.centerBottomBack, square.centerLeftBack, square.centerLeftFront);
			break;
			
		case 29:
			MeshFromPoints(square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 46:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 139:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 71:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 209:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 116:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 184:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 226:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerRightBack, square.centerTopBack, square.topLeftCenter, square.centerLeftFront, square.centerBottomFront, square.bottomRightCenter);
			break;
			
		case 23:
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 45:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 142:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 75:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 210:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 58:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 232:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 92:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 163:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 113:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 180:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
		case 197:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerBottomFront, square.bottomRightCenter, square.topRightCenter, square.centerLeftBack, square.centerLeftFront);
			MeshFromPoints(square.topRightCenter, square.centerTopBack, square.centerLeftBack);
			break;
			
		case 30:
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 83:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 178:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 77:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 202:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 120:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 43:
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 225:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 53:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 135:
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 172:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
		case 212:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopFront, square.centerBottomFront, square.bottomRightCenter, square.centerRightBack, square.centerLeftBack);
			break;
			
		case 27:
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 39:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 141:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 78:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 177:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 228:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 216:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 114:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 198:
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 89:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 54:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 169:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 201:
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 86:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 57:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 166:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 99:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 149:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 108:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 154:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 147:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 101:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 156:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
		case 106:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerLeftFront, square.centerTopFront, square.topRightCenter, square.centerRightBack);
			MeshFromPoints(square.centerRightFront, square.centerBottomFront, square.bottomRightCenter);
			break;
			
		case 102:
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			MeshFromPoints(square.bottomLeftCenter, square.centerBottomBack, square.centerLeftBack);
			MeshFromPoints(square.centerBottomFront, square.centerRightFront, square.bottomRightCenter);
			break;
		case 153:
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			MeshFromPoints(square.bottomLeftCenter, square.centerBottomBack, square.centerLeftBack);
			MeshFromPoints(square.centerBottomFront, square.centerRightFront, square.bottomRightCenter);
			break;
		
		// 5 points
		case 230:
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.bottomRightCenter, square.centerBottomFront, square.centerLeftFront, square.topLeftCenter);
			MeshFromPoints(square.centerRightFront, square.topRightCenter, square.centerTopFront);
			break;
		case 118:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.bottomRightCenter, square.centerBottomFront, square.centerLeftFront, square.topLeftCenter);
			MeshFromPoints(square.centerRightFront, square.topRightCenter, square.centerTopFront);
			break;
		case 185:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.bottomRightCenter, square.centerBottomFront, square.centerLeftFront, square.topLeftCenter);
			MeshFromPoints(square.centerRightFront, square.topRightCenter, square.centerTopFront);
			break;
		case 217:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.bottomRightCenter, square.centerBottomFront, square.centerLeftFront, square.topLeftCenter);
			MeshFromPoints(square.centerRightFront, square.topRightCenter, square.centerTopFront);
			break;
		case 110:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.bottomRightCenter, square.centerBottomFront, square.centerLeftFront, square.topLeftCenter);
			MeshFromPoints(square.centerRightFront, square.topRightCenter, square.centerTopFront);
			break;
		case 103:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.bottomRightCenter, square.centerBottomFront, square.centerLeftFront, square.topLeftCenter);
			MeshFromPoints(square.centerRightFront, square.topRightCenter, square.centerTopFront);
			break;
		case 157:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.bottomRightCenter, square.centerBottomFront, square.centerLeftFront, square.topLeftCenter);
			MeshFromPoints(square.centerRightFront, square.topRightCenter, square.centerTopFront);
			break;
		case 155:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.bottomRightCenter, square.centerBottomFront, square.centerLeftFront, square.topLeftCenter);
			MeshFromPoints(square.centerRightFront, square.topRightCenter, square.centerTopFront);
			break;
			
		case 233:
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 214:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 121:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 182:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 205:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 236:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 173:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 109:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 229:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 158:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 218:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 94:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 220:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 206:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 122:
			square.rotateRightVert();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 59:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 107:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 91:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 115:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 181:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 55:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 179:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 167:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
		case 151:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopBack, square.topRightCenter, square.bottomRightCenter, square.centerBottomFront, square.topLeftCenter, square.centerLeftBack);
			MeshFromPoints(square.bottomRightCenter, square.centerTopBack, square.topRightCenter);
			MeshFromPoints(square.centerTopFront, square.topLeftCenter, square.centerBottomFront);
			break;
			
		case 248:
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 244:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 242:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 241:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 143:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 79:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 31:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 47:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 171:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 234:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 186:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 174:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 93:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 87:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 213:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 117:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 62:
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 188:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 124:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 61:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 199:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 211:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 227:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;
		case 203:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topRightCenter, square.centerRightFront, square.centerLeftFront);
			break;

		// 6 points
		case 237:
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topLeftCenter);
			MeshFromPoints(square.bottomRightCenter, square.centerBottomFront, square.centerRightFront);
			break;
		case 222:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topLeftCenter);
			MeshFromPoints(square.bottomRightCenter, square.centerBottomFront, square.centerRightFront);
			break;
		case 123:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topLeftCenter);
			MeshFromPoints(square.bottomRightCenter, square.centerBottomFront, square.centerRightFront);
			break;
		case 183:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftBack, square.centerTopBack, square.topLeftCenter);
			MeshFromPoints(square.bottomRightCenter, square.centerBottomFront, square.centerRightFront);
			break;
			
		case 246:
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 249:
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 238:
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 187:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 231:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 219:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 221:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 119:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 159:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 111:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 189:
			square.rotateRightVert();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
		case 126:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.topLeftCenter, square.centerTopBack, square.topRightCenter, square.centerTopFront, square.centerLeftFront);
			MeshFromPoints(square.centerTopBack, square.centerRightBack, square.topRightCenter);
			break;
			
		case 252:
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 250:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 245:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 243:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 235:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 215:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 207:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 190:
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 175:
			square.rotateRightVert();
			square.rotateRightVert();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 125:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 95:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;
		case 63:
			square.rotateRightVert();
			MeshFromPoints(square.centerLeftFront, square.centerLeftBack, square.centerTopBack, square.centerTopFront);
			break;

		// 7 points
		case 254:
			MeshFromPoints(square.centerTopFront, square.centerLeftFront, square.topLeftCenter);
			break;
		case 253:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerLeftFront, square.topLeftCenter);
			break;
		case 251:
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerLeftFront, square.topLeftCenter);
			break;
		case 247:
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerLeftFront, square.topLeftCenter);
			break;
		case 239:
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopFront, square.centerLeftFront, square.topLeftCenter);
			break;
		case 223:
			square.rotateRightHoriz();
			square.rotateRightVert();
			square.rotateRightVert();
			MeshFromPoints(square.centerTopFront, square.centerLeftFront, square.topLeftCenter);
			break;
		case 191:
			square.rotateRightVert();
			MeshFromPoints(square.centerTopFront, square.centerLeftFront, square.topLeftCenter);
			break;
		case 127:
			square.rotateRightVert();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			square.rotateRightHoriz();
			MeshFromPoints(square.centerTopFront, square.centerLeftFront, square.topLeftCenter);
			break;
        }
    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 6)
			CreateTriangle(points[0], points[4], points[5]);
		if (points.Length >= 5)
			CreateTriangle(points[0], points[3], points[4]);
		if (points.Length >= 4)
			CreateTriangle(points[0], points[2], points[3]);
		if (points.Length >= 3)
			CreateTriangle(points[0], points[1], points[2]);

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
							controlNodes[x  , y+1, z+1], 
							controlNodes[x+1, y+1, z+1], 
							controlNodes[x+1, y  , z+1], 
							controlNodes[x  , y  , z+1],
							controlNodes[x  , y+1, z  ],
							controlNodes[x+1, y+1, z  ],
							controlNodes[x+1, y  , z  ],
							controlNodes[x  , y  , z  ]);
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
            topLeftFront      = _topLeftFront;
            topRightFront     = _topRightFront;
            bottomRightFront  = _bottomRightFront;
            bottomLeftFront   = _bottomLeftFront;
            topLeftBack       = _topLeftBack;
            topRightBack      = _topRightBack;
            bottomRightBack   = _bottomRightBack;
            bottomLeftBack    = _bottomLeftBack;

			centerTopFront    = topRightFront.left;
            centerRightFront  = bottomRightFront.above;
			centerBottomFront = bottomRightFront.left;
            centerLeftFront   = bottomLeftFront.above;
			topLeftCenter     = topLeftBack.front;
			topRightCenter    = topRightBack.front;
			bottomRightCenter = bottomRightBack.front;
			bottomLeftCenter  = bottomLeftBack.front;
			centerTopBack     = topRightBack.left;
            centerRightBack   = bottomRightBack.above;
			centerBottomBack  = bottomRightBack.left;
            centerLeftBack    = bottomLeftBack.above;

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

		public Node[] getNodes()
		{
			return new Node[]
			{centerTopFront, centerRightFront, centerBottomFront, centerLeftFront, centerTopBack, centerRightBack, centerBottomBack, centerLeftBack, topLeftCenter, topRightCenter, bottomRightCenter, bottomLeftCenter};
		}

		public ControlNode[] getActiveControlNodes()
		{
			return new ControlNode[]
			{topLeftBack, bottomLeftBack, topLeftFront, bottomLeftFront, topRightBack, bottomRightBack, topRightFront, bottomRightFront};;
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
        public Node above, left, front;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
        {
            active = _active;
			front = new Node(position + Vector3.up * squareSize / 2f);
            left = new Node(position + Vector3.left * squareSize / 2f);
			above = new Node(position + Vector3.forward * squareSize / 2f);
        }
    }
}