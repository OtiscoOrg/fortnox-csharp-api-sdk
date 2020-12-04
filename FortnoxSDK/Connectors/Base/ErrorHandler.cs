﻿using System;
using System.Net.Http;
using Fortnox.SDK.Entities;
using Fortnox.SDK.Exceptions;
using Fortnox.SDK.Serialization;
using Fortnox.SDK.Utility;

namespace Fortnox.SDK.Connectors.Base
{
    internal class ErrorHandler
    {
        private const string NoReponseMessage = @"No response from server. Check inner exception for details.";

        protected ISerializer Serializer { get; }

        public ErrorHandler()
        {
            Serializer = new JsonEntitySerializer();
        }

        public void HandleErrorResponse(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().GetResult();
            var errorInformation = ParseError(content);

            var exception = errorInformation != null ?
                new FortnoxApiException($"Request failed: {errorInformation.Message}") :
                new FortnoxApiException($"Request failed: {response.ReasonPhrase} ({(int)response.StatusCode})");

            exception.ResponseContent = content;
            exception.StatusCode = response.StatusCode;
            exception.ErrorInfo = errorInformation;

            throw exception;
        }

        public void HandleNoResponse(HttpRequestException ex)
        {
            throw new NoResponseException(NoReponseMessage, ex);
        }

        private ErrorInformation ParseError(string errorJson)
        {
            try
            {
                return Serializer.Deserialize<EntityWrapper<ErrorInformation>>(errorJson).Entity;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
