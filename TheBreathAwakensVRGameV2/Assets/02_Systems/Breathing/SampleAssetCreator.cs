using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SampleAssetCreator : MonoBehaviour
{

    [Header("Info")]
    [SerializeField] private string SampleFolderPath;


    [Header("Control")]
    [SerializeField] private bool CreateScriptableObject;
    [SerializeField] private bool SaveToAsset;

    private BreathingSample2 currentSample;
    private const string pathSeperator = "/";

    private string currentSamplePath;
    private string currentSampleName;

    private List<string> FolderNames = new List<string>();

    private DateTime currentDate;
    private int Year;
    private int Month;
    private int Day;

    private int currentHour;
    private int currentMinute;
    private int currentSecond;

    private const string dateSeperator = "-";
    private const string TimeSeperator = ";";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupSavingPathbreathingSample();
    }

    // Update is called once per frame
    void Update()
    {
        if (CreateScriptableObject)
        {
            CreateScriptableObject = false;
            CreateSample();
        }

        if (SaveToAsset)
        {
            SaveToAsset = false;
            SaveToAssets();
        }
    }

    private void SetupSavingPathbreathingSample()
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
        currentMinute = DateTime.Now.Minute;
        currentSecond = DateTime.Now.Second;

        string runTimeMomentFolderName = currentHour + TimeSeperator + currentMinute;
        string pathToHourFolderName = pathToDateFolderName + pathSeperator + runTimeMomentFolderName;

        if (AssetDatabase.IsValidFolder(pathToHourFolderName) == false)
        {
            AssetDatabase.CreateFolder(pathToDateFolderName, runTimeMomentFolderName);
        }
        else
        {
            runTimeMomentFolderName += TimeSeperator + currentSecond;
            AssetDatabase.CreateFolder(pathToDateFolderName, runTimeMomentFolderName);
        }



        Debug.Log("D : " + currentDate + " | H : " + currentDate.TimeOfDay + " | M : " + currentMinute);
    }

    private void CreateNewFolder()
    {
        //string parentFolderPath = PathBeforeSampleFolder + pathSeperator + SampleFoldername;
        DateTime TestRunDate = DateTime.Now.Date;

        //string folderName = TestRunDate;
        //AssetDatabase.CreateFolder(parentFolderPath);
    }

    private void CreateSample()
    {
        currentSample = new BreathingSample2();
        currentSample.SampleName = currentSampleName;
        currentSample.Path = SampleFolderPath + currentSampleName;
    }

    private void SaveToAssets()
    {
        //string fullPath = PathBeforeSampleFolder + pathSeperator + SampleFoldername + pathSeperator + currentSampleName + ".asset";
        //AssetDatabase.CreateAsset(currentSample, fullPath);

    }
}
