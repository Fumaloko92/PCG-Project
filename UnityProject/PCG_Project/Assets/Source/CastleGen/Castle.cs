using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Castle : System.IComparable<Castle> {

    public float fitness;
    //public float Fitness { get { return this.fitness; } set { fitness = value; } }

    CastleGenerator gen;
    Vector2 position;
    IntRange nodeSize;
    int[] constructIntervals;
    int buildingCount;
    int buildingsPlaced = 0;

    List<Node> nodes = new List<Node>();
    GameObject container;
    List<Vector3> wallCorners = new List<Vector3>();

    public Castle(CastleGenerator generator, int nodeMin, int nodeMax, int[] constructIntervals, int buildingCount)
    {
        this.fitness = 0;
        this.gen = generator;
        this.position = generator.Position;
        this.nodeSize = new IntRange(nodeMin, nodeMax);
        this.constructIntervals = constructIntervals;
        this.buildingCount = buildingCount;
    }

    //randomizes values - only run for the initial generation
    public void randomizeValues()
    {
        //nodesize, constructprobability, buildingcount
        nodeSize.min = Random.Range(10, 20);
        nodeSize.max = Random.Range(nodeSize.min, 40);

        constructIntervals = new int[Random.Range(1,100)];

        for (int i = 0; i < constructIntervals.Length; i++ )
        {
            constructIntervals[i] = Random.Range(0, 10);
        }

        //randomizes building count according to initial desired count +- 5 (atleast have 1 building)
        buildingCount = Random.Range(buildingCount - 5, buildingCount + 5);
        if (buildingCount < 1) buildingCount = 1;
    }

    //reproduces offsprings using 2 parents
    public Castle[] reproduce(Castle other)
    {
        Castle[] offspring = new Castle[2];

        int[] thisConstructInt = this.constructIntervals;
        int[] otherConstructInt = other.constructIntervals;

        int thisConInLength = (int)thisConstructInt.Length / 2;
        int otherIndex = (int)otherConstructInt.Length / 2;
        for (int i = 0; i < thisConInLength; i++)
        {
            if (otherConstructInt.Length > otherIndex)
            {
                thisConstructInt[i] = otherConstructInt[otherIndex];
                otherIndex++;
            }
            else
            {
                break;
            }
        }

        int otherConInLength = (int)otherConstructInt.Length / 2;
        int thisIndex = 0;
        for (int i = otherConInLength; i < otherConstructInt.Length; i++)
        {
            if (thisConstructInt.Length > thisIndex)
            {
                otherConstructInt[i] = thisConstructInt[thisIndex];
                thisIndex++;
            }
            else
            {
                break;
            }
        }

        offspring[0] = new Castle(this.gen, this.nodeSize.min, this.nodeSize.max,thisConstructInt, this.buildingCount);
        offspring[1] =  new Castle(this.gen, other.nodeSize.min, other.nodeSize.max,otherConstructInt, other.buildingCount);

        return offspring;
    }

    //mutate the values with a probability
    public void mutate()
    {
        if(Random.value < CastleGenerator.MUTATE_PROBABILITY)
        {
            nodeSize.min = Random.Range(10, 20);
            nodeSize.max = Random.Range(nodeSize.min, 40);
        }

        int intervalSizeDifference = Random.Range(-1, 2);
        int size = constructIntervals.Length + intervalSizeDifference;
        if(size < 1) size = 1;

        int[] temp = new int[size];

        for (int i = 0; i < temp.Length; i++ )
        {
            if (i < constructIntervals.Length)
            {
                temp[i] = constructIntervals[i];
            }
            else
            {
                temp[i] = Random.Range(0, 10); 
            }
        }
        constructIntervals = temp;

        for (int i = 0; i < constructIntervals.Length; i++)
        {
            if (Random.value < CastleGenerator.MUTATE_PROBABILITY)
            {
                int difference = Random.Range(-2, 3);

                constructIntervals[i] += difference;
                if (constructIntervals[i] < 0) constructIntervals[i] = 0;
            }
        }

        if (Random.value < CastleGenerator.MUTATE_PROBABILITY)
        {
            buildingCount = Random.Range(buildingCount - 5, buildingCount + 5);
            if (buildingCount < 1) buildingCount = 1;
        }
    }

    /// <summary>
    /// instantiate gameobjects to build final castle - phenotype of the genetic algorithm
    /// </summary>
    public void buildCastle()
    {
        //container
        container = new GameObject();
        container.transform.position = new Vector3(position.x, 0, position.y);
        container.transform.name = "Castle";

        //build buildings
        foreach (Node node in nodes)
        {
            if (node.building != null)
            {
                Building building = node.building;

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //go.GetComponent<MeshRenderer>().material.color = Color.red;
                go.name = "building [" + building.GetHashCode() + "]";
                go.transform.localScale = new Vector3(building.width, 5, building.height);
                go.transform.position = new Vector3(building.x, Terrain.activeTerrain.SampleHeight(new Vector3(building.x, 0, building.y)), building.y);

                go.transform.parent = container.transform;
            }
        }

        //build towers and walls
        List<Vector3> excludedCorners = new List<Vector3>();
        //corner pillars
        foreach (Vector3 corner in wallCorners)
        {
            GameObject gob = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //gob.GetComponent<MeshRenderer>().material.color = Color.yellow;
            gob.name = "pillar";
            gob.transform.localScale = new Vector3(1F, 20, 1F);
            gob.transform.position = new Vector3(corner.x, corner.y, corner.z);
            gob.transform.parent = container.transform;

            Vector3 XCorner = Vector3.zero;
            Vector3 ZCorner = Vector3.zero;

            //build wall
            foreach (Vector3 otherCorner in wallCorners)
            {
                if (!excludedCorners.Contains(otherCorner) && otherCorner != corner)
                {
                    if (Mathf.Approximately(corner.x, otherCorner.x))
                    {
                        if (XCorner == Vector3.zero)
                        {
                            XCorner = otherCorner;
                        }
                        else if (Vector3.Distance(corner, otherCorner) < Vector3.Distance(corner, XCorner))
                        {
                            XCorner = otherCorner;
                        }
                    }
                    if (Mathf.Approximately(corner.z, otherCorner.z))
                    {
                        if (ZCorner == Vector3.zero)
                        {
                            ZCorner = otherCorner;
                        }
                        else if (Vector3.Distance(corner, otherCorner) < Vector3.Distance(corner, ZCorner))
                        {
                            ZCorner = otherCorner;
                        }
                    }
                }
            }

            if (XCorner != Vector3.zero)
            {

                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = "wall X[" + corner + " - " + XCorner + "]";
                ///wall.AddComponent<MeshFilter>();

                genWall(wall.GetComponent<MeshFilter>(), corner, XCorner);

                wall.transform.parent = container.transform;
            }
            if (ZCorner != Vector3.zero)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = "wall Z[" + corner + " - " + ZCorner + "]";
                ///wall.AddComponent<MeshFilter>();

                genWall(wall.GetComponent<MeshFilter>(), corner, ZCorner);

                wall.transform.parent = container.transform;
            }
            excludedCorners.Add(corner);
        }
    }

    public void clear()
    {
        nodes.Clear();
        wallCorners.Clear();
        buildingsPlaced = 0;
    }
    public void generateCastle()
    {
        if (nodes == null || nodes.Count < 1)
        {
            createRoot(gen.Dimension);

            runBinaryParticioning();
            addBuildings();
            addCorners();
        }
    }

    void createRoot(Vector2 dim)
    {
        Node root = new Node(position.x, position.y, dim.x, dim.y, nodeSize.min);
        nodes.Add(root);
    }

    void runBinaryParticioning()
    {
        bool divide = true;

        //potential number of divisions
        int pass = Mathf.RoundToInt(gen.Dimension.x) / nodeSize.max;

        for (int j = 0; j < pass; j++ )
        {
            divide = false;

            List<Node> temp = nodes;

            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].leftNode == null && temp[i].rightNode == null)
                {
                    if (temp[i].width > nodeSize.max || temp[i].height > nodeSize.max || Random.Range(0, 11) > 5)
                    {
                        if (temp[i].divide())
                        {
                            nodes.Add(temp[i].leftNode);
                            nodes.Add(temp[i].rightNode);
                            divide = true;
                        }
                    }
                }
            }

            if(!divide)
            {
                break;
            }
        }
    }

    void addBuildings()
    {
        //remove all with children
        List<Node> temp = new List<Node>();
        foreach (Node n in nodes)
        {
            if (n.leftNode == null && n.rightNode == null)
            {
                temp.Add(n);
            }
        }
        nodes = temp;

        //sort with middle ones first
        nodes.Sort(delegate(Node a, Node b)
        {
            return Vector2.Distance(position, new Vector2(a.x, a.y)).CompareTo(Vector2.Distance(position, new Vector2(b.x, b.y)));
        });

        int nodeIndex = 999;
        
        int intervalIndex = 0;
        int interval = constructIntervals[intervalIndex];
        //make buildings
        foreach (Node n in nodes)
        {
            if (buildingCount > buildingsPlaced)
            {
                if(interval <= 0)
                {
                    n.createBuildingSpace();

                    buildingsPlaced++;

                    intervalIndex++;
                    if (intervalIndex >= constructIntervals.Length) intervalIndex = 0;

                    interval = constructIntervals[intervalIndex];
                }
                else
                {
                    interval--;
                }
            }
            else
            {
                nodeIndex = nodes.IndexOf(n);
                break;
            }
        }

        //remove unused nodes
        if (nodeIndex != 999)
        {
            nodes.RemoveRange(nodeIndex, nodes.Count - nodeIndex);
        }
    }

    void addCorners()
    {
        List<Vector2> corners = new List<Vector2>();
        //get all corners of nodes
        for (int i = 0; i < nodes.Count; i++)
        {
            float lowX = nodes[i].x - (nodes[i].width / 2);
            float higX = nodes[i].x + (nodes[i].width / 2);
            float lowY = nodes[i].y - (nodes[i].height / 2);
            float higY = nodes[i].y + (nodes[i].height / 2);

            corners.Add(new Vector2(lowX, lowY));
            corners.Add(new Vector2(lowX, higY));
            corners.Add(new Vector2(higX, lowY));
            corners.Add(new Vector2(higX, higY));
        }

        //exclude all corners that dont share position with other corners
        foreach (Vector2 pos in corners)
        {
            int points = 0;
            foreach (Vector2 otherPos in corners)
            {
                if (otherPos == pos)// Vector3.Distance(otherPos, pos) < 2)
                {
                    points++;
                }

                if (points >= 2)
                {
                    break;
                }
            }

            if (points < 2)
            {
                wallCorners.Add(new Vector3(pos.x, Terrain.activeTerrain.SampleHeight(new Vector3(pos.x, 0, pos.y)), pos.y));
            }
        }
    }

    void genWall(MeshFilter meshfilter, Vector3 corner, Vector3 otherCorner)
    {
        MeshFilter meshf = meshfilter;

        Mesh mesh = Mesh.Instantiate(meshf.sharedMesh) as Mesh;

        mesh.Clear();

        Vector3 v0 = new Vector3(corner.x + 0.25F, corner.y - 10F, corner.z + 0.25F);
        Vector3 v1 = new Vector3(corner.x - 0.25F, corner.y - 10F, corner.z + 0.25F);
        Vector3 v2 = new Vector3(otherCorner.x - 0.25F, otherCorner.y - 10F, otherCorner.z - 0.25F);
        Vector3 v3 = new Vector3(otherCorner.x + 0.25F, otherCorner.y - 10F, otherCorner.z - 0.25F);
        Vector3 v4 = new Vector3(corner.x + 0.25F, corner.y + 5F, corner.z + 0.25F);
        Vector3 v5 = new Vector3(corner.x - 0.25F, corner.y + 5F, corner.z + 0.25F);
        Vector3 v6 = new Vector3(otherCorner.x - 0.25F, otherCorner.y + 5F, otherCorner.z - 0.25F);
        Vector3 v7 = new Vector3(otherCorner.x + 0.25F, otherCorner.y + 5F, otherCorner.z - 0.25F);

        Vector3[] vertices = 
        {
            // Bottom Polygon
            v0, v1, v2, v0,
            // Left Polygon
            v7, v4, v0, v3,
            // Front Polygon
            v4, v5, v1, v0,
            // Back Polygon
            v6, v7, v3, v2,
            // Right Polygon
            v5, v6, v2, v1,
            // Top Polygon
            v7, v6, v5, v4
        };

        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normals = new Vector3[]
        {
            // Bottom Side Render
            down, down, down, down,
            // LEFT Side Render
            left, left, left, left,
            // FRONT Side Render
            front, front, front, front,
            // BACK Side Render
            back, back, back, back,
            // RIGTH Side Render
            right, right, right, right,
            // UP Side Render
            up, up, up, up
        };

        Vector2 _00_CORDINATES = new Vector2(0f, 0f);
        Vector2 _10_CORDINATES = new Vector2(1f, 0f);
        Vector2 _01_CORDINATES = new Vector2(0f, 1f);
        Vector2 _11_CORDINATES = new Vector2(1f, 1f);
        Vector2[] uvs = new Vector2[]
        {
            // Bottom
            _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
            // Left
            _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
            // Front
            _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
            // Back
            _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
            // Right
            _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
            // Top
            _11_CORDINATES, _01_CORDINATES, _00_CORDINATES, _10_CORDINATES,
        };


        int[] triangles = new int[]
        {
            // Cube Bottom Side Triangles
            3, 1, 0,
            3, 2, 1,    
            // Cube Left Side Triangles
            3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
            // Cube Front Side Triangles
            3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
            // Cube Back Side Triangles
            3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
            // Cube Rigth Side Triangles
            3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
            // Cube Top Side Triangles
            3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        meshfilter.mesh = mesh;
    }

    /// <summary>
    /// evaluate castle and save a fitness
    /// </summary>
    public void evaluate()
    {
        int subtraction = (buildingsPlaced > gen.DesiredBuildingCount ? buildingsPlaced-gen.DesiredBuildingCount : 0);
        buildingsPlaced = buildingsPlaced - subtraction;

        float buildingFitness = ((float)buildingsPlaced / (float)gen.DesiredBuildingCount);

        List<float> wallLengths = new List<float>();
        List<Vector3> excludedTowers = new List<Vector3>();
        foreach(Vector3 corner in wallCorners)
        {
            foreach(Vector3 other in wallCorners)
            {
                if(!excludedTowers.Contains(other))
                {
                    if(Mathf.Approximately(corner.x,other.x) || Mathf.Approximately(corner.z,other.z))
                    {
                        wallLengths.Add(Vector3.Distance(corner, other));
                    }
                }
            }
            excludedTowers.Add(corner);
        }

        float avgWallLength = 0;
        foreach(float lngth in wallLengths)
        {
            avgWallLength += lngth;
        }
        avgWallLength/=wallLengths.Count;

        float defenceFitness = 1-(avgWallLength / 100);


        fitness = (buildingFitness + defenceFitness) / 2;

    }

    public string output()
    {
        string str = "";

        str += "BULDINGCOUNT[" + buildingsPlaced + "]";

        str += " CONSTRUCTINTERVALS[";

        for(int i = 0; i < constructIntervals.Length; i++)
        {
            str += "" + constructIntervals[i];
            if (i < constructIntervals.Length-1) str += ",";
        }

        str += "]";

        str += " NODESIZE[" + nodeSize.min + "," + nodeSize.max + "]";

        return str;
    }

    public int CompareTo(Castle other)
    {
        if(fitness < other.fitness)
        {
            return 1;
        }
        if (fitness > other.fitness)
        {
            return -1;
        }
        return 0;
    }
}
