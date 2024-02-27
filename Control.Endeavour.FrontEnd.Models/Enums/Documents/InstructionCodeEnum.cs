using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Enums.Documents
{
    public enum InstructionCodeEnum : short
    {
        /// <summary>
        /// Revisar
        /// </summary>
        [ControlEnum("TAINS,RV", "REVISAR")]
        Review = 1,

        /// <summary>
        /// Aprobar
        /// </summary>
        [ControlEnum("TAINS,AP", "APROBAR")]
        Approve = 2,

        /// <summary>
        /// Firmar
        /// </summary>
        [ControlEnum("TAINS,FR", "FIRMAR")]
        Signature = 3,
    }
}
