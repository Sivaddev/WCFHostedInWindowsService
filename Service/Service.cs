using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface ICalculator
    {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
        [OperationContract]
        TimeSpan Uptime();
    }

    public class CalculatorService : ICalculator
    {
        // Implement the ICalculator methods.
        public double Add(double n1, double n2)
        {
            double result = n1 + n2;
            return result;
        }

        public double Subtract(double n1, double n2)
        {
            double result = n1 - n2;
            return result;
        }

        public double Multiply(double n1, double n2)
        {
            double result = n1 * n2;
            return result;
        }

        public double Divide(double n1, double n2)
        {
            double result = n1 / n2;
            return result;
        }

        public TimeSpan Uptime()
        {
            return DateTime.Now - CalculatorWindowsService.Junk.StartTime;
        }
    }


    public class CalculatorWindowsService : ServiceBase
    {
        public static SharedJunk Junk = new SharedJunk();
        public ServiceHost serviceHost = null;
        public CalculatorWindowsService()
        {
            // Name the Windows Service
            ServiceName = "WCFHostedInWindowsService";
        }

        public static void Main()
        {
            ServiceBase.Run(new CalculatorWindowsService());
        }


        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            Junk.StartTime = DateTime.Now;
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(CalculatorService));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }

    public class SharedJunk
    {
        public DateTime StartTime { set; get; }
    }

    // Provide the ProjectInstaller class which allows 
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "WCFHostedInWindowsService";
            Installers.Add(process);
            Installers.Add(service);
        }
    }

}
