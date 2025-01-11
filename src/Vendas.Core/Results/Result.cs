using OneOf;

namespace Vendas.Core.Results;

[GenerateOneOf]
public partial class Result<TSuccess, TError> : OneOfBase<TSuccess, TError>
{
}