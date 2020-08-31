using System;

namespace DecBackEnd.DataSchemas
{
    /// <summary>
    /// Heartbeat上傳資料結構
    /// </summary>
    [Serializable]
    public class Heartbeat

    {
        /// <summary>裝置名稱</summary>
        /// <example>ubuntu-test</example>
        public string DeviceName { get; set; }

        /// <summary>UUID</summary>
        /// <example>5e43e72f-cd40-46e4-a245-bc57993d1ea8</example>
        public string UUID { get; set; }

        /// <summary>上傳Server日期時間(UTC Time)</summary>
        public DateTime UTCDT { get; set; }
    }
}
