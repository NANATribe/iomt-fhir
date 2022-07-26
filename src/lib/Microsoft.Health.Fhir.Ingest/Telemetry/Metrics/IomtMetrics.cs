﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EnsureThat;
using Microsoft.Health.Common.Telemetry;
using Microsoft.Health.Fhir.Ingest.Data;

namespace Microsoft.Health.Fhir.Ingest.Telemetry
{
    /// <summary>
    /// Defines known metrics and metric dimensions for use in Application Insights
    /// </summary>
    public static class IomtMetrics
    {
        private static readonly string _nameDimension = DimensionNames.Name;
        private static readonly string _categoryDimension = DimensionNames.Category;
        private static readonly string _errorTypeDimension = DimensionNames.ErrorType;
        private static readonly string _errorSeverityDimension = DimensionNames.ErrorSeverity;
        private static readonly string _operationDimension = DimensionNames.Operation;
        private static readonly string _partitionDimension = DimensionNames.Identifier;

        private static Metric _deviceIngressSizeBytes = IomtMetricDefinition.DeviceIngressSizeBytes.CreateBaseMetric(Category.Traffic, ConnectorOperation.Normalization);

        private static Metric _notSupported = nameof(NotSupportedException).ToErrorMetric(ConnectorOperation.FHIRConversion, ErrorType.FHIRResourceError, ErrorSeverity.Warning);

        /// <summary>
        /// The latency between event ingestion and output to FHIR processor.
        /// </summary>
        /// <param name="partitionId">The partition id of the input events being consumed from the event hub partition</param
        public static Metric MeasurementIngestionLatency(string partitionId = null)
        {
            return IomtMetricDefinition.MeasurementIngestionLatency
                .CreateBaseMetric(Category.Latency, ConnectorOperation.FHIRConversion)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The latency between event ingestion and output to FHIR processor, in milliseconds.
        /// </summary>
        /// <param name="partitionId">The partition id of the input events being consumed from the event hub partition</param
        public static Metric MeasurementIngestionLatencyMs(string partitionId = null)
        {
            return IomtMetricDefinition.MeasurementIngestionLatencyMs
                .CreateBaseMetric(Category.Latency, ConnectorOperation.FHIRConversion)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The number of measurement groups generated by the FHIR processor based on provided input.
        /// </summary>
        /// <param name="partitionId">The partition id of the input events being consumed from the event hub partition</param
        public static Metric MeasurementGroup(string partitionId = null)
        {
            return IomtMetricDefinition.MeasurementGroup
                .CreateBaseMetric(Category.Traffic, ConnectorOperation.FHIRConversion)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The number of measurement readings to import to FHIR.
        /// </summary>
        /// <param name="partitionId">The partition id of the input events being consumed from the event hub partition</param
        public static Metric Measurement(string partitionId = null)
        {
            return IomtMetricDefinition.Measurement
                .CreateBaseMetric(Category.Traffic, ConnectorOperation.FHIRConversion)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The number of input events received.
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param
        public static Metric DeviceEvent(string partitionId = null)
        {
            return IomtMetricDefinition.DeviceEvent
                .CreateBaseMetric(Category.Traffic, ConnectorOperation.Normalization)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The number of normalized events generated for further processing.
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param
        public static Metric NormalizedEvent(string partitionId = null)
        {
            return IomtMetricDefinition.NormalizedEvent
                .CreateBaseMetric(Category.Traffic, ConnectorOperation.Normalization)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The number of input device events with no normalized events.
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param>
        public static Metric DroppedEvent(string partitionId = null)
        {
            return IomtMetricDefinition.DroppedEvent
               .CreateBaseMetric(Category.Traffic, ConnectorOperation.Normalization)
               .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The latency between the event ingestion time and normalization processing. An increase here indicates a backlog of messages to process.
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param
        public static Metric DeviceEventProcessingLatency(string partitionId = null)
        {
           return IomtMetricDefinition.DeviceEventProcessingLatency
                .CreateBaseMetric(Category.Latency, ConnectorOperation.Normalization)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The latency between the event ingestion time and normalization processing, in milliseconds. An increase here indicates a backlog of messages to process.
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param
        public static Metric DeviceEventProcessingLatencyMs(string partitionId = null)
        {
            return IomtMetricDefinition.DeviceEventProcessingLatencyMs
                .CreateBaseMetric(Category.Latency, ConnectorOperation.Normalization)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// A metric that measures the amount of data (in bytes) ingested by normalization processing.
        /// </summary>
        public static Metric DeviceIngressSizeBytes()
        {
            return _deviceIngressSizeBytes;
        }

        /// <summary>
        /// A metric for when FHIR resource does not support the provided type as a value.
        /// </summary>
        public static Metric NotSupported()
        {
            return _notSupported;
        }

        /// <summary>
        /// A metric for when a FHIR resource has been saved.
        /// </summary>
        /// <param name="resourceType">The type of FHIR resource that was saved.</param>
        /// <param name="resourceOperation">The operation performed on the FHIR resource.</param>
        public static Metric FhirResourceSaved(ResourceType resourceType, ResourceOperation resourceOperation)
        {
            return new Metric(
                "FhirResourceSaved",
                new Dictionary<string, object>
                {
                    { _nameDimension, $"{resourceType}{resourceOperation}" },
                    { _categoryDimension, Category.Traffic },
                    { _operationDimension, ConnectorOperation.FHIRConversion },
                });
        }

        public static Metric UnhandledException(string exceptionName, string connectorStage)
        {
            EnsureArg.IsNotNullOrWhiteSpace(exceptionName);

            return nameof(UnhandledException).ToErrorMetric(connectorStage, ErrorType.GeneralError, ErrorSeverity.Critical, errorName: exceptionName);
        }

        public static Metric HandledException(string exceptionName, string connectorStage)
        {
            return exceptionName.ToErrorMetric(connectorStage, ErrorType.GeneralError, ErrorSeverity.Critical);
        }

        /// <summary>
        /// The time it takes to generate a Normalized Event.
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param
        public static Metric NormalizedEventGenerationTimeMs(string partitionId = null)
        {
            return IomtMetricDefinition.NormalizedEventGenerationTimeMs
                .CreateBaseMetric(Category.Traffic, ConnectorOperation.Normalization)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The time taken to normalize all messages in the batch.
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param
        public static Metric NormalizationTimePerBatchMs(string partitionId = null)
        {
            return IomtMetricDefinition.NormalizationTimePerBatchMs
                .CreateBaseMetric(Category.Traffic, ConnectorOperation.Normalization)
                .AddDimension(_partitionDimension, partitionId);
        }

        /// <summary>
        /// The time taken to generate the Normalization Template.
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param
        public static Metric NormalizationTemplateGenerationMs()
        {
            return IomtMetricDefinition.NormalizationTemplateGenerationMs
                .CreateBaseMetric(Category.Traffic, ConnectorOperation.Normalization);
        }

        /// <summary>
        /// The time taken to submit a batch of Measurements to the internal EventHub
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param
        public static Metric MeasurementBatchSubmissionMs()
        {
            return IomtMetricDefinition.MeasurementBatchSubmissionMs
                .CreateBaseMetric(Category.Traffic, ConnectorOperation.Normalization);
        }

        /// <summary>
        /// The size of a batch of Measurements to submit to the internal EventHub
        /// </summary>
        /// <param name="partitionId">The partition id of the events being consumed from the event hub partition</param
        public static Metric MeasurementBatchSize()
        {
            return IomtMetricDefinition.MeasurementBatchSize
                .CreateBaseMetric(Category.Traffic, ConnectorOperation.Normalization);
        }
    }
}
