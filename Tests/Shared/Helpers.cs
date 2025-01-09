using Newtonsoft.Json.Linq;

#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.

namespace Tests.Shared;

public static class Helpers
{
	public static CartObject GetCartObject(JObject cart, long id)
	{
		JArray cartAsArray = (JArray)cart["data"];
		
		var cartObject = cartAsArray
			.Where(item => (long)item["product"]["id"] == id)
			.Select(item => new CartObject(
				ObjectId: (long)item["id"],
				Quantity: (long)item["quantity"]
			))
			.FirstOrDefault();
		
		return cartObject;
	}

	public static ProductObject GetProduct(JObject searchResult)
	{
		JArray resultAsArray = (JArray)searchResult["pageProps"]["data"]["products"];
		
		var searchObject = resultAsArray
			.Select(item => new ProductObject(
				Id: (long)item["id"],
				Sku: (string)item["sku"]
			))
			.FirstOrDefault();
    
		return searchObject;
	}

	public static ProductObject GetProductFromCart(JObject cart)
	{
		JArray resultAsArray = (JArray)cart["data"];
		
		var searchObject = resultAsArray
			.Select(item => new ProductObject(
				Id: (long)item["product"]["id"],
				Sku: (string)item["sku"]
			))
			.FirstOrDefault();
    
		return searchObject;
	}
	
	
	public static CategoryParams GetCategoryParams(JObject searchResult)
	{
		JArray resultAsArray = (JArray)searchResult["pageProps"]["data"]["categories"];
		
		var index = RndIndex(resultAsArray);
		
		var randomObject = resultAsArray[index];
			
		var categoryParams = new CategoryParams(
				CategoryId: (long)randomObject["id"],
				Headline: (string)randomObject["headline"],
				FullSlug: (string)randomObject["full_slug"]
		);
		
		return categoryParams;
	}
	
	public static FilterOptions GetFilterOptions(JObject searchResult)
	{
		JArray resultAsArray = (JArray)searchResult["pageProps"]["data"]["filters"];
		
		var index = RndIndex(resultAsArray);
		
		var randomObject = resultAsArray[index];

		var filterOptions = ((JArray)randomObject["filter_options"])
			.Select(option => new FilterOption(
				OptionId: (long)option["id"],
				Headline: (string)option["headline"]
			))
			.ToList();
		
		var filterIndex = RndIndex(filterOptions);
		var filterOption = filterOptions[filterIndex];
		
		var filter = new FilterOptions(
			FilterId: (long)randomObject["id"],
			Headline: (string)randomObject["headline"],
			Option: filterOption
		);
		
		return filter;
	}
	
	public static List<Stock> GetAllPrices(JObject searchResult)
	{
		JArray resultAsArray = (JArray)searchResult["pageProps"]["data"]["products"];
		
		if (resultAsArray.Count != 0)
		{
			
			return resultAsArray.Select(item => new Stock(
				StartPrice: (decimal)item["stock"]["start_price"],
				Price: (decimal)item["stock"]["price"]
			)).ToList();
		}
		
		return [];
	}
	
	public static string[] CutOnDashes(string input)
	{
		return input.Split('/');
	}

	public static string MakeCategoryUri(CategoryParams category, string query)
	{
		
		string[] slugs = CutOnDashes(category.FullSlug);
		
		return Endpoints.Category + "/" + //create category URI 
		       category.FullSlug + "/" +
		       category.CategoryId.ToString() + ".json" +
		       "?q=" + query +
		       "&type" + slugs[0] + 
		       "&type=" + slugs[1] +
		       "&type=" + slugs[2] +
		       "&type=" + category.CategoryId.ToString();
	}

	public static string MakeFilterUri(String categoryUri, FilterOptions filters)
	{
		return categoryUri + "&filter_options=" + filters.Option.OptionId.ToString(); 
	}

	public static string MakeDescendingSortUri(String normalUri, string query)
	{
		int index = normalUri.IndexOf(query);
		const string desc = "&ordering=-price";
		return normalUri.Substring(0, index + query.Length) + desc + normalUri.Substring(index + query.Length);
	}
	
	public static int RndIndex<T>(IEnumerable<T> array){
		return Random.Shared.Next(0, array.Count());
	}

	public static string MakeSearchPageUri(string query)
	{
		return $"{Endpoints.SearchPageResult}?q={query}";
	}
}