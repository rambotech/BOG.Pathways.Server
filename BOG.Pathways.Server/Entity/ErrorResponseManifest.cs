using BOG.Pathways.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOG.Pathways.Server.Entity
{
    public static class ErrorResponseManifest
    {
        static List<ErrorResponse> Errors = new List<ErrorResponse>
        {
            new ErrorResponse
            {
                ErrorLookup = 0,
                ErrorMessage = "success",
                Notes = null
            },
            new ErrorResponse
            {
                ErrorLookup = 1,
                ErrorMessage = "pathway identifier is not a valid format.",
                Notes = "must start with a letter, be 1 to 30 characters in length, and only contain letters, numbers, underscores and dashes."
            },
            new ErrorResponse
            {
                ErrorLookup = 2,
                ErrorMessage = "pathway identifier already in use."
            },
            new ErrorResponse
            {
                ErrorLookup = 3,
                ErrorMessage = "pathway identifier not found."
            }
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorLookup"></param>
        /// <returns></returns>
        public static ErrorResponse Get(int errorLookup)
        {
            return Errors.FirstOrDefault(x => x.ErrorLookup == errorLookup);
        }
    }
}
