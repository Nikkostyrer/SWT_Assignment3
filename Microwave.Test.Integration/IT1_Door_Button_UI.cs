using System;
using NUnit.Framework;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using MicrowaveOvenClasses.Boundary;
using NSubstitute;
using NSubstitute.Core.Arguments;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT1_Door_Button_UI
    {
        /**
         * Integration step connections under test.
         */
        UserInterface ui;
        Door door;
        Button powerButton;
        Button timeButton;
        Button startCancelButton;

        /**
         * Stubs for this integration step.
         */
        ILight light;
        IDisplay display;
        ICookController cookController;

        [SetUp]
        public void SetUp()
        {
            /**
             * Creating substitues for the stubs.
             */
            cookController = Substitute.For<ICookController>();
            display = Substitute.For<IDisplay>();
            light = Substitute.For<ILight>();

            powerButton = new Button();
            timeButton = new Button();
            startCancelButton = new Button();

            door = new Door();
            ui = new UserInterface(powerButton, timeButton, startCancelButton, door, display, light, cookController);
        }

        [Test]
        public void LightOnWhenDoorOpened()
        {
            // Check that light received TurnOn.
            door.Open();
            light.Received(1).TurnOn();

            // Check that light is neither turned on or off when door is open.
            door.Open();
            light.DidNotReceive().TurnOn();
            light.DidNotReceive().TurnOff();

            // Close the door.
            door.Close();
            light.Received(1).TurnOff();

            // Try and re-close the door.
            door.Close();
            light.DidNotReceive().TurnOff();
            light.DidNotReceive().TurnOn();
        }

        [Test]
        public void PowerPressing()
        {
            // Pressing power button shows power level.
            powerButton.Press();
            display.Received(1).ShowPower(50);

            // Increase power level by 50.
            powerButton.Press();
            display.Received(1).ShowPower(100);

            powerButton.Press();
            display.Received(1).ShowPower(150);

            powerButton.Press();
            display.Received(1).ShowPower(200);

            powerButton.Press();
            display.Received(1).ShowPower(250);

            powerButton.Press();
            display.Received(1).ShowPower(300);

            powerButton.Press();
            display.Received(1).ShowPower(350);

            powerButton.Press();
            display.Received(1).ShowPower(400);

            powerButton.Press();
            display.Received(1).ShowPower(450);

            powerButton.Press();
            display.Received(1).ShowPower(500);

            powerButton.Press();
            display.Received(1).ShowPower(550);

            powerButton.Press();
            display.Received(1).ShowPower(600);

            powerButton.Press();
            display.Received(1).ShowPower(650);

            powerButton.Press();
            display.Received(1).ShowPower(700);

            // Increasing power level beyond 700 makes it roll over.
            powerButton.Press();
            display.Received(1).ShowPower(50);

            // Closing door.
            door.Close();
            // Does nothing.
            display.DidNotReceive().ShowPower(Arg.Any<int>());

            // Opening door.
            door.Open();
            // Does not show power.
            display.DidNotReceive().ShowPower(Arg.Any<int>());
            // But it does turn on light.
            light.Received(1).TurnOn();
            // And clears display.
            display.Received(1).Clear();
        }

        [Test]
        public void PressOnTime()
        {
            // Press timeButton.
            timeButton.Press();
            // Nothing should happen.
            display.DidNotReceiveWithAnyArgs().ShowTime(Arg.Any<int>(), Arg.Any<int>());

            // Press powerButton to change UI state to SETPOWER
            powerButton.Press();
            // In SETPOWER state, when timeButton is pressed, state should change to SETTIME.
            timeButton.Press();
            // And ShowTime should be called in display.
            display.Received(1).ShowTime(1, 0);

            // Pressing powerButton in SETTIME should do nothing.
            powerButton.Press();
            display.DidNotReceive().ShowTime(Arg.Any<int>(), Arg.Any<int>());

            // Pressing timeButton should increase time.
            timeButton.Press();
            display.Received(1).ShowTime(2, 0);

            // Closing the door in this state does nothing.
            door.Close();
            display.DidNotReceive().ShowTime(Arg.Any<int>(), Arg.Any<int>());

            // Opening the door should reset everything.
            door.Open();
            light.Received(1).TurnOn();
            display.Received(1).Clear();
        }

        [Test]
        public void StartFromTimeAndOpen()
        {
            // Press power to go into SETPOWER.
            powerButton.Press();
            display.Received(1).ShowPower(Arg.Any<int>());

            // Press time button to go into SETTIME.
            timeButton.Press();
            display.Received(1).ShowTime(Arg.Any<int>(), Arg.Any<int>());

            // Press start button.
            startCancelButton.Press();
            // Light should turn on.
            light.Received(1).TurnOn();
            // Should start cooking.
            cookController.Received(1).StartCooking(50, 60);

            // Opening door.
            door.Open();
            // Stops the cooking.
            cookController.Received(1).Stop();
            light.DidNotReceive().TurnOff();
            light.DidNotReceive().TurnOn();
        }

        [Test]
        public void StartFromTimeAndCancel()
        {
            // Press power to go into SETPOWER.
            powerButton.Press();
            display.Received(1).ShowPower(Arg.Any<int>());

            // Press time button to go into SETTIME.
            timeButton.Press();
            display.Received(1).ShowTime(Arg.Any<int>(), Arg.Any<int>());

            // Press start button.
            startCancelButton.Press();
            // Light should turn on.
            light.Received(1).TurnOn();
            // Should start cooking.
            cookController.Received(1).StartCooking(50, 60);

            // Pressing cancel.
            startCancelButton.Press();
            // Stops the cooking.
            cookController.Received(1).Stop();
            light.Received(1).TurnOff();
            display.Received(1).Clear();
        }

        [Test]
        public void PressCancelFromPower()
        {
            // Press power to go into SETPOWER.
            powerButton.Press();
            display.Received(1).ShowPower(Arg.Any<int>());

            // Press cancel.
            startCancelButton.Press();
            // Turns off light.
            light.Received(1).TurnOff();
            // Clears display.
            display.Received(1).Clear();
        }

        [Test]
        public void StartFromTimeAndFinishCooking()
        {
            // Press power to go into SETPOWER.
            powerButton.Press();
            display.Received(1).ShowPower(Arg.Any<int>());

            // Press time button to go into SETTIME.
            timeButton.Press();
            display.Received(1).ShowTime(Arg.Any<int>(), Arg.Any<int>());

            // Press start button.
            startCancelButton.Press();
            // Light should turn on.
            light.Received(1).TurnOn();
            // Should start cooking.
            cookController.Received(1).StartCooking(50, 60);

            // Simulate cooking is done.
            ui.CookingIsDone();

            // Display clears.
            display.Received(1).Clear();
            // Light turns off.
            light.Received(1).TurnOff();

        }
    }
}
