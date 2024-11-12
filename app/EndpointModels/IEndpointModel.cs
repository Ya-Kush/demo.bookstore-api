namespace App.EndpointModels;

public interface IReceivedModel { }
public interface IReturnedModel { }

public interface IGetEndpointModel : IReturnedModel
{
    object? Links { get; set; }
}
public interface IPostEndpointModel : IReceivedModel { }
public interface IPutEndpointModel : IReceivedModel { }
public interface IPatchEndpointModel : IReceivedModel { }
