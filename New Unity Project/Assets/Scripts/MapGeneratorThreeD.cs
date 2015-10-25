using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGeneratorThreeD : MonoBehaviour
{
    [Range(0, 600)]
    public int width;
    [Range(0, 600)]
    public int height;
    [Range(0, 600)]
    public int depth;
    public int borderSize;
    public int roomThresholdSize;
    public int squareSize;
    public int smoothing;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,,] map;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        map = new int[width, height, depth];
        RandomFillMap();

        for (int i = 0; i < smoothing; i++)
        {
            SmoothMap();
        }

        //ProcessMap();

        int[,,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2, depth + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                for (int z = 0; z < borderedMap.GetLength(2); z++)
                {
                    if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize && z >= borderSize && z < depth + borderSize)
                    {
                        borderedMap[x, y, z] = map[x - borderSize, y - borderSize, z - borderSize];
                    }
                    else
                    {
                        borderedMap[x, y, z] = 1;
                    }
                }
            }
        }

        MeshGeneratorThreeD meshGen = GetComponent<MeshGeneratorThreeD>();
        meshGen.GenerateMesh(borderedMap, squareSize);
    }

    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);
        int wallThresholdSize = 50;

        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY, tile.tileZ] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY, tile.tileZ] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }
        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;

        ConnectClosestRooms(survivingRooms);
    }

    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {

        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, 5);
        }
    }

    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                for (int z = -r; z <= r; z++)
                {
                    if (x * x + y * y <= r * r)
                    {
                        int drawX = c.tileX + x;
                        int drawY = c.tileY + y;
                        int drawZ = c.tileZ + z;
                        if (IsInMapRange(drawX, drawY, drawZ))
                        {
                            map[drawX, drawY, drawZ] = 0;
                        }
                    }
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;
        int z = from.tileZ;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;
        int dz = to.tileZ - from.tileZ;

        int inverted = 0;
        int stepL = Math.Sign(dx);
        int stepM = Math.Sign(dy);
        int stepS = Math.Sign(dy);

        int longest  = Mathf.Abs(dx);
        int middle   = Mathf.Abs(dy);
        int shortest = Mathf.Abs(dz);

       if (longest > shortest && shortest > middle)
        {//long, short, mid
            inverted = 1;
            longest  = Mathf.Abs(dx);
            middle   = Mathf.Abs(dz);
            shortest = Mathf.Abs(dy);

            stepL = Math.Sign(dx);
            stepM = Math.Sign(dz);
            stepS = Math.Sign(dy);
        }
        else if (middle > longest && longest > shortest)
        {//mid, long, short
            inverted = 2;
            longest = Mathf.Abs(dy);
            middle = Mathf.Abs(dx);
            shortest = Mathf.Abs(dz);

            stepL = Math.Sign(dy);
            stepM = Math.Sign(dx);
            stepS = Math.Sign(dz);
        }
        else if (middle > shortest && shortest > longest)
        {//mid, short, long
            inverted = 3;
            longest = Mathf.Abs(dy);
            middle = Mathf.Abs(dz);
            shortest = Mathf.Abs(dx);

            stepL = Math.Sign(dy);
            stepM = Math.Sign(dz);
            stepS = Math.Sign(dx);
        }
        else if (shortest > longest && longest > middle)
        {//short, long, mid
            inverted = 4;
            longest = Mathf.Abs(dz);
            middle = Mathf.Abs(dx);
            shortest = Mathf.Abs(dy);

            stepL = Math.Sign(dz);
            stepM = Math.Sign(dx);
            stepS = Math.Sign(dy);
        }
        else if (shortest > middle && middle > longest)
        {//short, mid, long
            inverted = 5;
            longest = Mathf.Abs(dz);
            middle = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            stepL = Math.Sign(dz);
            stepM = Math.Sign(dy);
            stepS = Math.Sign(dx);
        }

        int gradientAccumulationS = longest / 2;
        int gradientAccumulationM = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y, z));

            switch (Convert.ToInt32(inverted/2))
            {
                case 0:
                    x+= stepL;
                    break;
                case 1:
                    y += stepL;
                    break;
                case 2:
                    z += stepL;
                    break;
            }

            gradientAccumulationS += shortest;
            if (gradientAccumulationS >= longest)
            {
                switch(inverted)
                {
                    case 0:
                        z += stepS;
                        break;
                    case 1:
                        y += stepS;
                        break;
                    case 2:
                        z += stepS;
                        break;
                    case 3:
                        x += stepS;
                        break;
                    case 4:
                        y += stepS;
                        break;
                    case 5:
                        x += stepS;
                        break;
                }
                gradientAccumulationS -= longest;
            }
            gradientAccumulationM += middle;
            if (gradientAccumulationM >= longest)
            {
                switch (inverted)
                {
                    case 0:
                        y += stepM;
                        break;
                    case 1:
                        z += stepM;
                        break;
                    case 2:
                        x += stepM;
                        break;
                    case 3:
                        z += stepM;
                        break;
                    case 4:
                        x += stepM;
                        break;
                    case 5:
                        y += stepM;
                        break;
                }
                gradientAccumulationM -= longest;
            }
        }

        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,,] mapFlags = new int[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (mapFlags[x, y, z] == 0 && map[x, y, z] == tileType)
                    {
                        List<Coord> newRegion = GetRegionTiles(x, y, z);
                        regions.Add(newRegion);

                        foreach (Coord tile in newRegion)
                        {
                            mapFlags[tile.tileX, tile.tileY, tile.tileZ] = 1;
                        }
                    }
                }
            }
        }

        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY, int startZ)
    {
        List<Coord> tiles = new List<Coord>();
        int[,,] mapFlags = new int[width, height, depth];
        int tileType = map[startX, startY, startZ];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY, startZ));
        mapFlags[startX, startY, startZ] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    for (int z = tile.tileZ - 1; z <= tile.tileZ + 1; z++)
                    {
                        if (IsInMapRange(x, y, z) && (y == tile.tileY || x == tile.tileX || z == tile.tileZ))
                        {
                            if (mapFlags[x, y, z] == 0 && map[x, y, z] == tileType)
                            {
                                mapFlags[x, y, z] = 1;
                                queue.Enqueue(new Coord(x, y, z));
                            }
                        }
                    }
                }
            }
        }

        return tiles;
    }

    bool IsInMapRange(int x, int y, int z)
    {
        return x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth;
    }


    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = new System.Random().NextDouble().ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1 || z == 0 || z == depth - 1)
                    {
                        map[x, y, z] = 1;
                    }
                    else
                    {
                        map[x, y, z] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                    }
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y, z);
                    if (neighbourWallTiles > 18)
                        map[x, y, z] = 1;
                    else if (neighbourWallTiles < 10)
                        map[x, y, z] = 0;
                }

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY, int gridZ)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                for (int neighbourZ = gridZ - 1; neighbourZ <= gridZ + 1; neighbourZ++)
                {
                    if (IsInMapRange(neighbourX, neighbourY, neighbourZ))
                    {
                        if (neighbourX != gridX || neighbourY != gridY || neighbourZ != gridZ)
                        {
                            wallCount += map[neighbourX, neighbourY, neighbourZ];
                        }
                    }
                    else
                    {
                        wallCount++;
                    }
                }
            }
        }

        return wallCount;
    }

    struct Coord
    {
        public int tileX;
        public int tileY;
        public int tileZ;

        public Coord(int x, int y, int z)
        {
            tileX = x;
            tileY = y;
            tileZ = z;
        }
    }


    class Room : IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room()
        {
        }

        public Room(List<Coord> roomTiles, int[,,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        for (int z = tile.tileZ - 1; z <= tile.tileZ + 1; z++)
                        {
                            if (x == tile.tileX || y == tile.tileY || z == tile.tileZ)
                            {
                                if (map[x, y, z] == 1)
                                {
                                    edgeTiles.Add(tile);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }
}