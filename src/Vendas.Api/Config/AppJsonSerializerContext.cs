using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Vendas.Api.Dtos;
using Vendas.Api.Features.Vendas;

namespace Vendas.Api.Config;

[JsonSourceGenerationOptions(WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    AllowTrailingCommas = false,
    PropertyNameCaseInsensitive = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    UseStringEnumConverter = true
)]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(CriarVendaRequest))]
[JsonSerializable(typeof(EditarVendaRequest))]
[JsonSerializable(typeof(VendaDto))]
internal sealed partial class AppJsonSerializerContext : JsonSerializerContext
{
}
