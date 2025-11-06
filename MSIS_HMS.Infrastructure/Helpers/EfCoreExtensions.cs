using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Infrastructure.Data;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;
using static MSIS_HMS.Infrastructure.Enums.DbEnum;

namespace MSIS_HMS.Infrastructure.Helpers
{
    public static class EfCoreExtensions
    {
        public static DataSet Execute_SP(this ApplicationDbContext context, string SpName, Dictionary<string, object> parameters = null)
        {
            var conSql = context.Database.GetDbConnection().ConnectionString;
            return Execute_SP(conSql, SpName, parameters);
        }

        public static DataSet Execute_SP(string connectionString, string SpName, Dictionary<string, object> parameters = null)
        {
            var conSql = connectionString;
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(conSql.ToString()))
            {
                using (SqlCommand sqlCmd = new SqlCommand(SpName, sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandTimeout = 120;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            sqlCmd.Parameters.AddWithValue(p.Key, p.Value);
                        }
                    }
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                    sqlConn.Close();
                }
            }
            return ds;
        }

        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetFilteredProperties();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(System.DayOfWeek))
                {
                    DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                    property.SetValue(item, day, null);
                }
                else
                {
                    if (row[property.Name] == DBNull.Value)
                        property.SetValue(item, null, null);
                    else
                        property.SetValue(item, row[property.Name], null);
                }
            }
            return item;
        }

        private static PropertyInfo[] GetFilteredProperties(this Type type)
        {
            return type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(SkipPropertyAttribute), true).Length == 0).ToArray();
        }

        public static async Task<T> AddUserAndTimestamp<T>(this UserManager<ApplicationUser> _userManager, T entity, ClaimsPrincipal User, DbActionEnum action) where T : new()
        {
            switch (action)
            {
                case DbActionEnum.Create:
                    var user = await _userManager.GetUserAsync(User);
                    entity.SetValue("CreatedAt", DateTime.Now);
                    entity.SetValue("CreatedBy", _userManager.GetUserId(User));
                    entity.SetValue("UpdatedAt", DateTime.Now);
                    entity.SetValue("BranchId", user.BranchId);
                    
                    // entity.SetValue("OutletId", user.OutletId);
                    break;
                case DbActionEnum.Update:
                    entity.SetValue("UpdatedAt", DateTime.Now);
                    entity.SetValue("UpdatedBy", _userManager.GetUserId(User));
                    break;
                default: break;
            }
            return entity;
        }

        public static async Task<int?> GetDoctorId(this UserManager<ApplicationUser> _userManager, ClaimsPrincipal User)
        {
            var user = await _userManager.GetUserAsync(User);
            return user.DoctorId;
        }

        public static void SetValue<T>(this T sender, string propertyName, object value)
        {
            var propertyInfo = sender.GetType().GetProperty(propertyName);

            if (propertyInfo is null) return;

            var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (propertyInfo.PropertyType.IsEnum)
            {
                propertyInfo.SetValue(sender, Enum.Parse(propertyInfo.PropertyType, value.ToString()!));
            }
            else
            {
                var safeValue = (value == null) ? null : Convert.ChangeType(value, type);
                propertyInfo.SetValue(sender, safeValue, null);
            }
        }

        public static T2 GetValue<T1, T2>(this T1 sender, string propertyName)
        {
            var propertyInfo = sender.GetType().GetProperty(propertyName);

            if (propertyInfo is null) return default(T2);

            return (T2)propertyInfo.GetValue(sender, null);
        }

        public static object GetValue<T>(this T sender, string propertyName)
        {
            var propertyInfo = sender.GetType().GetProperty(propertyName);

            if (propertyInfo is null) return null;

            return propertyInfo.GetValue(sender, null);
        }

        public static SelectList GetSelectListItems<T>(this ICollection<T> entities, string key, string value1, string value2 = null, object selectedValue = null) where T : new()
        {
            if(string.IsNullOrEmpty(value2))
            {
                return new SelectList(entities, key, value1, selectedValue);
            }
            var selectList = entities.ToList().Select(x => new { Id = x.GetValue(key), Name = x.GetValue(value1).ToString() + " (" + x.GetValue(value2).ToString() + ")" });
            return new SelectList(selectList, "Id", "Name", selectedValue);
        }

        public static SelectList GetSelectListItems<T>(this ICollection<T> entities, string key, string value1, object selectedValue = null) where T : new()
        {
            return entities.GetSelectListItems(key, value1, null, selectedValue);
        }
        public static SelectList GetSelectListItems<T>(this ICollection<T> entities, string key, string value1, string value2) where T : new()
        {
            return entities.GetSelectListItems(key, value1, value2, null);
        }
        public static SelectList GetSelectListItems<T>(this ICollection<T> entities, string key, string value1) where T : new()
        {
            return entities.GetSelectListItems(key, value1, null, null);
        }
    }
}
