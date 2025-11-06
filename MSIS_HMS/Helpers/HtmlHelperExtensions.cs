using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Infrastructure.Enums;

namespace MSIS_HMS.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static IEnumerable<SelectListItem> GetEnumSelectListWithDefaultValue<TEnum>(this IHtmlHelper htmlHelper, TEnum defaultValue)
            where TEnum : struct
        {
            var selectList = htmlHelper.GetEnumSelectList<TEnum>().ToList();
            selectList.Single(x => x.Value == $"{(int)(object)defaultValue}").Selected = true;
            return selectList;
        }
        public static IEnumerable<SelectListItem> GetEnumSelectListWithDefaultValue<TEnum>(this IHtmlHelper htmlHelper, string stringValue)
            where TEnum : struct
        {
            var selectList = htmlHelper.GetEnumSelectList<TEnum>().ToList();
            if (!string.IsNullOrEmpty(stringValue))
            {
                TEnum defaultValue = EnumExtension.ParseEnum<TEnum>(stringValue);
                selectList.Single(x => x.Value == $"{(int)(object)defaultValue}").Selected = true;
            }
            return selectList;
        }
    }
}
