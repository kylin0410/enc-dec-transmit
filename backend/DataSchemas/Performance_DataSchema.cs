using System;
using System.Collections.Generic;

namespace DecBackEnd.DataSchemas
{

    /// <summary>
    /// �į�W�Ǹ�Ƶ��c
    /// </summary>
    public class PerformanceData
    {
        /// <summary>�˸m�W��</summary>
        /// <example>ubuntu</example>
        public string DeviceName { get; set; }

        /// <summary>UUID(�YHW_ComputerSystemInfo�̪�ProductUUID)</summary>
        /// <example>5e43e72f-cd40-46e4-a245-bc57993d1ea8</example>
        public string UUID { get; set; }

        /// <summary>�W��Server����ɶ�(UTC Time)</summary>
        public DateTime UTCDT { get; set; }

        /// <summary>CPU Usage Data</summary>
        public CPUUsage CPUUsage { get; set; }
    }

    /// <summary>CPU Usage Data</summary>
    public class CPUUsage
    {
        /// <summary>CPU�ƶq(psutil)</summary>
        /// <example>2</example>
        public int CSNumberOfProcessors { get; set; }

        /// <summary>CPU�޿�ƶq(psutil)</summary>
        /// <example>2</example>
        public int CSNumberOfLogicalProcessors { get; set; }

        /// <summary>CPU�ϥβv(%) - �h��</summary>
        /// <example>12.3</example>
        public List<float> CPUPercent { get; set; }
    }
}
