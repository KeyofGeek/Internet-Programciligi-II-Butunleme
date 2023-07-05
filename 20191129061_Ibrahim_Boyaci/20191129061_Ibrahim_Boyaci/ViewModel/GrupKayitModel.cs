using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20191129061_Ibrahim_Boyaci.ViewModel
{
    public class GrupKayitModel
    {
        public string kayitId { get; set; }
        public string kayGrupId { get; set; }
        public GruplarModel grupBilgi { get; set; } //
        public string kayUyeId { get; set; }
        public UyelerModel uyeBilgi { get; set; } //
    }
}