using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.XtraSpellChecker;

namespace Dataedo.App.UserControls.PanelControls.TableUserControlHelpers;

internal static class SpellCheckerHelper
{
	public static SpellCheckerOpenOfficeDictionary englishDictionary = GetSpellCheckerOnce(englishDictionary);

	public static SpellCheckerOpenOfficeDictionary GetSpellCheckerOnce(SpellCheckerOpenOfficeDictionary englishDictionary)
	{
		if (englishDictionary != null)
		{
			return englishDictionary;
		}
		SpellCheckerOpenOfficeDictionary spellCheckerOpenOfficeDictionary = new SpellCheckerOpenOfficeDictionary();
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dataedo.App.UserControls.PanelControls.TableUserControlHelpers.Dictionaries.en_US.aff");
		Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dataedo.App.UserControls.PanelControls.TableUserControlHelpers.Dictionaries.en_US.dic");
		Stream manifestResourceStream3 = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dataedo.App.UserControls.PanelControls.TableUserControlHelpers.Dictionaries.EnglishAlphabet.txt");
		if (manifestResourceStream == null || manifestResourceStream2 == null || manifestResourceStream3 == null)
		{
			return null;
		}
		spellCheckerOpenOfficeDictionary.LoadFromStream(manifestResourceStream2, manifestResourceStream, manifestResourceStream3);
		spellCheckerOpenOfficeDictionary.Encoding = Encoding.UTF8;
		return spellCheckerOpenOfficeDictionary;
	}
}
