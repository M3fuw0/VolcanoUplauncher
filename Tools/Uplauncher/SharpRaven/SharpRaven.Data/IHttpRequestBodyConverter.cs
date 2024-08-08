namespace SharpRaven.Data
{
	public interface IHttpRequestBodyConverter
	{
		bool Matches(string contentType);

		bool TryConvert(dynamic httpContext, out object converted);
	}
}
