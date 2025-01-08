﻿using Tests.Shared;
using Veli;

namespace Tests.ComponentTests;

[Collection("VeliClientCollection")]
public class CartTests(VeliClientFixture fixture)
{
	private readonly VeliClient _client = fixture.Client;
	
}