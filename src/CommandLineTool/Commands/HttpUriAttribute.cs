using System;
using System.ComponentModel.DataAnnotations;
using Beefweb.CommandLineTool.Services;

namespace Beefweb.CommandLineTool.Commands;

[AttributeUsage(AttributeTargets.Property)]
public sealed class HttpUriAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is Uri uri)
        {
            return ValidateUri(uri);
        }

        if (value is string str)
        {
            return Uri.TryCreate(str, UriKind.Absolute, out var parsedUri)
                ? ValidateUri(parsedUri)
                : new ValidationResult(Messages.HttpUrlRequired);
        }

        return new ValidationResult(Messages.HttpUrlRequired);
    }

    private static ValidationResult? ValidateUri(Uri uri)
    {
        return ClientProvider.IsHttpScheme(uri)
            ? ValidationResult.Success
            : new ValidationResult(Messages.HttpUrlRequired);
    }
}
