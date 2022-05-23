﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using EnsureThat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Health.Fhir.Ingest.Template
{
    public class JsonPathExpressionEvaluator : IExpressionEvaluator
    {
        private readonly string _jsonPathExpression;
        private readonly ILineInfo _lineInfo;

        public JsonPathExpressionEvaluator(string jsonPathExpression, ILineInfo lineInfo)
        {
            _jsonPathExpression = EnsureArg.IsNotNullOrWhiteSpace(jsonPathExpression, nameof(jsonPathExpression));
            _lineInfo = EnsureArg.IsNotNull(lineInfo, nameof(lineInfo));
        }

        public JToken SelectToken(JToken data)
        {
            EnsureArg.IsNotNull(data, nameof(data));
            try
            {
                return data.SelectToken(_jsonPathExpression);
            }
            catch (JsonException e)
            {
                throw new TemplateExpressionException($"Unable to retrieve JsonToken using expression {_jsonPathExpression}", e, _lineInfo);
            }
        }

        public IEnumerable<JToken> SelectTokens(JToken data)
        {
            EnsureArg.IsNotNull(data, nameof(data));
            try
            {
                return data.SelectTokens(_jsonPathExpression);
            }
            catch (JsonException e)
            {
                throw new TemplateExpressionException($"Unable to retrieve JsonTokens using expression {_jsonPathExpression}", e, _lineInfo);
            }
        }
    }
}
