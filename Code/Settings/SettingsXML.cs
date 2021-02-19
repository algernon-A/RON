using System.Xml.Serialization;


namespace RON
{
	[XmlRoot("RON")]
	public class NetworkReplacerSettingsFile
	{
		// Language.
		[XmlElement("Language")]
		public string Language
		{
			get => Translations.Language;

			set => Translations.Language = value;
		}
	}
}
