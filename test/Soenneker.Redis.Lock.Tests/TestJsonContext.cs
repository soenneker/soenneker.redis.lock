using System.Text.Json;
using System.Text.Json.Serialization;

namespace Soenneker.Redis.Lock.Tests;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(string))]
internal partial class TestJsonContext : JsonSerializerContext;
