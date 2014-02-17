using System.Web.Http;

namespace SampleApi.Controllers
{
    /// <summary>
    /// Api endpoints actions for Addresses
    /// </summary>
    public class AddressController : ApiController
    {
        ///// <summary>
        ///// Retrieve a list of all Addresses
        ///// </summary>
        ///// <returns>A list of all available Addresses</returns>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        /// <summary>
        /// Retrieve an Address by its id
        /// </summary>
        /// <param name="id">The address' id</param>
        /// <returns>The requested Address</returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Create a new Address
        /// </summary>
        /// <param name="value">The new address</param>
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        /// Update an existing Address
        /// </summary>
        /// <param name="id">The address' id</param>
        /// <param name="value">The updated address</param>
        public void Put(int id, [FromBody]string value)
        {
        }

        /// <summary>
        /// Deletes an Address
        /// </summary>
        /// <param name="id">The address' id</param>
        public void Delete(int id)
        {
        }
    }
}
