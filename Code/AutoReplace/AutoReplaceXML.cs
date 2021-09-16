using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using ColossalFramework;


namespace RON
{

	[XmlRoot("RONAutoReplace")]
	public class AutoReplaceXML
	{
		internal static readonly string ReplacementFileName = "RON-replacements.xml";

		// List of auto-replacements
		[XmlArray("autoreplace")]
		[XmlArrayItem("replace")]
		public List<ReplaceEntry> AutoReplacements { get; set; }


		/// <summary>
		/// Load auto-replace configuration from XML file.
		/// </summary>
		internal static AutoReplaceXML LoadSettings()
		{
			Logging.Message("loading auto-replace settings");

			try
			{
				// Get the current assembly path and append our auto-replace settings file name.
				string assemblyPath = ModUtils.GetAssemblyPath();
				if (!assemblyPath.IsNullOrWhiteSpace())
				{
					string fileName = Path.Combine(assemblyPath, ReplacementFileName);

					// Check to see if configuration file exists.
					if (File.Exists(fileName))
					{
						// Read it.
						using (StreamReader reader = new StreamReader(fileName))
						{
							XmlSerializer xmlSerializer = new XmlSerializer(typeof(AutoReplaceXML));
							if ((xmlSerializer.Deserialize(reader) is AutoReplaceXML autoReplaceFile))
							{
								// Successful read.
								Logging.Message("successfully read auto-replace settings file with ", autoReplaceFile.AutoReplacements.Count.ToString(), " entries");

								return autoReplaceFile;
							}
							else
							{
								Logging.Error("couldn't deserialize auto-replace file");
							}
						}
					}
					else
					{
						Logging.Message("no auto-replace file found");
					}
				}
			}
			catch (Exception e)
			{
				Logging.LogException(e, "exception reading XML auto-replace file");
			}

			// If we got here, something went wrong; return null.
			return null;
		}

		/*
		/// <summary>
		/// Dump currently loaded network names to XML file.
		/// </summary>
		internal static void SaveFile ()
		{
			try
			{
				// Pretty straightforward.  Serialisation is within GBRSettingsFile class.
				using (StreamWriter writer = new StreamWriter("RON - netdump.xml"))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(AutoReplaceXML));

					AutoReplaceXML xmlFile = new AutoReplaceXML
					{
						AutoReplacements = new List<ReplaceEntry>
						{
							new ReplaceEntry
							{
								aiType ="header",
								targetName = "thisTarget",
								replacementName = "thisReplacement"
							}
						}
					};


					for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); ++i)
					{
						NetInfo thisNet = PrefabCollection<NetInfo>.GetLoaded(i);

						if (thisNet != null)
						{
							Package.Asset asset = PackageManager.FindAssetByName(thisNet.name, Package.AssetType.Object);

							string packagePath = asset?.package?.packagePath;

							ReplaceEntry newEntry = new ReplaceEntry
							{
								aiType = thisNet.GetAI().GetType().ToString(),
								targetName = thisNet.name,
								replacementName = thisNet.name
							};

							if (packagePath != null)
							{
								newEntry.filename = Path.GetFileName(packagePath);
							}

							xmlFile.AutoReplacements.Add(newEntry);
						}
					}

					xmlSerializer.Serialize(writer, xmlFile);
				}
			}
			catch (Exception e)
			{
				Logging.LogException(e, "exception saving XML auto-replace file");
			}
		}*/
	}


	/// <summary>
	/// XML network replacement entry format.
	/// </summary>
	public class ReplaceEntry
	{
		[XmlAttribute("type")]
		[DefaultValue("")]
		public string aiType;

		[XmlAttribute("target")]
		[DefaultValue("")]
		public string targetName;

		[XmlAttribute("replacement")]
		[DefaultValue("")]
		public string replacementName;

		[XmlAttribute("filename")]
		[DefaultValue("")]
		public string filename;
	}
}