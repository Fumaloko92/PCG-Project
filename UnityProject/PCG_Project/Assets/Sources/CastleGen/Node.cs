using UnityEngine;
using System.Collections;

public class Node
{
    public float x, y, width, height;
    public int minSize;

    public Node leftNode, rightNode;
    public Building building;

    public Node(float x, float y, float width, float height, int minSize)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.minSize = minSize;
    }

    public bool divide()
    {
        //if we already divided it, then don't do it again
        if(leftNode != null || rightNode != null)
        {
            return false;
        }

        //which direction to divide
        bool divideHor = false;
        if(height > width)
        {
            divideHor = true;
        }

        float maxSize = (divideHor ? height : width) - minSize;
        if (maxSize <= minSize) //if too small
        {
            return false;
        }

        float dividePoint = Random.Range(minSize, maxSize);
        
        if(divideHor)
        {
            //Debug.Log("height");
            leftNode = new Node(x, y - (height / 2) + (dividePoint / 2), width, dividePoint, minSize);

            rightNode = new Node(x, y + (dividePoint / 2), width, height - dividePoint, minSize);
        }
        else
        {
            //Debug.Log("width");
            leftNode = new Node(x - (width / 2) + (dividePoint / 2), y, dividePoint, height, minSize);

            rightNode = new Node(x + (dividePoint / 2), y, width - dividePoint, height, minSize);
        }

        return true;
    }

    public void createBuildingSpace()
    {
        float sizeX = Random.Range(3,width-1);
        float sizeY = Random.Range(3, height - 1);

        float posX = Random.Range(-(width/2)+(sizeX/2)+1, (width/2) - (sizeX/2)-1);
        float posY = Random.Range(-(height / 2) + (sizeY / 2)+1, (height / 2) - (sizeY / 2)-1);


        building = new Building(x + posX, y + posY, sizeX, sizeY);
    }
}
