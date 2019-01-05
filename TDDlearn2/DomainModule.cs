using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDDlearn2
{
    public class DomainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDataStoreProvider>().To<DataStoreProvider>();
            Bind<ILoggingProvider>().To<LoggingProvider>();
            Bind<IWebServiceProvider>().To<WebServiceProvider>();
        }
    }
}
