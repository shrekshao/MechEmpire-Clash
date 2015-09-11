using UnityEngine;
using System.Collections;
using System;

public class TextFileFinder : MonoBehaviour {

	public event EventHandler RecordLoaded;

	protected string m_textPath;
	
	protected FileBrowser m_fileBrowser;

	public RecordManager recordManager;
	//public string m_content;

	[SerializeField]
	protected GUISkin customSkin;
	[SerializeField]
	protected Texture2D	m_directoryImage,
	m_fileImage;


	
	protected void OnGUI () {
		GUI.skin = customSkin ;
		if (m_fileBrowser != null) {
			m_fileBrowser.OnGUI();
		} else {
			OnGUIMain();
		}
	}
	
	protected void OnGUIMain() {
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Text File", GUILayout.Width(100));
		GUILayout.FlexibleSpace();
		GUILayout.Label(m_textPath ?? "none selected");
		//GUILayout.TextArea (m_content ?? "NULL");
		if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) {
			m_fileBrowser = new FileBrowser(
				new Rect(100, 100, 600, 500),
				"选择战斗录像txt",
				FileSelectedCallback
				);
			m_fileBrowser.SelectionPattern = "*.txt";
			m_fileBrowser.DirectoryImage = m_directoryImage;
			m_fileBrowser.FileImage = m_fileImage;
		}
		GUILayout.EndHorizontal();
	}
	
	protected void FileSelectedCallback(string path) {
		m_fileBrowser = null;
		m_textPath = path;



		FileReadWriteManager fileManager = new FileReadWriteManager();
		string content = fileManager.ReadTextFile (m_textPath);

		recordManager = new RecordManager(content);

		//Console.WriteLine("finish reading...");


		//dispatch events
		if (RecordLoaded != null)
		{
			RecordLoaded(this, EventArgs.Empty);
		}

		Destroy(this);
	}
}