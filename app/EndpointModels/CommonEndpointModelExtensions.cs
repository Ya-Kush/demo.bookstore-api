namespace App.EndpointModels;

public static class CommonEndpointModelExtensions
{
    public static T WithLinks<T>(this T model, object links) where T : IGetEndpointModel
    {
        model.Links = links;
        return model;
    }
}