using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MSIS_HMS.Models
{
    public static class ResponseModel
    {
        public static ObjectResult Error(string error)
        {
            return new ObjectResult(new { error })
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }
    }
}
