using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;

namespace KompetansetorgetXamarin.Controllers
{
    public abstract class BaseController
    {
        // Change this to http://kompetansetorget.uia.no/api/
        protected string Adress = "http://kompetansetorgetserver1.azurewebsites.net/api/";

    }
}
