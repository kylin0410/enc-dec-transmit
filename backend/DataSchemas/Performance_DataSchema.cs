using System;
using System.Collections.Generic;

namespace DecBackEnd.DataSchemas
{

    /// <summary>
    /// 效能上傳資料結構
    /// </summary>
    public class PerformanceData
    {
        /// <summary>裝置名稱</summary>
        /// <example>ubuntu</example>
        public string DeviceName { get; set; }

        /// <summary>UUID(即HW_ComputerSystemInfo裡的ProductUUID)</summary>
        /// <example>5e43e72f-cd40-46e4-a245-bc57993d1ea8</example>
        public string UUID { get; set; }

        /// <summary>上傳Server日期時間(UTC Time)</summary>
        public DateTime UTCDT { get; set; }

        /// <summary>CPU Usage Data</summary>
        public CPUUsage CPUUsage { get; set; }
    }

    /// <summary>CPU Usage Data</summary>
    public class CPUUsage
    {
        /// <summary>CPU數量(psutil)</summary>
        /// <example>2</example>
        public int CSNumberOfProcessors { get; set; }

        /// <summary>CPU邏輯數量(psutil)</summary>
        /// <example>2</example>
        public int CSNumberOfLogicalProcessors { get; set; }

        /// <summary>CPU使用率(%) - 多筆</summary>
        /// <example>12.3</example>
        public List<float> CPUPercent { get; set; }
    }
}
