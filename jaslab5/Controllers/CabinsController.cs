using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using FluentNHibernate.Conventions;

namespace jaslab5.Controllers
{
    public class CabinsController : Controller
    {
        private static readonly Cabin _EmptyCabin = new Cabin();

        public ActionResult Index()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var genericDao = new NHibernateDAOFactory(session).getCabinDAO();
                ViewBag.Message = "Cabins List";

                return View("index", new Tuple<IEnumerable<Cabin>, Cabin>(
                    new List<Cabin>(genericDao.GetAll()), _EmptyCabin
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
        
        public ActionResult Report()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var cabins = new NHibernateDAOFactory(session).getCabinDAO().GetAll();
                var content = Pdf.PdfSharpConvert(ToTable(cabins));
                return new FileContentResult(content, "application/pdf");
            }
        }

        public static string ToTable(IList<Cabin> cabins)
        {
            const string header = "<table style=\"width:100%\" border=1 bordercolor=black>" +
                                  "<caption>Cabins</caption>" +
                                  "<thead>" +
                                  "<tr>" +
                                  "<th>Name</th>" +
                                  "<th>Square</th>" +
                                  "<th>Class</th>" +
                                  "<th>Passengers</th>" +
                                  "</tr>" +
                                  "</thead>" +
                                  "<tbody>";
            const string footer = "</tbody></table>";

            var builder = new StringBuilder();
            builder.Append(header);

            if (cabins.IsEmpty())
            {
                builder.Append(ToRow(_EmptyCabin));
            }
            else
            {
                foreach (var cabin in cabins)
                {
                    builder.Append(ToRow(cabin));
                }
            }
            

            builder.Append(footer);
            
            return builder.ToString();
        }

        public static string ToRow(Cabin cab)
        {
            return "<tr>" +
                   $"<td>{cab.CabinName}</td>" +
                   $"<td>{cab.Square}</td>" +
                   $"<td>{cab.ClassName}</td>" +
                   $"<td style=\"height=0px\">{PassengersController.ToTable(cab.Passengers, true)}</td>" +
                   "</tr>";
        }
    }
}