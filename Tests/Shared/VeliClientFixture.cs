using Veli;

namespace Tests.Shared;

public class VeliClientFixture : IDisposable
{
	public VeliClient Client { get; private set; }

	public VeliClientFixture()
	{
		Client = new VeliClient();
	}
	
	public void Dispose()
	{
		Client.Dispose();	
	}
	
}