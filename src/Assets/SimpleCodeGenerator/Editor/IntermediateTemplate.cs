using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleCodeGenerator.Editor
{
    public class IntermediateTemplate
    {
        private readonly object data;
        private readonly List<string> content;

        public IntermediateTemplate(IEnumerable<string> template, object data)
        {
            this.data = data;
            content = new List<string>(template);
        }

        public IntermediateTemplate RenderStandaloneValues()
        {
            var result = new IntermediateTemplate(content, data);

            for (int i = 0; i < result.content.Count; i++)
            {
                result.content[i] = InsertValuesFromObject(result.content[i], data);
            }

            return result;
        }

        public IntermediateTemplate RenderForLoops()
        {
            var result = new IntermediateTemplate(Enumerable.Empty<string>(), data);

            var forBlock = new Stack<ForBlockInfo>();

            for (int i = 0; i < content.Count; i++)
            {
                Match forLoopStartMatch = Regex.Match(content[i], @"\{\{\s*for\s+(?<itemName>\S+)\s+in\s+(?<collectionName>\S+)\s*\}\}");

                if (forLoopStartMatch.Success)
                {
                    forBlock.Push(new ForBlockInfo
                    {
                        StartLineNumber = i + 1,
                        ItemIdentifier = forLoopStartMatch.Groups["itemName"].Value,
                        CollectionIdentifier = forLoopStartMatch.Groups["collectionName"].Value
                    });
                    continue;
                }

                if (Regex.IsMatch(content[i], @"\{\{\s*end\s*\}\}")) // Is end of for loop
                {
                    ForBlockInfo forBlockInfo = forBlock.Pop();
                    int endLineNumber = i - 1;

                    if (TryGetPropertyValue(data, forBlockInfo.CollectionIdentifier, out object collectionObject))
                    {
                        if (collectionObject is IEnumerable enumerable)
                        {
                            var collection = enumerable.Cast<object>();

                            foreach (object item in collection)
                            {
                                for (int lineNumber = forBlockInfo.StartLineNumber; lineNumber <= endLineNumber; lineNumber++)
                                {
                                    result.content.Add(InsertValuesFromObject(content[lineNumber], item, forBlockInfo.ItemIdentifier));
                                }
                            }
                        }
                    }

                    continue;
                }

                if (forBlock.Count == 0)
                    result.content.Add(content[i]);
            }

            return result;
        }

        private static string InsertValuesFromObject(string input, object objectToInsert, string propertyRootPath = "")
        {
            while (true)
            {
                Match valueMatch = Regex.Match(input, @"\{\{\s*(?<value>\S+)\s*\}\}");

                if (!valueMatch.Success)
                    break;

                string propertyPath = valueMatch.Groups["value"].Value;

                propertyPath = RerootPropertyPath(propertyPath, propertyRootPath);

                if (!TryGetPropertyValue(objectToInsert, propertyPath, out object propertyValue))
                    break;

                input = input.Replace(valueMatch.Value, propertyValue.ToString());
            }

            return input;
        }

        private static string RerootPropertyPath(string currentPath, string newRootPath)
        {
            if (currentPath == newRootPath)
                return "";

            if (!newRootPath.EndsWith("."))
                newRootPath += ".";

            if (currentPath.StartsWith(newRootPath))
                currentPath = currentPath[newRootPath.Length..];

            return currentPath;
        }

        private static bool TryGetPropertyValue(object obj, string propertyPath, out object propertyValue)
        {
            if (string.IsNullOrWhiteSpace(propertyPath))
            {
                propertyValue = obj;
                return true;
            }

            string[] properties = propertyPath.Split('.');
            object currentObject = obj;

            foreach (string property in properties)
            {
                PropertyInfo propertyInfo = currentObject.GetType().GetProperty(property);

                if (propertyInfo != null)
                {
                    currentObject = propertyInfo.GetValue(currentObject);
                    continue;
                }

                FieldInfo fieldInfo = currentObject.GetType().GetField(property);

                if (fieldInfo != null)
                {
                    currentObject = fieldInfo.GetValue(currentObject);
                    continue;
                }

                propertyValue = null;
                return false;
            }

            propertyValue = currentObject;
            return true;
        }

        public string Build()
        {
            var result = new StringBuilder();

            foreach (string s in content)
                result.AppendLine(s);

            return result.ToString();
        }

        private struct ForBlockInfo
        {
            public int StartLineNumber;
            public string ItemIdentifier;
            public string CollectionIdentifier;
        }
    }
}