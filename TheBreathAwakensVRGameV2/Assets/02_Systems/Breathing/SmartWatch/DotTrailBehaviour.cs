using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;

public class DotTrailBehaviour : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private bool messageReceived;
    [SerializeField] private bool useOwnUpdate;

    [Header("info")]
    [SerializeField] private int MaxTrailPoints = 20;
    [SerializeField] private List<TrailPoint> allPoints = new List<TrailPoint>();
    [SerializeField] private List<TrailPoint> activePoints = new List<TrailPoint>();
    [SerializeField] private Stack<TrailPoint> deActivePoints = new Stack<TrailPoint>();

    [SerializeField] private float speed;


    [Header("Ref")]
    [SerializeField] private MessageFinishedReceived messagefinishedEvent;
    [SerializeField] private RectTransform ThresholdTrans;
    [SerializeField] private GameObject TrailPrefab;
    [SerializeField] private LineRenderer lineRender;
    [SerializeField] private RectTransform dot;

    private Vector2 moveDir = Vector2.left;
    private TrailPoint trailPoint = new TrailPoint { IsActive = true };
    private Vector2 pos;
    private Vector2 velocity;

    private Stack<TrailPoint> trailPointsActiveBuffer = new Stack<TrailPoint>();
    private Stack<TrailPoint> trailPointsDeActiveBuffer = new Stack<TrailPoint>();

    private float threshold;



    private void Start()
    {
        if (!useOwnUpdate) return;
        Init();
        Activate();
    }

    private void Update()
    {
        if (!useOwnUpdate) return;
        if (messageReceived)
        {
            messageReceived = false;
            HandleMessageFinishedEvent();
        }
        UpdateActiveTrailpoints();
    }
    private void OnDisable()
    {
        if (!useOwnUpdate) return;
        OnDeactivation();
    }
    public void Init()
    {
        SetupTrail();
        threshold = ThresholdTrans.localPosition.x;
    }

    public void OnUpdate()
    {
        UpdateActiveTrailpoints();
    }


    public void Activate()
    {
        messagefinishedEvent.OnDataReceivedEvent += HandleMessageFinishedEvent;

    }

    public void Deactivate()
    {
        messagefinishedEvent.OnDataReceivedEvent -= HandleMessageFinishedEvent;
    }


    public void OnDeactivation()
    {
        messagefinishedEvent.OnDataReceivedEvent -= HandleMessageFinishedEvent;

    }
    private void SetupTrail()
    {
        for (int i = 0; i < MaxTrailPoints; i++)
        {
            //RectTransform transform = new RectTransform();


            GameObject gameObject = Instantiate(TrailPrefab, transform);

            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

            rectTransform.name = i.ToString();

            TrailPoint point = new TrailPoint
            {
                Id = i,
                IsActive = false,
                Transform = rectTransform,
            };

            deActivePoints.Push(point);
            allPoints.Add(point);
            gameObject.SetActive(false);
        }

    }
    private void HandleMessageFinishedEvent()
    {
        if (deActivePoints.Count <= 0) return;
        TrailPoint trailPoint = deActivePoints.Pop();

        trailPoint.Position = dot.localPosition;

        trailPointsActiveBuffer.Push(trailPoint);

        //Debug.Log("trailPoint = " + trailPoint + " x | count : " + trailPointsActiveBuffer.Count + " x "/* + " | OxygonUsed : " + oxygonUsed*/);
    }

    private void UpdateActiveList()
    {
        while (trailPointsActiveBuffer.Count > 0)
        {
            TrailPoint trailPoint = trailPointsActiveBuffer.Pop();
            trailPoint.Transform.gameObject.SetActive(true);

            activePoints.Add(trailPoint);
        }

    }

    public void UpdateActiveTrailpoints()
    {
        if (trailPointsActiveBuffer.Count > 0)
        {
            UpdateActiveList();
        }

        if (activePoints.Count <= 0) return;

        for (int i = 0; i < activePoints.Count; i++)
        {
            trailPoint = activePoints[i];

            pos = trailPoint.Position;

            velocity = speed * moveDir * Time.deltaTime;
            pos += velocity;

            // updating new pos 
            trailPoint.Position = pos;
            trailPoint.Transform.localPosition = pos;

            // updating list trailpoint (not a reference type)
            activePoints[i] = trailPoint;

            if (pos.x < threshold)
            {
                trailPointsDeActiveBuffer.Push(trailPoint);
                //Debug.Log("threshold reached");
            }
        }

        if (trailPointsDeActiveBuffer.Count > 0)
        {
            RemoveFromeActiveList();
        }

        if (activePoints.Count > 1)
        {
            Vector3[] linePositionsArray = SetPositionsArray();
            //lineRender.useWorldSpace = false;
            lineRender.positionCount = linePositionsArray.Length;
            lineRender.SetPositions(linePositionsArray);
            //Debug.Log("count : " + lineRender.positionCount);
        }

    }

    private void RemoveFromeActiveList()
    {
        while (trailPointsDeActiveBuffer.Count > 0)
        {
            TrailPoint trailPoint = trailPointsDeActiveBuffer.Pop();
            trailPoint.Transform.gameObject.SetActive(false);
            activePoints.RemoveAt(0);
            deActivePoints.Push(trailPoint);
        }
    }

    private Vector3[] SetPositionsArray()
    {
        Vector3[] linePositionsArray = new Vector3[activePoints.Count];

        for (int i = 0; i < activePoints.Count; i++)
        {
            linePositionsArray[i] = activePoints[i].Position;
            //linePositionsArray[i] = activePoints[i].Transform.localPosition;
        }

        return linePositionsArray;

    }

}
