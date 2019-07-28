using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using Dapper;

namespace BX.Repository.Base
{
    public static class SimpleCRUD
    {
        private static readonly IDictionary<string, string> ColumnNames = new Dictionary<string, string>();

        private static readonly IDictionary<Type, string> TableNames = new Dictionary<Type, string>();

        private static string _parameterPrefix;

        private static string _encapsulation = @"""{0}""";

        private static string _getIdentitySql = "SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [id]";

        private static ITableNameResolver _tableNameResolver = new TableNameResolver();

        public static IEnumerable<T> GetList<T>(this IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            Type currentType = typeof(T);
            List<PropertyInfo> idProps = GetIdProperties(currentType).ToList();
            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");

            /////string name = GetTableName(currentType);
            string name = currentType.Name;

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties((T)Activator.CreateInstance(typeof(T))).ToArray());
            sb.AppendFormat(" from {0}", name);

            sb.Append(" " + conditions);

            return connection.Query<T>(sb.ToString(), parameters, transaction, true, commandTimeout);
        }

        public static bool Insert(this IDbConnection connection, object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            List<PropertyInfo> idProps = GetIdProperties(entityToInsert.GetType()).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Insert<T> only supports an entity with a [Key] or Id property");
            if (idProps.Count() > 1)
                throw new ArgumentException("Insert<T> only supports an entity with a single [Key] or Id property");

            string name = GetTableName(entityToInsert);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("insert into {0}", name);
            sb.Append(" (");
            BuildInsertParameters(entityToInsert, sb);
            sb.Append(") ");
            sb.Append("values");
            sb.Append(" (");
            BuildInsertValues(entityToInsert, sb);
            sb.Append(")");

            IEnumerable<dynamic> r = connection.Query(sb.ToString(), entityToInsert, transaction, true, commandTimeout);

            return r.Any();
        }

        public static bool Update(this IDbConnection connection, object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            List<PropertyInfo> idProps = GetIdProperties(entityToUpdate.GetType()).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            string name = GetTableName(entityToUpdate);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update {0}", name);

            sb.AppendFormat(" set ");
            BuildUpdateSet(entityToUpdate, sb);
            sb.Append(" where ");
            BuildWhere(sb, idProps, entityToUpdate);

            return connection.Execute(sb.ToString(), entityToUpdate, transaction, commandTimeout) == 1;
        }

        //build select clause based on list of properties skipping ones with the IgnoreSelect and NotMapped attribute
        private static void BuildSelect(StringBuilder sb, IEnumerable<PropertyInfo> props)
        {
            IList<PropertyInfo> propertyInfos = props as IList<PropertyInfo> ?? props.ToList();
            bool addedAny = false;
            for (int i = 0; i < propertyInfos.Count(); i++)
            {
                if (propertyInfos.ElementAt(i).GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreSelectAttribute).Name || attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;

                if (addedAny)
                    sb.Append(",");
                sb.Append(GetColumnName(propertyInfos.ElementAt(i)));
                //if there is a custom column name add an "as customcolumnname" to the item so it maps properly
                if (propertyInfos.ElementAt(i).GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) != null)
                    sb.Append(" as " + Encapsulate(propertyInfos.ElementAt(i).Name));
                addedAny = true;
            }
        }

        private static void BuildInsertParameters(object entityToInsert, StringBuilder sb)
        {
            PropertyInfo[] props = GetScaffoldableProperties(entityToInsert).ToArray();

            for (int i = 0; i < props.Count(); i++)
            {
                PropertyInfo property = props.ElementAt(i);
                if (property.PropertyType != typeof(Guid)
                    && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                    && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                    continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreInsertAttribute).Name)) continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;

                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))) continue;
                if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) && property.PropertyType != typeof(Guid)) continue;

                sb.Append(GetColumnName(property));
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);
        }

        private static void BuildInsertValues(object entityToInsert, StringBuilder sb)
        {
            PropertyInfo[] props = GetScaffoldableProperties(entityToInsert).ToArray();
            for (int i = 0; i < props.Count(); i++)
            {
                PropertyInfo property = props.ElementAt(i);
                if (property.PropertyType != typeof(Guid)
                    && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                    && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                    continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreInsertAttribute).Name)) continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))) continue;

                if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) &&
                    property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) &&
                    property.PropertyType != typeof(Guid)) continue;

                string castToJson = property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(JsonTypeAttribute).Name)
                    ? "::json"
                    : String.Empty;

                sb.AppendFormat(_parameterPrefix + "{0}" + castToJson, property.Name);
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);
        }

        private static void BuildUpdateSet(object entityToUpdate, StringBuilder sb)
        {
            PropertyInfo[] nonIdProps = GetUpdateableProperties(entityToUpdate).ToArray();

            for (int i = 0; i < nonIdProps.Length; i++)
            {
                PropertyInfo property = nonIdProps[i];
                string castToJson = property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(JsonTypeAttribute).Name)
                    ? "::json"
                    : String.Empty;

                sb.AppendFormat("{0} = " + _parameterPrefix + "{1}{2}", GetColumnName(property), property.Name, castToJson);
                if (i < nonIdProps.Length - 1)
                    sb.AppendFormat(", ");
            }
        }

        private static void BuildWhere(StringBuilder sb, IEnumerable<PropertyInfo> idProps, object sourceEntity, object whereConditions = null)
        {
            PropertyInfo[] propertyInfos = idProps.ToArray();
            for (int i = 0; i < propertyInfos.Count(); i++)
            {
                bool useIsNull = false;

                //match up generic properties to source entity properties to allow fetching of the column attribute
                //the anonymous object used for search doesn't have the custom attributes attached to them so this allows us to build the correct where clause
                //by converting the model type to the database column name via the column attribute
                PropertyInfo propertyToUse = propertyInfos.ElementAt(i);
                PropertyInfo[] sourceProperties = GetScaffoldableProperties(sourceEntity).ToArray();
                for (int x = 0; x < sourceProperties.Count(); x++)
                {
                    if (sourceProperties.ElementAt(x).Name == propertyInfos.ElementAt(i).Name)
                    {
                        propertyToUse = sourceProperties.ElementAt(x);

                        if (whereConditions != null && propertyInfos.ElementAt(i).CanRead && (propertyInfos.ElementAt(i).GetValue(whereConditions, null) == null || propertyInfos.ElementAt(i).GetValue(whereConditions, null) == DBNull.Value))
                        {
                            useIsNull = true;
                        }
                        break;
                    }
                }
                sb.AppendFormat(
                    useIsNull ? "{0} is null" : "{0} = " + _parameterPrefix + "{1}",
                    GetColumnName(propertyToUse),
                    propertyInfos.ElementAt(i).Name);

                if (i < propertyInfos.Count() - 1)
                    sb.AppendFormat(" and ");
            }
        }

        public static Guid SequentialGuid()
        {
            Guid tempGuid = Guid.NewGuid();
            byte[] bytes = tempGuid.ToByteArray();
            DateTime time = DateTime.Now;
            bytes[3] = (byte)time.Year;
            bytes[2] = (byte)time.Month;
            bytes[1] = (byte)time.Day;
            bytes[0] = (byte)time.Hour;
            bytes[5] = (byte)time.Minute;
            bytes[4] = (byte)time.Second;
            return new Guid(bytes);
        }

        //Get all properties that are named Id or have the Key attribute
        //For Get(id) and Delete(id) we don't have an entity, just the type so this method is used
        private static IEnumerable<PropertyInfo> GetIdProperties(Type type)
        {
            List<PropertyInfo> tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)).ToList();
            return tp.Any() ? tp : type.GetProperties().Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        }

        //Get all properties that are not decorated with the Editable(false) attribute
        private static IEnumerable<PropertyInfo> GetScaffoldableProperties(object entity)
        {
            IEnumerable<PropertyInfo> props = entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(EditableAttribute).Name && !IsEditable(p)) == false);
            return props.Where(p => p.PropertyType.IsSimpleType() || IsEditable(p));
        }

        private static string Encapsulate(string databaseword)
        {
            return string.Format(_encapsulation, databaseword);
        }

        //Gets the table name for this type
        //For Get(id) and Delete(id) we don't have an entity, just the type so this method is used
        //Use dynamic type to be able to handle both our Table-attribute and the DataAnnotation
        //Uses class name by default and overrides if the class has a Table attribute
        private static string GetTableName(object entity)
        {
            Type type = entity.GetType();


            string tableName;

            if (TableNames.TryGetValue(type, out tableName))
                return tableName;

            tableName = _tableNameResolver.ResolveTableName(type);
            TableNames[type] = tableName;

            return tableName;
        }

        private static string GetColumnName(PropertyInfo propertyInfo)
        {
            string columnName = (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).Name;

            return columnName;
        }

        private static IEnumerable<PropertyInfo> GetUpdateableProperties(object entity)
        {
            IEnumerable<PropertyInfo> updateableProperties = GetScaffoldableProperties(entity);
            //remove ones with ID
            updateableProperties = updateableProperties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            //remove ones with key attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name) == false);
            //remove ones that are readonly
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => (attr.GetType().Name == typeof(ReadOnlyAttribute).Name) && IsReadOnly(p)) == false);
            //remove ones with IgnoreUpdate attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreUpdateAttribute).Name) == false);
            //remove ones that are not mapped
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name) == false);

            return updateableProperties;
        }

        private static IEnumerable<PropertyInfo> GetAllProperties(object entity)
        {
            if (entity == null) entity = new { };
            return entity.GetType().GetProperties();
        }

        //Determine if the Attribute has an AllowEdit key and return its boolean state
        //fake the funk and try to mimick EditableAttribute in System.ComponentModel.DataAnnotations
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private static bool IsEditable(PropertyInfo pi)
        {
            object[] attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(EditableAttribute).Name);
                if (write != null)
                {
                    return write.AllowEdit;
                }
            }
            return false;
        }

        /// <summary>
        /// Optional IgnoreSelect attribute. Custom for Dapper.SimpleCRUD to exclude a property from
        /// Select methods
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class IgnoreSelectAttribute : Attribute
        {
        }

        /// <summary>
        /// Optional IgnoreInsert attribute. Custom for Dapper.SimpleCRUD to exclude a property from
        /// Insert methods
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class IgnoreInsertAttribute : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class IgnoreUpdateAttribute : Attribute
        {
        }

        /// <summary>
        /// Optional data type attribute. Specify the data type of the column is json
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class JsonTypeAttribute : Attribute
        {
        }

        /// <summary>
        /// Optional Column attribute. You can use the System.ComponentModel.DataAnnotations version in
        /// its place to specify the table name of a poco
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class ColumnAttribute : Attribute
        {
            /// <summary>
            /// Name of the column
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Optional Column attribute.
            /// </summary>
            /// <param name="columnName"></param>
            public ColumnAttribute(string columnName)
            {
                this.Name = columnName;
            }
        }

        /// <summary>
        /// Optional Editable attribute. You can use the System.ComponentModel.DataAnnotations version in
        /// its place to specify the properties that are editable
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class EditableAttribute : Attribute
        {
            /// <summary>
            /// Does this property persist to the database?
            /// </summary>
            public bool AllowEdit { get; private set; }

            /// <summary>
            /// Optional Editable attribute.
            /// </summary>
            /// <param name="iseditable"></param>
            public EditableAttribute(bool iseditable)
            {
                this.AllowEdit = iseditable;
            }
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class TableAttribute : Attribute
        {
            /// <summary>
            /// Optional Table attribute.
            /// </summary>
            /// <param name="tableName"></param>
            public TableAttribute(string tableName)
            {
                Name = tableName;
            }

            /// <summary>
            /// Name of the table
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Name of the schema
            /// </summary>
            public string Schema { get; set; }
        }
        /// <summary>
        /// Optional Readonly attribute. You can use the System.ComponentModel version in its place to
        /// specify the properties that are editable
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class ReadOnlyAttribute : Attribute
        {
            /// <summary>
            /// Does this property persist to the database?
            /// </summary>
            public bool IsReadOnly { get; private set; }

            /// <summary>
            /// Optional ReadOnly attribute.
            /// </summary>
            /// <param name="isReadOnly"></param>
            public ReadOnlyAttribute(bool isReadOnly)
            {
                this.IsReadOnly = isReadOnly;
            }

        }

        /// <summary>
        /// Optional NotMapped attribute. You can use the System.ComponentModel.DataAnnotations version
        /// in its place to specify that the property is not mapped
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class NotMappedAttribute : Attribute
        {
        }

        /// <summary>
        /// Optional Key attribute. You can use the System.ComponentModel.DataAnnotations version in its
        /// place to specify the Primary Key of a poco
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class KeyAttribute : Attribute
        {
        }

        /// <summary>
        /// Optional Key attribute. You can use the System.ComponentModel.DataAnnotations version in its
        /// place to specify a required property of a poco
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class RequiredAttribute : Attribute
        {
        }

        //Determine if the Attribute has an IsReadOnly key and return its boolean state
        //fake the funk and try to mimick ReadOnlyAttribute in System.ComponentModel
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private static bool IsReadOnly(PropertyInfo pi)
        {
            object[] attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(ReadOnlyAttribute).Name);
                if (write != null)
                {
                    return write.IsReadOnly;
                }
            }
            return false;
        }

        public class TableNameResolver : ITableNameResolver
        {
            public virtual string ResolveTableName(Type type)
            {
                string tableName = Encapsulate(type.Name);

                dynamic tableattr = type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(TableAttribute).Name) as dynamic;
                if (tableattr != null)
                {
                    tableName = Encapsulate(tableattr.Name);
                    if (!String.IsNullOrEmpty(tableattr.Schema))
                    {
                        string schemaName = Encapsulate(tableattr.Schema);
                        tableName = String.Format("{0}.{1}", schemaName, tableName);
                    }
                }

                return tableName;
            }
        }

    }

    public interface ITableNameResolver
    {
        string ResolveTableName(Type type);
    }

    internal static class TypeExtension
    {
        //You can't insert or update complex types. Lets filter them out.
        public static bool IsSimpleType(this Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            type = underlyingType ?? type;
            List<Type> simpleTypes = new List<Type>
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(bool),
                typeof(string),
                typeof(char),
                typeof(Guid),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(byte[])
            };
            return simpleTypes.Contains(type) || type.IsEnum;
        }
    }
}
