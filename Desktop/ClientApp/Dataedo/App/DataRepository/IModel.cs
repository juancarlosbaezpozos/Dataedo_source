using System;

namespace Dataedo.App.DataRepository;

public interface IModel : ICloneable
{
	int Id { get; }
}
