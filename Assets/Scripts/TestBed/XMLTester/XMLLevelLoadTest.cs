using UnityEngine;

using System;
using System.Collections;
using System.IO;
using System.Xml;

public class XMLLevelLoadTest : MonoBehaviour {

    public string title;
    public int intValue;
    public string fileName;

    public bool xmlWritten = false;

	// Use this for initialization
	void Start () 
    {
        SaveLevelXML();
	}
	
	// Update is called once per frame
	void Update () 
    {
        Debug.Log(Application.persistentDataPath);
        Debug.Log(Application.dataPath);
	
	}

    void LoadLevelXML()
    {
        string path = Application.dataPath + @"/Levels/";

        if (fileName.Contains(".xml"))
        {
            path += fileName;
        }
        else
        {
            path += fileName + @".xml";
        }

        StreamReader sr = File.OpenText(path);

        XmlDocument levelXml = new XmlDocument();
        levelXml.Load(sr);

        XmlNodeList stepList = levelXml.GetElementsByTagName("Step");

        foreach (XmlNode step in stepList)
        {
            title = step["Title"].InnerText;
            if (Int32.TryParse(step["IntValue"].InnerText, out intValue))
            {
                //nothing
            }
            else
            {
                intValue = 0;
            }



        }
    }


    void SaveLevelXML()
    {
        string path = Application.dataPath + @"/Levels/" + fileName + @".xml";

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        //System.IO.File.CreateText(path);


        XmlDocument levelXml = new XmlDocument();
        XmlNode rootNode = levelXml.CreateElement("recipe");
        levelXml.AppendChild(rootNode);

        XmlNode stepNode = levelXml.CreateElement("step");
        XmlAttribute stepNumberAttribute = levelXml.CreateAttribute("number");
        stepNumberAttribute.Value = "1";
        stepNode.Attributes.Append(stepNumberAttribute);

        XmlNodeInnerText(ref levelXml, ref stepNode, "title", title);
        XmlNodeInnerText(ref levelXml, ref stepNode, "intValue", intValue.ToString());

        rootNode.AppendChild(stepNode);

        levelXml.Save(path);

        /*
        using (StringWriter str = new StringWriter())
        using (XmlTextWriter xml = new XmlTextWriter(str))
        {
            xml.WriteStartDocument();
            xml.WriteWhitespace("\n");
            xml.WriteStartElement("Recipe");
            xml.WriteWhitespace("\n");

            xml.WriteStartElement("Step");
            xml.WriteWhitespace("\n");
            {
                xml.WriteElementString("Title", title);
                xml.WriteWhitespace("\n");
                xml.WriteElementString("IntValue", intValue.ToString());
                xml.WriteWhitespace("\n");
            }
            xml.WriteEndElement();
            xml.WriteWhitespace("\n");

            xml.WriteEndElement(); //Recipe
            xml.WriteEndDocument();

            TextWriter textStream = File.CreateText(path);
            textStream.WriteLine(str);
            textStream.Close();

        }
         */

        /*
        XmlDocument LevelXml = new XmlDocument();

        LevelXml.CreateXmlDeclaration("1.0", String.Empty, "yes");
        XmlElement recipeNode = LevelXml.CreateElement("Recipe");
        XmlAttribute recipeID = LevelXml.CreateAttribute("IntValue");
        recipeID.Value = intValue.ToString();
        recipeNode.SetAttributeNode(recipeID);
        XmlElement recipeTitle = LevelXml.CreateElement("Title");
        recipeTitle.InnerText = title;
        recipeNode.AppendChild(recipeTitle);

        StreamWriter outStream = System.IO.File.CreateText(path);

        LevelXml.Save(outStream);

        outStream.Close();
         */

        xmlWritten = true;
        
    }

    private void XmlNodeInnerText(ref XmlDocument xmldoc, ref XmlNode parentNode, string name, string value)
    {
        XmlNode newNode = xmldoc.CreateElement(name);
        newNode.InnerText = value;
        parentNode.AppendChild(newNode);
    }

    private void SaveFile(string fullPath, string contents)
    {
        using (StreamWriter writer = File.CreateText(fullPath))
        {
            writer.Write(contents);
        }

    }

    private string LoadFile(string fullPath)
    {
        string output = "";

        try
        {
            using (StreamReader reader = File.OpenText(fullPath))
            {
                output = reader.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Debug.Log("The file could not be opened.");
            Debug.Log(e.Message);
        }

        return output;
    }
}
