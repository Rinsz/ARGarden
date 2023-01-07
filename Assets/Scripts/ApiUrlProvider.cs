using System;
using JetBrains.Annotations;
using Models.Descriptors;

internal static class ApiUrlProvider
{
    public static string GetMetasUrl(ModelGroup? modelGroup, [CanBeNull] string modelName, int skip = 0, int take = 100) =>
        $"{ApiConstants.MetasRootUrl}?" +
        (modelGroup is null or ModelGroup.Unknown ? string.Empty : $"modelGroup={modelGroup}&") +
        (string.IsNullOrWhiteSpace(modelName) ? string.Empty : $"modelName={modelName}&") +
        $"skip={skip}&" +
        $"take={take}";
    
    public static string GetImageUrl(Guid modelId, int version) =>
        $"{ApiConstants.ImagesRootUrl}/{modelId}/{version}";

    public static string GetBundleUrl(Guid modelId, int version) =>
        $"{ApiConstants.BundlesRootUrl}/{modelId}/{version}";
}