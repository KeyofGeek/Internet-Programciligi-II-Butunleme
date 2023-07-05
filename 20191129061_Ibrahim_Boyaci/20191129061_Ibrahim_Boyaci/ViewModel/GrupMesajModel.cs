using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20191129061_Ibrahim_Boyaci.ViewModel
{
    public class GrupMesajModel
    {
        public string gmMesajId { get; set; }
        public string gmGrupId { get; set; }
        public GruplarModel grupBilgi { get; set; } //
        public string gmUyeId { get; set; }
        public UyelerModel uyeBilgi { get; set; }//
        public string gmMesaj { get; set; }
    }
}