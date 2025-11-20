using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SmartTrade.Services.Helpers;

/// <summary>
/// Helper class for model validation using Data Annotations
/// </summary>
public class ValidationHelper
{
    /// <summary>
    /// Validates a model object using ValidationContext and throws ArgumentException if validation fails
    /// </summary>
    /// <param name="obj">Model object to validate</param>
    /// <exception cref="ArgumentException">Thrown when one or more validation errors are found</exception>
    public static void ModelValidation(object obj)
    {
        // Create validation context
        ValidationContext validationContext = new ValidationContext(obj);
        List<ValidationResult> validationResults = new List<ValidationResult>();

        // Validate the model object and collect errors
        bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

        if (!isValid)
        {
            // Throw exception with all validation error messages
            string errorMessages = string.Join("\n", validationResults.Select(vr => vr.ErrorMessage));
            throw new ArgumentException(errorMessages);
        }
    }
}

