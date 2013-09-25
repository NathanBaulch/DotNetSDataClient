﻿using System.Windows.Forms;
using Saleslogix.SData.Client.Core;

namespace SDataClientApp
{
    public class BaseControl : UserControl
    {
        public ISDataService Service { get; set; }
        public ToolStripItem StatusLabel { get; set; }

        public new virtual void Refresh()
        {
        }
    }
}