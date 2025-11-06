using System;

namespace MSIS_HMS.Infrastructure.Helpers
{
    public static class StringExtensions
    {
        public static string GetAge(DateTime? Dob, int? Year, int? Month, int? Day)
        {
            var age = "";
            var _age = 0;
            if (Dob != null)
            {
                var dateOfBirth = Convert.ToDateTime(Dob);
                _age = DateTime.Now.Year - dateOfBirth.Year;
                if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                    _age = _age - 1;

                return _age + "Y";
            }
            if (Dob == null || _age == 0)
            {
                age = (Year != null && Year > 0) ? Year + "Y " : "";
                age += (Month != null && Month > 0) ? Month + "M " : "";
                age += (Day != null && Day > 0) ? Day + "D" : "";
            }
            return age;
        }
    }
}
