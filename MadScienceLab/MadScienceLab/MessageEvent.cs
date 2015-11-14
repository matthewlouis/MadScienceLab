using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MadScienceLab
{
    class MessageEvent : CellObject
    {
        public string Message {get; set;}
        public MessageEvent(int column, int row) : base(column, row)
        {
        }
    }
}
