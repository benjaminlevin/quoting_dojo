using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using DbConnection;

namespace quotingDojo{

using Newtonsoft.Json;

    public static class SessionExtensions // Somewhere in your namespace, outside other classes
    {
        public static void SetObjectAsJson(this ISession session, string key, object value) // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
        {
            session.SetString(key, JsonConvert.SerializeObject(value)); // This helper function simply serializes theobject to JSON and stores it as a string in session
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)  // generic type T is a stand-in indicating that we need to specify the type on retrieval
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);   // Upon retrieval the object is deserialized based on the type we specified
        }
    }

    public class QuoteController : Controller{
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            string delete = $"DELETE FROM quotes WHERE id=3";
            DbConnector.Execute(delete);
            return View("add");
        }
    
        [HttpGet]
        [Route("/quotes/")]
        public IActionResult Quotes()
        {
            List<Dictionary<string, object>> AllQuotes = DbConnector.Query("SELECT * FROM quotes ORDER BY created_at DESC");
            Console.WriteLine(AllQuotes[0]["id"]);
            ViewBag.Quotes=(List<Dictionary<string,object>>)AllQuotes;
            return View("index");
        }

        [HttpPost]
        [Route("/quotes")]
        public IActionResult Add(string name, string quote)
        {
            if((name == null)||(quote == null))
            {
                var Error = "Entries may not be blank.";
                ViewBag.Error = Error;
                return View("Error");
            } 
            else if((name.Length < 2)||(quote.Length < 5))
            {
                var Error = "I doubt you wrote anything meaningful... try again.";
                ViewBag.Error = Error;
                return View("Error");
            }
            else
            {
                string query = $"INSERT INTO quotes (user, quote) VALUES ('{name}', '{quote}')";
                DbConnector.Execute(query);
                return Redirect("/quotes");
            }
        }
    }
}

//from solution

// using Microsoft.AspNetCore.Mvc;
// using System.Linq;
// using DbConnection;
// using System;

// namespace QuotingDojo {
//     public class QuoteController : Controller {
//         [HttpGet]
//         [Route("")]
//         public IActionResult Index() {
//             if(TempData["Error"] != null){
//                 ViewBag.Error = TempData["Error"];
//             }
//             return View();
//         }

//         [HttpPost]
//         [Route("/quotes")]
//         public IActionResult Create(string name, string content){
//             if(name == "" || content == ""){
//                 TempData["Error"] = "Neither field should be empty!";
//                 return RedirectToAction("Index");
//             }
//             //Add the quote to the database
//             string query = $"INSERT INTO quotes (content, poster, created_at, updated_at) VALUES ('{content}', '{name}', NOW(), NOW());";
//             DbConnector.Execute(query);
//             return RedirectToAction("Quotes");    
//         }

//         [HttpGet]
//         [Route("/quotes")]
//         public IActionResult Quotes(){
//             //Get all quotes
//             string query = "SELECT * FROM quotes";
//             var quotes = DbConnector.Query(query);

//             //Sort them by created time descending
//             quotes = quotes.OrderByDescending((quote) => quote["created_at"]).ToList();

//             //Format all of the dates
//             foreach(var quote in quotes){
//                 DateTime created = (DateTime)quote["created_at"];
//                 string formatted_created = String.Format("{0:h:mm tt MMMM d yyyy}", created);
//                 quote["created_at"] = formatted_created;
//             }

//             ViewBag.Quotes = quotes;
//             return View();
//         }
//     }
// }