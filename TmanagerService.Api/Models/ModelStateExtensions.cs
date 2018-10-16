using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace TmanagerService.Api.Models
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            return modelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToList();
        }
    }
}
