﻿using aspprojet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aspprojet.Controllers
{
    public class GmailController : Controller
    {
        // GET: Gmail
        public ActionResult Send()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Send(Gmail gmail)
        {
            //gmail.sendMail();
            return View();
        }
    }
}