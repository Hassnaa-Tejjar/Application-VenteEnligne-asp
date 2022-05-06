using aspprojet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aspprojet.Controllers
{
    public class AdminController : Controller
    {

        ecommerceEntities1 db = new ecommerceEntities1();
        // GET: Admin
        public ActionResult dashAdmin()
        {
            return View();
        }
        public ActionResult authAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult authAdmin(admin clt)

        {

            var verifier = db.admin.Where(c => c.email.Equals(clt.email) && c.codesecret.Equals(clt.codesecret)).SingleOrDefault();
            if (verifier != null)

            {
                Session["adminId"] = verifier.Id.ToString();
                Session["adminprenom"] = verifier.prenom.ToString();
                Session["adminnom"] = verifier.nom.ToString();
                Session["adminemail"] = verifier.email.ToString();
                return Redirect(Url.Action("statistiques", "Admin"));
            }
            else
            {
                ViewBag.error = "Code Secret ou Email incorrect!";
                return View();
            }

        }
        public ActionResult Deconnecter()
        {
            Session["adminnom"] = null;
            Session["adminprenom"] = null;
            Session["adminId"] = null;

            return Redirect(Url.Action("authAdmin", "Admin"));

        }
        public ActionResult statistiques()
        {
           
            return View();
         
           
        }
        [HttpGet]
        public ActionResult statistiques(client c)
        {
            int count = db.client.Count();
            ViewBag.clt = count;
            int countprop = db.proprietaire.Count();
            ViewBag.prop = countprop;
            int countprod = db.article.Count();
            ViewBag.prod = countprod;
            int countmsg = db.message.Count() + db.messageprop.Count();
            ViewBag.msg = countmsg;
            var tables = new Tables
            {
                clients = db.client.OrderByDescending(x => x.Id).ToList().Take(5),
                proprietaires = db.proprietaire.OrderByDescending(x => x.Id).ToList().Take(5),
               
            };
            return View(tables);
           
        }
        public ActionResult msg()
        {

            return View();


        }
        [HttpGet]
        public ActionResult msg(msg m)
        {
           
            var msgs = new msg
            {
                clients = db.message.OrderByDescending(x => x.Id).ToList().Take(5),
                proprietaires = db.messageprop.OrderByDescending(x => x.Id).ToList().Take(5),

            };
            return View(msgs);

        }
        public ActionResult historiqueP()
        {

            return View();


        }
        [HttpGet]
        public ActionResult historiqueP(proprietaire p)
        {
            if (Session["historique"] != null)
            {
                List<historique> hist = (List<historique>)Session["historique"];
                var list = hist.OrderByDescending(x=>DateTime.Now).ToList();
                return View(list);
            }
            else
            {
                return View();
            }

        }
        public ActionResult historiquec()
        {

            return View();


        }
        [HttpGet]
        public ActionResult historiquec(client c)
        {
            if (Session["historique"] != null)
            {
                List<historique> hist = (List<historique>)Session["historique"];
                var list = hist.OrderByDescending(x => x.date).ToList();
                return View(list);
            }
            else
            {
                return View();
            }

        }
        public ActionResult clients()
        {

            return View();

        }
        [HttpGet]
        public ActionResult clients(client c)
        {


            var list = db.client.OrderByDescending(x => x.Id).ToList();
               
          
            return View(list);

        }
        public ActionResult proprietaires()
        {

            return View();

        }
        [HttpGet]
        public ActionResult propriétaires(proprietaire p)
        {


            var list = db.proprietaire.OrderByDescending(x => x.Id).ToList();


            return View(list);

        }
        public ActionResult detailspropr()
        {

            return View();

        }
        [HttpGet]
        public ActionResult detailspropr(int id, proprietaire p)
        {
            var pr = db.proprietaire.Where(c => c.Id == id).FirstOrDefault();
            return View(pr);
        }
        public ActionResult detailsclts()
        {

            return View();

        }
        [HttpGet]
        public ActionResult detailsclts(int id, client p)
        {
            var cl = db.client.Where(c => c.Id == id).FirstOrDefault();
            return View(cl);
        }
        public ActionResult suppclts(int id)
        {
            var data = db.client.FirstOrDefault(x => x.Id == id);
            var ms = db.message.FirstOrDefault(x => x.fk_client == id);
            if (data != null && ms != null)
            {
                Deletemsg(ms.Id);
                db.client.Remove(data);
                db.SaveChanges();
                return RedirectToAction("clients");
            }
            else if (data != null)
            {
                db.client.Remove(data);
                db.SaveChanges();
                return RedirectToAction("clients");
            }
                return View("clients");
        }
        public ActionResult DeleteArt(int id)
        {
            var data = db.article.FirstOrDefault(x => x.Id == id);
            if (data != null)
            {
                db.article.Remove(data);
                db.SaveChanges();
                return RedirectToAction("propriétaires");
            }
            else
                return View("propriétaires");
        }
        public ActionResult Deletemsg(int id)
        {
            var data = db.message.FirstOrDefault(x => x.Id == id);
            if (data != null)
            {
                db.message.Remove(data);
                db.SaveChanges();
                return RedirectToAction("msg");
            }
            else
                return View("msg");
        }
        public ActionResult Deletemsgprop(int id)
        {
            var data = db.messageprop.FirstOrDefault(x => x.Id == id);
            if (data != null)
            {
                db.messageprop.Remove(data);
                db.SaveChanges();
                return RedirectToAction("msg");
            }
            else
                return View("msg");
        }
        public ActionResult supppropr(int id)
        {
            var data = db.proprietaire.FirstOrDefault(x => x.Id == id);
            var ar = db.article.FirstOrDefault(x => x.fk_prop == id);
            var ms = db.messageprop.FirstOrDefault(x => x.fk_propmsg == id);
            if (data != null && ar!=null && ms!=null)
            {
        
                DeleteArt(ar.Id);
                Deletemsgprop(ms.Id);
               
                db.proprietaire.Remove(data);
                
                db.SaveChanges();
                return RedirectToAction("propriétaires");
            }
            else if(data != null && ar != null)
            {
                DeleteArt(ar.Id);
               

                db.proprietaire.Remove(data);

                db.SaveChanges();
                return RedirectToAction("propriétaires");
            }
            else if(data != null && ms != null)
            {

                Deletemsgprop(ms.Id);

                db.proprietaire.Remove(data);

                db.SaveChanges();
                return RedirectToAction("propriétaires");
            }
                return View("propriétaires");
        }
        public ActionResult Editclts(int id)
        {

            var data = db.client.Where(x => x.Id == id).SingleOrDefault();
            
           
            return View(data);

        }
        [HttpPost]
        public ActionResult Editclts(int id, client a)
        {

            var data = db.client.FirstOrDefault(x => x.Id == id);

            if (data != null)
            {
                
               
                data.Id = a.Id;
                data.nom = a.nom;
                data.prenom = a.prenom;
                data.email = a.email;
                data.adresse = a.adresse;
                data.cin = a.cin;
                data.password = a.password;
                data.telephone = a.telephone;
                data.sexe = a.sexe;
                data.age = a.age;
                if (ModelState.IsValid) {
                db.SaveChanges();

                return RedirectToAction("clients"); }
                else { return View("clients"); }
            }
            else
                return View("clients");

        }
        public ActionResult Editpropr(int id)
        {

            var data = db.proprietaire.Where(x => x.Id == id).SingleOrDefault();


            return View(data);

        }
        [HttpPost]
        public ActionResult Editpropr(int id, proprietaire a)
        {

            var data = db.proprietaire.FirstOrDefault(x => x.Id == id);

            if (data != null)
            {


                data.Id = a.Id;
                data.nom = a.nom;
                data.prenom = a.prenom;
                data.email = a.email;
                data.adresse = a.adresse;
                data.cin = a.cin;
                data.password = a.password;
                data.telephone = a.telephone;
                data.sexe = a.sexe;
                data.fonction = a.fonction;
                data.societe = a.societe;
                data.age = a.age;

                db.SaveChanges();

                return RedirectToAction("propriétaires");
            }
            else
                return View("propriétaires");

        }
        public ActionResult addFavorisAdm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult addFavorisAdm(int id)
        {
            if (Session["favclient"] == null)
            {
                var i = Convert.ToInt32(Session["clientId"]);
                List<FavorisAdmin> clt = new List<FavorisAdmin>();
                var client = db.client.Find(id);
              
                clt.Add(new FavorisAdmin()
                {
               

                    client = client
                });
                Session["favclient"] = clt;
            }
            else
            {
                List<FavorisAdmin> clt= (List<FavorisAdmin>)Session["favclient"];
                var i = Convert.ToInt32(Session["clientId"]);
               
                var client = db.client.Find(id);
                var c = 0;
                foreach (var item in clt)
                {
                    if (client.Id == item.client.Id)
                    {
                        c++;
                    }
                }
                if (c == 0)
                {
                    clt.Add(new FavorisAdmin()
                    {
                        
                        client = client
                    });
                    Session["favclient"] = clt;
                }
            }
            return View("favorisAdClt");
        }
        public ActionResult favorisAdClt()
        {
            return View();
        }
        [HttpGet]
        public ActionResult favorisAdClt(client c)
        {
            if (Session["favclient"] != null)
            {
                List<FavorisAdmin> clt = (List<FavorisAdmin>)Session["favclient"];
                var list = clt.ToList();
                return View(list);
            }
            else
            {
                return View();
            }
        }
        public ActionResult Deletefavclt(int id)
        {
            List<FavorisAdmin> list = (List<FavorisAdmin>)Session["favclient"];

            foreach (var item in list)
            {
                if (item.client.Id == id)
                {

                    list.Remove(item);
                    break;

                }
            }
            Session["favclient"] = list;
            return RedirectToAction("clients");
        }
        public ActionResult addFavorisAdmprop()
        {
            return View();
        }

        [HttpGet]
        public ActionResult addFavorisAdmprop(int id)
        {
            if (Session["favprop"] == null)
            {
                var i = Convert.ToInt32(Session["propId"]);
                List<FavorisAdprop> prop = new List<FavorisAdprop>();
                var propr = db.proprietaire.Find(id);

                prop.Add(new FavorisAdprop()
                {


                    proprietaire = propr
                });
                Session["favprop"] = prop;
            }
            else
            {
                List<FavorisAdprop> prop = (List<FavorisAdprop>)Session["favprop"];
                var i = Convert.ToInt32(Session["propId"]);

                var propr = db.proprietaire.Find(id);
                var c = 0;
                foreach (var item in prop)
                {
                    if (propr.Id == item.proprietaire.Id)
                    {
                        c++;
                    }
                }
                if (c == 0)
                {
                    prop.Add(new FavorisAdprop()
                    {

                        proprietaire = propr
                    });
                    Session["favprop"] = prop;
                }
            }
            return View("favorisAdClt");
        }
        public ActionResult Deletefavpropr(int id)
        {
            List<FavorisAdprop> list = (List<FavorisAdprop>)Session["favprop"];

            foreach (var item in list)
            {
                if (item.proprietaire.Id == id)
                {

                    list.Remove(item);
                    break;

                }
            }
            Session["favprop"] = list;
            return RedirectToAction("propriétaires");
        }

        public ActionResult addlistenoireprop()
        {
            return View();
        }

        [HttpGet]
        public ActionResult addlistenoireprop(int id)
        {
            if (Session["listenoireprop"] == null)
            {
                var i = Convert.ToInt32(Session["propId"]);
                List<Listenoireprop> prop = new List<Listenoireprop>();
                List<FavorisAdprop> f = (List<FavorisAdprop>)Session["favprop"];
                var propr= db.proprietaire.Find(id);
                var x = 0;
                foreach (var item in f)
                {
                    if (propr.Id == item.proprietaire.Id)
                    {
                        x++;
                    }
                }
                if (x == 0) { 
                prop.Add(new Listenoireprop()
                {


                    proprietaire = propr
                });
                Session["listenoireprop"] = prop;}
                else
                {
                    Deletefavpropr(propr.Id);
                   
                        prop.Add(new Listenoireprop()
                        {

                            proprietaire = propr
                        });
                        Session["listenoireprop"] = prop;
                    
                }
            }
            else
            {
                List<Listenoireprop> prop = (List<Listenoireprop>)Session["listenoireprop"];
                List<FavorisAdprop> f = (List<FavorisAdprop>)Session["favprop"];
                var i = Convert.ToInt32(Session["propId"]);

                var propr = db.proprietaire.Find(id);
                var c = 0;
                var x = 0;
                foreach(var item in f)
                {
                    if (propr.Id == item.proprietaire.Id)
                    {
                        x++;
                    }
                }
                foreach (var item in prop)
                {
                    if (propr.Id == item.proprietaire.Id)
                    {
                        c++;
                    }
                }
                if (x == 0) { 
                      if (c == 0)
                      {
                    prop.Add(new Listenoireprop()
                    {

                        proprietaire = propr
                    });
                    Session["listenoireprop"] = prop;
                    }
                }
                else
                {
                    Deletefavpropr(propr.Id);
                    if (c == 0)
                    {
                        prop.Add(new Listenoireprop()
                        {

                            proprietaire = propr
                        });
                        Session["listenoireprop"] = prop;
                    }
                }
            }
            return View("listenoire");
        }
        public ActionResult listenoire()
        {
            return View();
        }
        [HttpGet]
        public ActionResult listenoire(client c)
        {
            if (Session["listenoireclt"] != null)
            {
                List<Listenoireclt> clt = (List<Listenoireclt>)Session["listenoireclt"];
                var list = clt.ToList();
                return View(list);
            }
            else
            {
                return View();
            }
        }
        public ActionResult Deletelistenoireprop(int id)
        {
            List<Listenoireprop> list = (List<Listenoireprop>)Session["listenoireprop"];

            foreach (var item in list)
            {
                if (item.proprietaire.Id == id)
                {

                    list.Remove(item);
                    break;

                }
            }
            Session["listenoireprop"] = list;
            return RedirectToAction("propriétaires");
        }
        public ActionResult addlistenoireclt()
        {
            return View();
        }

        [HttpGet]
        public ActionResult addlistenoireclt(int id)
        {
            if (Session["listenoireclt"] == null)
            {
                var i = Convert.ToInt32(Session["clientId"]);
                List<Listenoireclt> client = new List<Listenoireclt>();
                List<FavorisAdmin> cl = (List<FavorisAdmin>)Session["favclient"];
                var clt = db.client.Find(id);
                var x = 0;
                foreach (var item in cl)
                {
                    if (clt.Id == item.client.Id)
                    {
                        x++;
                    }
                }
                if (x == 0) { 
                client.Add(new Listenoireclt()
                {


                    client = clt
                });
                Session["listenoireclt"] = client;}
                else
                {
                    Deletefavclt(clt.Id);
                   
                        client.Add(new Listenoireclt()
                        {

                            client = clt
                        });
                        Session["listenoireclt"] = client;
                    

                }
            }
            else
            {
                List<Listenoireclt> client = (List<Listenoireclt>)Session["listenoireclt"];
                List<FavorisAdmin> cl = (List<FavorisAdmin>)Session["favclient"];
                var i = Convert.ToInt32(Session["clientId"]);

                var clt = db.client.Find(id);
                var c = 0;
                var x = 0;
                foreach (var item in cl)
                {
                    if (clt.Id == item.client.Id)
                    {
                        x++;
                    }
                }
                foreach (var item in client)
                {
                    if (clt.Id == item.client.Id)
                    {
                        c++;
                    }
                }
                if (x == 0) { 
                if (c == 0)
                {
                    client.Add(new Listenoireclt()
                    {

                        client = clt
                    });
                    Session["listenoireclt"] = client;
                }}
                else
                {
                    Deletefavclt(clt.Id);
                    if (c == 0)
                    {
                        client.Add(new Listenoireclt()
                        {

                            client = clt
                        });
                        Session["listenoireclt"] = client;
                    }
                }
            }
            return View("listenoire");
        }
        public ActionResult Deletelistenoireclt(int id)
        {
            List<Listenoireclt> list = (List<Listenoireclt>)Session["listenoireclt"];

            foreach (var item in list)
            {
                if (item.client.Id == id)
                {

                    list.Remove(item);
                    break;

                }
            }
            Session["listenoireclt"] = list;
            return RedirectToAction("clients");
        }


        public ActionResult recherche(string searchString)
        {
            var client = from c in db.client select c;

            if (!string.IsNullOrEmpty(searchString))

            {
                var f = client.Where(s => s.nom.ToLower().Contains(searchString.ToLower()) || s.prenom.ToLower().Contains(searchString.ToLower())  || s.email.ToLower().Contains(searchString.ToLower())).ToList();

                return View("clients", f);
            }

            return View("clients", client);
        }


        public ActionResult addclient()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addclient(client c)
        {
            if (ModelState.IsValid)
            {
               
                    db.client.Add(c);
                    db.SaveChanges();
                    return Redirect(Url.Action("clients", "Admin"));
                
               
            }

            else
            {
                return View();
            }
            return View();
        }
        public ActionResult addprop()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addprop(proprietaire p)
        {
            if (ModelState.IsValid)
            {

                db.proprietaire.Add(p);
                db.SaveChanges();
                return Redirect(Url.Action("propriétaires", "Admin"));


            }

            else
            {
                return View();
            }
            return View();
        }















    }

}