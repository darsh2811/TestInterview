
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public enum AnchorPoint
    {
        TopLeft,
        TopHalf,
        TopRight,
        RightHalf,
        BottomRight,
        BottomHalf,
        BottomLeft,
        LeftHalf,
        Center
    }

    public int widthSegments = 1;
    public int lengthSegments = 1;
    int planesToLoad=4; 
    public float width = 1.0f;
    public float length = 1.0f;
    public Orientation orientation = Orientation.Horizontal;
    public AnchorPoint anchor = AnchorPoint.Center;
    public bool addCollider = false;
    public bool createAtOrigin = true;
    public string optionalName;
    bool ChangedFormation;
    bool isfirttime,FadeInv,FadeOutv;
    Vector3 offset;

    public Text percantageText;
    int percantageVisible=100;

    static Camera cam;
    static Camera lastUsedCam;

    public Material[] matWithTexture;
    public string[] scaleRotateAnim;

    private bool canFade;
    private Color alphaColor;
    private float timeToFade = 5f;

    private float targetAlpha;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.current;
        // Hack because camera.current doesn't return editor camera if scene view doesn't have focus
        if (!cam)
            cam = lastUsedCam;
        else
            lastUsedCam = cam;
        isfirttime = true;
        Render();
       
        //OnCreate();
    }


    void Render() { 

        if(GameObject.Find("Container")!=null)
        {
            foreach (Transform child in GameObject.Find("Container").transform)
            {
                Destroy(child.gameObject);
            }
        }
        for (int i=0;i<planesToLoad;i++)
        {
            OnCreate(i);

        }
       isfirttime = false;
        percantageText.text = percantageVisible.ToString();
        targetAlpha = matWithTexture[1].color.a;

    }


    // Set Anchor Position , widht, length of planes
    void setAnchorPoint(int n, GameObject gm)
    {
        if (ChangedFormation == true)
        { 
           if (n == 1)
            {
                anchor = AnchorPoint.TopHalf;
                width = 9.6f;
                length = 5;
                gm.name = "Top";
                offset = new Vector3(0,0f,0);
                //alphaColor.a = 255;

            }
            else if (n == 2)
            {
                anchor = AnchorPoint.BottomLeft;
                width = 4.5f;
                length = 4.5f;
                gm.name = "Right";
                offset = new Vector3(0.31f, 0.4f, 0);
                gm.AddComponent<Animator>();

            }
            else if(n==3)
            {
                anchor = AnchorPoint.BottomRight;
                width = 4.5f;
                length = 4.5f;
                gm.name = "Left";
                offset = new Vector3(-0.31f, 0.4f, 0);
                gm.AddComponent<Animator>();
            }
           
                gm.transform.SetParent(GameObject.Find("Container").transform);
                gm.transform.position = new Vector3(0, 1, 0) + offset;

        }
        else
        {
            if (n == 0)
            {
                anchor = AnchorPoint.Center;
                width = 11;
                length = 11;
                gm.name = "Container";
                gm.AddComponent<CalculateArea>();
                gm.transform.position = new Vector3(0, 1, 0) + offset;
            }
            else if (n == 1)
            {
                anchor = AnchorPoint.BottomHalf;
                width = 9.6f;
                length = 5;
                gm.name = "Top";
                offset = new Vector3(0,0,0);
            }
            else if (n == 2)
            {
                anchor = AnchorPoint.TopLeft;
                width = 4.5f;
                length = 4.5f;
                gm.name = "Right";
                offset = new Vector3(0.31f, -.6f, 0);
                gm.AddComponent<Animator>();
            }
            else
            {
                anchor = AnchorPoint.TopRight;
                width = 4.5f;
                length = 4.5f;
                gm.name = "Left";
                offset = new Vector3(-0.31f, -.6f, 0);
                gm.AddComponent<Animator>();
            }
            if (n != 0)
            {
                gm.transform.SetParent(GameObject.Find("Container").transform);
                gm.transform.position = new Vector3(0, 1, 0) + offset;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    
        Color curColor = matWithTexture[1].color;
        float alphaDiff = Mathf.Abs(curColor.a - this.targetAlpha);
        if (alphaDiff > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, this.timeToFade * Time.deltaTime);
            matWithTexture[1].color = curColor;
        }
        else {
            FadeIn();
        }


    }


    //Part 4 Change Planes Positions
    public void changeFormation() {
        ChangedFormation = !ChangedFormation;
        planesToLoad = 4;
        Render();
    }


    //Part 3 Zoom Out Planes
    public void ZoomOut() {
        Camera cn = Camera.main;
        cn.fieldOfView += 12;
        percantageVisible -= 10;
        percantageText.text = percantageVisible.ToString();
    }

    //Part 3 Zoom In Planes
    public void ZoomIn()
    {
        Camera cn = Camera.main;
        if (cn.fieldOfView <= 60)
        {
            cn.fieldOfView = 60;
            percantageVisible = 100;
        }
        else
        {
            cn.fieldOfView -= 12;
            percantageVisible += 10;
        }
        percantageText.text = percantageVisible.ToString();
    }


    // Create Planes 
    void OnCreate(int n)
    {
       
        if ((ChangedFormation == true && n != 0) || (isfirttime == true && n==0) || (GameObject.Find("Container")!=null && n!=0))
        {
            GameObject plane = new GameObject();
            if (!string.IsNullOrEmpty(optionalName))
                plane.name = optionalName;
            else
                plane.name = "Plane";

            if (!createAtOrigin && cam)
                plane.transform.position = cam.transform.position + cam.transform.forward * 5.0f;
            else
                plane.transform.position = Vector3.zero;

            Vector2 anchorOffset;
            string anchorId;
            setAnchorPoint(n, plane);
            switch (anchor)
            {
                case AnchorPoint.TopLeft:
                    anchorOffset = new Vector2(-width / 2.0f, length / 2.0f);
                    anchorId = "TL";
                    break;
                case AnchorPoint.TopHalf:
                    anchorOffset = new Vector2(0.0f, length / 2.0f);
                    anchorId = "TH";
                    break;
                case AnchorPoint.TopRight:
                    anchorOffset = new Vector2(width / 2.0f, length / 2.0f);
                    anchorId = "TR";
                    break;
                case AnchorPoint.RightHalf:
                    anchorOffset = new Vector2(width / 2.0f, 0.0f);
                    anchorId = "RH";
                    break;
                case AnchorPoint.BottomRight:
                    anchorOffset = new Vector2(width / 2.0f, -length / 2.0f);
                    anchorId = "BR";
                    break;
                case AnchorPoint.BottomHalf:
                    anchorOffset = new Vector2(0.0f, -length / 2.0f);
                    anchorId = "BH";
                    break;
                case AnchorPoint.BottomLeft:
                    anchorOffset = new Vector2(-width / 2.0f, -length / 2.0f);
                    anchorId = "BL";
                    break;
                case AnchorPoint.LeftHalf:
                    anchorOffset = new Vector2(-width / 2.0f, 0.0f);
                    anchorId = "LH";
                    break;
                case AnchorPoint.Center:
                default:
                    anchorOffset = Vector2.zero;
                    anchorId = "C";
                    break;
            }

            MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
            plane.AddComponent(typeof(MeshRenderer));

            string planeAssetName = plane.name + widthSegments + "x" + lengthSegments + "W" + width + "L" + length + (orientation == Orientation.Horizontal ? "H" : "V") + anchorId + ".asset";
            // Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor/" + planeAssetName, typeof(Mesh));
        Mesh m = (Mesh)Resources.Load(Application.persistentDataPath + "/" + planeAssetName, typeof(Mesh));
            
            // Debug.Log(m);
            if (m == null)
            {
                m = new Mesh();
                m.name = plane.name;

                int hCount2 = widthSegments + 1;
                int vCount2 = lengthSegments + 1;
                int numTriangles = widthSegments * lengthSegments * 6;
                int numVertices = hCount2 * vCount2;

                Vector3[] vertices = new Vector3[numVertices];
                Vector2[] uvs = new Vector2[numVertices];
                int[] triangles = new int[numTriangles];

                int index = 0;
                float uvFactorX = 1.0f / widthSegments;
                float uvFactorY = 1.0f / lengthSegments;
                float scaleX = width / widthSegments;
                float scaleY = length / lengthSegments;
                for (float y = 0.0f; y < vCount2; y++)
                {
                    for (float x = 0.0f; x < hCount2; x++)
                    {
                        if (orientation == Orientation.Horizontal)
                        {
                            vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x, 0.0f, y * scaleY - length / 2f - anchorOffset.y);
                        }
                        else
                        {
                            vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x, y * scaleY - length / 2f - anchorOffset.y, 0.0f);
                        }
                        uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
                    }
                }

                index = 0;
                for (int y = 0; y < lengthSegments; y++)
                {
                    for (int x = 0; x < widthSegments; x++)
                    {
                        triangles[index] = (y * hCount2) + x;
                        triangles[index + 1] = ((y + 1) * hCount2) + x;
                        triangles[index + 2] = (y * hCount2) + x + 1;

                        triangles[index + 3] = ((y + 1) * hCount2) + x;
                        triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                        triangles[index + 5] = (y * hCount2) + x + 1;
                        index += 6;
                    }
                }

                m.vertices = vertices;
                m.uv = uvs;
                m.triangles = triangles;
                m.RecalculateNormals();

            }

            meshFilter.sharedMesh = m;

            meshFilter.GetComponent<MeshRenderer>().material = matWithTexture[n];
            m.RecalculateBounds();

            if (addCollider)
                plane.AddComponent(typeof(BoxCollider));

        }
    }


    // Play Animation on Planes
    public void PlayAnimation()
    {
        GameObject.Find("Container").transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController = Resources.Load(scaleRotateAnim[0]) as RuntimeAnimatorController;
        GameObject.Find("Container").transform.GetChild(2).GetComponent<Animator>().runtimeAnimatorController = Resources.Load(scaleRotateAnim[0]) as RuntimeAnimatorController;
        FadeOut();
    }

    public void FadeOut()
    {
        this.targetAlpha = 0.0f;
    }

    public void FadeIn()
    {
        this.targetAlpha = 1.0f;
    }

}


