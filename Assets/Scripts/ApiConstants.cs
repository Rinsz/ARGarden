internal static class ApiConstants
{
#if DEBUG
    private const string BaseApiUrl = "http://argarden.ml/";
#else
    private const string BaseApiUrl = "http://argarden.ml/";
#endif

    internal const string MetasRootUrl = BaseApiUrl + "api/models/metas";
    internal const string BundlesRootUrl = BaseApiUrl + "api/models/bundles";
    internal const string ImagesRootUrl = BaseApiUrl + "api/models/images";
}