using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMPSMassDeploymentTool
{
    public class CrestronSignal
    {
        public string SignalName { get; set; }
        public uint SignalIndex { get; set; }
        public byte SignalType { get; set; }
        public byte SignalFlags { get; set; }

        public override string ToString()
        {
            return SignalName + " - [Index: " + SignalIndex + "] [Type: " + SignalType.ToString() + "]";
        }

        public bool HasSignalValue { get; set; }
        public string SignalValue { get; set; }
    }
}
