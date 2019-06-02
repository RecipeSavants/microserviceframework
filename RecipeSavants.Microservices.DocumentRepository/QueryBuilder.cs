using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace RecipeSavants.Microservices.DocumentRepository
{
    public class QueryBuilder<T> where T : class
    {
        //github.com/matanshidlov/Lambda-To-Sql/blob/master/Lambda-To-Sql/Lambda-To-Sql/QueryBuilder.cs
        public QueryBuilder(int? DocumentType)
        {
            _where = null;
            _DocumentType = DocumentType;
        }

        private Expression<Func<T, bool>> _where { get; set; }
        private int? _DocumentType { get; set; }

        public QueryBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            _where = _where == null
                ? expression
                : Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(_where.Body, expression.Body),
                    _where.Parameters);
            return this;
        }

        public string Where()
        {
            var where = ConvertExpressionToString(_where.Body);
            if (_DocumentType.HasValue)
            {
                return string.IsNullOrEmpty(where) ? string.Empty : string.Format("WHERE c.DocumentType={0} AND {1}", _DocumentType.Value, where);
            }
            else
            {
                return string.IsNullOrEmpty(where) ? string.Empty : string.Format("WHERE {0}", where);
            }

        }

        private List<string> Properties()
        {
            var properties = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(CustomColumn)));
            return properties.Select(prop => prop.Name).ToList();
        }

        public static string ConvertExpressionToString(Expression body)
        {
            if (body == null)
            {
                return string.Empty;
            }
            if (body is ConstantExpression)
            {
                return ValueToString(((ConstantExpression)body).Value);
            }
            if (body is MemberExpression)
            {
                var member = ((MemberExpression)body);
                if (member.Member.MemberType == MemberTypes.Property)
                {
                    return "c." + member.Member.Name;
                }
                var value = GetValueOfMemberExpression(member);
                if (value is IEnumerable && !(value is string))
                {
                    var sb = new StringBuilder();
                    foreach (var item in value as IEnumerable)
                    {
                        sb.AppendFormat("{0},", ValueToString(item));
                    }
                    return sb.Remove(sb.Length - 1, 1).ToString();
                }
                return ValueToString(value);
            }
            if (body is UnaryExpression)
            {
                return ConvertExpressionToString(((UnaryExpression)body).Operand);
            }
            if (body is BinaryExpression)
            {
                var binary = body as BinaryExpression;
                return string.Format("({0}{1}{2})", ConvertExpressionToString(binary.Left),
                    ConvertExpressionTypeToString(binary.NodeType),
                    ConvertExpressionToString(binary.Right));
            }
            if (body is MethodCallExpression)
            {
                var method = body as MethodCallExpression;
                switch (method.Method.Name)
                {
                    case "SqlLength":
                        return $"LENGTH({ConvertExpressionToString(method.Arguments[0])})";
                    case "SqlConcat":
                        return $"CONCAT({ConvertExpressionToString(method.Arguments[0])}, {ConvertExpressionToString(method.Arguments[1])})";
                    case "SqlContains":
                        return $"CONTAINS({ConvertExpressionToString(method.Arguments[0])}, {ConvertExpressionToString(method.Arguments[1])})";
                    case "SqlStartsWith":
                        return $"STARTSWITH({ConvertExpressionToString(method.Arguments[0])}, {ConvertExpressionToString(method.Arguments[1])})";
                    case "SqlEndsWith":
                        return $"ENDSWITH({ConvertExpressionToString(method.Arguments[0])}, {ConvertExpressionToString(method.Arguments[1])})";
                    case "SqlIndexOf":
                        return $"INDEX_OF({ConvertExpressionToString(method.Arguments[0])}, {ConvertExpressionToString(method.Arguments[1])})";
                    case "SqlLeft":
                        return $"LEFT({ConvertExpressionToString(method.Arguments[0])}, {ConvertExpressionToString(method.Arguments[1])})";
                    case "SqlRight":
                        return $"RIGHT({ConvertExpressionToString(method.Arguments[0])}, {ConvertExpressionToString(method.Arguments[1])})";
                    case "SqlSubstring":
                        return $"SUBSTRING({ConvertExpressionToString(method.Arguments[0])}, {ConvertExpressionToString(method.Arguments[1])}, {ConvertExpressionToString(method.Arguments[2])})";
                    case "SqlLower":
                        return $"LOWER({ConvertExpressionToString(method.Arguments[0])})";
                    case "SqlUpper":
                        return $"UPPER({ConvertExpressionToString(method.Arguments[0])})";
                    case "SqlReplace":
                        return $"REPLACE({ConvertExpressionToString(method.Arguments[0])}, {ConvertExpressionToString(method.Arguments[1])}, {ConvertExpressionToString(method.Arguments[2])})";
                    case "SqlReverse":
                        return $"REVERSE({ConvertExpressionToString(method.Arguments[0])})";
                    case "SqlArrayLength":
                        return $"ARRAY_LENGTH({ConvertExpressionToString(method.Arguments[0])})";
                    case "ToString":
                        return ConvertExpressionToString(method.Object);
                    default:
                        return string.Format("({0} IN ({1}))", ConvertExpressionToString(method.Arguments[0]),
                                      ConvertExpressionToString(method.Object));
                }

            }
            if (body is LambdaExpression)
            {
                return ConvertExpressionToString(((LambdaExpression)body).Body);
            }
            return "";
        }

        private static string ValueToString(object value)
        {
            if (value is bool)
            {
                return value.ToString().ToLower();
            }
            if (value is string)
            {
                return string.Format("'{0}'", value);
            }
            if (value is DateTime)
            {
                return string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", value);
            }
            if (value is null)
            {
                return "null";
            }
            return value.ToString();
        }

        private static object GetValueOfMemberExpression(MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        private static string ConvertExpressionTypeToString(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.And:
                    return " AND ";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Or:
                    return " OR ";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Not:
                    return "NOT";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                default:
                    return "";
            }
        }


    }

    public static class StringLambdaExtension
    {
        /// <summary>
        ///  LENGTH(str_expr) Returns the number of characters of the specified string expression.
        /// </summary>
        /// <param name="str">str_expr</param>
        /// <returns></returns>
        public static int SqlLength(this string str)
        {
            return str.Length;
        }
        /// <summary>
        /// CONCAT (str_expr, str_expr[, str_expr]) Returns a string that is the result of concatenating two or more string values.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static string SqlConcat(this string str, string str2)
        {
            return str + str2;
        }
        /// <summary>
        /// CONTAINS (str_expr, str_expr) Returns a Boolean indicating whether the first string expression contains the second.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool SqlContains(this string str, string str2)
        {
            return str.IndexOf(str2) == 0;
        }
        /// <summary>
        /// STARTSWITH (str_expr, str_expr) Returns a Boolean indicating whether the first string expression ends with the second.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool SqlStartsWith(this string str, string str2)
        {
            return str.IndexOf(str2) != -1;
        }
        /// <summary>
        /// ENDSWITH (str_expr, str_expr) Returns a Boolean indicating whether the first string expression ends with the second.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool SqlEndsWith(this string str, string str2)
        {
            return str.IndexOf(str2) != -1;
        }
        /// <summary>
        /// INDEX_OF (str_expr, str_expr) Returns the starting position of the first occurrence of the second string expression within the first specified string expression, or -1 if the string is not found.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static int SqlIndexOf(this string str, string str2)
        {
            return str.IndexOf(str2);
        }
        /// <summary>
        /// LEFT (str_expr, num_expr)    Returns the left part of a string with the specified number of characters.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="num_expr"></param>
        /// <returns></returns>
        public static string SqlLeft(this string str, int num_expr)
        {
            return str.Substring(0, num_expr);
        }
        /// <summary>
        /// RIGHT (str_expr, num_expr)  Returns the right part of a string with the specified number of characters.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="num_expr"></param>
        /// <returns></returns>
        public static string SqlRight(this string str, int num_expr)
        {
            return str.Substring(0, num_expr);
        }
        /// <summary>
        /// SUBSTRING(str_expr, num_expr, num_expr) Returns part of a string expression.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="num_expr1"></param>
        /// <param name="num_expr2"></param>
        /// <returns></returns>
        public static string SqlSubstring(this string str, int num_expr1, int num_expr2)
        {
            return str.Substring(num_expr1, num_expr2);
        }

        /// <summary>
        /// LOWER (str_expr)   Returns a string expression after converting uppercase character data to lowercase.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SqlLower(this string str)
        {
            return str.ToLower();
        }
        /// <summary>
        /// UPPER (str_expr)     Returns a string expression after converting lowercase character data to uppercase.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SqlUpper(this string str)
        {
            return str.ToUpper();
        }
        /// <summary>
        /// REPLACE(str_expr, str_expr, str_expr) Replaces all occurrences of a specified string value with another string value.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="oldstr"></param>
        /// <param name="newstr"></param>
        /// <returns></returns>
        public static string SqlReplace(this string str, string oldstr, string newstr)
        {
            return str.Replace(oldstr, newstr);
        }
        /// <summary>
        /// REVERSE (str_expr)    Returns the reverse order of a string value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SqlReverse(this string str)
        {
            return str;
        }
        /// <summary>
        /// ARRAY_LENGTH(arr_expr)Returns the number of elements of the specified array expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <returns></returns>
        public static int SqlArrayLength<T>(this List<T> l)
        {
            return l.Count;
        }
        public static int SqlArrayLength(this Array l)
        {
            return l.Length;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CustomColumn : Attribute
    {
        public CustomColumn([CallerLineNumber] int order = 0)
        {
            Order = order;
        }
        public int Order { get; private set; }
        public bool Primary { get; set; }
    }
}
