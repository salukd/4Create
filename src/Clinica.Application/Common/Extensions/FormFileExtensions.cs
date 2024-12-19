using System.Text;
using Microsoft.AspNetCore.Http;

namespace Clinica.Application.Common.Extensions;

public static class FormFileExtensions
{
    public static async Task<string> ReadContentAsync(this IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}