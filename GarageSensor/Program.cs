using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace GarageSensor
{
    public class Program
    {
        public static void Main()
        {
            //Set the analog pin to monitor Pin 0
            var garageSensor = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A0);
            //Set sensor range
            garageSensor.SetRange(0, 1024);
            //Program loop
            while (true)
            {
                Debug.Print(garageSensor.Read().ToString());
                Thread.Sleep(1000);
            }
        }

    }
}
