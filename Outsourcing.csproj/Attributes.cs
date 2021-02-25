using System;

namespace Outsourcing
{
    class Route : Attribute
    {
        private string path;

        public Route(string path)
        {
            this.path = path;
        }
    }

    class HttpPatch : Attribute
    {
    }

    class HttpPost : Attribute
    {
    }

    class HttpPut : Attribute
    {
    }

    class HttpGet : Attribute
    {
    }

    class HttpHead : Attribute
    {
    }

    class HttpDelete : Attribute
    {
    }

    interface Controller
    {
    }
}