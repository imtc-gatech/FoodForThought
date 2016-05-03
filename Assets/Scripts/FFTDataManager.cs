using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
//using Ionic.Zip;

//using System.Data;
//using System.Xml.Linq;
//this include will be needed to parse the resultant logs in the future

public class FFTDataManager : MonoBehaviour {
	
	public static string DirectoryName = @"ParticipantLogs";
	public static string extFrag = @".xml";
	public static string extMain = @".xml";
	
	public bool Initialized = false;
	public string DataPath = "";
	public string XmlMasterFilename = ""; //.xml
	
	public string XmlFragmentInfo = "info"; //.xmlpart
	public string XmlFragmentPlanning = "planning"; //.xmlpart
	public string XmlFragmentGameplay = "gameplay"; //.xmlpart
	public string XmlFragmentActions = "actions"; //.xmlpart
	public string XmlFragmentResults = "results"; //.xmlpart
	public string XmlFragmentDistractions = "distractions"; //.xmlpart
	
	public string LevelProgressID = "";
	public DateTime CurrentDateTime;
	
	//variables for "deep logging"
	
	public int currentArrayIndex = 0;
	static int DataArraySize = 64;
	
	public FFTStepAction[] ActionChunksTaken;
	public FFTStepAction[] WorkingActionChunk = new FFTStepAction[DataArraySize];
	
	public Queue<FFTStepAction> PostponedActions = new Queue<FFTStepAction>(); 
	
	public bool dataLocked = false;
	public int chunkcount = 0;

	// Use this for initialization
	void Start () {
		WorkingActionChunk = new FFTStepAction[DataArraySize];
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}
	
	void OnDestroy() {
		//TODO: make sure all files are updated appropriately when this object is destroyed	
	}
	
	public void AddAction(FFTStepAction action)
	{
		//Debug.Log("addAction:" + action.SecondsSinceLevelLoad);
		if (dataLocked)
		{
			PostponedActions.Enqueue(action);
			return;
		}
		dataLocked = true;
		WorkingActionChunk[currentArrayIndex] = action;
		currentArrayIndex++;
		if (currentArrayIndex >= DataArraySize)
		{
			ClearDataArray();
			currentArrayIndex = 0;
		}
		dataLocked = false;
			
		//TODO: Sort the data into "buckets" as it is received.
		//ActionsTaken.Add(action);
	}
	
	void ClearDataArray()
	{
		if (ActionChunksTaken == null || ActionChunksTaken.Length == 0)
			ActionChunksTaken = WorkingActionChunk;
		else
		{
			FFTStepAction[]ConcatChunks = new FFTStepAction[ActionChunksTaken.Length + WorkingActionChunk.Length];
			ActionChunksTaken.CopyTo(ConcatChunks, 0);
			WorkingActionChunk.CopyTo(ConcatChunks, ActionChunksTaken.Length);
			ActionChunksTaken = ConcatChunks;
		}
		chunkcount++;
		WorkingActionChunk = null;
		WorkingActionChunk = new FFTStepAction[DataArraySize];
	}
	
	void ProcessPostponedActions()
	{
		if (dataLocked)
			return;
		while (PostponedActions.Peek() != null)
		{
			if (!dataLocked)
				AddAction(PostponedActions.Dequeue());		
		}
	}
	
	//using http://stackoverflow.com/questions/6436562/how-to-append-data-to-xml-file-using-c-sharp
	
	/// <summary>
	/// Create our parent XML file as well as any needed fragments.
	/// </summary>
	public void CreateFiles()
    {
		if (Initialized)
		{
			Debug.Log("ERROR: Attempted to create XML logs more than once. FFTDataManager should be created at the beginning of each level.");
			return;
		}
		
		// formatting to prevent overwriting previously created recipes by accident
        string timeStampString;
		CurrentDateTime = FFTGameManager.Instance.CurrentLevel.TimeInitialized;
        System.DateTime dtn = CurrentDateTime;
        string dateString = dtn.Year.ToString().Substring(2) + "-" + dtn.Month.ToString("D2") + "-" + dtn.Day.ToString("D2");
        string currentTimeString = dtn.Hour.ToString("D2") + "." + dtn.Minute.ToString("D2") +"." + dtn.Second.ToString("D2");
		
		LevelProgressID = FFTGameManager.Instance.LevelsAttempted.ToString("D3");
	
		string levelTitle = FFTGameManager.Instance.CurrentLevel.TitleofLevel;
        
        timeStampString = dateString + "_" + currentTimeString;

        int participantID = FFTGameManager.Instance.CurrentLevel.ParticipantID;
        string experimentalCondition = FFTGameManager.Instance.CurrentLevel.ParticipantGroup.ToString();
        int sessionID = FFTGameManager.Instance.CurrentLevel.SessionID;
		
		string directory, path;
		
		directory = GetRootPath();
		
		string participantDirectoryName = participantID.ToString() + experimentalCondition;
		string sessionDirectoryName = sessionID.ToString("D3") + "_" + dateString;
		
		directory += @"/" + DirectoryName + @"/";
		
		directory += @"/" + participantDirectoryName + @"/";
		
		directory += @"/" + sessionDirectoryName + @"/";
		
		XmlMasterFilename = participantDirectoryName + "_" + sessionDirectoryName + "_" + 
			currentTimeString + "_" + LevelProgressID + "_" + levelTitle;
		
		DataPath = directory; 
		
		if (!System.IO.Directory.Exists(DataPath))
        {
            System.IO.Directory.CreateDirectory(DataPath);
        }
		
		//we will create these "as needed" so we can create placeholders if they do not exist on level end (if level skips, etc)
		/*
		CreateEmptyFileFragment(XmlFragmentPlanning);
		CreateEmptyFileFragment(XmlFragmentGameplay);
		CreateEmptyFileFragment(XmlFragmentActions);
		CreateEmptyFileFragment(XmlFragmentResults);
		*/
		
		//CreateMasterXmlFile(XmlMasterFilename);
		
		Initialized = true;
    }
	
	public static string GetRootPath()
	{
		string directory = Application.dataPath;
		
		//Debug.Log(System.IO.Directory.GetParent(Application.dataPath).ToString());
		
		if (!Application.isEditor)
		{
			directory = System.IO.Directory.GetParent(directory).ToString();
			
			//if OSX, go one directory higher (Applications/RootFolder/ instead of Applications/RootFolder/FFT_Package/
			if (Application.platform == RuntimePlatform.OSXPlayer)
				directory = System.IO.Directory.GetParent(directory).ToString();		
		}
		else 
		{
			if (FFTGameManager.Instance.UseDebugLogDirectory) //avoids rebuild of solution every time you log data
			{
				directory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			}
			else
			{
				
			}
		}
		
		return directory;
	}
	
	public void AppendGameAction(FFTStepAction action)
    {
		/*
        var keyEvent = new XElement("KeyEvent",
            new XElement("Key", new XAttribute("key", key.Key.ToString())), 
            new XElement("At", DateTime.Now.ToString())
            );
        File.AppendAllText("slave.xmlpart", keyEvent.ToString());
        */
    }
	

	
	string GetPathFromFragmentName(string fragment)
	{
		return DataPath + @"/" + XmlMasterFilename + "--" + fragment + extFrag;	
	}
	

	
	
	#region Unused Fragment Methods
	void CreateFragmentReference(ref XmlDocument doc, ref XmlNode parent, string fragmentName)
	{
		//xmlns:xi="http://www.w3.org/2001/XInclude"
		
	}
	
	void CreateEmptyFileFragment(string fragmentFilename)
	{
		string path = GetPathFromFragmentName(fragmentFilename);
		if (!File.Exists(path))
        {
            File.WriteAllText(path, ""); // simply create the file
        }
	}
	#endregion
	
	void CreateMasterXmlFile(string masterFilename)
	{
		DateTime dtn = CurrentDateTime;
		FFTGameManager GM = FFTGameManager.Instance; //convenience 
		string path = DataPath + masterFilename + extMain;
		
		if (!File.Exists(path))
        {
			XmlDocument levelXml = new XmlDocument();
        	{
	            XmlNode rootNode = levelXml.CreateElement("CogGameLog");
	            levelXml.AppendChild(rootNode);
				//CreateInfoNode(ref levelXml, ref rootNode, dtn);
				
				//Info, Planning, Gameplay, Actions, Results to be added here.
			}
			
			levelXml.Save(path);
			
			
			//Aborted Attempts that are Unsupported in Unity
			/*
			//Aborted Suggestion (no System.Data)
			XmlTextReader info = new XmlTextReader(GetPathFromFragmentName(XmlFragmentInfo));
			XmlTextReader gameplay = new XmlTextReader(GetPathFromFragmentName(XmlFragmentGameplay));
	
			DataSet dsInfo = new DataSet();
			dsInfo.ReadXml(info);
			DataSet dsGameplay = new DataSet();
			dsGameplay.ReadXml(gameplay);
			
			dsInfo.Merge(dsGameplay);
			dsInfo.WriteXml(path);
			
			//XDocument code (no Xml.Linq in Unity :-( )
			
	        XDocument doc = new XDocument(
				new XElement("Info",
					new XElement("Participant",
						new XAttribute("ParticipantID", GM.CurrentLevel.ParticipantID.ToString()),
						new XAttribute("SessionID", GM.CurrentLevel.SessionID.ToString()),
						new XAttribute("ExperimentalCondition", GM.CurrentLevel.Condition.ToString())
						),
					new XElement("Level",
						new XAttribute("Number", GM.LevelsAttempted),
						new XAttribute("Title", GM.CurrentLevel.TitleofLevel)
						),
					new XElement("Time",
						new XAttribute("TimestampInSeconds", ConvertDateTimeToSecondsTimestamp(dtn))
						)
				),
	            new XElement("Planning",
	            	new XElement("{http://www.w3.org/2001/XInclude}include",
	                    new XAttribute("href", (XmlFragmentPlanning + extFrag))
	                    )
	            ),
				new XElement("Gameplay",
	            	new XElement("{http://www.w3.org/2001/XInclude}include",
	                    new XAttribute("href", (XmlFragmentGameplay + extFrag))
	                    )
	            ),
				new XElement("Actions",
	            	new XElement("{http://www.w3.org/2001/XInclude}include",
	                    new XAttribute("href", (XmlFragmentActions + extFrag))
	                    )
	            ),
				new XElement("Planning",
	            	new XElement("{http://www.w3.org/2001/XInclude}include",
	                    new XAttribute("href", (XmlFragmentResults + extFrag))
	                    )
	            )
	        );
	        doc.Save(DataPath + masterFilename + extMain);
	        */
        }
	}
	
	#region Fragment INFO 
	public void CreateInfoFragment()
	{
		DateTime dtn = CurrentDateTime;
		
		FFTGameManager GM = FFTGameManager.Instance; //convenience
		
		XmlDocument doc = new XmlDocument();
		
		XmlNode root = doc.CreateElement("Info");
		doc.AppendChild(root);
		{
			XmlNode participant = doc.CreateElement("Participant");
        	root.AppendChild(participant);
			XmlNodeAppendAttribute(ref doc, ref participant, "ParticipantID", GM.CurrentLevel.ParticipantID.ToString());
			XmlNodeAppendAttribute(ref doc, ref participant, "SessionID", GM.CurrentLevel.SessionID.ToString());
			XmlNodeAppendAttribute(ref doc, ref participant, "ExperimentalCondition", GM.CurrentLevel.Condition.ToString());
		}
		{
			XmlNode level = doc.CreateElement("Level");
        	root.AppendChild(level);
			XmlNodeAppendAttribute(ref doc, ref level, "Number", GM.LevelsAttempted.ToString());
			XmlNodeAppendAttribute(ref doc, ref level, "Title", GM.CurrentLevel.TitleofLevel);
		}
		{
			XmlNode time = doc.CreateElement("Time");
        	root.AppendChild(time);
			XmlNodeAppendAttribute(ref doc, ref time, "TimestampInSeconds", ConvertDateTimeToSecondsTimestamp(dtn));
		}
		
		doc.Save(GetPathFromFragmentName(XmlFragmentInfo));
		
	}
	#endregion
	
	#region Fragment LEVEL OVERVIEW
	public void CreateLevelOverviewFragment()
	{
		if (FFTGameManager.Instance.State < FFTGameManager.GameState.Results)
			return;
		XmlDocument levelXml = new XmlDocument();
		
		XmlNode levelOverviewNode = levelXml.CreateElement("LevelOverview");
		levelXml.AppendChild(levelOverviewNode);
		
		XmlNodeAppendAttribute(ref levelXml, ref levelOverviewNode, "Title", FFTGameManager.Instance.CurrentLevel.TitleofLevel);

        XmlNode timeNode = levelXml.CreateElement("TimeElapsed");
        levelOverviewNode.AppendChild(timeNode);
		
		XmlNodeAppendAttribute(ref levelXml, ref timeNode, "Planning", FFTGameManager.Instance.LevelPlanningElapsedTime.ToString());
        XmlNodeAppendAttribute(ref levelXml, ref timeNode, "Gameplay", FFTGameManager.Instance.LevelGameplayElapsedTime.ToString());
		XmlNodeAppendAttribute(ref levelXml, ref timeNode, "Review", FFTGameManager.Instance.LevelReviewElapsedTime.ToString());
        XmlNodeAppendAttribute(ref levelXml, ref timeNode, "Total", FFTGameManager.Instance.SessionElapsedTime.ToString());
		XmlNodeAppendAttribute(ref levelXml, ref timeNode, "IsTimedLevel", FFTGameManager.Instance.CurrentLevel.TimedLevel.ToString());
        XmlNodeAppendAttribute(ref levelXml, ref timeNode, "TimeLimit", FFTGameManager.Instance.CurrentLevel.TimeLimit.ToString());
		
		/*
		XmlNodeAppendAttribute(ref levelXml, ref timeNode, "PlanningTimeElapsed", FFTGameManager.Instance.LevelPlanningElapsedTime.ToString());
        XmlNodeAppendAttribute(ref levelXml, ref timeNode, "LevelTimeElapsed", FFTGameManager.Instance.LevelGameplayElapsedTime.ToString());
        XmlNodeAppendAttribute(ref levelXml, ref timeNode, "TotalTimeElapsed", FFTGameManager.Instance.SessionElapsedTime.ToString());
		XmlNodeAppendAttribute(ref levelXml, ref timeNode, "IsTimedLevel", FFTGameManager.Instance.CurrentLevel.TimedLevel.ToString());
        XmlNodeAppendAttribute(ref levelXml, ref timeNode, "TimeLimit", FFTGameManager.Instance.CurrentLevel.TimeLimit.ToString());
		XmlNodeAppendAttribute(ref levelXml, ref timeNode, "ReviewTimeElapsed", FFTGameManager.Instance.LevelReviewElapsedTime.ToString());		
		 */
		if (FFTGameManager.Instance.ApplicationEnding)
		{
			XmlNodeAppendAttribute(ref levelXml, ref timeNode, "LastLevelOfSession", FFTGameManager.Instance.ApplicationEnding.ToString());		
		}
		
        XmlNode scoreNode = levelXml.CreateElement("Scoring");
        levelOverviewNode.AppendChild(scoreNode);

        XmlNodeAppendAttribute(ref levelXml, ref scoreNode, "TotalStars", FFTGameManager.Instance.CurrentScore.RecipeTotalStarRating().ToString());
        //XmlNodeAppendAttribute(ref levelXml, ref scoreNode, "levelName", FFTGameManager.Instance.CurrentLevel.LevelSource.name);
		
		//string levelPath = FFTGameManager.Instance.CurrentLevel.GetCurrentLevelPathFromExternalList();
		//levelPath = levelPath.Substring(levelPath.IndexOf("LevelData"));

        XmlNode detailNode = levelXml.CreateElement("Detail");
        scoreNode.AppendChild(detailNode);

        foreach (KeyValuePair<string, List<FFTStepReport>> pair in FFTGameManager.Instance.CurrentScore.Dict)
        {
            XmlNode dishNode = levelXml.CreateElement("Dish");
            XmlNodeAppendAttribute(ref levelXml, ref dishNode, "Name", pair.Key);
            detailNode.AppendChild(dishNode);
            int i = 1;
            List<FFTStepReport> reports = pair.Value;
            foreach (FFTStepReport report in reports)
            {
                XmlNode stepNode = levelXml.CreateElement("Step");
                XmlNodeAppendAttribute(ref levelXml, ref stepNode, "ID", i.ToString());
                XmlNodeAppendAttribute(ref levelXml, ref stepNode, "Stars", report.StarRating.ToString());
                XmlNodeAppendAttribute(ref levelXml, ref stepNode, "FeedbackText", report.Feedback.ToString());
                dishNode.AppendChild(stepNode);
                i++;
            }
        }
		levelXml.Save(GetPathFromFragmentName(XmlFragmentGameplay));
	}
	#endregion
	
	#region Fragment MOUSE ACTIONS
	public void CreateActionsFragment()
	{
		ClearDataArray();
		
		XmlDocument doc = new XmlDocument();
		
		XmlNode root = doc.CreateElement("Actions");
		doc.AppendChild(root);
		
		XmlNode mouse = doc.CreateElement("Mouse");
		root.AppendChild(mouse);
		
		XmlNode planning = doc.CreateElement("Planning");
		mouse.AppendChild(planning);
		XmlNode gameplay = doc.CreateElement("Gameplay");
		mouse.AppendChild(gameplay);
		XmlNode results = doc.CreateElement("Results");
		mouse.AppendChild(results);
		XmlNode other = doc.CreateElement("Other");
		mouse.AppendChild(other);
		
		foreach (FFTStepAction action in ActionChunksTaken)
		{
			if (action == null)
				continue;
			XmlNode actNode = doc.CreateElement("Event");
			switch (action.GameState)
			{
				case FFTGameManager.GameState.Planning:
					planning.AppendChild(actNode);
					break;
				case FFTGameManager.GameState.Gameplay:
					gameplay.AppendChild(actNode);
					break;
				case FFTGameManager.GameState.Results:
					results.AppendChild(actNode);
					break;
				default:
					other.AppendChild(actNode);
					XmlNodeAppendAttribute(ref doc, ref actNode, "State", action.GameState.ToString());
					break;
			}
			XmlNodeAppendAttribute(ref doc, ref actNode, "Time", action.SecondsSinceLevelLoad.ToString());
			
			XmlNode typeNode = doc.CreateElement(action.Type.ToString());
			actNode.AppendChild(typeNode);
			
			switch (action.Type)
			{
				case FFTStepAction.ActionType.Slot:
					XmlNodeAppendAttribute(ref doc, ref typeNode, "Type", action.Slot_TypeOfSlot.ToString());
					XmlNodeAppendAttribute(ref doc, ref typeNode, "Interaction", action.Slot_InteractionType.ToString());
					XmlNodeAppendAttribute(ref doc, ref typeNode, "Action", action.Slot_ActionTaken.ToString());
					switch (action.Slot_TypeOfSlot)
					{
					case FFTSlot.SlotType.Counter:
						
						break;
					case FFTSlot.SlotType.Station:
						XmlNodeAppendAttribute(ref doc, ref typeNode, "Station", action.Slot_Station_Type.ToString());
						XmlNodeAppendAttribute(ref doc, ref typeNode, "Number", action.Slot_Station_SlotID.ToString());
						break;
					case FFTSlot.SlotType.Results:
					
						break;
					}
					
					
					if (action.AssociatedWithGameAction)
					{
						XmlNode gameAction = doc.CreateElement("Association");
						typeNode.AppendChild(gameAction);
						XmlNodeAppendAttribute(ref doc, ref gameAction, "UID", action.DishUID.ToString());
						XmlNodeAppendAttribute(ref doc, ref gameAction, "Dish", action.DishID.ToString());
						XmlNodeAppendAttribute(ref doc, ref gameAction, "Step", (action.StepID + 1).ToString());
						
					}
					
					break;
				case FFTStepAction.ActionType.Button:
					XmlNodeAppendAttribute(ref doc, ref typeNode, "Interaction", action.Slot_InteractionType.ToString());
					XmlNodeAppendAttribute(ref doc, ref typeNode, "Action", action.Slot_ActionTaken.ToString());
					break;
				case FFTStepAction.ActionType.Empty:
					break;
			}
		}
		
		doc.Save(GetPathFromFragmentName(XmlFragmentActions));
	}
	#endregion
	
	#region Fragment DISTRACTIONS 
	public void CreateDistractionFragment(List<FFTDistractionAction> actions)
	{	
		Debug.Log ("Distraction Actions Logged");
		XmlDocument doc = new XmlDocument();
		
		XmlNode root = doc.CreateElement("Distractions");
		doc.AppendChild(root);
		
		foreach (FFTDistractionAction action in actions)
		{
			XmlNode actNode = doc.CreateElement(action.actionType.ToString());
			root.AppendChild(actNode);
			switch (action.actionType)
			{
			case FFTDistractionAction.Type.Button:
				XmlNodeAppendAttribute(ref doc, ref actNode, "timestamp", action.timestamp.ToString());
				XmlNodeAppendAttribute(ref doc, ref actNode, "type", action.distractionType.ToString());
				XmlNodeAppendAttribute(ref doc, ref actNode, "activated", action.enabled.ToString());
				XmlNodeAppendAttribute(ref doc, ref actNode, "instanceID", action.instanceID.ToString ());
				break;
			case FFTDistractionAction.Type.Distraction:
				XmlNodeAppendAttribute(ref doc, ref actNode, "timestamp", action.timestamp.ToString());
				XmlNodeAppendAttribute(ref doc, ref actNode, "type", action.distractionType.ToString());
				XmlNodeAppendAttribute(ref doc, ref actNode, "activated", action.enabled.ToString());
				XmlNodeAppendAttribute(ref doc, ref actNode, "instanceID", action.instanceID.ToString ());
				break;
			case FFTDistractionAction.Type.LevelFinished:
				//only used when the level ends and any remaining distractions have been cleared out... allows seeing if a player finished the level with distractions active or not
				XmlNodeAppendAttribute(ref doc, ref actNode, "timestamp", action.timestamp.ToString());
				break;
			}
			 
			//if button, this denotes whether the game switched to that distraction hunting mode
			//if distraction, this denotes whether it was spawned or it was destroyed
			//destroyed distractions should be ignored 
			
		}
		
		doc.Save(GetPathFromFragmentName(XmlFragmentDistractions));
	}
	#endregion
	
	#region Convenience Methods For XML
	public static string ConvertDateTimeToSecondsTimestamp(DateTime dtn)
	{
		int timeStampInSeconds = dtn.Second + (dtn.Minute * 60) + (dtn.Hour * 60 * 60);
		return timeStampInSeconds.ToString("D4");
	}
	
	/// <summary>
    /// Creates a new XML child node with a passed in name and value.
    /// </summary>
    /// <param name="xmldoc">Parent XML Document</param>
    /// <param name="parentNode">Parent XML Node that will be given the new child.</param>
    /// <param name="name">Name/Label of the new XML Node.</param>
    /// <param name="value">Value of the new XML Node.</param>
    private static void XmlNodeNewChildWithInnerText(ref XmlDocument xmldoc, ref XmlNode parentNode, string name, string value)
    {
        XmlNode newNode = xmldoc.CreateElement(name);
        newNode.InnerText = value;
        parentNode.AppendChild(newNode);
    }

    /// <summary>
    /// Appends a new XML Attribute to an existing XML Node.
    /// </summary>
    /// <param name="xmldoc">Parent XML Document</param>
    /// <param name="node">XML Node to append the XML Attribute to.</param>
    /// <param name="name">Name/Label of the new XML Attribute.</param>
    /// <param name="value">Value of the new XML Attribute.</param>
    private static void XmlNodeAppendAttribute(ref XmlDocument xmldoc, ref XmlNode node, string name, string value)
    {
        XmlAttribute attribute = xmldoc.CreateAttribute(name);
        attribute.Value = value;
        node.Attributes.Append(attribute);
    }
	#endregion

	#region Cleanup for DataLogging Directories
	
	public static void ArchiveAllData()
	{
		CreateDataArchiveBackup();
		CreateDailyParticipantLogsArchive();
	}
	
	public static void CreateDataArchiveBackup()
	{
		/*
		using (ZipFile zip = new ZipFile())
        {
			string outputDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			
			string dataPath = GetRootPath() + @"/" + DirectoryName + @"/";
			
			System.DateTime dtn = System.DateTime.Now;
			
			string date = dtn.Month.ToString() + "-" + dtn.Day.ToString() + "-" + dtn.Year.ToString() + "_" + dtn.Hour.ToString() + "." + dtn.Minute.ToString();
			
			zip.AddDirectory(dataPath, "ParticipantLogs");
    		zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G") ; 
    		zip.Save(outputDirectory + @"/" + "FullDataBackup" + "_" + date + ".zip");
        }	
		
		Debug.Log("Data Archive created.");
		
*/
	}
	
	public static void CreateDailyParticipantLogsArchive()
	{
		/*
		using (ZipFile zip = new ZipFile())
        {
			string outputDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			
			string dataPath = FFTGameManager.Instance.CurrentLevel.WorkingPath + @"/";
			
			System.DateTime dtn = System.DateTime.Now;
			
			string date = dtn.Month.ToString() + "-" + dtn.Day.ToString() + "-" + dtn.Year.ToString() + "_" + dtn.Hour.ToString() + "." + dtn.Minute.ToString();
			
			zip.AddDirectory(dataPath, "ParticipantLevelProgress");
    		zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G") ; 
    		zip.Save(outputDirectory + @"/" + "ParticipantLevelProgress" + "_" + date + ".zip");
        }	
		
		Debug.Log("Participant Level Progress Archive created.");
		*/
	}
	
	public static void ExpandCompressedParticipantLevelProgress()
	{
		/*
		string dataPath = FFTGameManager.Instance.CurrentLevel.WorkingPath + @"/";
		string parentPath = System.IO.Directory.GetParent(dataPath).ToString();
		string parentParentPath = System.IO.Directory.GetParent(parentPath).ToString();
		string[] participantZips = System.IO.Directory.GetFiles(dataPath, "*.zip");
		if (participantZips.Length < 1)
			return;
		
		foreach (string zipFile in participantZips)
		{
			Debug.Log(zipFile);
			using (ZipFile zip = ZipFile.Read(zipFile))
			{
				zip.ExtractAll(parentParentPath, ExtractExistingFileAction.OverwriteSilently);
			}
			
		}
		*/
	}
	
	#endregion
}
