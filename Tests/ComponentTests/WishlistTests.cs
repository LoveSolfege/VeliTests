using Tests.Shared;
using Veli;

namespace Tests.ComponentTests;

[Collection("VeliClientCollection")]
public class WishlistTests(VeliClientFixture fixture)
{
	private readonly VeliClient _client = fixture.Client;
	
}