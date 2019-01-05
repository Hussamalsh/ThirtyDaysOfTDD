using Ninject;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDDlearn2
{
    public class BusinessServiceFactory
    {
        public static BusinessService GetNewBusinessService()
        {
            var dataStoreProvider = GetNewDataStoreProvider();
            var loggingProvider = GetNewLoggingProvider();
            var webServiceProvider = GetNewWebServiceProvider();

            return new BusinessService(dataStoreProvider, loggingProvider, webServiceProvider);
        }

        private static IWebServiceProvider GetNewWebServiceProvider()
        {
            return new WebServiceProvider();
        }

        private static ILoggingProvider GetNewLoggingProvider()
        {
            return new LoggingProvider();
        }

        private static IDataStoreProvider GetNewDataStoreProvider()
        {
            return new DataStoreProvider();
        }
    }

    public class BusinessService
    {
        private IDataStoreProvider _dataStoreProvider;
        private ILoggingProvider _loggingProvider;
        private IWebServiceProvider _webServiceProvider;

        public BusinessService(IDataStoreProvider dataStoreProvider,
            ILoggingProvider loggingProvider, IWebServiceProvider webServiceProvider)
        {
            _dataStoreProvider = dataStoreProvider;
            _loggingProvider = loggingProvider;
            _webServiceProvider = webServiceProvider;
        }
    }

    public class DataStoreProvider : IDataStoreProvider
    {


    }

    public interface IDataStoreProvider
    {


    }

    public class LoggingProvider : ILoggingProvider
    {


    }

    public interface ILoggingProvider
    {


    }

    public class WebServiceProvider : IWebServiceProvider
    {


    }

    public interface IWebServiceProvider
    {


    }

    public class BusinessServiceConsumer
    {
        private BusinessService _businessService;

        public BusinessServiceConsumer()
        {
            var kernel = new StandardKernel(new DomainModule());

            _businessService = kernel.Get<BusinessService>();
        }
    }
}
