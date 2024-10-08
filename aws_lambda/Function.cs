using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace aws_lambda;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    //public string FunctionHandler(string input, ILambdaContext context)
    //{
    //    return input.ToUpper();
    //}

    public CreateProductResponse FunctionHandler(CreateProductRequest request, ILambdaContext context)
    {
        var response = new CreateProductResponse();
        response.ProductId = Guid.NewGuid().ToString();
        response.ProductName = request.ProductName;
        response.ProductDescription = request.ProductDescription;
        return response;

    }
    public class CreateProductResponse
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
    }
    public class CreateProductRequest
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
    }

}
