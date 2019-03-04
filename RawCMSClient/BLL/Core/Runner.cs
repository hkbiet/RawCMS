﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RawCMS.Client.BLL.Core
{
    public class Runner
    {
        private readonly ILogger<Runner> _logger;

        private bool _verbose { get; set; }
        private bool _pretty { get; set; }

        public void SetVerbose(bool verbose)
        {
            _verbose = verbose;
            if (_verbose) _logger.LogInformation("verbose mode enabled.");
            
        }
        public void SetPretty(bool pretty)
        {
            _pretty = pretty;
        }

        public Runner(ILogger<Runner> logger)
        {
            _logger = logger;
        }

        public void Debug(int eventId, Exception e, string message, object[] args)
        {
            _logger.LogDebug(eventId, e, message, args);
        }

        public void Debug(string message, object[] args)
        {
            _logger.LogDebug(message, args);
            // to avoid duplicate log line. only in info level
            if (_verbose && !_logger.IsEnabled(LogLevel.Debug)) _logger.LogInformation(message, args);
        }
        public void Debug(string message)
        {
            _logger.LogDebug(message);
            // to avoid duplicate log line. only in info level
            if (_verbose && !_logger.IsEnabled(LogLevel.Debug)) _logger.LogInformation(message);
        }
        public void Info(string message, object[] args)
        {
            _logger.LogInformation(message, args);
        }
        public void Info(string message)
        {
            _logger.LogInformation(message);
        }

        public void Warn(string message, object[] args)
        {
            _logger.LogWarning(message, args);
        }
        public void Warn(string message)
        {
            _logger.LogWarning(message);
        }
        public void Error(string message, object[] args)
        {
            _logger.LogError(message, args);
        }
        public void Error(string message)
        {
            _logger.LogError(message);
        }
        public void Error(string message, Exception e)
        {
            _logger.LogError(e, message);
        }

        public void Trace(string message, object[] args)
        {
            _logger.LogTrace(message, args);
        }
        public void Trace(string message)
        {
            _logger.LogTrace(message);
        }
        public void Fatal(string message, object[] args)
        {
            _logger.LogCritical(message, args);
        }
        public void Fatal(string message)
        {
            _logger.LogCritical(message);
        }

        internal void Response(string contentResponse)
        {
            _logDataCall(contentResponse, "Response");
        }

        internal void Request(string contentRequest)
        {
            _logDataCall(contentRequest, "Request");
            
        }

        internal void _logDataCall(string content, string direction)
        {
            var ret = string.Empty;

            if (string.IsNullOrEmpty(content))
            {
                Debug("Request has no data.");
                return;
            }

            if (_pretty)
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject(content);
                    ret = JsonConvert.SerializeObject(obj, Formatting.Indented);

                }
                catch(Exception e)
                {
                    Warn($"error parsing reponse: {e.Message}");
                }
            }

            Debug(string.Format("\n------------- {0} -------------\n\n{1}\n\n-------------------------------------\n", direction, ret));

        }
    }
}
