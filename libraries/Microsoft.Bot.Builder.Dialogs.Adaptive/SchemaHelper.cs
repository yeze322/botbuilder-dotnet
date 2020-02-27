// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive
{
    internal class SchemaHelper
    {
        public SchemaHelper(JObject schema)
        {
            Schema = schema;
            Property = CreateProperty(Schema);
        }

        public JObject Schema { get; }

        public PropertySchema Property { get; }

        public JArray Required()
            => Schema["required"] as JArray ?? new JArray();

        public PropertySchema PathToSchema(string path)
        {
            var property = Property;
            var steps = path.Split('.', '[');
            var step = 0;
            while (property != null && step < steps.Length)
            {
                var found = false;
                foreach (var child in property.Children)
                {
                    if (child.Name == steps[step])
                    {
                        property = child;
                        while (++step < steps.Length && (steps[step] == "." || steps[step] == "[]"))
                        {
                        }

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    property = null;
                }
            }

            return property;
        }

        private PropertySchema CreateProperty(JObject schema, string path = "")
        {
            var type = schema["type"].Value<string>();
            var children = new List<PropertySchema>();
            if (type == "array")
            {
                path += "[]";
                var items = schema["items"];
                if (items == null) 
                {
                    items = schema["contains"];
                }
                
                if (items != null)
                {
                    if (items is JObject itemsSchema)
                    {
                        schema = itemsSchema;
                        type = schema["type"].Value<string>();
                    }
                    else
                    {
                        throw new ArgumentException($"{path} has an items array which is not supported");
                    }
                }
                else
                {
                    throw new ArgumentException($"{path} is an array with missing 'items' or 'contains' property");
                }
            }

            if (type == "object")
            {
                foreach (JProperty prop in schema["properties"])
                {
                    if (!prop.Name.StartsWith("$"))
                    {
                        var newPath = path == string.Empty ? prop.Name : $"{path}.{prop.Name}";
                        children.Add(CreateProperty((JObject)prop.Value, newPath));
                    }
                }
            }

            return new PropertySchema(path, schema, children);
        }
    }
}
