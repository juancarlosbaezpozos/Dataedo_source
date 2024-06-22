using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dataedo.App.Tools;

public class OrdinalPositionComparer : IComparer<string>
{
	public int Compare(string x, string y)
	{
		string[] array = (from z in x.Split('.')
			where !string.IsNullOrEmpty(z)
			select z)?.ToArray();
		string[] array2 = (from z in y.Split('.')
			where !string.IsNullOrEmpty(z)
			select z)?.ToArray();
		int num = 0;
		while (true)
		{
			bool flag = array.Length > num;
			bool flag2 = array2.Length > num;
			if (flag && !flag2)
			{
				return 1;
			}
			if (!flag && flag2)
			{
				return -1;
			}
			if (!flag && !flag2)
			{
				return 0;
			}
			decimal num2 = decimal.Parse(array[num], CultureInfo.InvariantCulture);
			decimal num3 = decimal.Parse(array2[num], CultureInfo.InvariantCulture);
			if (num2 > num3)
			{
				return 1;
			}
			if (num2 < num3)
			{
				break;
			}
			num++;
		}
		return -1;
	}
}
