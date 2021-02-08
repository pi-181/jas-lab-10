using System.Collections.Generic;
using NHibernate;

namespace jaslab5
{
    public interface IGenericDAO<T>
    {
        void SaveOrUpdate(T item);

        T GetById(int id);

        IList<T> GetAll();

        void Delete(T item);

    }
    
    public class GenericDAO<T> : IGenericDAO<T>
    { 
        protected ISession session;

        public GenericDAO() { }

        public GenericDAO(ISession session)
        {
            this.session = session;
        }

        public void SaveOrUpdate(T item)
        {
            var transaction = session.BeginTransaction();
            session.SaveOrUpdate(item);
            transaction.Commit();
            session.Flush();
        }

        public T GetById(int id)
        {
            return session.Get<T>(id);
        }

        public IList<T> GetAll()
        {
            return session
                .CreateCriteria(typeof(T))
                .List<T>();
        }

        public void Delete(T item)
        {
            var transaction = session.BeginTransaction();
            session.Delete(item);
            transaction.Commit();
            session.Flush();
        }
    }

    public interface IPassengerDAO : IGenericDAO<Passenger>
    {
        IEnumerable<Passenger> GetPassengerByCabin(int cabinId);
    }
    
    public class PassengerDAO : GenericDAO<Passenger>, IPassengerDAO
    {
        public PassengerDAO(ISession session) : base(session)
        {
            
        }

        public IEnumerable<Passenger> GetPassengerByCabin(int cabin_id)
        {
            return session.CreateSQLQuery("SELECT * FROM passengers WHERE cabin_id = " + cabin_id)
                .AddEntity("Passenger", typeof(Passenger))
                .List<Passenger>();
        }
    }

    public abstract class DAOFactory
    {
        public abstract IGenericDAO<Cabin> getCabinDAO();
        public abstract IPassengerDAO getPassengerDAO();
    }
    
    public class NHibernateDAOFactory : DAOFactory
    {
        /** NHibernate sessionFactory */
        protected ISession session = null;

        public NHibernateDAOFactory(ISession session)
        {
            this.session = session;
        }

        public override IGenericDAO<Cabin> getCabinDAO() 
        {
            return new GenericDAO<Cabin>(session);
        }

        public override IPassengerDAO getPassengerDAO()
        {
            return new PassengerDAO(session);
        }
    }
    
}