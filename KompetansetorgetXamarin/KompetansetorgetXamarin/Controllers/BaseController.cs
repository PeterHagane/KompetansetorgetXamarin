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
        protected string Adress = BackendAdresses.apiUri + BackendAdresses.apiVersion;

    }
}
