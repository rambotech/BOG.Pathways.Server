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
                ErrorLookup = -1,
                ErrorMessage = "You are not playing nice",
                Notes = "You need credentials to access this site's methods"
            },
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
            },
            new ErrorResponse
            {
                ErrorLookup = 4,
                ErrorMessage = "site payload maximum count reached."
            },
            new ErrorResponse
            {
                ErrorLookup = 5,
                ErrorMessage = "pathway payload area already has the maximum size in use."
            },
            new ErrorResponse
            {
                ErrorLookup = 6,
                ErrorMessage = "pathway payload area is empty."
            },
            new ErrorResponse
            {
                ErrorLookup = 6,
                ErrorMessage = "pathway reference storage is already at capacity."
            },
            new ErrorResponse
            {
                ErrorLookup = 7,
                ErrorMessage = "pathway reference storage is empty."
            },
            new ErrorResponse
            {
                ErrorLookup = 8,
                ErrorMessage = "pathway deposit requires valid pathway token for write."
            },
            new ErrorResponse
            {
                ErrorLookup = 9,
                ErrorMessage = "pathway withdrawal requires valid pathway token for read."
            },
            new ErrorResponse
            {
                ErrorLookup = 10,
                ErrorMessage = "pathway withdrawal failed after 3 tries."
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
