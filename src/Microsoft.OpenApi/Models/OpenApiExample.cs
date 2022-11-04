﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Example Object.
    /// </summary>
    public class OpenApiExample : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible, IEffective<OpenApiExample>
    {
        /// <summary>
        /// Short description for the example.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Long description for the example.
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Embedded literal example. The value field and externalValue field are mutually
        /// exclusive. To represent examples of media types that cannot naturally represented
        /// in JSON or YAML, use a string value to contain the example, escaping where necessary.
        /// </summary>
        public IOpenApiAny Value { get; set; }

        /// <summary>
        /// A URL that points to the literal example.
        /// This provides the capability to reference examples that cannot easily be
        /// included in JSON or YAML documents.
        /// The value field and externalValue field are mutually exclusive.
        /// </summary>
        public string ExternalValue { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Reference object.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Indicates object is a placeholder reference to an actual object and does not contain valid data.
        /// </summary>
        public bool UnresolvedReference { get; set; } = false;

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiExample() {}

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiExample"/> object
        /// </summary>
        public OpenApiExample(OpenApiExample example)
        {
            Summary = example?.Summary ?? Summary;
            Description = example?.Description ?? Description;
            Value = OpenApiAnyCloneHelper.CloneFromCopyConstructor(example?.Value);
            ExternalValue = example?.ExternalValue ?? ExternalValue;
            Extensions = example?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(example.Extensions) : null;
            Reference = example?.Reference != null ? new(example?.Reference) : null;
            UnresolvedReference = example?.UnresolvedReference ?? UnresolvedReference;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExample"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            var target = this;

            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    Reference.SerializeAsV3(writer);
                    return;
                }
                else
                {
                    target = GetEffective(Reference.HostDocument);
                }
            }
            target.SerializeAsV3WithoutReference(writer);
        }

        /// <summary>
        /// Returns an effective OpenApiExample object based on the presence of a $ref 
        /// </summary>
        /// <param name="doc">The host OpenApiDocument that contains the reference.</param>
        /// <returns>OpenApiExample</returns>
        public OpenApiExample GetEffective(OpenApiDocument doc)
        {
            if (this.Reference != null)
            {
                return doc.ResolveReferenceTo<OpenApiExample>(this.Reference);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // value
            writer.WriteOptionalObject(OpenApiConstants.Value, Value, (w, v) => w.WriteAny(v));

            // externalValue
            writer.WriteProperty(OpenApiConstants.ExternalValue, ExternalValue);

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExample"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Example object of this form does not exist in V2.
            // V2 Example object requires knowledge of media type and exists only
            // in Response object, so it will be serialized as a part of the Response object.
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            // Example object of this form does not exist in V2.
            // V2 Example object requires knowledge of media type and exists only
            // in Response object, so it will be serialized as a part of the Response object.
        }
    }
}
