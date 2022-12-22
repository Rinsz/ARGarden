using System;

internal static class ApiUrlProvider
{
    public static string GetMetasUrl(int skip = 0, int take = 100) =>
        $"{ApiConstants.MetasRootUrl}?skip={skip}&take={take}";
    
    public static string GetImageUrl(Guid modelId, int version) =>
        $"{ApiConstants.ImagesRootUrl}/{modelId}/{version}";

    public static string GetBundleUrl(Guid modelId, int version) =>
        $"{ApiConstants.BundlesRootUrl}/{modelId}/{version}";
}