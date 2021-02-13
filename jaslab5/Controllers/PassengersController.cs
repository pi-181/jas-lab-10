using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

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
                ViewBag.Message = $"Cabin: {cab.CabinName}. Editing passenger: {p.FirstName} {p.LastName}";
                
                return View("Index", new Tuple<int, IEnumerable<Passenger>, Passenger>(
                    cab.Id, new List<Passenger>(passDao.GetPassengerByCabin(cab.Id)), p
                ));
            }
        }
        
        public ActionResult Report(int cabinId)
        {
            IList<Passenger> passengers;
            using (var session = NHibernateHelper.OpenSession())
            {
                passengers = new NHibernateDAOFactory(session).getPassengerDAO().GetPassengerByCabin(cabinId);
            }
            
            var content = Pdf.PdfSharpConvert(ToTable(passengers, false));
            return new FileContentResult(content, "application/pdf");
        }
        
        public static string ToTable(IList<Passenger> passengers, bool builtIn)
        {
            const string standaloneContainer = "<caption>Passengers</caption><thead>" +
                                              "<tr>" +
                                              "<th>Name</th>" +
                                              "<th>Surname</th>" +
                                              "<th>Sex</th>" +
                                              "</tr>" +
                                              "</thead>" +
                                              "<tbody>";
            var header = "<table border=1 bordercolor=black style=\"width:100%\">";
            
            if (!builtIn) header += standaloneContainer;
            var footer = (builtIn ? "" : "</tbody>") + "</table>";

            var builder = new StringBuilder();
            builder.Append(header);
            
            if (builtIn) // crutch to hide blank row, library bug
            {
                builder.Append("<tr border=0 style=\"height=0px;\">" +
                               "<td border=0 style=\"height=0px;\"></td>" +
                               "<td border=0 style=\"height=0px;\"></td>" +
                               "<td border=0 style=\"height=0px;\"></td>" +
                               "</tr>");
            }
            foreach (var passenger in passengers)
            {
                builder.Append(ToRow(passenger));
            }
            
            builder.Append(footer);
            return builder.ToString();
        }
        
        public static string ToRow(Passenger p)
        {
            return $"<tr><td>{p.FirstName}</td><td>{p.LastName}</td><td>{p.Sex}</td></tr>";
        }
    }
}