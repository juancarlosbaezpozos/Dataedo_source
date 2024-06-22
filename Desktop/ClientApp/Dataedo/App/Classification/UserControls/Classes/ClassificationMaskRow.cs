using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Enums;

namespace Dataedo.App.Classification.UserControls.Classes;

public class ClassificationMaskRow : IStatedObject
{
	public ManagingRowsEnum.ManagingRows RowState { get; set; }

	public string MaskName { get; set; }

	[Browsable(false)]
	public List<ClassificationMaskPresence> PresenceInClassificators { get; private set; }

	[Browsable(false)]
	public List<ClassificationMaskPatternRow> Patterns { get; set; }

	public string AllPatterns => string.Join(", ", Patterns.Select((ClassificationMaskPatternRow x) => x.Mask));

	public ClassificationMaskRow()
	{
		PresenceInClassificators = new List<ClassificationMaskPresence>();
		Patterns = new List<ClassificationMaskPatternRow>();
	}

	public void SetClassificatorsInPresence(IEnumerable<IClassificatorModel> classificatorModels, int? classificatorId)
	{
		if (classificatorId.HasValue)
		{
			PresenceInClassificators.Add(new ClassificationMaskPresence(this, classificatorModels.Where((IClassificatorModel x) => x.Id == classificatorId.Value).First()));
			PresenceInClassificators.AddRange(from x in classificatorModels
				where x.Id != classificatorId.Value
				select new ClassificationMaskPresence(this, x));
		}
		else
		{
			PresenceInClassificators.AddRange(classificatorModels.Select((IClassificatorModel x) => new ClassificationMaskPresence(this, x)));
		}
	}

	public ClassificationMaskPatternRow AddNewPattern()
	{
		string newPatternMask = "%new%pattern%";
		int num = 1;
		while (Patterns.Where((ClassificationMaskPatternRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Deleted && x.Mask == newPatternMask).Any())
		{
			newPatternMask = $"%new%pattern% ({num++})";
		}
		ClassificationMaskPatternRow classificationMaskPatternRow = new ClassificationMaskPatternRow(this)
		{
			Mask = newPatternMask,
			DataTypes = "text",
			RowState = ManagingRowsEnum.ManagingRows.Added,
			IsColumn = true,
			IsTitle = true
		};
		Patterns.Add(classificationMaskPatternRow);
		this.SetUpdatedIfNotAdded();
		return classificationMaskPatternRow;
	}

	public void DeletePattern(ClassificationMaskPatternRow pattern)
	{
		if (Patterns.Contains(pattern))
		{
			if (pattern.RowState == ManagingRowsEnum.ManagingRows.Added)
			{
				Patterns.Remove(pattern);
			}
			else
			{
				pattern.RowState = ManagingRowsEnum.ManagingRows.Deleted;
			}
			this.SetUpdatedIfNotAdded();
		}
	}

	public bool AnyChangesMade()
	{
		if (!this.IsChanged() && !PresenceInClassificators.Where((ClassificationMaskPresence x) => x.IsChanged).Any())
		{
			return Patterns.Where((ClassificationMaskPatternRow x) => x.IsChanged()).Any();
		}
		return true;
	}

	public void SetUnchanged()
	{
		RowState = ManagingRowsEnum.ManagingRows.Unchanged;
		Patterns.ForEach(delegate(ClassificationMaskPatternRow x)
		{
			x.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
		});
		PresenceInClassificators.ForEach(delegate(ClassificationMaskPresence x)
		{
			x.IsChanged = false;
		});
	}
}
