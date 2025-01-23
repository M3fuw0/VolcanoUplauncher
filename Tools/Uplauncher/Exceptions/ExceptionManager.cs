using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SharpRaven;
using SharpRaven.Data;

namespace Uplauncher.Exceptions
{
    public class ExceptionManager
    {
        private readonly List<Exception> m_exceptions = new List<Exception>();
        private readonly RavenClient _ravenClient;

        public ReadOnlyCollection<Exception> Exceptions => m_exceptions.AsReadOnly();

        public ExceptionManager(RavenClient ravenClient)
        {
            _ravenClient = ravenClient;
        }

        public static void RegisterException(Exception ex)
        {
            //if (App.IsExceptionLoggerEnabled)
            //{
            //    _ravenClient.Capture(new SentryEvent(ex));
            //}
            // m_exceptions.Add(ex);
        }
    }
}