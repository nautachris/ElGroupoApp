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
    }
}
