﻿using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ApplicationInsights.ServiceFabric.Module
{

    /// <summary>
    /// Telemetry module tracking requests using service remoting.
    /// </summary>
    public class ServiceRemotingRequestTrackingTelemetryModule : ITelemetryModule, IDisposable
    {
        private ServiceRemotingServerEventListener _serviceRemotingServerEventListener;
        private bool _correlationHeadersEnabled = true;
        private string _telemetryChannelEnpoint;
        private TelemetryClient _telemetryClient;


        /// <summary>
        /// Gets or sets a value indicating whether the component correlation headers would be set on service remoting responses.
        /// </summary>
        public bool SetComponentCorrelationHttpHeaders
        {
            get
            {
                return _correlationHeadersEnabled;
            }

            set
            {
                _correlationHeadersEnabled = value;
            }
        }


        /// <summary>
        /// Gets or sets the endpoint that is to be used to get the application insights resource's profile (appId etc.).
        /// </summary>
        public string ProfileQueryEndpoint { get; set; }

        internal string EffectiveProfileQueryEndpoint
        {
            get
            {
                return string.IsNullOrEmpty(this.ProfileQueryEndpoint) ? _telemetryChannelEnpoint : this.ProfileQueryEndpoint;
            }
        }

        /// <summary>
        /// Initializes the telemetry module.
        /// </summary>
        /// <param name="configuration">Telemetry configuration to use for initialization.</param>
        public void Initialize(TelemetryConfiguration configuration)
        {
            _telemetryClient = new TelemetryClient(configuration);

            if (configuration != null && configuration.TelemetryChannel != null)
            {
                _telemetryChannelEnpoint = configuration.TelemetryChannel.EndpointAddress;
            }

            _serviceRemotingServerEventListener = new ServiceRemotingServerEventListener(
                                configuration,
                                this.EffectiveProfileQueryEndpoint,
                                this.SetComponentCorrelationHttpHeaders);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Provides the mechanism to detect redundant calls.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _serviceRemotingServerEventListener.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
