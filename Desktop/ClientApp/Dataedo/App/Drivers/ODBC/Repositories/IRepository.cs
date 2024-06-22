using System.Collections.Generic;
using Dataedo.App.Drivers.ODBC.ValueObjects;

namespace Dataedo.App.Drivers.ODBC.Repositories;

internal interface IRepository
{
	void Store(Driver driver);

	bool Has(string uid);

	bool Has(Driver driver);

	Driver Load(string uid);

	Driver Load(DriverMetaFile meta);

	IEnumerable<DriverMetaFile> List();
}
