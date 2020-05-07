﻿using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.IO.Ports;
using System.Text;
using System.Threading;
using Iot.Device.DHTxx;
using Iot.Device.GrovePiDevice.Sensors;
using Iot.Units;

namespace appPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            /*
            Demo1();
           */
            Demo2();
        }

        public static void Demo1()
        {
            GpioController controller = new GpioController(PinNumberingScheme.Board);
            var pin = 40;
            var lightTime = 300;

            controller.OpenPin(pin, PinMode.Output);
            try
            {
                while (true)
                {
                    controller.Write(pin, PinValue.High);
                    Thread.Sleep(lightTime);
                    controller.Write(pin, PinValue.Low);
                    Thread.Sleep(lightTime);
                }
            }
            finally
            {
                controller.ClosePin(pin);
            }
        }

        public static void Demo2()
        {

            // to get port name you can use SerialPort.GetPortNames()
            string portName = "/dev/ttyACM0";
            int baudRate = 9600;
            var finished = false;

            using (SerialPort sp = new SerialPort(portName))
            {
                try
                {

                    sp.Encoding = Encoding.UTF8;
                    sp.BaudRate = baudRate;
                    sp.ReadTimeout = 3000;
                    sp.WriteTimeout = 1000;
                    sp.Open();
                    Console.WriteLine($"Opened");

                    Console.CancelKeyPress += (a, b) => {
                        finished = true;
                        // close port to kill pending operations
                        sp.Close();
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fffff} Closed");
                    };

                    Console.WriteLine("Type '!q' or Ctrl-C to exit...");

                    while (!finished)
                    {
                        //var line = Console.ReadKey();
                        //if (line.Key == ConsoleKey.Escape)
                        //    break;


                        // if RATE is set to really high Arduino may fail to respond in time
                        // then on the next command you might get an old message
                        // ReadExisting will read everything from the internal buffer
                        string existingData = sp.ReadExisting();
                        Console.WriteLine($"-{DateTime.Now:HH:mm:ss.fffff} {existingData}");
                        if (!existingData.Contains('\n') && !existingData.Contains('\r'))
                        {
                            // we didn't get the response yet, let's wait for it then
                            try
                            {
                                Console.WriteLine($"+{DateTime.Now:HH:mm:ss.fffff} {sp.ReadLine()}");
                            }
                            catch (TimeoutException)
                            {
                                Console.WriteLine($"ERROR: No response in {sp.ReadTimeout}ms.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error {ex.Message}");
                    if (sp?.IsOpen== true)
                    {
                        sp.Close();
                    }
                }
            }


        }
    }
}
