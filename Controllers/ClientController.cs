using aspprojet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace aspprojet.Controllers
{
    public class ClientController : Controller
    {
       
        ecommerceEntities1 db = new ecommerceEntities1();
        //dashboard du client
        public ActionResult Send()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Send(Gmail gmail)
        {
         
                MailMessage m = new MailMessage(Session["clientemail"].ToString(), gmail.To);
                m.Subject = gmail.Subject;
                m.Body = gmail.Body;
                m.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Timeout = 1000000;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                NetworkCredential nc = new NetworkCredential(Session["clientemail"].ToString(), Session["clientpass"].ToString());
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = nc;
                smtp.Send(m);
            
            return View();
        }
        public ActionResult dashClient()
        {
            return View();
        }
        //Inscription du client
        public ActionResult inscClient()
        {
            return View();
        }
        [HttpPost]
        public ActionResult inscClient(client c)
        {
            if (ModelState.IsValid)
            {
                var verifier = db.client.Where(clt => clt.email.Equals(c.email)).SingleOrDefault();
                if (verifier == null)
                {
                    Session["clientId"] = c.Id.ToString();
                    Session["clientprenom"] = c.prenom.ToString();
                    Session["clientnom"] = c.nom.ToString();
                    Session["clientemail"] = c.email.ToString();
                    Session["clientpass"] = c.password.ToString();
                    db.client.Add(c);
                    db.SaveChanges();
                    return Redirect(Url.Action("articleClient", "Client"));
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
        //Authentification du client
        public ActionResult authClient()
        {
            return View();
        }
        [HttpPost]
        public ActionResult authClient(client clt)

        {

            var verifier = db.client.Where(c => c.email.Equals(clt.email) && c.password.Equals(clt.password)).SingleOrDefault();
            if (verifier != null)
            {
                Session["clientId"] = verifier.Id.ToString();
                Session["clientprenom"] = verifier.prenom.ToString();
                Session["clientnom"] = verifier.nom.ToString();
                Session["clientemail"] = verifier.email.ToString();
                Session["clientpass"] = verifier.password.ToString();
                if (Session["historiqueclt"] == null)
                {
                    var j = Convert.ToInt32(Session["clientId"]);
                    List<historiqueclt> hist = new List<historiqueclt>();
                    var cl = db.client.Find(j);
                    hist.Add(new historiqueclt()
                    {
                        action = "s'est connecté(e)",
                        client = cl,
                        date = DateTime.Now
                    });
                    Session["historiqueclt"] = hist;
                }
                else
                {
                    List<historiqueclt> hist = (List<historiqueclt>)Session["historiqueclt"];
                    var j = Convert.ToInt32(Session["clientId"]);

                    var cl = db.client.Find(j);
                    hist.Add(new historiqueclt()
                    {
                        action = "s'est connecté(e)",
                        client = cl,
                        date = DateTime.Now
                    });
                    Session["historiqueclt"] = hist;
                }
                return Redirect(Url.Action("articleClient", "Client"));
            }
            else
            {
                ViewBag.error = "Mot de passe ou Email incorrect!";
                return View();
            }

        }
        //Déconnexion 
        public ActionResult Deconnecter()
        {
            if (Session["historiqueclt"] == null)
            {
                var j = Convert.ToInt32(Session["clientId"]);
                List<historiqueclt> hist = new List<historiqueclt>();
                var cl = db.client.Find(j);
                hist.Add(new historiqueclt()
                {
                    action = "s'est déconnecté(e)",
                   client = cl,
                    date = DateTime.Now
                });
                Session["historiqueclt"] = hist;
            }
            else
            {
                List<historiqueclt> hist = (List<historiqueclt>)Session["historiqueclt"];
                var j = Convert.ToInt32(Session["clientId"]);

                var cl = db.client.Find(j);
                hist.Add(new historiqueclt()
                {
                    action = "s'est déconnecté(e)",
                    client = cl,
                    date = DateTime.Now
                });
                Session["historique"] = hist;
            }
            Session["clientnom"] = null;
            Session["clientprenom"] = null;
            Session["clientId"] = null;

            return Redirect(Url.Action("Acceuil", "Accueil"));

        }
        //Liste des articles
        public ActionResult articleClient()
        {
            return View();
        }
        [HttpGet]
        public ActionResult articleClient(article c)
        {
            var list = db.article.ToList();
            return View(list);
        }
        public ActionResult histoClient()
        {
            return View();
        }
        [HttpGet]
        public ActionResult histoClient(article c)
        {
            var j = Convert.ToInt32(Session["clientId"]);
    
            var cl = db.client.Find(j);
            List<historiqueclt> hist = (List<historiqueclt>)Session["historiqueclt"];
            if (Session["historiqueclt"] != null )
            {
              
                        var list = hist.Where(x=>x.client.Id==cl.Id).ToList();
                        return View(list);
                    
                

              
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
            var list = db.article.Where(x=>x.fk_offre!=null).ToList();
            return View(list);
        }
        //Ajouter au panier
        public ActionResult addPanier()
        {
            return View();
        }

        [HttpGet]
        public ActionResult addPanier(int id)
        {
            if (Session["cart"] == null)
            {               
                var i = Convert.ToInt32(Session["clientId"]);
                List<Panier> cart = new List<Panier>();
                var product = db.article.Find(id);
                var client = db.client.Find(i);
                cart.Add(new Panier()
                {
                   art = product,
                   
                    clt=client
                });
                Session["cart"] = cart;
            }
            else 
            {
                List<Panier> cart = (List<Panier>)Session["cart"];
                var i = Convert.ToInt32(Session["clientId"]);
                var product = db.article.Find(id);
                var client = db.client.Find(i);
                var c = 0;
                foreach(var item in cart)
                {
                    if (product.Id == item.art.Id)
                    {
                        c++;
                    }
                }
                if (c == 0) { 
                cart.Add(new Panier()
                {
                    art = product,
                   
                      clt = client
                });
                Session["cart"] = cart;}
            }
            if (Session["historiqueclt"] == null)
            {
                var j = Convert.ToInt32(Session["clientId"]);
                List<historiqueclt> hist = new List<historiqueclt>();
                var cl = db.client.Find(j);
                var ar = db.article.Find(id);
                hist.Add(new historiqueclt()
                {
                    action = "a ajouté l'article "+ ar.Id.ToString()+" au panier",
                    client = cl,
                    date = DateTime.Now
                });
                Session["historiqueclt"] = hist;
            }
            else
            {
                List<historiqueclt> hist = (List<historiqueclt>)Session["historiqueclt"];
                var j = Convert.ToInt32(Session["clientId"]);
                var ar = db.article.Find(id);
                var cl = db.client.Find(j);
                hist.Add(new historiqueclt()
                {
                    action = "a ajouté l'article " + ar.Id.ToString() + " au panier",
                    client = cl,
                    date = DateTime.Now
                });
                Session["historiqueclt"] = hist;
            }
            return View("panierClt");
        }
      
       //Supprimer article du panier
        public ActionResult DeleteArt(int id)
        {
            List<Panier> cart = (List<Panier>)Session["cart"];
          
           foreach(var item in cart) {
                if (item.art.Id == id) { 

            cart.Remove(item);
                break;
           
            }}
            Session["cart"] = cart;
            if (Session["historiqueclt"] == null)
            {
                var j = Convert.ToInt32(Session["clientId"]);
                List<historiqueclt> hist = new List<historiqueclt>();
                var cl = db.client.Find(j);
                var ar = db.article.Find(id);
                hist.Add(new historiqueclt()
                {
                    action = "a supprimé l'article " + ar.Id.ToString() + " du panier",
                    client = cl,
                    date = DateTime.Now
                });
                Session["historiqueclt"] = hist;
            }
            else
            {
                List<historiqueclt> hist = (List<historiqueclt>)Session["historiqueclt"];
                var j = Convert.ToInt32(Session["clientId"]);
                var ar = db.article.Find(id);
                var cl = db.client.Find(j);
                hist.Add(new historiqueclt()
                {
                    action = "a supprimé l'article " + ar.Id.ToString() + " du panier",
                    client = cl,
                    date = DateTime.Now
                });
                Session["historiqueclt"] = hist;
            }
            return RedirectToAction("articleClient"); 
        }
        //affichage du panier
       public ActionResult panierClt()
        {
            return View();
        }
        [HttpGet]
        public ActionResult panierClt(article c)
        {
            if (Session["cart"] != null)
            {
                List<Panier> cart = (List<Panier>)Session["cart"];
                var list = cart.ToList();
                return View(list);
            }
            else
            {
                return View();
            }
        }
      //Rechercher un article
        public ActionResult recherche(string searchString)
        {
            var art = from m in db.article select m;

            if (!string.IsNullOrEmpty(searchString))

            {
                var f = art.Where(s => s.nom_art.ToLower().Contains(searchString.ToLower())||  s.proprietaire.nom.ToLower().Contains(searchString.ToLower())||  s.proprietaire.prenom.ToLower().Contains(searchString.ToLower())).ToList();
             
                return View("articleClient", f);
            }
  
            return View("articleClient",art);
        }

        //ajouter favoris
         public ActionResult addfavoris()
       {
           return View();
       }
       [HttpGet]
       public ActionResult addfavoris(int id)
       {
           if (Session["favoris"] == null)
           {
               var j = Convert.ToInt32(Session["clientId"]);
               List<Favoris> cart = new List<Favoris>();
               var product = db.article.Find(id);
               var client = db.client.Find(j);
               cart.Add(new Favoris()
               {
                   produit = product,

                   client = client
               });
               Session["favoris"] = cart;
           }
           else
           {
               List<Favoris> cart = (List<Favoris>)Session["favoris"];
               var j = Convert.ToInt32(Session["clientId"]);
               var product = db.article.Find(id);
               var client = db.client.Find(j);
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
                   cart.Add(new Favoris()
                   {
                       produit = product,

                       client = client
                   });
                   Session["favoris"] = cart;
               }
           }
            if (Session["historiqueclt"] == null)
            {
                var j = Convert.ToInt32(Session["clientId"]);
                List<historiqueclt> hist = new List<historiqueclt>();
                var cl = db.client.Find(j);
                var ar = db.article.Find(id);
                hist.Add(new historiqueclt()
                {
                    action = "a ajouté l'article " + ar.Id.ToString() + " au favoris",
                    client = cl,
                    date = DateTime.Now
                });
                Session["historiqueclt"] = hist;
            }
            else
            {
                List<historiqueclt> hist = (List<historiqueclt>)Session["historiqueclt"];
                var j = Convert.ToInt32(Session["clientId"]);
                var ar = db.article.Find(id);
                var cl = db.client.Find(j);
                hist.Add(new historiqueclt()
                {
                    action = "a ajouté l'article " + ar.Id.ToString() + " au favoris",
                    client = cl,
                    date = DateTime.Now
                });
                Session["historiqueclt"] = hist;
            }
            return View("favorisClt");
       }
        //Supprimer article du favoris
        public ActionResult Deletefav(int id)
        {
            List<Favoris> cart = (List<Favoris>)Session["favoris"];

            foreach (var item in cart)
            {
                if (item.produit.Id == id)
                {

                    cart.Remove(item);
                    break;

                }
            }
            Session["favoris"] = cart;
            if (Session["historiqueclt"] == null)
            {
                var j = Convert.ToInt32(Session["clientId"]);
                List<historiqueclt> hist = new List<historiqueclt>();
                var cl = db.client.Find(j);
                var ar = db.article.Find(id);
                hist.Add(new historiqueclt()
                {
                    action = "a supprimé l'article " + ar.Id.ToString() + " du favoris",
                    client = cl,
                    date = DateTime.Now
                });
                Session["historiqueclt"] = hist;
            }
            else
            {
                List<historiqueclt> hist = (List<historiqueclt>)Session["historiqueclt"];
                var j = Convert.ToInt32(Session["clientId"]);
                var ar = db.article.Find(id);
                var cl = db.client.Find(j);
                hist.Add(new historiqueclt()
                {
                    action = "a supprimé l'article " + ar.Id.ToString() + " du favoris",
                    client = cl,
                    date = DateTime.Now
                });
                Session["historiqueclt"] = hist;
            }
            return RedirectToAction("articleClient");
        }
        //liste des favoris
        public ActionResult favorisClt()
       {
           return View();
       }
       [HttpGet]
       public ActionResult favorisClt(article c)
       {
            if (Session["favoris"] != null) { 
           List<Favoris> cart = (List<Favoris>)Session["favoris"];
           var list = cart.ToList();
           return View(list);}
            else
            {
                return View();
            }
       }
       //contact administrateur
        public ActionResult contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult contact(message m)
        {
            if (ModelState.IsValid)
            {
                m.fk_client = Convert.ToInt32(Session["clientId"]);
                db.message.Add(m);
                db.SaveChanges();
                if (Session["historiqueclt"] == null)
                {
                    var j = Convert.ToInt32(Session["clientId"]);
                    List<historiqueclt> hist = new List<historiqueclt>();
                    var cl = db.client.Find(j);
               
                    hist.Add(new historiqueclt()
                    {
                        action = "vous a envoyé un message",
                        client = cl,
                        date = DateTime.Now
                    });
                    Session["historiqueclt"] = hist;
                }
                else
                {
                    List<historiqueclt> hist = (List<historiqueclt>)Session["historiqueclt"];
                    var j = Convert.ToInt32(Session["clientId"]);
                   
                    var cl = db.client.Find(j);
                    hist.Add(new historiqueclt()
                    {
                        action = "vous a envoyé un message",
                        client = cl,
                        date = DateTime.Now
                    });
                    Session["historiqueclt"] = hist;
                }
                return Redirect(Url.Action("articleClient", "Client"));
            }
            else
            {
                return View();
            }
                
        }




            //affichage des catégories
            public ActionResult cat1()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Vêtement_femmes")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat2()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Vêtement_Hommes")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat3()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Vêtement_enfants")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat4()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Maquillage")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat5()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Soins")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat6()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Decoration")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat7()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Rangement")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat8()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Organisation")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat9()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Pour_maison")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat10()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Informatique")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat11()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Montres")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat12()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Automobile")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat13()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("Motor")).ToList();
            return View("articleClient", f);
        }
        public ActionResult cat14()
        {
            var art = from m in db.article select m;
            var f = art.Where(s => s.categorie.Equals("bicyclette")).ToList();
            return View("articleClient", f);
        }
    }
}
