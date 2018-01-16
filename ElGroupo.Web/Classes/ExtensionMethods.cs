using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq.Expressions;

namespace ElGroupo.Web.Classes
{
    public static class ExtensionMethods
    {
        public static string GetDisplayName<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> method)
        {

            var memberInfo = GetMemberInfo(method);
            var attr = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            var name = attr == null ? memberInfo.Name : attr.Name;
            //var attr2 = memberInfo.GetCustomAttribute<RequiredAttribute>();
            //if (attr2 != null) name += "*";

            return name;


        }
        public static DisplayAttribute GetDisplayAttribute<TEntity, TProperty>(
            this Expression<Func<TEntity, TProperty>> method)
        {
            var memberInfo = GetMemberInfo(method);
            return memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
        }

        public static MemberInfo GetMemberInfo<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr =
                    ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            var memberInfo = memberExpr.Member;
            return memberInfo;
        }
        public static string GetDisplayName<T>(this T instance) where T : struct
        {
            try
            {
                var info = typeof(T).GetField(instance.ToString());

                //Get the Description Attributes
                var attributes = (DisplayAttribute[])info.GetCustomAttributes(typeof(DisplayAttribute), false);

                //Only capture the description attribute if it is a concrete result (i.e. 1 entry)
                if (attributes.Length == 1)
                {
                    if (!String.IsNullOrEmpty(attributes[0].Description))
                    {
                        return attributes[0].Description;
                    }
                    return attributes[0].Name;
                }
                else //Use the value for display if not concrete result
                {
                    return instance.ToString();
                }
            }
            catch (Exception ex)
            {
                return instance.ToString();
            }

        }
        public static string GetDisplayName(this MemberInfo info)
        {
            var attributes = (DisplayAttribute[])info.GetCustomAttributes(typeof(DisplayAttribute), false);
            //Only capture the description attribute if it is a concrete result (i.e. 1 entry)
            if (attributes.Length == 1)
            {
                if (!String.IsNullOrEmpty(attributes[0].Description))
                {
                    return attributes[0].Description;
                }
                return attributes[0].Name;
            }
            return info.Name;

        }


        public static DateTime GetUTCInZone(this TimeZoneInfo tzi)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi);
        }
        public static bool IsDST(this TimeZoneInfo tzi, DateTime dt)
        {
            return tzi.IsDaylightSavingTime(dt);
        }

        public static DateTime ToUTC(this DateTime dt, TimeZoneInfo tzi)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dt, tzi);
        }

        public static DateTime FromUTC(this DateTime dt, TimeZoneInfo tzi)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dt, tzi);
        }
        public static DateTime FromUTC(this DateTime dt, string tzId)
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            return TimeZoneInfo.ConvertTimeFromUtc(dt, tzi);
        }

        /// <summary>
        /// takes an input date in local time zone and checks if if is in daylight savings.  If it is, and the original date was not, it is converted (and vise versa)
        /// </summary>
        /// <param name="dt">Date assumed to be in the local time zone</param>
        /// <param name="originalDST">boolean specifying whether the date we're comparing it to was in DST</param>
        /// <param name="tzi">time zone</param>
        /// <returns>adjusted time in local time zone</returns>
        public static DateTime AdjustForDST(this DateTime dt, bool originalDST, TimeZoneInfo tzi, bool convertToUtc)
        {
            if (!tzi.SupportsDaylightSavingTime) return dt;
            var rule = tzi.GetAdjustmentRules().First(x => x.DateStart <= dt && dt <= x.DateEnd);
            DateTime adjustedDate = dt;
            DateTime dstStart = rule.DaylightTransitionStart.DateFromTransitionTime(dt.Year);
            DateTime dstEnd = rule.DaylightTransitionEnd.DateFromTransitionTime(dt.Year);
            bool adjust = false;
            if (originalDST && (dt > dstEnd || dt < dstStart))
            {
                //check if the date occurs during standard time
                adjust = true;


            }
            else if (!originalDST && dt > dstStart && dt < dstEnd)
            {
                //check if the date occurs during DST
                adjust = true;

            }
            if (adjust)
            {
                if (convertToUtc)
                {
                    //3/10 @ 6:30 PM is the same as 3/11 @ 7:30 in UTC b/c the conversion function knows a time change occured
                    //b/c we want to schedule at 3-10 6:30 MST and 3-11 6:30 MDT (which is an hour later UTC time), we need to add the delta after converting to UTC 
                    adjustedDate = adjustedDate.ToUTC(tzi);
                    adjustedDate = adjustedDate.Add(rule.DaylightDelta);

                    return adjustedDate;
                }
                return adjustedDate.Add(rule.DaylightDelta);
            }
            else
            {
                if (convertToUtc) adjustedDate = adjustedDate.ToUTC(tzi);
                return adjustedDate;
            }


        }
        public static DateTime DateFromTransitionTime(this TimeZoneInfo.TransitionTime tt, int year)
        {
            //i.e. 4th day of 3rd week
            var month = new DateTime(year, tt.Month, 1);
            //base date is the first day of the first week
            DateTime baseDate = DateTime.Now;
            //an array with the date of month that the start of each week is?

            //jan 2018, monday is the 1st - or 2nd day of 1st week
            switch (month.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    baseDate = month;
                    break;
                case DayOfWeek.Monday:
                    baseDate = month.AddDays(-1);
                    break;
                case DayOfWeek.Tuesday:
                    baseDate = month.AddDays(-2);
                    break;
                case DayOfWeek.Wednesday:
                    baseDate = month.AddDays(-3);
                    break;
                case DayOfWeek.Thursday:
                    baseDate = month.AddDays(-4);
                    break;
                case DayOfWeek.Friday:
                    baseDate = month.AddDays(-5);
                    break;
                case DayOfWeek.Saturday:
                    baseDate = month.AddDays(-6);
                    break;
            }

            int daysToAdd = 0;
            switch (tt.Week)
            {
                case 1:
                    daysToAdd = 7 + (int)tt.DayOfWeek;
                    break;
                case 2:
                    daysToAdd = 14 + (int)tt.DayOfWeek;
                    break;
                case 3:
                    daysToAdd = 21 + (int)tt.DayOfWeek;
                    break;
                case 4:
                    daysToAdd = 28 + (int)tt.DayOfWeek;
                    break;
                default:
                    daysToAdd = 35 + (int)tt.DayOfWeek;
                    break;
            }
            var adjusted = baseDate.AddDays(daysToAdd);
            adjusted = new DateTime(adjusted.Year, adjusted.Month, adjusted.Day, tt.TimeOfDay.Hour, tt.TimeOfDay.Minute, tt.TimeOfDay.Second);
            return adjusted;
            //week of month is sunday based
        }
    }
}
