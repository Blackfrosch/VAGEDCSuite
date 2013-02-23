using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAGSuite
{
    class SymbolTranslator
    {
        public string TranslateSymbolToHelpText(string varName)
        {
            string description = string.Empty;
            if (varName.StartsWith("Driver wish"))
            {
                description = "Determines the requested injection quantity (in milligram per stroke) by the driver. The driver requests this by applying the throttle position.";
            }
            else if (varName.StartsWith("Torque limiter"))
            {
                description = "Limits the requested injection quantity from the Driver wish map depending on engine speed and atmospheric pressure";
            }
            else if (varName.StartsWith("Smoke limiter"))
            {
                description = "Limits the injection quantity by the amount of air actually ingested by the engine. If 60 mg IQ is requested but the engine only gets 300 mg of air, that would result in an AFR of 5 which is obviously not good. This map allows you to compensate for that. Sometimes there are multiple maps, of which one is selected based on coolant temperature.";
            }
            else if (varName.StartsWith("Injector duration"))
            {
                description = "Shows how many crankshaft degrees it takes to achieve the injected amount of fuel. Increasing this map will flow more fuel.";
            }
            else if (varName.StartsWith("N75 duty cycle"))
            {
                description = "The duty cycle applied to the N75 (wastegate) valve. Lower values will open the VNT turbo vanes more (thus decreasing turbo speed) and can be used to stop boost spikes. When you inject more fuel, the exhaust volume will increase so you may need to lower the duty cycle in that area.";
            }
            else if(varName.StartsWith("SVBL Boost limiter"))
            {
                description = "The maximum allowed boost pressure in the system";
            }
            else if (varName.StartsWith("Boost target map"))
            {
                description = "The targeted boost in millibar depending on engine speed and injected quantity.";
            }
            else if (varName.StartsWith("Boost limit map"))
            {
                description = "This map will limit the targeted boost depending on engine speed and atmospheric pressure. The reason is that the compressor cannot deliver the same amount of air when the atmospheric pressure is low.";
            }
            else if (varName.StartsWith("EGR"))
            {
                description = "Exhaust gas return map. This map indicates how much the EGR valve may compensate in the airflow depending on load and engine speed.";
            }
            else if (varName.StartsWith("Start of injection"))
            {
                description = "This map determines at what crankangle the injection cycle should start. Measured in degrees before top dead center (BTDC) and often depends on coolant temperature.";
            }
            else if (varName.StartsWith("Injection duration limiter"))
            {
                description = "This map limits the injector duration for any given engine speed and injection quantity.";
            }
            else if (varName.StartsWith("IQ by MAF limiter"))
            {
                description = "This map limits the injection quantity for any given airflow and engine speed.";
            }
            else if (varName.StartsWith("IQ by MAP limiter"))
            {
                description = "This map limits the injection quantity for any given intake manifold pressure (boost pressure) and engine speed.";
            }
            else if (varName.StartsWith("IQ by air intake temp"))
            {
                description = "Injection quantity correction map based on air intake temperature (after intercooler!).";
            }     
            else if (varName.StartsWith("Fuel volume correction map"))
            {
                description = "Injection quantity correction map based on temperature.";
            }
            else if (varName.StartsWith("Launch control map"))
            {
                description = "Torque limit map that can be used to create a launch control function. Axis are engine speed and Ratio between vehicle and engine speed. Output is the maximum allowed injection quantity (IQ) for that given point.";
            }
            return description;
        }
    }
}
