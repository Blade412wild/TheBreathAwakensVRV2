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

    public void SaveNewBreathSample(BreathingSample2 currentBreathSample)
    {
        if(!createdRunTimeFolder)
        {
            createdRunTimeFolder = true;
            SetupSavingPathbreathingSample();
        }
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

    private void SetupSavingPathbreathingSample() //TODO create folder only when saving
    {
        currentDate = DateTime.Now;


        Year = currentDate.Year;
        Month = currentDate.Month;
        Day = currentDate.Day;

        string DateFolderName = Day + dateSeperator + Month + dateSeperator + Year;
        string pathToDateFolderName = SampleFolderPath + pathSeperator + DateFolderName;

        if (AssetDatabase.IsValidFolder(pathToDateFolderName) == false)
        {
            AssetDatabase.CreateFolder(SampleFolderPath, DateFolderName);
        }


        currentHour = DateTime.Now.Hour;

        string hourFolder = currentHour + TimeSeperator + "00";
        string pathTohourFolderFolderName = pathToDateFolderName;
        string hourTargetPath = pathTohourFolderFolderName + pathSeperator + hourFolder;

        if (AssetDatabase.IsValidFolder(hourTargetPath) == false)
        {
            AssetDatabase.CreateFolder(pathTohourFolderFolderName, hourFolder);
            CurrentSampleFolderPath = pathTohourFolderFolderName;
        }


        currentMinute = DateTime.Now.Minute;
        currentSecond = DateTime.Now.Second;

        string runTimeMomentFolderName = currentHour + TimeSeperator + currentMinute + TimeSeperator + currentSecond;
        string pathToRunTimeFolderName = hourTargetPath;
        string runtimePath = pathToRunTimeFolderName + pathSeperator + runTimeMomentFolderName;

        if (AssetDatabase.IsValidFolder(runtimePath) == false)
        {
            AssetDatabase.CreateFolder(pathToRunTimeFolderName, runTimeMomentFolderName);
        }

        CurrentSampleFolderPath = runtimePath;
        Debug.Log(CurrentSampleFolderPath);
    }
}
