using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using _20191129061_Ibrahim_Boyaci.Models;
using _20191129061_Ibrahim_Boyaci.ViewModel;

namespace _20191129061_Ibrahim_Boyaci.Controllers
{
    public class ServisController : ApiController
    {
        mesajDBEntities1 db = new mesajDBEntities1();
        SonucModel sonuc = new SonucModel();

        #region Uyeler

        [HttpGet]
        [Route("api/uyelerliste")]
        public List<UyelerModel> UyelerListe()
        {
            List<UyelerModel> liste = db.Uyeler.Select(x => new UyelerModel()
            {
                uyeId = x.uyeId,
                uyeAdSoyad = x.uyeAdSoyad,
                uyeEPosta = x.uyeEPosta,
                uyeSifre = x.uyeSifre
            }).ToList();

            return liste;
        }

        [HttpGet]
        [Route("api/uyebyid/{uyeId}")]
        public UyelerModel UyeById(string uyeId)
        {
            UyelerModel uye = db.Uyeler.Where(s => s.uyeId == uyeId).Select(x=>new UyelerModel()
            {
                uyeId = x.uyeId,
                uyeAdSoyad = x.uyeAdSoyad,
                uyeEPosta = x.uyeEPosta,
                uyeSifre = x.uyeSifre
            }).FirstOrDefault();

            return uye;
        }

        [HttpPost]
        [Route("api/uyeekle")]
        public SonucModel UyeEkle(UyelerModel model)
        {
            if (db.Uyeler.Count(s=>s.uyeEPosta == model.uyeEPosta) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Bu E-Postaya kayıtlı üye bulunmaktadır.";
                return sonuc;
            }

            Uyeler yeni = new Uyeler();
            yeni.uyeId = Guid.NewGuid().ToString();
            yeni.uyeAdSoyad = model.uyeAdSoyad;
            yeni.uyeEPosta = model.uyeEPosta;
            yeni.uyeSifre = model.uyeSifre;

            db.Uyeler.Add(yeni);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Yeni üye oluşturuldu";
            return sonuc;
        }

        [HttpPut]
        [Route("api/uyeduzenle")]
        public SonucModel UyeDuzenle(UyelerModel model)
        {
            Uyeler uye = db.Uyeler.Where(s => s.uyeId == model.uyeId).FirstOrDefault();
            if (uye == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Üye kaydı bulunamadı";
                return sonuc;

            }

            uye.uyeAdSoyad = model.uyeAdSoyad;
            uye.uyeEPosta = model.uyeEPosta;
            uye.uyeSifre = model.uyeSifre;
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Üye kaydı düzenlendi";

            return sonuc;
        }

        [HttpDelete]
        [Route("api/uyesil/{uyeId}")]
        public SonucModel UyeSil(string uyeId)
        {
            Uyeler uye = db.Uyeler.Where(s => s.uyeId == uyeId).FirstOrDefault();
            if (uye == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Üye kaydı bulunamadı";
                return sonuc;

            }

            if (db.Mesajlar.Count(s => s.gonderenId == uyeId) > 0 ||
                db.Mesajlar.Count(s => s.aliciId == uyeId) > 0 ||
                db.Grup_Mesaj.Count(s => s.gmUyeId == uyeId) > 0 ||
                db.Toplu.Count(s => s.tGonderenId == uyeId) > 0 ||
                db.Toplu.Count(s => s.tAlicilarId == uyeId) > 0 ||
                db.Grup_Kayit.Count(s=>s.kayUyeId==uyeId)>0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Başka tablolarda kaydı olan üyeler silinemez";

                return sonuc;
            }

            db.Uyeler.Remove(uye);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Üye kaydı silindi";

            return sonuc;
        }

        #endregion

        #region Mesajlar

        [HttpPost]
        [Route("api/mesajgonder")]
        public SonucModel MesajGonder(MesajlarModel model)
        {
            Mesajlar mesaj = new Mesajlar();
            mesaj.mesajId = Guid.NewGuid().ToString();
            mesaj.gonderenId = model.gonderenId;
            mesaj.aliciId = model.aliciId;
            mesaj.mesaj = model.mesaj;
            db.Mesajlar.Add(mesaj);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Mesaj gönderildi";
            return sonuc;
        }

        [HttpPost]
        [Route("api/toplumesajgonder")]
        public SonucModel TopluMesaj(TopluModel model)
        {
            foreach (var uye in model.aliciId)
            {
                Mesajlar mesaj = new Mesajlar();
                mesaj.mesajId = Guid.NewGuid().ToString();
                mesaj.gonderenId = model.tGonderenId;
                mesaj.mesaj = model.tMesaj;
                mesaj.aliciId = uye;

                db.Mesajlar.Add(mesaj);
                db.SaveChanges();
            }

            sonuc.islem = true;
            sonuc.mesaj = "Mesajlar gönderildi";
            return sonuc;
        }

        [HttpGet]
        [Route("api/tummesajlarlistele")]
        public List<MesajlarModel> TumMesajlarListele()
        {
            List<MesajlarModel> liste = db.Mesajlar.Select(x => new MesajlarModel()
            {
                mesajId = x.mesajId,
                gonderenId = x.gonderenId,
                aliciId = x.aliciId,
                
                mesaj = x.mesaj
            }).ToList();

            foreach (var uye in liste)
            {
                uye.gonderenBilgi = UyeById(uye.gonderenId);
                uye.aliciBilgi = UyeById(uye.aliciId);
            }

            return liste;
        }

        [HttpGet]
        [Route("api/mesajlistele/{gonderenId}/{aliciId}")]
        public List<MesajlarModel> GonderenMesajListele(string gonderenId, string aliciId)
        {
            List<MesajlarModel> liste = db.Mesajlar.Where(s => s.gonderenId == gonderenId && s.aliciId == aliciId).Select(x => new MesajlarModel()
            {
                mesajId = x.mesajId,
                gonderenId = x.gonderenId,
                aliciId =x.aliciId,
    
                mesaj =x.mesaj
            }).ToList();

            foreach (var uye in liste)
            {
                uye.gonderenBilgi = UyeById(uye.gonderenId);
                uye.aliciBilgi = UyeById(uye.aliciId);
            }

            return liste;
        }

        [HttpPut]
        [Route("api/mesajduzenle")]
        public SonucModel MesajDuzenle(MesajlarModel model)
        {
            Mesajlar mesaj = db.Mesajlar.Where(s => s.mesajId == model.mesajId).FirstOrDefault();
            if (mesaj == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "mesaj bulunamadı";
                return sonuc;

            }
            mesaj.mesaj = model.mesaj;
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Mesaj düzenlendi";

            return sonuc;
        }

        [HttpDelete]
        [Route("api/mesajssil/{mesajId}")]
        public SonucModel MesajSil(string mesajId)
        {
            Mesajlar mesaj = db.Mesajlar.Where(s => s.mesajId == mesajId).FirstOrDefault();
            if (mesaj == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Mesaj bulunamadı";
                return sonuc;

            }

            db.Mesajlar.Remove(mesaj);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Mesaj silindi";

            return sonuc;
        }


        #endregion

        #region Gruplar

        [HttpPost]
        [Route("api/grupolustur")]
        public SonucModel GrupOlustur(GruplarModel model)
        {
            Gruplar grup = new Gruplar();
            grup.grupId = Guid.NewGuid().ToString();
            grup.grupAdi = model.grupAdi;
            db.Gruplar.Add(grup);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Grup oluşturuldu";
            return sonuc;
        }

        [HttpGet]
        [Route("api/gruplarlistele")]
        public List<GruplarModel> GruplarListele()
        {
            List<GruplarModel> liste = db.Gruplar.Select(x => new GruplarModel()
            {
                grupId=x.grupId,
                grupAdi=x.grupAdi
            }).ToList();

            return liste;
        }

        [HttpGet]
        [Route("api/grupbyid/{grupId}")]
        public GruplarModel GrupById(string grupId)
        {
            GruplarModel grup = db.Gruplar.Where(s => s.grupId == grupId).Select(x => new GruplarModel()
            {
                grupId=x.grupId,
                grupAdi=x.grupAdi
            }).FirstOrDefault();

            return grup;
        }

        [HttpPut]
        [Route("api/gruplarduzenle")]
        public SonucModel GruplarDuzenle(GruplarModel model)
        {
            Gruplar grup = db.Gruplar.Where(s => s.grupId == model.grupId).FirstOrDefault();
            if (grup == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Grup bulunamadı";
                return sonuc;

            }
            grup.grupAdi = model.grupAdi;
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Grup düzenlendi";

            return sonuc;
        }

        [HttpDelete]
        [Route("api/grupsil/{grupId}")]
        public SonucModel GrupSil(string grupId)
        {
            Gruplar grup = db.Gruplar.Where(s => s.grupId == grupId).FirstOrDefault();
            if (grup == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Grup bulunamadı";
                return sonuc;

            }

            db.Gruplar.Remove(grup);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Grup silindi";

            return sonuc;
        }

        [HttpPost]
        [Route("api/grupuyekayit")]
        public SonucModel GrupUyeKayit(GrupKayitModel model)
        {
            if (db.Grup_Kayit.Count(s=>s.kayGrupId == model.kayGrupId) >0 &&
                db.Grup_Kayit.Count(s => s.kayUyeId == model.kayUyeId) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Bu üye bu gruba zaten kayıtlıdır";
                return sonuc;
            }

            Grup_Kayit kayit = new Grup_Kayit();
            kayit.kayitId = Guid.NewGuid().ToString();
            kayit.kayGrupId = model.kayGrupId;
            kayit.kayUyeId = model.kayUyeId;
            db.Grup_Kayit.Add(kayit);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Kayit yapıldı";
            return sonuc;
        }

        [HttpGet]
        [Route("api/grupuyelistele")]
        public List<GrupKayitModel> GrupUyeListele()
        {

            List<GrupKayitModel> liste = db.Grup_Kayit.Select(x => new GrupKayitModel()
            {
                kayitId = x.kayitId,
                kayGrupId=x.kayGrupId,
                kayUyeId=x.kayUyeId
            }).ToList();

            foreach (var kayit in liste)
            {
                kayit.grupBilgi = GrupById(kayit.kayGrupId);
                kayit.uyeBilgi = UyeById(kayit.kayUyeId);
            }

            return liste;
        }

        [HttpGet]
        [Route("api/grupuyeleri/{kayGrupId}")]
        public List<GrupKayitModel> GrupUyeleri(string kayGrupId)
        {
            List<GrupKayitModel> grup = db.Grup_Kayit.Where(s => s.kayGrupId == kayGrupId).Select(x => new GrupKayitModel()
            {
                kayitId = x.kayitId,
                kayGrupId = x.kayGrupId,
                kayUyeId = x.kayUyeId
            }).ToList();
            

            foreach (var kayit in grup)
            {
                kayit.grupBilgi = GrupById(kayit.kayGrupId);
                kayit.uyeBilgi = UyeById(kayit.kayUyeId);
            }

            return grup;
        }

        [HttpGet]
        [Route("api/uyegruplari/{kayUyeId}")]
        public List<GrupKayitModel> UyeGruplari(string kayUyeId)
        {
            List<GrupKayitModel> grup = db.Grup_Kayit.Where(s => s.kayUyeId == kayUyeId).Select(x => new GrupKayitModel()
            {
                kayitId = x.kayitId,
                kayGrupId = x.kayGrupId,
                kayUyeId = x.kayUyeId
            }).ToList();


            foreach (var kayit in grup)
            {
                kayit.grupBilgi = GrupById(kayit.kayGrupId);
                kayit.uyeBilgi = UyeById(kayit.kayUyeId);
            }

            return grup;
        }

        [HttpDelete]
        [Route("api/grupkayitsil/{kayitId}")]
        public SonucModel GrupKayitId(string kayitId)
        {
            Grup_Kayit kayit = db.Grup_Kayit.Where(s => s.kayitId == kayitId).FirstOrDefault();
            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kayıt bulunamadı";
                return sonuc;

            }

            db.Grup_Kayit.Remove(kayit);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Kayıt silindi";

            return sonuc;
        }

        [HttpPost]
        [Route("api/grupmesajgonder")]
        public SonucModel GrupMesajGonder(GrupMesajModel model)
        {

            Grup_Mesaj mesaj = new Grup_Mesaj();
            mesaj.gmMesajId = Guid.NewGuid().ToString();
            mesaj.gmGrupId = model.gmGrupId;
            mesaj.gmUyeId = model.gmUyeId;
            mesaj.gmMesaj = model.gmMesaj;

            db.Grup_Mesaj.Add(mesaj);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Mesaj gönderildi";
            return sonuc;
        }

        [HttpGet]
        [Route("api/tummesajlar")]
        public List<GrupMesajModel> TumMesajlar()
        {

            List<GrupMesajModel> mesaj = db.Grup_Mesaj.Select(x => new GrupMesajModel()
            {
                gmMesajId = x.gmMesajId,
                gmGrupId = x.gmGrupId,
                gmUyeId = x.gmUyeId,
                gmMesaj = x.gmMesaj
            }).ToList();


            foreach (var kayit in mesaj)
            {
                kayit.grupBilgi = GrupById(kayit.gmGrupId);
                kayit.uyeBilgi = UyeById(kayit.gmUyeId);
            }

            return mesaj;
        }

        [HttpGet]
        [Route("api/grupmesajlari/{gmGrupId}")]
        public List<GrupMesajModel> GrupMesajlari(string gmGrupId)
        {
            List<GrupMesajModel> mesaj = db.Grup_Mesaj.Where(s => s.gmGrupId == gmGrupId).Select(x => new GrupMesajModel()
            {
                gmMesajId = x.gmMesajId,
                gmGrupId = x.gmGrupId,
                gmUyeId = x.gmUyeId,
                gmMesaj=x.gmMesaj
            }).ToList();


            foreach (var kayit in mesaj)
            {
                kayit.grupBilgi = GrupById(kayit.gmGrupId);
                kayit.uyeBilgi = UyeById(kayit.gmUyeId);
            }

            return mesaj;
        }

        [HttpPut]
        [Route("api/grupmesajduzenle")]
        public SonucModel GrupMesajDuzenle(GrupMesajModel model)
        {
            Grup_Mesaj mesaj = db.Grup_Mesaj.Where(s => s.gmMesajId == model.gmMesajId).FirstOrDefault();
            if (mesaj == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Mesaj bulunamadı";
                return sonuc;

            }
            mesaj.gmMesaj = model.gmMesaj;
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Mesaj düzenlendi";

            return sonuc;
        }

        [HttpDelete]
        [Route("api/grupmesajsil/{gmMesajId}")]
        public SonucModel GrupMesajSil(string gmMesajId)
        {
            Grup_Mesaj mesaj = db.Grup_Mesaj.Where(s => s.gmMesajId == gmMesajId).FirstOrDefault();
            if (mesaj == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Mesaj bulunamadı";
                return sonuc;

            }

            db.Grup_Mesaj.Remove(mesaj);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Mesaj silindi";

            return sonuc;
        }

        #endregion

    }
}
