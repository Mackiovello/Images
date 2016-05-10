using System;
using System.Collections.Generic;
using System.Web.Security;
using Starcounter;

namespace Images.Helpers
{
    public class Partial
    {
        #region Fields
        public static string MappingUriPrefix => UriMapping.MappingUriPrefix;
        public static string OntologyMappingUriPrefix => UriMapping.OntologyMappingUriPrefix;
        #endregion
        #region Handle.Get replacement
        public static void Register(string uriTemplate, Func<Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(uriTemplate, code, ho);
        }

        public static void Register(ushort port, string uriTemplate, Func<Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(port, uriTemplate, code, ho);
        }

        public static void Register<T>(string uriTemplate, Func<T, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(uriTemplate, code, ho);
        }

        public static void Register<T>(ushort port, string uriTemplate, Func<T, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(port, uriTemplate, code, ho);
        }

        public static void Register<T1, T2>(string uriTemplate, Func<T1, T2, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(uriTemplate, code, ho);
        }

        public static void Register<T1, T2>(ushort port, string uriTemplate, Func<T1, T2, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(port, uriTemplate, code, ho);
        }

        public static void Register<T1, T2, T3>(string uriTemplate, Func<T1, T2, T3, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(uriTemplate, code, ho);
        }

        public static void Register<T1, T2, T3>(ushort port, string uriTemplate, Func<T1, T2, T3, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(port, uriTemplate, code, ho);
        }

        public static void Register<T1, T2, T3, T4>(string uriTemplate, Func<T1, T2, T3, T4, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(uriTemplate, code, ho);
        }

        public static void Register<T1, T2, T3, T4>(ushort port, string uriTemplate, Func<T1, T2, T3, T4, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(port, uriTemplate, code, ho);
        }

        public static void Register<T1, T2, T3, T4, T5>(string uriTemplate, Func<T1, T2, T3, T4, T5, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(uriTemplate, code, ho);
        }

        public static void Register<T1, T2, T3, T4, T5>(ushort port, string uriTemplate, Func<T1, T2, T3, T4, T5, Response> code, HandlerOptions ho = null)
        {
            AddSelfOnlyOption(ref ho);
            Handle.GET(port, uriTemplate, code, ho);
        }
        #endregion
        #region UriMapping.Map replacement
        public static void Map(string appUriToMap, string mapUri, string method)
        {
            UriMapping.Map(appUriToMap, mapUri, method);
        }

        public static void Map(string appUriToMap, string mapUri, Func<string, string> converterTo, Func<string, string> converterFrom, string method)
        {
            UriMapping.Map(appUriToMap, mapUri, converterTo, converterFrom, method);
        }

        private static void AddSelfOnlyOption(ref HandlerOptions options)
        {
            if (options == null)
            {
                options = new HandlerOptions();
            }
            options.SelfOnly = true;
        }
        #endregion
        #region UriMapping.OntologyMap replacement
        public static void Map(string appUriToMap, string mappedClassInfo)
        {
            UriMapping.OntologyMap(appUriToMap, mappedClassInfo);
        }

        public static void Map(string appUriToMap, string mappedClassInfo, Func<string, string> converterToClass, Func<string, string> converterFromClass)
        {
            UriMapping.OntologyMap(appUriToMap, mappedClassInfo, converterToClass, converterFromClass);
        }

        public static void Map<T>(string appUriToMap)
        {
            UriMapping.OntologyMap<T>(appUriToMap);
        }
        #endregion
        #region Self.GET replacement
        public static Response Get(string uri)
        {
            return Self.GET(uri);
        }

        public static Response Get(ushort port, string uri)
        {
            return Self.GET(port, uri);
        }

        public static Response Get(string uri, Func<Response> substituteHandler)
        {
            return Self.GET(uri, substituteHandler);
        }

        public static Response Get(string uri, Dictionary<string, string> headersDictionary = null, HandlerOptions ho = null)
        {
            return Self.GET(uri, headersDictionary, ho);
        }

        public static Response Get(ushort port, string uri, Func<Response> substituteHandler)
        {
            return Self.GET(port, uri, substituteHandler);
        }

        public static Response Get(ushort port, string uri, Dictionary<string, string> headersDictionary = null, HandlerOptions ho = null)
        {
            return Self.GET(port, uri, headersDictionary, ho);
        }

        public static T Get<T>(string uri, Func<Response> substituteHandler)
        {
            return Self.GET<T>(uri, substituteHandler);
        }

        public static T Get<T>(string uri, HandlerOptions ho = null)
        {
            return Self.GET<T>(uri, ho);
        }

        public static T Get<T>(ushort port, string uri, Func<Response> substituteHandler)
        {
            return Self.GET<T>(port, uri, substituteHandler);
        }

        public static T Get<T>(ushort port, string uri, HandlerOptions ho = null)
        {
            return Self.GET<T>(port, uri, ho);
        }
        #endregion
    }
}
