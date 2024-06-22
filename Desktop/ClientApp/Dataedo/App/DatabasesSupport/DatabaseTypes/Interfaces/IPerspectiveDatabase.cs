using System.Collections.Generic;
using System.Windows.Forms;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;

public interface IPerspectiveDatabase
{
	List<string> GetPerspectiveNames(string connectionString, object connection, string databaseName, Form owner);
}
