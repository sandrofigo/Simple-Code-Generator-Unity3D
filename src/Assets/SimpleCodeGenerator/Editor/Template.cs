using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SimpleCodeGenerator.Editor
{
    public class Template
    {
        private readonly string[] content;

        private Template(string[] content)
        {
            this.content = content;
        }

        public static Template ParseFromFile(AbsolutePath path)
        {
            return new Template(File.ReadAllLines(path));
        }

        public string Render(object data)
        {
            string[] contentCopy = (string[])content.Clone();

            var result = new StringBuilder();

            // 1. Insert all direct values from the data object
            for (int i = 0; i < contentCopy.Length; i++)
            {
                contentCopy[i] = InsertValuesFromObject(contentCopy[i], data);
            }

            // 2. Handle for loops
            var forBlock = new Stack<ForBlockInfo>();

            for (int i = 0; i < contentCopy.Length; i++)
            {
                Match forLoopStartMatch = Regex.Match(contentCopy[i], @"\{\{\s*for\s+(?<itemName>\S+)\s+in\s+(?<collectionName>\S+)\s*\}\}");

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

                if (Regex.IsMatch(contentCopy[i], @"\{\{\s*end\s*\}\}")) // Is end of for loop
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
                                    result.AppendLine(InsertValuesFromObject(contentCopy[lineNumber], item, forBlockInfo.ItemIdentifier));
                                }
                            }
                        }
                    }

                    continue;
                }

                if (forBlock.Count == 0)
                    result.AppendLine(contentCopy[i]);
            }

            return result.ToString();
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
            if (!newRootPath.EndsWith("."))
                newRootPath += ".";

            if (currentPath.StartsWith(newRootPath))
                currentPath = currentPath[newRootPath.Length..];

            return currentPath;
        }

        private static bool TryGetPropertyValue(object obj, string propertyPath, out object propertyValue)
        {
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

        private struct ForBlockInfo
        {
            public int StartLineNumber;
            public string ItemIdentifier;
            public string CollectionIdentifier;
        }
    }
}