using System;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace GarageDoor
{
    public class GarageSensor : IDisposable
    {
        SecretLabs.NETMF.Hardware.AnalogInput garageSensor = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A0);
        
        public GarageDoorStatus Status
        {
            get
            {
                //Set the analog pin to monitor Pin 0
                
                //Set sensor range
                garageSensor.SetRange(0, 1024);
                //Program loop
                if (garageSensor.Read() < 1024)
                {
                    return GarageDoorStatus.Open;
                }
                return GarageDoorStatus.Closed; 
            }
        }

        ~GarageSensor()
        {
            Dispose();
        }

        public void Dispose()
        {
            garageSensor.Dispose();
        }
    }
}
