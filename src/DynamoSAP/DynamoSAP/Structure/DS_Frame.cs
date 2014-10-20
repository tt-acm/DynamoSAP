using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamoSAP.Structure
{
    public class DS_Frame
    {
        public string Label
        {
            get { return "hello"; }
        }

        public static string CreateFrmFromCrv(string hi)
        {
            if (hi == "hello")
            {
                return "hello";
            }
            else { return "hey"; }
        
        }
    }
}
