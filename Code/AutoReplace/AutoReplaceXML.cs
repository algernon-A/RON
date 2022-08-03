// <copyright file="AutoReplaceXML.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RON
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Xml;
	using System.Xml.Serialization;
	using AlgernonCommons;
	using ColossalFramework;

	/// <summary>
	/// RON auto-replace XML file.
	/// </summary>
	[XmlRoot("RONAutoReplace")]
	public class AutoReplaceXML
	{
		// Replacement file nicknames.
		internal static readonly string[] nicknames = new string[(int)Replacements.NumReplacements]
		{
			"NAR-R2",
			"NAR-BP",
			"NExt2",
			"MOM"
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
			string assemblyPath = AssemblyUtils.AssemblyPath;
			if (!assemblyPath.IsNullOrWhiteSpace())
			{
				// Iterate through each nickname and load the associated replacement file.
				string dirPath = Path.Combine(assemblyPath, "Replacements");
				for (int i = 0; i < (int)Replacements.NumReplacements; ++i)
				{
					result[i] = LoadReplacementFile(nicknames[i], Path.Combine(dirPath, nicknames[i] + "-replacements.xml"));
				}
			}

			return result;
		}

		/// <summary>
		/// Dump currently loaded network names to XML file.
		/// </summary>
		internal static void SaveFile()
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
		}

		/// <summary>
		/// Load auto-replace configuration from XML file.
		/// </summary>
		/// <param name="nickname">File nickname (for logging).</param>
		/// <param name="fileName">File name (full path).</param>
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

		/// <summary>
		/// Replacement index enum.
		/// </summary>
		public enum Replacements : int
		{
			NAR_R2 = 0,
			NAR_BP,
			NExt2,
			MOM,
			NumReplacements
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
}