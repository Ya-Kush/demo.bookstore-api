namespace App.EndpointModels;

public static class EndpointModelExtensions
{
    public static T SetLinks<T>(this T model, object links) where T : struct, IGetEndpointModel
    {
        model.Links = links;
        return model;
    }
}
