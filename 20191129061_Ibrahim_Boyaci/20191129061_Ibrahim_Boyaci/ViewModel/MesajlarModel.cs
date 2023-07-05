using _20191129061_Ibrahim_Boyaci.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _20191129061_Ibrahim_Boyaci.ViewModel
{
    public class MesajlarModel
    {
        public string mesajId { get; set; }
        public string gonderenId { get; set; }
        public UyelerModel gonderenBilgi { get; set; } //
        public string aliciId { get; set; }
        public UyelerModel aliciBilgi { get; set; } //
        public string mesaj { get; set; }
    }
}