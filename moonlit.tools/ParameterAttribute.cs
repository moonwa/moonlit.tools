using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moonlit.Configuration.ConsoleParameter;

namespace Moonlit.Tools
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ParameterAttribute : Attribute, IParameter
    {
        private string _parameterName;

        public ParameterAttribute()
        {
        }

        public ParameterAttribute(string parameterName)
        {
            _parameterName = parameterName;
        }

        public string Description { get; set; }
        public bool Required { get; set; }
        public string Prefixs { get; set; }

        public string ParameterName
        {
            get { return _parameterName; }
        }


        public static ParameterAttribute GetParameter(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute<ParameterAttribute>(false);
            if (attr != null)
            {
                if (string.IsNullOrEmpty(attr._parameterName))
                {
                    attr._parameterName = propertyInfo.Name;
                }
            }
            return attr;
        }

        private bool IsEnum(Type propertyType)
        {
            if (typeof(Nullable<>).IsAssignableFrom(propertyType))
            {
                Type[] genericArgs = propertyType.GetGenericArguments();
                return genericArgs[0].IsEnum;
            }
            return propertyType.IsEnum;
        }

        private bool IsDefinition(Type propertyType)
        {
            return propertyType == typeof(bool) || propertyType == typeof(bool?);
        }

        public Parameter CreateParameter(PropertyInfo property)
        {
            var entities = new string[0];
            if (!string.IsNullOrWhiteSpace(Prefixs))
                entities = Prefixs.Split(new[] { ',', ';', ' ' });

            if (IsDefinition(property.PropertyType))
            {
                IEnumerable<PrefixEntity> prefixs = entities.Select(x => x.Length == 1 ? (PrefixEntity)(ShortPrefix)x[0] : (PrefixEntity)(LongOrSplitPrefix)x).Union(new[] { (LongOrSplitPrefix)_parameterName });
                return new DefinitionParameter(ParameterName, Description, prefixs.ToArray());
            }
            else if (IsEnum(property.PropertyType))
            {
                IEnumerable<PrefixEntity> prefixs = entities.Select(x => x.Length == 1 ? (PrefixEntity)(ShortPrefix)x[0] : (PrefixEntity)(LongOrSplitPrefix)x).Union(new[] { (LongOrSplitPrefix)_parameterName });
                return new EnumParameter(_parameterName, property.PropertyType,
                                         Enum.GetValues(property.PropertyType).GetValue(0), Description,
                                         prefixs.ToArray());
            }
            else
            {
                IEnumerable<PrefixEntity> prefixs = entities.Select(x => x.Length == 1 ? (PrefixEntity)(ShortPrefix)x[0] : (PrefixEntity)(LongOrSplitPrefix)x).Union(new[] { (LongOrSplitPrefix)_parameterName });
                return new ValueParameter(_parameterName, Description, Required, prefixs.ToArray());
            }
        }

        public object GetValue(Parser parser, PropertyInfo property)
        {
            var parameterName = ParameterName ?? property.Name;
            if (IsDefinition(property.PropertyType))
            {
                var definition = parser.GetEntity<DefinitionParameter>(parameterName);
                return definition.Defined;
            }
            if (IsEnum(property.PropertyType))
            {
                var enumParameter = parser.GetEntity<EnumParameter>(parameterName);
                if (enumParameter.Defined)
                    return enumParameter.Value;
                else
                    return null;
            }
            else
            {
                var valueParameter = parser.GetEntity<ValueParameter>(parameterName);
                if (valueParameter.Defined)
                    return valueParameter.Value;
                else return null;
            }
        }

        public void Set(Parser parser, PropertyInfo property)
        {
            parser.AddArguments(CreateParameter(property));
        }

        public string Desc(PropertyInfo property)
        {
            var parameterName = ParameterName ?? property.Name;
            var prefixs = Prefixs?.Split(new[] { ',', ';', ' ' }) ?? new string[0];
            IEnumerable<PrefixEntity> prefixEntities = prefixs.Select(x => x.Length == 1 ? (PrefixEntity)(ShortPrefix)x[0] : (PrefixEntity)(LongOrSplitPrefix)x).Union(new[] { (LongOrSplitPrefix)_parameterName });
            var prefix = prefixEntities.FirstOrDefault();
            if (IsDefinition(property.PropertyType))
            {
                if (prefix == null)
                {
                    return $"--{parameterName}";
                }
                else
                {
                    return $"-{prefix?.Key}";
                }
            }
            if (IsEnum(property.PropertyType))
            {
                var names = string.Join("|", Enum.GetNames(property.PropertyType));
                if (prefix == null)
                {
                    return $"--{parameterName} ({names})";
                }
                else
                {
                    return $"-{prefix?.Key}  ({names})";
                }
            }
            if (prefix == null)
            {
                return $"--{parameterName} {parameterName}";
            }
            else
            {
                return $"-{prefix?.Key} {parameterName}";
            }
        }
    }
}