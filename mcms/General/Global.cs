using System;
using System.Collections.Generic;
using System.Text;

namespace mcms.General
{
    class Global
    {
        private static readonly Global instance = new Global();
        
        static Global()
        {

        }
        public bool isRefreshWorkDetail { get; set; }
        public bool isNewSQA { get; set; }



        public static Global Instance
        {
            get
            {
                return instance;
            }
        }
    }



}
