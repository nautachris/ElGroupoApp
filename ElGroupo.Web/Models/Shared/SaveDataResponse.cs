using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Shared
{
    public class SaveDataResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public object ResponseData { get; set; }



        public static SaveDataResponse Ok()
        {
            return new SaveDataResponse { Success = true };
        }
        public static SaveDataResponse IncludeData(object responseData)
        {
            return new SaveDataResponse { Success = true, ResponseData = responseData };
        }
        public static SaveDataResponse FromException(Exception ex)
        {
            return new SaveDataResponse { Success = false, ErrorMessage = ex.ToString() };
        }

        public static SaveDataResponse FromErrorMessage(string message)
        {
            return new SaveDataResponse { Success = false, ErrorMessage = message };
        }
    }


}
