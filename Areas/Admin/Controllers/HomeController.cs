﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QuanLyLogin.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            if (Session["UserNameNV"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}