using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Cfg;
using NHibernate.Transform;
using System.Collections;
using System.Diagnostics.Contracts;
using System.IO;
using NHibernate.Linq;


namespace Development.Dal.Base
{
    public class GenericRepository
    {
        protected ISessionFactory _sessionFactory = null;

        public GenericRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }


        #region Public Methods

        /// <summary>
        /// Deletes an object of a specified type.
        /// </summary>
        /// <param name="itemsToDelete">The items to delete.</param>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        public void Delete<T>(T item)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.Delete(item);
        }
        public void CreatdynamicTable(string query)
        {
            ISession session = _sessionFactory.OpenSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            var result = session.CreateQuery(query);
            result.ExecuteUpdate();
        }

        public IQueryable<T> Query<T>()
        {
            ISession session = _sessionFactory.OpenSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            return session.Query<T>();

        }
        public IQueryable<T> GetallAttributes<T>(string entityName)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            //ICriteria targetObjects = session.CreateCriteria(entityName);
            return session.Query<T>();
        }

        /// <summary>
        /// Deletes objects of a specified type.
        /// </summary>
        /// <param name="itemsToDelete">The items to delete.</param>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        public void Delete<T>(IList<T> itemsToDelete)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;

            foreach (T item in itemsToDelete)
            {
                session.Delete(item);
            }

        }

        public void DeleteByID<TEntity>(object id)
        {
            var queryString = string.Format("delete {0} where id = :id",
                                            typeof(TEntity));
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            session.CreateQuery(queryString)
                          .SetParameter("id", id)
                          .ExecuteUpdate();
        }

        //delete without condition
        public void DeleteByID<TEntity>(string propertyName, object id)
        {
            var queryString = string.Format("delete  {0} where " + propertyName + " = : " + propertyName,
                                            typeof(TEntity));
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            session.CreateQuery(queryString)
                          .SetParameter(propertyName, id)
                          .ExecuteUpdate();
        }

        //delete with condition
        public void DeleteByID<TEntity>(IList<MultiProperty> prpList)
        {
            StringBuilder str = new StringBuilder();
            foreach (var value in prpList.ToList())
            {
                str.Append(" and ");
                str.Append(value.propertyName + "=" + value.propertyValue.ToString());
            }
            var queryString = string.Format("delete  {0} where 1=1 " + str.ToString(),
                                            typeof(TEntity));
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            session.CreateQuery(queryString)
                          .ExecuteUpdate();
        }

        //kanchana
        public void UpdateByID<TEntity>(IList<MultiProperty> prpList, IList<MultiProperty> condList)
        {

            StringBuilder str = new StringBuilder();
            str.Append("update  {0} set  ");
            foreach (var val in prpList.ToList())
            {
                str.Append(val.propertyName + "='" + val.propertyValue.ToString() + "',");
            }
            str.Remove(str.Length - 1, 1);
            str.Append(" where 1=1 ");
            foreach (var value in condList.ToList())
            {
                str.Append(" and ");
                str.Append(value.propertyName + "=" + value.propertyValue.ToString());
            }
            var queryString = string.Format(str.ToString(),
                                            typeof(TEntity));
            ISession session = _sessionFactory.GetCurrentSession();
            session.CreateQuery(queryString)
                          .ExecuteUpdate();
        }

      

        public T Get<T>(object id)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            return session.Get<T>(id);
        }

        public T GetbyCriteria<T>(string field1, string field2, object propertyValue1, object propertyValue2)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;

            ICriteria criteria = session.CreateCriteria(typeof(T));
            criteria.Add(Expression.Eq(field1, propertyValue1)).Add(Expression.Eq(field2, propertyValue2));

            return criteria.UniqueResult<T>();
           
        }

        public T Get<T>(string propertyName, object propertyValue)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;

            // Create a criteria object with the specified criteria
            ICriteria criteria = session.CreateCriteria(typeof(T));
            criteria.Add(Expression.Eq(propertyName, propertyValue));
            return criteria.UniqueResult<T>();
        }

        public IList<T> GetAll<T>()
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            ICriteria targetObjects = session.CreateCriteria(typeof(T));
            return targetObjects.List<T>();
        }
        public IList<T> GetAll<T>(string propertyName, object propertyValue)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            ICriteria targetObjects = session.CreateCriteria(typeof(T));
            return targetObjects.List<T>();
        }
        public IList<T> GetAll<T>(string entityName)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            ICriteria targetObjects = session.CreateCriteria(entityName);
            return targetObjects.List<T>();
        }


        /// <summary>
        /// Retrieves objects of a specified type where a specified property equals a specified value.
        /// </summary>
        /// <typeparam name="T">The type of the objects to be retrieved.</typeparam>
        /// <param name="propertyName">The name of the property to be tested.</param>
        /// <param name="propertyValue">The value that the named property must hold.</param>
        /// <returns>A list of all objects meeting the specified criteria.</returns>
        public IList<T> GetEquals<T>(string propertyName, object propertyValue)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;

            // Create a criteria object with the specified criteria
            ICriteria criteria = session.CreateCriteria(typeof(T));
            criteria.Add(Expression.Eq(propertyName, propertyValue));

            // Get the matching objects
            IList<T> matchingObjects = criteria.List<T>();

            // Set return value
            return matchingObjects;

        }

        /// <summary>
        /// Retrieves objects of a specified type where a specified property equals a specified value (multiple property condition check in where clause).
        /// </summary>
        /// <typeparam name="T">The type of the objects to be retrieved.</typeparam>
        /// <MultiProperty> using this class set following properties.</MultiProperty>
        /// <param name="propertyName">The name of the property to be tested.</param>
        /// <param name="propertyValue">The value that the named property must hold.</param>
        /// <returns>A list of all objects meeting the specified criteria.</returns>
        public IList<T> GetEquals<T>(IList<MultiProperty> prpList)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;

            // Create a criteria object with the specified criteria
            ICriteria criteria = session.CreateCriteria(typeof(T));
            foreach (var value in prpList.ToList())
            {
                criteria.Add(Expression.Eq(value.propertyName, value.propertyValue));
            }
            // Get the matching objects
            IList<T> matchingObjects = criteria.List<T>();

            // Set return value
            return matchingObjects;

        }

        /// <summary>
        /// Saves an object and its persistent children.
        /// </summary>
        public void Save<T>(T item)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            session.SaveOrUpdate(item);

        }

        public void Save<T>(IList<T> items)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            foreach (T item in items)
            {
                session.SaveOrUpdate(item);
            }

        }

        public void CreateQuery(string query)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            session.CreateSQLQuery(query).ExecuteUpdate();
        }
        public int GetQuery(string query)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            var result = session.CreateSQLQuery(query).ExecuteUpdate();
            return Convert.ToInt16(result);
        }

        public IList GetColumnName(string query)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            var results = session.CreateSQLQuery(query).List();
            return results;
        }

        public IList ExecuteQuery(string query)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            var results = session.CreateSQLQuery(query).SetResultTransformer(Transformers.AliasToEntityMap).List();
            return results;
        }

        public List<Hashtable> ReportExecuteQuery(string query)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            var results = session.CreateSQLQuery(query).SetResultTransformer(Transformers.AliasToEntityMap).List<Hashtable>().ToList();
            return results;
        }

        public IList<T> GetAllColumnsByUniquekey<T>(string propertyName, string propertyValue)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;

            ICriteria criteria = session.CreateCriteria(typeof(T));
            string likeoperator = "%";
            criteria.Add(Expression.Like(propertyName, likeoperator + propertyValue.TrimStart() + likeoperator));

            IList<T> matchingObjects = criteria.List<T>();

            return matchingObjects;
        }
        public void SaveByentity<T>(string entityname, T item)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            session.SaveOrUpdate(entityname, item);

        }

        public void SaveDynamicEntity<T>(string entityname,IList<T> items)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            foreach (T item in items)
            {
                session.SaveOrUpdate(entityname,item);
            }

        }

        public IList ExecuteQuerywithParam(string query1, IList<MultiProperty> ParamColl)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            var query = session.CreateSQLQuery(query1);
            if (ParamColl != null)
            {
                for (int i = 0; i < ParamColl.Count(); i++)
                {

                    query.SetParameter(ParamColl[i].propertyName, ParamColl[i].propertyValue);
                }
            }
            var result = query.SetResultTransformer(Transformers.AliasToEntityMap).List();
            return result;
        }

        public IList ExecuteQuerywithMinParam(string query1, params object[] ParamList)
        {
            ISession session = _sessionFactory.GetCurrentSession();
            session.CacheMode = CacheMode.Ignore;
            session.FlushMode = FlushMode.Commit;
            var query = session.CreateSQLQuery(query1);
            if (ParamList != null)
            {
                for (int i = 0; i < ParamList.Count(); i++)
                {

                    query.SetParameter(i, ParamList[i]);
                }
            }
            var result = query.SetResultTransformer(Transformers.AliasToEntityMap).List();
            return result;
        }


        #endregion


    }
}
