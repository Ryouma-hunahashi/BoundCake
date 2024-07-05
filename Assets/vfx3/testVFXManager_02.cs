using UnityEngine;
using UnityEngine.VFX;

[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
struct CustomData
{
    public Vector3 color;
    public Vector3 position;
}

public class testVFXManager_02 : MonoBehaviour
{
    CustomData data;
    [SerializeField]VisualEffect effect;
    GraphicsBuffer waveStartTimeBuffer;
    GraphicsBuffer waveStartPosBuffer;
    GraphicsBuffer waveSpeedBuffer;
    float[] waveStartTimeArray = {0,0,0};//óvëfêîÇÕÇRå¬
    int[] waveStartPosArray = { 0, 0, 0 };
    float[] waveSpeedArray = { 0, 0, 0 };
    float TestStartTime = 0;
    int arrayCnt = 0;
    int pushCnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        //effect = GetComponent<VisualEffect>();
        waveStartTimeBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 3, sizeof(float));
        waveStartPosBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 3, sizeof(int));
        waveSpeedBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 3, sizeof(float));
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space))
        {

            arrayCnt++;
            if (arrayCnt > 1 )
            {
                arrayCnt = 0;
            }
            TestStartTime = Time.time;
            //effect.SetFloat("startTime", TestStartTime);
            waveStartTimeArray[arrayCnt] = TestStartTime;
            waveStartPosArray[arrayCnt] = 0 + 30 * arrayCnt;
            waveSpeedArray[arrayCnt] = 5 + -10 * arrayCnt;

            waveStartTimeBuffer.SetData(waveStartTimeArray );
            waveStartPosBuffer.SetData(waveStartPosArray );
            waveSpeedBuffer.SetData(waveSpeedArray );
            effect.SetGraphicsBuffer("waveStartTimeBuffer",waveStartTimeBuffer);
            effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
            effect.SetGraphicsBuffer("waveSpeedBuffer",waveSpeedBuffer);
            effect.SetInt("arrayCnt",arrayCnt);
            

        }
        //waveStartTimeBuffer.SetData(waveStartTimeArray );
        //waveStartPosBuffer.SetData(waveStartPosArray );
        //effect.SetGraphicsBuffer("waveStartTimeBuffer",waveStartTimeBuffer);
        //effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
    }

}
