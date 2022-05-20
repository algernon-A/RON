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
		public enum Replacements : int
        {
			NExt2 = 0,
			NAR = 1,
			NumReplacements = 2
        }


		// Replacement file nicknames.
		internal static readonly string[] nicknames = new string[(int)Replacements.NumReplacements]
		{
			"NExt2",
			"NAR"
		};


		// List of auto-replacements
		[XmlArray("autoreplace")]
		[XmlArrayItem("replace")]
		public List<ReplaceEntry> AutoReplacements { get; set; }


		/// <summary>
		/// Load auto-replace configuration from XML file.
		/// </summary>
		internal static AutoReplaceXML[] LoadReplacements()
		{
			Logging.Message("loading auto-replace settings");

			AutoReplaceXML[] result = new AutoReplaceXML[(int)Replacements.NumReplacements];

			// Get the current assembly path.
			string assemblyPath = ModUtils.GetAssemblyPath();
			if (!assemblyPath.IsNullOrWhiteSpace())
			{
				// Iterate through each nickname and load the associated replacement file.
				for (int i = 0; i < (int)Replacements.NumReplacements; ++i)
				{
					result[i] = LoadReplacementFile(nicknames[i], Path.Combine(assemblyPath, nicknames[i] + "-replacements.xml"));
				}
			}

			return result;
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
							ColossalFramework.Packaging.Package.Asset asset = ColossalFramework.Packaging.PackageManager.FindAssetByName(thisNet.name, ColossalFramework.Packaging.Package.AssetType.Object);

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


		/// <summary>
		/// Load auto-replace configuration from XML file.
		/// </summary>
		/// <param name="nickname">File nickname (for logging)</param>
		/// <param name="fileName">File name (full path)</param>
		/// <returns></returns>
		internal static AutoReplaceXML LoadReplacementFile(string nickname, string fileName)
		{
			try
			{
				// Check to see if configuration file exists.
				if (File.Exists(fileName))
				{
					// Read it.
					using (StreamReader reader = new StreamReader(fileName))
					{
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(AutoReplaceXML));
						if (xmlSerializer.Deserialize(reader) is AutoReplaceXML autoReplaceFile)
						{
							// Successful read.
							Logging.Message("successfully read auto-replace settings file for ", nickname, " with ", autoReplaceFile.AutoReplacements.Count, " entries");

							return autoReplaceFile;
						}
						else
						{
							Logging.Error("couldn't deserialize auto-replace file for ", nickname);
						}
					}
				}
				else
				{
					Logging.Message("no auto-replace file found for ", nickname);
				}
			}
			catch (Exception e)
			{
				Logging.LogException(e, "exception reading XML auto-replace file ", nickname);
			}

			// If we got here, something went wrong; return null.
			return null;
		}
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