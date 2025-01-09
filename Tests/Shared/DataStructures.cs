namespace Tests.Shared;

public class DataStructures
{
	
}

public record CartObject(long ObjectId, long Quantity);
public record ProductObject(long Id, string Sku);
public record CategoryParams(long CategoryId, string Headline, string FullSlug);
public record FilterOptions(long FilterId, string Headline, FilterOption Option);
public record FilterOption(long OptionId, string Headline);
public record Stock(decimal StartPrice, decimal Price);