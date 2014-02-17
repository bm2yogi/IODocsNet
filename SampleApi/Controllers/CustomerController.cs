﻿using System.Web.Http;

namespace SampleApi.Controllers
{
    /// <summary>
    /// Api endpoints for Customers
    /// </summary>
    public class CustomerController : ApiController
    {
        //// GET api/customer
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        /// <summary>
        /// Retrieve a customer by its id
        /// </summary>
        /// <param name="id">The customer's id</param>
        /// <returns>The requested customer</returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="value">The new customer</param>
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        /// Update an existing Customer
        /// </summary>
        /// <param name="id">The customer's id</param>
        /// <param name="value">The updated customer</param>
        public void Put(int id, [FromBody]string value)
        {
        }

        public void Delete(int id)
        {
        }
    }
}
