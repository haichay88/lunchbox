using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace Bizkasa.Bizlunch.Business.Utils
{
    public class ReflectorUtility
    {
        public static object FollowPropertyPath(object value, string path)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (path == null) throw new ArgumentNullException("path");

            Type currentType = value.GetType();

            object obj = value;
            foreach (string propertyName in path.Split('.'))
            {
                if (currentType != null)
                {
                    PropertyInfo property = null;
                    int brackStart = propertyName.IndexOf("[");
                    int brackEnd = propertyName.IndexOf("]");

                    property = currentType.GetProperty(brackStart > 0 ? propertyName.Substring(0, brackStart) : propertyName);
                    if (property != null)
                    {
                        obj = property.GetValue(obj, null);

                        if (brackStart > 0)
                        {
                            string index = propertyName.Substring(brackStart + 1, brackEnd - brackStart - 1);
                            foreach (Type iType in obj.GetType().GetInterfaces())
                            {
                                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                                {
                                    obj = typeof(ReflectorUtility).GetMethod("GetDictionaryElement")
                                                         .MakeGenericMethod(iType.GetGenericArguments())
                                                         .Invoke(null, new object[] { obj, index });
                                    break;
                                }
                                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IList<>))
                                {
                                    obj = typeof(ReflectorUtility).GetMethod("GetListElement")
                                        .MakeGenericMethod(iType.GetGenericArguments())
                                        .Invoke(null, new object[] { obj, index });
                                    break;
                                }
                            }
                        }

                        currentType = obj != null ? obj.GetType() : null; //property.PropertyType;
                    }
                    else return null;
                }
                else return null;
            }
            return obj;
        }
        public static TValue GetDictionaryElement<TKey, TValue>(IDictionary<TKey, TValue> dict, object index)
        {
            TKey key = (TKey)Convert.ChangeType(index, typeof(TKey), null);
            return dict[key];
        }
        public static T GetListElement<T>(IList<T> list, object index)
        {
            int m_Index = Convert.ToInt32(index);
            T m_T = list.Count > m_Index ? list[m_Index] : default(T);

            return m_T;
        }
    }

    public class XmlUtility
    {
        public static string Serialize<T>(T value)
        {
            string m_Xml = string.Empty;

            XmlSerializer m_XmlSerializer = new XmlSerializer(typeof(T));
            using (MemoryStream m_MemoryStream = new MemoryStream())
            {
                m_XmlSerializer.Serialize(m_MemoryStream, value);
                m_MemoryStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader m_StreamReader = new StreamReader(m_MemoryStream))
                {
                    m_Xml = m_StreamReader.ReadToEnd();
                }
            }

            return m_Xml;
        }
        public static T DeSerialize<T>(string xml)
        {
            T m_Value = default(T);

            if (!string.IsNullOrEmpty(xml))
            {
                XmlSerializer m_XmlSerializer = new XmlSerializer(typeof(T));
                using (StringReader m_StringReader = new StringReader(xml))
                {
                    m_Value = (T)m_XmlSerializer.Deserialize(m_StringReader);
                }
            }

            return m_Value;
        }
        public static string Serialize<T>(T value, Type[] types)
        {
            string m_Xml = string.Empty;

            XmlSerializer m_XmlSerializer = new XmlSerializer(typeof(T), types);
            using (MemoryStream m_MemoryStream = new MemoryStream())
            {
                m_XmlSerializer.Serialize(m_MemoryStream, value);
                m_MemoryStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader m_StreamReader = new StreamReader(m_MemoryStream))
                {
                    m_Xml = m_StreamReader.ReadToEnd();
                }
            }

            return m_Xml;
        }
    }
    public class EncryptDecryptUtility
    {
        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            return Encrypt(toEncrypt, useHashing, Default_Key);
        }
        public static string Encrypt(string toEncrypt, bool useHashing, string key)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string cipherString, bool useHashing)
        {
            return Decrypt(cipherString, useHashing, Default_Key);
        }
        public static string Decrypt(string cipherString, bool useHashing, string key)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private static string Default_Key = "P@ssw0rd";

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input. 
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class XsltUtility
    {
        public string Transform(string xmlData, string xsl)
        {
            if (string.IsNullOrEmpty(xsl) || string.IsNullOrEmpty(xmlData))
                return string.Empty;

            string m_Html = string.Empty;

            //Xsl
            XmlNode m_XmlNodeXsl = this.CreateXmlNode(xsl);
            XslCompiledTransform m_XslCompiledTransform = new XslCompiledTransform();
            XsltSettings m_XsltSettings = XsltSettings.Default;
            m_XslCompiledTransform.Load(m_XmlNodeXsl, m_XsltSettings, new XmlUrlResolver());

            using (StringReader m_StringReader = new StringReader(xmlData))
            {
                using (XmlReader m_XmlReader = XmlReader.Create(m_StringReader))
                {
                    using (StringWriter m_StringWriter = new StringWriter())
                    {
                        m_XslCompiledTransform.Transform(m_XmlReader, null, m_StringWriter);
                        m_Html = m_StringWriter.ToString();
                    }
                }
            }

            return m_Html;
        }

        private XmlNode CreateXmlNode(string xmlData)
        {
            XmlNode m_XmlNodeData = null;

            XmlDocument m_XmlDocument = new XmlDocument();
            m_XmlDocument.LoadXml(xmlData);
            if (m_XmlDocument.ChildNodes.Count > 0)
                m_XmlNodeData = m_XmlDocument.ChildNodes.Cast<XmlNode>().LastOrDefault();

            return m_XmlNodeData;
        }
    }

    public class TextUtility
    {
        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public static string RemoveSign4VietnameseString(string str)
        {
            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
    }

    public class ExpressionUtility
    {
        public static Expression<Func<TEntity, bool>> BuildEqual<TEntity>(string propertyName, object value) where TEntity : class
        {
            Expression<Func<TEntity, bool>> m_Expression = null;

            var m_TypeEntity = typeof(TEntity);
            var m_ParameterEntity = Expression.Parameter(m_TypeEntity, "e");
            var m_Property = Expression.Property(m_ParameterEntity, propertyName);
            var m_ConstantFalse = Expression.Constant(false);
            m_Expression = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(m_Property, m_ConstantFalse), new[] { m_ParameterEntity });

            return m_Expression;
        }

        public static Expression<Func<TTarget, bool>> Transform<TSource, TTarget>(Expression<Func<TSource, bool>> expression)
        {
            Expression<Func<TTarget, bool>> m_Expression = null;

            //parameter that will be used in generated expression
            var param = Expression.Parameter(typeof(TTarget));
            //visiting body of original expression that gives us body of the new expression
            var body = new Visitor<TTarget>(param).Visit(expression.Body);
            //generating lambda expression form body and parameter 
            //notice that this is what you need to invoke the Method_2
            m_Expression = Expression.Lambda<Func<TTarget, bool>>(body, param);

            return m_Expression;
        }
        public static PropertyInfo GetPropertyName<T>(System.Linq.Expressions.Expression<Func<T, object>> property)
        {
            System.Linq.Expressions.LambdaExpression lambda = (System.Linq.Expressions.LambdaExpression)property;
            System.Linq.Expressions.MemberExpression memberExpression;

            if (lambda.Body is System.Linq.Expressions.UnaryExpression)
            {
                System.Linq.Expressions.UnaryExpression unaryExpression = (System.Linq.Expressions.UnaryExpression)(lambda.Body);
                memberExpression = (System.Linq.Expressions.MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                memberExpression = (System.Linq.Expressions.MemberExpression)(lambda.Body);
            }

            return ((PropertyInfo)memberExpression.Member);
        }

    }
    public class Visitor<T> : ExpressionVisitor
    {
        ParameterExpression _parameter;

        //there must be only one instance of parameter expression for each parameter 
        //there is one so one passed here
        public Visitor(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        //this method replaces original parameter with given in constructor
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameter;
        }

        //this one is required because PersonData does not implement IPerson and it finds
        //property in PersonData with the same name as the one referenced in expression 
        //and declared on IPerson
        protected override Expression VisitMember(MemberExpression node)
        {
            Expression m_Expression = null;

            //only properties are allowed if you use fields then you need to extend
            // this method to handle them
            if (node.Member.MemberType != System.Reflection.MemberTypes.Property)
                m_Expression = base.VisitMember(node);
            else
            {
                //name of a member referenced in original expression in your 
                //sample Id in mine Prop
                var memberName = node.Member.Name;
                //find property on type T (=PersonData) by name
                var otherMember = typeof(T).GetProperty(memberName);
                //visit left side of this expression p.Id this would be p
                var inner = Visit(node.Expression);
                m_Expression = Expression.Property(inner, otherMember);
            }

            return m_Expression;
        }
    }

    public class EntityComparation
    {
        public static bool Compare<TEntity>(TEntity source, TEntity target, params Func<TEntity, object>[] fields)
        {
            bool m_Result = true;

            if (fields != null)
            {
                foreach (Func<TEntity, object> m_Field in fields)
                {
                    var m_FieldSource = m_Field(source) + string.Empty;
                    var m_FieldTarget = m_Field(target) + string.Empty;

                    m_Result &= m_FieldSource.Equals(m_FieldTarget);
                }
            }
            else
                m_Result = false;

            return m_Result;
        }
        public static ComparationResult<TEntity> CompareExpression<TEntity>(TEntity source, TEntity target, params Expression<Func<TEntity, object>>[] expressionFields)
        {
            ComparationResult<TEntity> m_ComparationResult = new ComparationResult<TEntity>();

            if (expressionFields != null)
            {
                foreach (Expression<Func<TEntity, object>> m_ExpressionField in expressionFields)
                {
                    bool m_Result = false;
                    var m_FieldSource = m_ExpressionField.Compile()(source) + string.Empty;
                    var m_FieldTarget = m_ExpressionField.Compile()(target) + string.Empty;

                    m_Result = m_FieldSource.Equals(m_FieldTarget);
                    if (!m_Result)
                    {
                        m_ComparationResult.NotMatchExpressions.Add(m_ExpressionField);
                        m_ComparationResult.NotMatchProperties.Add(ExpressionUtility.GetPropertyName<TEntity>(m_ExpressionField));
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("Expression Fields is null!");
            }

            return m_ComparationResult;
        }
    }

    public class ComparationResult<TEntity>
    {
        public ComparationResult()
        {
            this.NotMatchProperties = new List<PropertyInfo>();
            this.NotMatchExpressions = new List<Expression<Func<TEntity, object>>>();
        }

        public List<PropertyInfo> NotMatchProperties { get; set; }
        public List<Expression<Func<TEntity, object>>> NotMatchExpressions { get; set; }
    }
}