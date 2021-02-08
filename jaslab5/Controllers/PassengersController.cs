using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace jaslab5.Controllers
{
    public class PassengersController : Controller
    {
        
        public ActionResult GetByCabin(int cabinId)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var factory = new NHibernateDAOFactory(session);
                var pass = factory.getPassengerDAO();
                var cab = factory.getCabinDAO();

                var cabin = cab.GetById(cabinId);
                ViewBag.Message = $"Cabin: {cabin.CabinName}.";
                return View("index", new Tuple<int, IEnumerable<Passenger>, Passenger>(
                    cabinId, new List<Passenger>(pass.GetPassengerByCabin(cabinId)), new Passenger
                    {
                        Cabin = cabin
                    }
                ));
            }
        }
        
        [HttpPost]
        public ActionResult PostPassenger(int cabinId, Passenger passenger)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var daoFactory = new NHibernateDAOFactory(session);
                var passengerCabin = daoFactory.getCabinDAO().GetById(cabinId);
                if (passengerCabin == null)
                    throw new NullReferenceException("Cabin can't be null!");
                
                passenger.Cabin = passengerCabin;
                daoFactory.getPassengerDAO().SaveOrUpdate(passenger);
            }
            
            return RedirectToAction("GetByCabin", new { cabinId });
        }
        
        public ActionResult Delete(int cabinId, int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var genericDao = new NHibernateDAOFactory(session).getPassengerDAO();
                genericDao.Delete(genericDao.GetById(id));
            }
            return RedirectToAction("GetByCabin", new { cabinId });
        }
        
        public ActionResult Edit(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var factory = new NHibernateDAOFactory(session);
                var passDao = factory.getPassengerDAO();
                var p = passDao.GetById(id);
                
                var cab = p.Cabin;
                ViewBag.Message = $"Cabin: {cab.CabinName}. Editing passenger: ${p.FirstName} ${p.LastName}";
                
                return View("Index", new Tuple<int, IEnumerable<Passenger>, Passenger>(
                    cab.CabinId, new List<Passenger>(passDao.GetPassengerByCabin(cab.CabinId)), p
                ));
            }
        }
    }
}