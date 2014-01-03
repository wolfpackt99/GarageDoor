using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace GarageDoor
{

    public class GarageActivator : IDisposable
    {
        private OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        private OutputPort Garage2CarOpener = new OutputPort(Pins.GPIO_PIN_D13, false);

        public void Activate()
        {
            led.Write(true);                //Light on-board LED for visual cue
            Garage2CarOpener.Write(true);   //"Push" garage door button
            Thread.Sleep(1000);             //For 1 second
            led.Write(false);               //Turn off on-board LED
            Garage2CarOpener.Write(false);  //Turn off garage door button
        }

        ~GarageActivator()
        {
            Dispose();
        }

        public void Dispose()
        {
            led.Dispose();
            Garage2CarOpener.Dispose();
        }
    }
}
