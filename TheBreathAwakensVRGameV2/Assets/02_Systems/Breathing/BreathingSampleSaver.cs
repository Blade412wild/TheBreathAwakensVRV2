using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BreathingSampleSaver
{

    [Header("Control")]
    [SerializeField] private bool CreateScriptableObject;
    [SerializeField] private bool SaveToAsset;

    public string CurrentSampleFolderPath { get; private set; }

    private const string SampleFolderPath = "Assets/10_BreathingSamples";

    private const string pathSeperator = "/";
    private const string dateSeperator = "-";
    private const string TimeSeperator = ";";

    private string currentSampleName;

    private DateTime currentDate;
    private int Year;
    private int Month;
    private int Day;

    private int currentHour;
    private int currentMinute;
    private int currentSecond;

    private int breathingSampleFileCounter = 0;
    private string currentSampleFileName;

    private bool createdRunTimeFolder;
    private bool saveFlag = false;
    private List<BreathingSampleClass> breathingSampleBuffer = new List<BreathingSampleClass>();

    private string folderPath;

    //private bool message

    public BreathingSampleSaver()
    {
        currentDate = DateTime.Now;
    }

    ~BreathingSampleSaver()
    {
        OnDisable();
    }
    public void OnUpdate()
    {
        //if (saveFlag)
        //{
        //    saveFlag = false;
        //    SaveBuffer();
        //}
    }

    public void OnDisable()
    {
        if (saveFlag)
        {
            saveFlag = false;
            SaveBuffer();
        }
    }

    private void SaveBuffer()
    {
        Debug.Log("Save buffer");
        if (!createdRunTimeFolder)
        {
            createdRunTimeFolder = true;
            SetupFolders();
        }

        // save buffer
        foreach (BreathingSampleClass breathingSampleClass in breathingSampleBuffer)
        {
            BreathingSample2 breathingSampleScriptableObject = ConvertSampleClassToScriptableObject(breathingSampleClass);
            Save(breathingSampleScriptableObject);
        }

        breathingSampleBuffer.Clear();
        saveFlag = false;
    }

    private void Save(BreathingSample2 currentBreathSample)
    {

        currentSampleFileName = "Sample " + breathingSampleFileCounter.ToString();
        string fullPath = CurrentSampleFolderPath + pathSeperator + currentSampleFileName + ".asset";

        if (AssetDatabase.AssetPathExists(fullPath) == false)
        {
            AssetDatabase.CreateAsset(currentBreathSample, fullPath);
            breathingSampleFileCounter++;
        }
        else
        {
            Debug.LogWarning("BreathingSample " + currentSampleFileName + " Already Exist");
        }

    }

    private BreathingSample2 ConvertSampleClassToScriptableObject(BreathingSampleClass breathingSampleClass)
    {
        BreathingSample2 breathingSampleScriptableObject = ScriptableObject.CreateInstance<BreathingSample2>();
        breathingSampleScriptableObject.breathingCycles = breathingSampleClass.breathingCycles;
        breathingSampleScriptableObject.Curve = breathingSampleClass.Curve;

        breathingSampleScriptableObject.TotalBreathingCycles = breathingSampleClass.TotalBreathingCycles;
        breathingSampleScriptableObject.TotalDuration = breathingSampleClass.TotalDuration;

        breathingSampleScriptableObject.AvarageInhaleSpeed = breathingSampleClass.AvarageInhaleSpeed;
        breathingSampleScriptableObject.PeakInhaleSpeed = breathingSampleClass.PeakInhaleSpeed;

        breathingSampleScriptableObject.AvarageExhaleSpeed = breathingSampleClass.AvarageExhaleSpeed;
        breathingSampleScriptableObject.PeakExhaleSpeed = breathingSampleClass.PeakExhaleSpeed;

        breathingSampleScriptableObject.AvarageTimeBetweenCycles = breathingSampleClass.AvarageTimeBetweenCycles;


        return breathingSampleScriptableObject;

    }


    public void SaveNewBreathSample(BreathingSampleClass breathingSampleClass)
    {
        breathingSampleBuffer.Add(breathingSampleClass);
        saveFlag = true;
    }

    private void SetupFolders() //TODO create folder only when saving
    {
        //currentDate = DateTime.Now;


        Year = currentDate.Year;
        Month = currentDate.Month;
        Day = currentDate.Day;

        string DateFolderName = Day + dateSeperator + Month + dateSeperator + Year;
        string pathToDateFolderName = SampleFolderPath + pathSeperator + DateFolderName;

        if (AssetDatabase.IsValidFolder(pathToDateFolderName) == false)
        {
            AssetDatabase.CreateFolder(SampleFolderPath, DateFolderName);
        }


        currentHour = currentDate.Hour;

        string hourFolder = currentHour + TimeSeperator + "00";
        string pathTohourFolderFolderName = pathToDateFolderName;
        string hourTargetPath = pathTohourFolderFolderName + pathSeperator + hourFolder;

        if (AssetDatabase.IsValidFolder(hourTargetPath) == false)
        {
            AssetDatabase.CreateFolder(pathTohourFolderFolderName, hourFolder);
            CurrentSampleFolderPath = pathTohourFolderFolderName;
        }


        currentMinute = currentDate.Minute;
        currentSecond = currentDate.Second;

        string runTimeMomentFolderName = currentHour + TimeSeperator + currentMinute + TimeSeperator + currentSecond;
        string pathToRunTimeFolderName = hourTargetPath;
        string runtimePath = pathToRunTimeFolderName + pathSeperator + runTimeMomentFolderName;
        folderPath = runtimePath;

        if (AssetDatabase.IsValidFolder(runtimePath) == false)
        {
            AssetDatabase.CreateFolder(pathToRunTimeFolderName, runTimeMomentFolderName);
        }

        CurrentSampleFolderPath = runtimePath;
        Debug.Log(CurrentSampleFolderPath);

    }
}
