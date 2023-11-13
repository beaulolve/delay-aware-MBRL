using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;


public struct SaveInfo
{
    public string SavePath;
    public byte[] bytes;
}
public class MulCameraControl : MonoBehaviour
{
    public List<Camera> Cameras;
    //旋转变量;
    private float m_deltX = 0f;
    private float m_deltY = 0f;
    //缩放变量;
    private float m_distance = 10f;
    private float m_mSpeed = 5f;
    //移动变量;
    private Vector3 m_mouseMovePos = Vector3.zero;
    //平移速度
    float Speed = 200f;

    // 图片编号
    private int index = 0;

    private float passedTime; // default 0
    public float targetTime=10;  // set time interval

    bool runThread = false;

    Thread thread;
    List<SaveInfo> saveInfos = new List<SaveInfo>();

    //Repete();   <-This is the call in the function for update.



    void Start()
    {
        //  transform.localPosition = new Vector3(0, m_distance, 0);
        runThread = true;
        //Debug.Log(runThread);
        thread = new Thread(SendThread);
        thread.Start(); //开启进程
    }
    private void OnDisable()
    {


        runThread = false;
        thread.Abort();
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * Speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Time.deltaTime * Speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * -Speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.left * Time.deltaTime * -Speed);
        }
        if (passedTime > targetTime)
        {
            //  put function here

            Debug.Log(Time.time);

            //
            passedTime = 0; //enter next loop
          //  if (Input.GetKey(KeyCode.Q))
           // {
                for (int i = 0; i < Cameras.Count; i++)
                {
                    CameraCapture(Cameras[i], new Rect(0, 0, 1920, 1080), index, i);
                    //  Texture2D tx = RendererTexToTexture2D(Cameras[i].targetTexture);
                    //  SaveToPNG(tx);
                    //  AssetDatabase.Refresh();//资源文件刷新
                }
                index++;
          //  }
        }
        passedTime += Time.deltaTime;
        /*
                   
                }*/

        //鼠标左键控制旋转
        if (Input.GetMouseButton(0))
        {
            m_deltX += Input.GetAxis("Mouse X") * m_mSpeed;
            m_deltY -= Input.GetAxis("Mouse Y") * m_mSpeed;
            m_deltX = ClampAngle(m_deltX, -360, 360);//旋转幅度 左右
            m_deltY = ClampAngle(m_deltY, -70, 70);//旋转幅度 上下
            transform.rotation = Quaternion.Euler(m_deltY, m_deltX, 0);
        }
        //鼠标滑轮缩放
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //自由缩放方式;
            m_distance = Input.GetAxis("Mouse ScrollWheel") * 10f;
            transform.localPosition = transform.position + transform.forward * m_distance;
        }

    }
     // 发送消息进程
    public void SendThread()
    {
        //Debug.Log(runThread);
        while (runThread)
        {
            //Debug.Log("ok");
            lock (this)
            {
                //Debug.Log(connected);
                if (saveInfos.Count != 0)
                {
                    Debug.Log("ok");
                    foreach (SaveInfo m in saveInfos)
                    {
                        System.IO.File.WriteAllBytes(m.SavePath, m.bytes);//写入数据
                    }

                    saveInfos.Clear();
                }
            }
        }
    }

    public void QueueMessage(SaveInfo m)
    {
        lock (this)
        {
            saveInfos.Add(m);
        }
    }


    float ClampAngle(float angle, float minAngle, float maxAgnle)
    {
        if (angle <= -360)
            angle += 360;
        if (angle >= 360)
            angle -= 360;

        return Mathf.Clamp(angle, minAngle, maxAgnle);
    }
    public string GetFilePath()
    {
        return Application.dataPath + index+ "/截图" + Random.Range(0, 10000) + ".png";
    }

    private void CameraCapture(Camera camera, Rect rect,int idx,int i)
    {
        string path = Application.dataPath + "/次数"+idx + "uav" + i + ".png";
        RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, -1);//创建一个RenderTexture对象 

        camera.gameObject.SetActive(true);//启用截图相机
        camera.targetTexture = render;//设置截图相机的targetTexture为render
        camera.Render();//手动开启截图相机的渲染

        RenderTexture.active = render;//激活RenderTexture
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);//新建一个Texture2D对象
        tex.ReadPixels(rect, 0, 0);//读取像素
        tex.Apply();//保存像素信息

        camera.targetTexture = null;//重置截图相机的targetTexture
        RenderTexture.active = null;//关闭RenderTexture的激活状态
        Object.Destroy(render);//删除RenderTexture对象

        SaveInfo saveInfo = new SaveInfo();
        saveInfo.SavePath = path;
        saveInfo.bytes = tex.EncodeToPNG();
        QueueMessage(saveInfo);

        //Debug.Log(string.Format("截取了一张图片: {0}", path));

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();//刷新Unity的资产目录
#endif

    }

    private Texture2D RendererTexToTexture2D(RenderTexture rt)
    {
        int width = rt.width;
        int height = rt.height;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture.active = rt;
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();
        return texture2D;
    }

    private void SaveToPNG(Texture2D tx)
    {
        string path = Application.dataPath + "/Scenes/PoseEstimation/images/" + index + ".png";
        byte[] m_bytes = tx.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, m_bytes);

    }

}
