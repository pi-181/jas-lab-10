using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace jaslab5.Controllers
{
    public class CabinsController : Controller
    {
        private readonly Cabin _emptyCabin = new Cabin();

        public ActionResult Index()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var genericDao = new NHibernateDAOFactory(session).getCabinDAO();
                ViewBag.Message = "Cabins List";

                return View("index", new Tuple<IEnumerable<Cabin>, Cabin>(
                    new List<Cabin>(genericDao.GetAll()), _emptyCabin
                ));
            }
        }

        [HttpPost]
        public ActionResult PostCabin(Cabin cabin)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                new NHibernateDAOFactory(session).getCabinDAO().SaveOrUpdate(cabin);
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var genericDao = new NHibernateDAOFactory(session).getCabinDAO();
                genericDao.Delete(genericDao.GetById(id));
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var genericDao = new NHibernateDAOFactory(session).getCabinDAO();
                var cabin = genericDao.GetById(id);
                ViewBag.Message = "Editing cabin: " + cabin.CabinName;

                return View("index", new Tuple<IEnumerable<Cabin>, Cabin>(
                    new List<Cabin>(genericDao.GetAll()), cabin
                ));
            }
        }
    }
}