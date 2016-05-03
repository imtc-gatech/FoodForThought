using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

public class FFTLevel : MonoBehaviour {
	
	public static string DataPathRoot; //= Application.dataPath; assigned in Awake()
	public static string ExternalLevelFolderRoot = "LevelData";
	public static string ControlLevelFolder = "Control";
	public static string ExperimentalLevelFolder = "Experimental";
	
	public static string LevelProgressFolder = "";
	public static string LevelProgressWorkingFolder = "ParticipantLevelProgress";
	public static string LevelProgressWorkingFolderBackup = "Temp";
	public static string LevelProgressWorkingFolderArchive = "Archive";
	public static string LevelProgressWorkingFolderError = "Error";
	
	public static string LevelLogExtensionName = @"LevelProgress.xml";

	public TouchScreenKeyboard keyboard;

	public XmlDocument LevelProgress;
	public string WorkingPath = "";
	public string WorkingPathBackup = "";
	public string WorkingFileLocation { get { return WorkingPath + GetLevelLogName(TimeInitialized); } }
	public string WorkingFileLocationBackup 
	{ 
		get 
		{ 
			string backupPath = WorkingPathBackup + GetLevelLogNameWithoutExtension(TimeInitialized);
			backupPath += "_" + FFTDataManager.ConvertDateTimeToSecondsTimestamp(System.DateTime.Now);
			backupPath += "_" + LevelLogExtensionName;
			return backupPath;
		} 
	}
	
	public bool LevelLogOpen = false;
	
	public System.DateTime TimeInitialized;
	

    public enum ExperimentalCondition
    {
        A=0,
        B=1
    }
	
	public enum NSFStudyGroup
    {
        E=0, //Experimental
        C=1 //Control
    }
	
	public enum LoadType
	{
		Previous,
		Repeat,
		Next
	}

    public TextAsset LevelSource; //legacy

    public TextAsset CurrentLevelSource
    {
        get
        {
            if (LevelSources == null || LevelSources[CurrentLevelIndex] == null)
                return null;
            if (CurrentLevelIndex >= LevelSources.Length)
                CurrentLevelIndex = 0;
            return LevelSources[CurrentLevelIndex];
        }
    }

    public TextAsset[] LevelSources;
	public string[] ExternalLevelSources;
	
	public string TitleofLevel = "";
	
    public int CurrentLevelIndex = 0;
	public int CurrentExternalLevelIndex = 0;
	
    public ExperimentalCondition Condition = ExperimentalCondition.B; //retain high detail
	public NSFStudyGroup ParticipantGroup = NSFStudyGroup.C; //defaults to Control(B)

    public int ParticipantID = 0;
    public int SessionID = 0;

    //public FFTResultsScreen.InfoDensity Complexity = FFTResultsScreen.InfoDensity.Verbose;
    public FFTResultsScreen.InfoDensity Complexity 
    {
        get
        {
            return (FFTResultsScreen.InfoDensity)Condition;
        }
        set
        {
            Condition = (ExperimentalCondition)value;
        }
    }

    //STORED IN RECIPECARD / FFTRecipe
    //public FFTCustomer.Name CustomerName = FFTCustomer.Name.Monkey;
    //public string FlavorText = "";

    [HideInInspector]
    public bool TimedLevel = false;
    [HideInInspector]
    public float TimeLimit = 0.0f;
	
	void Awake() {
		//DataPathRoot = FFTDataManager.GetRootPath(); //Application.dataPath;

		// Mobile streaming assets hack to emulate desktop build patterns.
		DataPathRoot = Application.dataPath + "/Raw";
		TimeInitialized = System.DateTime.Now;
		
		WorkingPath = DataPathRoot + @"/" + FFTDataManager.DirectoryName + @"/" + LevelProgressWorkingFolder + @"/";
		WorkingPathBackup = WorkingPath + LevelProgressWorkingFolderBackup + @"/";
		
		if (!System.IO.Directory.Exists(WorkingPath))
        {
            System.IO.Directory.CreateDirectory(WorkingPath);
        }
		if (!System.IO.Directory.Exists(WorkingPathBackup))
        {
            System.IO.Directory.CreateDirectory(WorkingPathBackup);
        }
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		if (keyboard != null) {
			if (keyboard.done) {
				string newLevel = keyboard.text;
				int requestedLevel = int.Parse(newLevel);
				Debug.Log("Requested level " + requestedLevel);
				keyboard = null;
				
				FFTGameManager.Instance.LoadNewLevel (LoadType.Repeat, requestedLevel);
			}
		}

	}

	public void SetLevel(int level) {
		ReportExternalLevelFinished (LoadType.Repeat, level);
	}

    public void NextLevel()
    {
		if (FFTGameManager.Instance.UseExternalLevels)
		{
			ReportExternalLevelFinished(LoadType.Next);
		}
		else
		{
	        if (LevelSources != null)
	        {
	            CurrentLevelIndex++;
	            if (CurrentLevelIndex >= LevelSources.Length)
	                CurrentLevelIndex = 0;
	        }
		}
    }
	
	public void PreviousLevel()
    {
		if (FFTGameManager.Instance.UseExternalLevels)
		{
			ReportExternalLevelFinished(LoadType.Previous);
		}
		else
		{
	        if (LevelSources != null)
	        {
	            CurrentLevelIndex--;
	            if (CurrentLevelIndex < 0)
	                CurrentLevelIndex = LevelSources.Length - 1;
	        }
		}
    }

    public TextAsset GetCurrentLevelSource()
    {
		if (FFTGameManager.Instance.UseExternalLevels)
		{
			return null;
		}
		else
		{
			if (LevelSources == null || LevelSources.Length == 0)
	            return LevelSource;
	        else
	        {
	            return CurrentLevelSource;
	        }
		}
        
    }

	public void LevelChangeButton() {
		Debug.Log ("Pressed level change button.");
		if (!Application.isEditor) {
			string dontMonitor = "";
			keyboard = TouchScreenKeyboard.Open (dontMonitor, TouchScreenKeyboardType.NumberPad, false, false, false, false, "Enter Level");
		}
	}
	
	public void BuildExternalLevelList()
	{
		//TextAsset[] levelsToUse;
		
		string navigationPath;
		
		if (Application.isEditor)
			navigationPath = Application.dataPath; //use levels from Assets project folder if in Editor mode
		else
			navigationPath = DataPathRoot;
		
		navigationPath +="/"+ExternalLevelFolderRoot+"/";
		if (ParticipantGroup == NSFStudyGroup.E)
			navigationPath += ExperimentalLevelFolder;
		else if (ParticipantGroup == NSFStudyGroup.C)
			navigationPath += ControlLevelFolder;
		
		
		/* TODO: Broken attempt to parse out the level identifier from the filename and sort the levels that way...
		string[] unprocessedExternalLevels = System.IO.Directory.GetFiles(navigationPath, "*.xml");
		
		SortedList sortedLevels = new SortedList();
		
		foreach (string file in unprocessedExternalLevels)
		{
			//Debug.Log(file);
			string fileName = file.Substring(file.LastIndexOf('/') + 1);
			fileName = fileName.TrimStart('0');
			Debug.Log(fileName);
			string levelNumberString = fileName.Substring(0, fileName.LastIndexOf('_'));
			int levelNumber = -1000;
			int.TryParse(levelNumberString, out levelNumber);
			if (sortedLevels.ContainsKey(levelNumber))
			{
				levelNumber++;
			}
			sortedLevels.Add(levelNumber, file);
		}
		
		List<string> processedLevels = new List<string>();
		
		for (int i = 0; i < sortedLevels.Count; i++)
		{
			processedLevels.Add((string)sortedLevels.GetByIndex(i));	
		}
		
		ExternalLevelSources = processedLevels.ToArray(); //
		
		*/
		
		ExternalLevelSources = System.IO.Directory.GetFiles(navigationPath, "*.xml");
		
		Array.Sort(ExternalLevelSources);
		
	}
	
	public string GetCurrentLevelPathFromExternalList()
	{
		if (ExternalLevelSources != null && ExternalLevelSources.Length > CurrentExternalLevelIndex)
		{
			return ExternalLevelSources[CurrentExternalLevelIndex];	
		}
		return "";
	}
	
	public string GetCurrentExternalLevelFilename()
	{
		string currentPath = GetCurrentLevelPathFromExternalList();
		string currentFile = currentPath.Substring(currentPath.LastIndexOf('/'));
		currentFile = currentFile.Trim('/');
		return currentFile;
	}
	
	public void ReportExternalLevelFinished(LoadType loadType, int explicitLevel = -1)
	{
		//TODO write to levelXML for participant that keeps track of progress
		//do this in a "Changed" directory rather than the root so that we do 
		//not overwrite new files with old ones during the file transfer process...

		if (explicitLevel >= 0) {
			CurrentExternalLevelIndex = explicitLevel;
		} else {
			switch (loadType) {
			case LoadType.Next:
				CurrentExternalLevelIndex++;
				break;
			case LoadType.Previous:
				CurrentExternalLevelIndex--;
				break;
			case LoadType.Repeat:
				//nothing to CurrentExternalLevelIndex...
				break;
				
			}
		}
	
		if (FFTGameManager.Instance.LogActions)
		{
			AppendLatestLevelToLog();
		}
		
		//should hopefully never have this happen...
		if (CurrentExternalLevelIndex >= ExternalLevelSources.Length)
			CurrentExternalLevelIndex = 0;
		if (CurrentExternalLevelIndex < 0)
			CurrentExternalLevelIndex = ExternalLevelSources.Length -1;
	}
	
	public string GetLevelLogName(System.DateTime dtn)
	{
		return GetLevelLogNameWithoutExtension(dtn) + "_" + LevelLogExtensionName;
	}
	
	public string GetLevelLogNameWithoutExtension(System.DateTime dtn)
	{
		string timeStamp = "";
		string dateString = dtn.Year.ToString().Substring(2) + "-" + dtn.Month.ToString("D2") + "-" + dtn.Day.ToString("D2");
        string currentTimeString = dtn.Hour.ToString("D2") + "." + dtn.Minute.ToString("D2") +"." + dtn.Second.ToString("D2");
        timeStamp = dateString + "_" + currentTimeString;
		
		string participantID = FFTGameManager.Instance.CurrentLevel.ParticipantID.ToString();
		
		return participantID + "_" + timeStamp;
		
	}
	
		
	public void OpenLevelLog()
	{
		return;

		if (!FFTGameManager.Instance.LogActions || LevelLogOpen)
			return;
		
		/*
		//LevelLogOpen = true;
		
		string rootDataPath = FFTDataManager.GetRootPath();
		//+ LevelProgressFolder;
		
		WorkingPath = rootDataPath + @"/" + FFTDataManager.DirectoryName + @"/" + LevelProgressWorkingFolder + @"/";
		WorkingPathBackup = WorkingPath + LevelProgressWorkingFolderBackup + @"/";
		
		if (!System.IO.Directory.Exists(WorkingPath))
        {
            System.IO.Directory.CreateDirectory(WorkingPath);
        }
		if (!System.IO.Directory.Exists(WorkingPathBackup))
        {
            System.IO.Directory.CreateDirectory(WorkingPathBackup);
        }
		
		*/
		
		LevelProgress = new XmlDocument();
		
		string[] levelLogs = System.IO.Directory.GetFiles(WorkingPath, "*.xml");
		
		string participantID = ParticipantID.ToString();
		
		//file format = PARTICIPANT_ID_YY.MM.DD_HH.MM.SS_LevelProgress.xml
		
		string latestFile = "";
		int latestDate = 0;
		int latestTime = 0;
		
		foreach (string fileName in levelLogs)
		{
			string file = fileName.Substring(fileName.LastIndexOf('/') + 1);
			if (file.Contains(participantID))
			{
				Debug.Log(file);
				string[] parts = file.Split('_');
				int daysProgressed = DaysProgressedFromDateString(parts[1]);
				int secondsProgressed = SecondsProgressedFromTimeString(parts[2]);
				if (daysProgressed > latestDate)
				{
					latestFile = fileName;
					latestDate = daysProgressed;
					latestTime = secondsProgressed;
				}
				else if (daysProgressed == latestDate)
				{
					if (secondsProgressed > latestTime)
					{
						latestFile = fileName;
						latestTime = secondsProgressed;
					}
				}
			}
			//Debug.Log(file);
		}
		Debug.Log("Latest file: " + latestFile);
		
		if (File.Exists(latestFile))
        {
			LevelProgress.Load(latestFile);
            
			//Now figure out which level we want to start with
			string levelToLoad = "";
			XmlNode latestLevel = LevelProgress.FirstChild.LastChild;
			
			foreach (XmlNode node in latestLevel.ChildNodes)
			{
				switch (node.Name)
				{
				case "sessionID":
					break;
				case "expCond":
					break;					
				case "levelAttempt":
					break;
				case "fileName":
					levelToLoad = node.InnerText;
					break;
				}
			}
			//Debug.Log("Latest level found:" + levelToLoad);
			for (int i=0; i < ExternalLevelSources.Length; i++)
			{
				string levelSource = ExternalLevelSources[i];
				if (levelSource.Contains(levelToLoad))
				{
					CurrentExternalLevelIndex = i;
					break;
				}
			}
        }
		else 
		{
			//we have no logs yet, so we'll create a new one
			
			XmlNode rootNode = LevelProgress.CreateElement("CogGameLevelProgress");
	        LevelProgress.AppendChild(rootNode);
			
			//LevelProgress.CreateNode( = new XmlDocument();
		}
		
		SaveLevelProgressToWorkingFileLocation();
		
	}
	
	void SaveLevelProgressToWorkingFileLocation()
	{
		return;

		if (File.Exists(WorkingFileLocation))
        {	
			System.IO.File.Copy(WorkingFileLocation, WorkingFileLocationBackup, true);
		}
		
		LevelProgress.Save(WorkingFileLocation);
	}
	
	public void CloseLevelLog()
	{
		return;

		SaveLevelProgressToWorkingFileLocation();
	}
	
	public void AppendLatestLevelToLog()
	{
		return;

		XmlDocumentFragment levelFrag = LevelProgress.CreateDocumentFragment();	
		string xmlData = "<level>";
		xmlData += "<sessionID>" + SessionID.ToString("D3") + "</sessionID>";
		xmlData += "<expCond>" + ParticipantGroup.ToString() + "</expCond>";
		xmlData += "<levelAttempt>" + FFTGameManager.Instance.LevelsAttempted.ToString("D3") + "</levelAttempt>";
		xmlData += "<fileName>" + GetCurrentExternalLevelFilename() + "</fileName>";
		xmlData += "</level>";
		
		levelFrag.InnerXml = xmlData;
		
		LevelProgress.DocumentElement.AppendChild(levelFrag);
		
		SaveLevelProgressToWorkingFileLocation();
		
	}
	
	static int DaysProgressedFromDateString(string date)
	{
		int result = 0;
		string[] dateParts = date.Split('-');
		int year = int.Parse(dateParts[0]);
		int month = int.Parse(dateParts[1]);
		int day = int.Parse(dateParts[2]);
		
		result += year * 365;
		result += month * 31;
		result += day;
		
		return result;
		
	}
	
	static int SecondsProgressedFromTimeString(string time)
	{
		int result = 0;
		string[] timeParts = time.Split('.');
		int hours = int.Parse(timeParts[0]);
		int minutes = int.Parse(timeParts[1]);
		int seconds = int.Parse(timeParts[2]);
		
		result += hours * 60 * 60;
		result += minutes * 60;
		result += seconds;
		
		return result;
		
	}
	
	/*
	 
	public static T[] GetAtPath<T> (string path) {

        ArrayList al = new ArrayList();
        string [] fileEntries = Directory.GetFiles(Application.dataPath+"/"+path);
        foreach(string fileName in fileEntries)
        {
            int index = fileName.LastIndexOf("/");
            string localPath = "Assets/" + path;
            if (index > 0)
                localPath += fileName.Substring(index);
            Object t = Resources.LoadAssetAtPath(localPath, typeof(T));
            if(t != null)
                al.Add(t);
        }

        T[] result = new T[al.Count];

        for(int i=0;i<al.Count;i++)
            result[i] = (T)al[i];

        return result;
    }
    */
	
		#region Archival of Materials
	
	public void CleanParticipantLevelProgress()
	{
		//REMOVE WHEN READY TO USE FUNCTION
		return;
		
		string[] levelLogs = System.IO.Directory.GetFiles(WorkingPath, "*.xml");
		string[] tempLogs = System.IO.Directory.GetFiles(WorkingPathBackup, "*.xml");
		
		Dictionary<int, string> latestLogs = new Dictionary<int, string>();
		Dictionary<int, string> latestTempLogs = new Dictionary<int, string>();
		
		List<string> filesToArchive = new List<string>();
		List<string> unknownFilesToMove = new List<string>();
		
		foreach (string path in levelLogs)
		{
			int participantID = participantIDFromLevelProgressXMLPath(path);
			
			if (participantID == -1) //error in processing file, unknown filename
			{
				Debug.Log("PID not found in " + path);
				unknownFilesToMove.Add(path);
			}
			else
			{
				if (latestLogs.ContainsKey(participantID))
				{
					//this ID exists, so replace current 'latest' file with this one	
					filesToArchive.Add(latestLogs[participantID]);
					latestLogs[participantID] = path;
				}
				else
				{
					//this ID has not been seen yet, so this is our current 'latest' file	
					latestLogs.Add(participantID, path);
				}
			}
		}
		
		foreach (string tPath in tempLogs)
		{
			//we check for participantID in the dictionary, if it does not exist, we "hold on" to the latest file
			//utilizing a separate dictionary.
			int participantID = participantIDFromLevelProgressXMLPath(tPath);
			if (participantID < 0)
				continue;
			if (latestLogs.ContainsKey(participantID) || participantID == 0)
				continue;
			else
			{
				if (latestTempLogs.ContainsKey(participantID))
				{
					//this ID exists, so replace current 'latest' file with this one	
					latestTempLogs[participantID] = tPath;
				}
				else
				{
					//this ID has not been seen yet, so this is our current 'latest' file	
					latestTempLogs.Add(participantID, tPath);
				}
				
			}
		}
		
		//TODO Move all of the filesToArchive to "Archive", unknownFilesToMove to "Error"
		//TODO Move all of the latestTempLogs to the main directory before we "flush"
		//TODO Verify the remaining files are the ones listed in latestLogs, abort if they don't match
		
		//with the latest files safely secured, flush the temporary directory
		System.IO.Directory.Delete(WorkingPathBackup, true);
	}
				
	int participantIDFromLevelProgressXMLPath(string path)
	{
		string file = path.Substring(path.LastIndexOf('/') + 1);
		string participantIDString = file.Substring(0, file.IndexOf('_'));
		int participantID = -1;
		int.TryParse(participantIDString, out participantID);
		return participantID;
	}
	
	#endregion
	
}
