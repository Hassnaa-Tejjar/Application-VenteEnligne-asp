using aspprojet.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aspprojet.Controllers
{
    public class PropController : Controller
    {
        // GET: Prop
        ecommerceEntities1 db = new ecommerceEntities1();
        public ActionResult dashProp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult dashProp(proprietaire p)
        {
            if (Session["propId"] == null )
            {
                return RedirectToAction("authProp");
            }
            else
            {
    
                return View();
            }

        }
  
        public ActionResult inscProp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult inscProp(proprietaire c)
        {
            if (ModelState.IsValid)
            {
                var verifier = db.proprietaire.Where(clt => clt.email.Equals(c.email)).SingleOrDefault();
                if (verifier == null)
                {
                    Session["propId"] = c.Id.ToString();
                    Session["propprenom"] = c.prenom.ToString();
                    Session["propnom"] = c.nom.ToString();
                    Session["propemail"] = c.email.ToString();
                    db.proprietaire.Add(c);
                    db.SaveChanges();
                    return Redirect(Url.Action("articleProp", "Prop"));
                }
                else
                {
                    ViewBag.error = "Email existe déjà!";
                    return View();
                }
            }

            else
            {
                return View();
            }
            return View();
        }
        public ActionResult authProp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult authProp(proprietaire clt)

        {

            var verifier = db.proprietaire.Where(c => c.email.Equals(clt.email) && c.password.Equals(clt.password)).SingleOrDefault();
            if (verifier != null)
            {
                Session["propId"] = verifier.Id.ToString();
                Session["propprenom"] = verifier.prenom.ToString();
                Session["propnom"] = verifier.nom.ToString();
                Session["propemail"] = verifier.email.ToString();
            if (Session["historique"] == null)
            {
                var j = Convert.ToInt32(Session["propId"]);
                List<historique> hist = new List<historique>();
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "s'est connecté(e)",
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            else
            {
                List<historique> hist = (List<historique>)Session["historiquep"];
                var j = Convert.ToInt32(Session["propId"]);

                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "s'est connecté(e)",
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
                return Redirect(Url.Action("articleProp", "Prop"));
            }
            else
            {
                ViewBag.error = "Mot de passe ou Email incorrect!";
                return View();
            }
           
        }
        public ActionResult histoProp()
        {
            return View();
        }
        [HttpGet]
        public ActionResult histoProp(article c)
        {
            var j = Convert.ToInt32(Session["propId"]);

            var cl = db.proprietaire.Find(j);
            List<historique> hist = (List<historique>)Session["historique"];
            if (Session["historique"] != null)
            {

                var list = hist.Where(x => x.proprietaire.Id == cl.Id).ToList();
                return View(list);




            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult AddArticle()
        {
            var offrelist = db.offre.ToList();
            ViewBag.offrelist = new SelectList(offrelist, "Id", "prix_offre");
          
            return View();
        }
        [HttpPost]
        public ActionResult AddArticle(article c)
        {

           
            string fileName = Path.GetFileNameWithoutExtension(c.ImageFile.FileName);
            string extension = Path.GetExtension(c.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            c.img_art = "~/Image/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
            c.ImageFile.SaveAs(fileName);
            if (ModelState.IsValid)
            {
                c.fk_prop = Convert.ToInt32(Session["propId"]);
                db.article.Add(c);
                db.SaveChanges();
                if (Session["historique"] == null)
                {
                    var j = Convert.ToInt32(Session["propId"]);
                    List<historique> hist = new List<historique>();
                    var ar = c.Id.ToString();
                    var prop = db.proprietaire.Find(j);
                    hist.Add(new historique()
                    {
                        action = "a ajouté l'article " + ar,
                        proprietaire = prop,
                        date = DateTime.Now
                    });
                    Session["historique"] = hist;
                }
                else
                {
                    List<historique> hist = (List<historique>)Session["historique"];
                    var j = Convert.ToInt32(Session["propId"]);
                    var ar = c.Id.ToString();
                    var prop = db.proprietaire.Find(j);
                    hist.Add(new historique()
                    {
                        action = "a ajouté l'article " + ar,
                        proprietaire = prop,
                        date = DateTime.Now
                    });
                    Session["historique"] = hist;
                }
                return Redirect(Url.Action("articleProp", "Prop"));
            }

            return View();

        }
        public ActionResult articleProp()
        {
            return View();
        }
        [HttpGet]
        public ActionResult articleProp(article a)
        {
            var list = db.article.ToList();
            return View(list);
        }
        public ActionResult articlePropId()
        {
            return View();
        }
        [HttpGet]
        public ActionResult articlePropId(article a,proprietaire p)
        {
            var rawData = (from s in db.article select s).ToList();
            var art = from s in rawData select s;
            var useId = Session["propId"];
            return View(art.Where(t => t.fk_prop.ToString() == useId.ToString()).ToList());
                
            

        }
        public ActionResult articleCat()
        {
            return View();
        }
        [HttpGet]
        public ActionResult articleCat(string femme)
        {
            var rawData = (from s in db.article select s).ToList();
            var art = from s in rawData select s;          
            return View(art.Where(c=>c.categorie.ToString()== "Vêtement_femmes"));

        }
        
        public ActionResult Deconnecter()
        {
          
            if (Session["historique"] == null)
            {
                var j = Convert.ToInt32(Session["propId"]);
                List<historique> hist = new List<historique>();
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "s'est déconnecté(e)",
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            else
            {
                List<historique> hist = (List<historique>)Session["historique"];
                var j = Convert.ToInt32(Session["propId"]);
               
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "s'est déconnecté(e)" ,
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            Session["propnom"] = null;
            Session["propprenom"] = null;
            Session["propId"] = null;
            return Redirect(Url.Action("authProp", "Prop"));

        }
        public ActionResult detailsArt()
        {
            return View();
        }
        [HttpGet]
        public ActionResult detailsArt(int id,article a)
        {
            var article = db.article.Where(c=>c.Id==id).FirstOrDefault();
            if (Session["historique"] == null)
            {
                var j = Convert.ToInt32(Session["propId"]);
                List<historique> hist = new List<historique>();
                var ar = a.Id.ToString();
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "a consulté les détails de l'article " + ar,
                    proprietaire = prop,
                    date = DateTime.Now
                }) ;
                Session["historique"] = hist;
            }
            else
            {
                List<historique> hist = (List<historique>)Session["historique"];
                var j = Convert.ToInt32(Session["propId"]);
                var ar = a.Id.ToString();
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "a consulté les détails de l'article " + ar,
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            return View(article);
        }
        public ActionResult DeleteArt(int id)
        {
            var data = db.article.FirstOrDefault(x => x.Id == id);
            if (data != null)
            {
                db.article.Remove(data);
                db.SaveChanges();
                if (Session["historique"] == null)
                {
                    var j = Convert.ToInt32(Session["propId"]);
                    List<historique> hist = new List<historique>();
                    var ar = data.Id.ToString();
                    var prop = db.proprietaire.Find(j);
                    hist.Add(new historique()
                    {
                        action = "a supprimé l'article  " + ar,
                        proprietaire = prop,
                        date = DateTime.Now
                    });
                    Session["historique"] = hist;
                }
                else
                {
                    List<historique> hist = (List<historique>)Session["historique"];
                    var j = Convert.ToInt32(Session["propId"]);
                    var ar = data.Id.ToString();
                    var prop = db.proprietaire.Find(j);
                    hist.Add(new historique()
                    {
                        action = "a supprimé l'article " + ar,
                        proprietaire = prop,
                        date = DateTime.Now
                    });
                    Session["historique"] = hist;
                }
                return RedirectToAction("articlePropId");
            }
            else
                return View("articlePropId");
        }

     
        public ActionResult EditArt(int id)
        {

            var data = db.article.Where(x => x.Id == id).SingleOrDefault();
            Session["imgPath"] = data.img_art;
            var offrelist = db.offre.ToList();
            ViewBag.offrelist = new SelectList(offrelist, "Id", "prix_offre");
            return View(data);

        }
          [HttpPost]
          [ValidateAntiForgeryToken]
          public ActionResult EditArt(int id, article a)
          {

              var data = db.article.FirstOrDefault(x => x.Id == id);

              if (data != null)
              {
                string fileName = Path.GetFileNameWithoutExtension(a.ImageFile.FileName);
                string extension = Path.GetExtension(a.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                a.img_art = "~/Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                a.ImageFile.SaveAs(fileName);
                data.fk_prop = Convert.ToInt32(Session["propId"]);
                data.ImageFile = a.ImageFile;
                  data.nom_art = a.nom_art;
                  data.description = a.description;
                  data.categorie = a.categorie;
                  data.prix = a.prix;
                  data.img_art = a.img_art;
                  data.marque = a.marque;
                data.fk_offre = a.fk_offre;
               

                  db.SaveChanges();
                if (Session["historique"] == null)
                {
                    var j = Convert.ToInt32(Session["propId"]);
                    List<historique> hist = new List<historique>();
                    var ar = a.Id.ToString();
                    var prop = db.proprietaire.Find(j);
                    hist.Add(new historique()
                    {
                        action = "a modifié l'article " + ar,
                        proprietaire = prop,
                        date = DateTime.Now
                    });
                    Session["historique"] = hist;
                }
                else
                {
                    List<historique> hist = (List<historique>)Session["historique"];
                    var j = Convert.ToInt32(Session["propId"]);
                    var ar = a.Id.ToString();
                    var prop = db.proprietaire.Find(j);
                    hist.Add(new historique()
                    {
                        action = "a modifié l'article " + ar,
                        proprietaire = prop,
                        date = DateTime.Now
                    });
                    Session["historique"] = hist;
                }
                return RedirectToAction("articlePropId");
              }
              else
                  return View("articlePropId");

          }
        public ActionResult profileProp()
        {
            return View();
        }
        [HttpGet]
        public ActionResult profileProp(int id)
        {
            var p = db.proprietaire.Where(c => c.Id == id).FirstOrDefault();
            if (Session["historique"] == null)
            {
                var j = Convert.ToInt32(Session["propId"]);
                List<historique> hist = new List<historique>();
               
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "a consulté son profile" ,
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            else
            {
                List<historique> hist = (List<historique>)Session["historique"];
                var j = Convert.ToInt32(Session["propId"]);
             
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "a consulté son profile " ,
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            return View(p);

        }
        public ActionResult AddOffre()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddOffre(offre o)
        {
            if (ModelState.IsValid)
            {
                db.offre.Add(o);
                db.SaveChanges();
                return Redirect(Url.Action("articleProp", "Prop"));
            }
            else
            {
                return View();
            }
        }



        public ActionResult addfavorisProp()
        {
            return View();
        }
        [HttpGet]
        public ActionResult addfavorisProp(int id)
        {
            if (Session["favorisp"] == null)
            {
                var j = Convert.ToInt32(Session["propId"]);
                List<FavorisProp> cart = new List<FavorisProp>();
                var product = db.article.Find(id);
                var prop = db.proprietaire.Find(j);
                cart.Add(new FavorisProp()
                {
                    produit = product,
                    prop=prop
                  
                });
                Session["favorisp"] = cart;
            }
            else
            {
                List<FavorisProp> cart = (List<FavorisProp>)Session["favorisp"];
                var j = Convert.ToInt32(Session["propId"]);
                var product = db.article.Find(id);
                var prop= db.proprietaire.Find(j);
                var c = 0;
                foreach (var item in cart)
                {
                    if (product.Id == item.produit.Id)
                    {
                        c++;
                    }
                }
                if (c == 0)
                {
                    cart.Add(new FavorisProp()
                    {
                        produit = product,

                        prop = prop
                    }); 
                    Session["favorisp"] = cart;
                }
            }
             if (Session["historique"] == null)
            {
                var j = Convert.ToInt32(Session["propId"]);
                List<historique> hist = new List<historique>();
                var ar = db.article.Find(id).Id.ToString();
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "a ajouté l'article" + ar+" au favoris",
                    proprietaire = prop,
                    date = DateTime.Now
                }) ;
                Session["historique"] = hist;
            }
            else
            {
                List<historique> hist = (List<historique>)Session["historique"];
                var j = Convert.ToInt32(Session["propId"]);
                var ar = db.article.Find(id).Id.ToString();
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "a ajouté l'article" + ar + " au favoris",
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            return View("favorisProp");
        }
        //Supprimer article du favoris
        public ActionResult Deletefav(int id)
        {
            List<FavorisProp> cart = (List<FavorisProp>)Session["favorisp"];

            foreach (var item in cart)
            {
                if (item.produit.Id == id)
                {

                    cart.Remove(item);
                    break;

                }
            }
            Session["favorisp"] = cart;
            if (Session["historique"] == null)
            {
                var j = Convert.ToInt32(Session["propId"]);
                List<historique> hist = new List<historique>();
                var ar = db.article.Find(id).Id.ToString();
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "a supprimé l'article" + ar + " du favoris",
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            else
            {
                List<historique> hist = (List<historique>)Session["historique"];
                var j = Convert.ToInt32(Session["propId"]);
                var ar = db.article.Find(id).Id.ToString();
                var prop = db.proprietaire.Find(j);
                hist.Add(new historique()
                {
                    action = "a supprimé l'article" + ar + " du favoris",
                    proprietaire = prop,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            return RedirectToAction("articleProp");
        }
        //liste des favoris
        public ActionResult favorisProp()
        {
            return View();
        }
        [HttpGet]
        public ActionResult favorisProp(article c)
        {
            if (Session["favorisp"] != null)
            {
                List<FavorisProp> cart = (List<FavorisProp>)Session["favorisp"];
                var list = cart.ToList();
                return View(list);
            }
            else
            {
                return View();
            }
        }
        public ActionResult recherche(string searchString)
        {
            var art = from m in db.article select m;

            if (!string.IsNullOrEmpty(searchString))

            {
                var f = art.Where(s => s.nom_art.ToLower().Contains(searchString.ToLower()) || s.proprietaire.nom.ToLower().Contains(searchString.ToLower()) || s.proprietaire.prenom.ToLower().Contains(searchString.ToLower())).ToList();

                return View("articleProp", f);
            }

            return View("articleProp", art);
        }



        //affichage des catégories
        public ActionResult cat1()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Vêtement_femmes")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat2()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Vêtement_Hommes")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat3()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Vêtement_enfants")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat4()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Maquillage")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat5()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Soins")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat6()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Decoration")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat7()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Rangement")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat8()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Organisation")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat9()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Pour_maison")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat10()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Informatique")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat11()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Montres")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat12()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Automobile")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat13()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Motor")).ToList();
            return View("articleProp", f);
        }
        public ActionResult cat14()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("bicyclette")).ToList();
            return View("articleProp", f);
        }
        //contact administrateur
         public ActionResult contact()
        {
            return View();
        }
        [HttpPost]
      public ActionResult contact(messageprop m)
        {
            if (ModelState.IsValid)
            {
                m.fk_propmsg = Convert.ToInt32(Session["propId"]);
                db.messageprop.Add(m);
                db.SaveChanges();
                if (Session["historique"] == null)
                {
                    var j = Convert.ToInt32(Session["propId"]);
                    List<historique> hist = new List<historique>();
                   
                    var prop = db.proprietaire.Find(j);
                    hist.Add(new historique()
                    {
                        action = "vous a envoyé un message" ,
                        proprietaire = prop,
                        date = DateTime.Now
                    });
                    Session["historique"] = hist;
                }
                else
                {
                    List<historique> hist = (List<historique>)Session["historique"];
                    var j = Convert.ToInt32(Session["propId"]);
                 
                    var prop = db.proprietaire.Find(j);
                    hist.Add(new historique()
                    {
                        action = "vous a envoyé un message",
                        proprietaire = prop,
                        date = DateTime.Now
                    });
                    Session["historique"] = hist;
                }
                return Redirect(Url.Action("articleProp", "Prop"));
            }
            else
            {
                return View();
            }

        }
     
        public ActionResult offreClient()
        {
            return View();
        }
        [HttpGet]
        public ActionResult offreClient(article c)
        {
            var list = db.article.Where(x => x.fk_offre != null).ToList();
            return View(list);
        }
        //Ajouter au panier
        public ActionResult addPanier()
        {
            return View();
        }
    }
}