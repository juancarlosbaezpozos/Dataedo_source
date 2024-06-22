using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dataedo.App.Data.Tableau;

public static class RequestHelper
{
    private static JObject SendGraphQLRequestBody(TableauConnection conn, string query)
    {
        string graphQLUrl = conn.GraphQLUrl;
        GraphQLHttpClient client = new GraphQLHttpClient(graphQLUrl, new NewtonsoftJsonSerializer());
        try
        {
            ((HttpHeaders)client.HttpClient.DefaultRequestHeaders).Add("Authorization", "Bearer " + conn.Token);
            GraphQLRequest request = new GraphQLRequest(query);
            GraphQLResponse<JObject> result = Task.Run(() => client.SendQueryAsync<JObject>(request)).Result;
            GraphQLError graphQLError = result.Errors?.FirstOrDefault();
            if (graphQLError != null)
            {
                throw new Exception(graphQLError.Message);
            }
            return result.Data;
        }
        finally
        {
            if (client != null)
            {
                ((IDisposable)client).Dispose();
            }
        }
    }

    public static JObject SendGraphQLRequest(TableauConnection conn, string query)
    {
        try
        {
            return SendGraphQLRequestBody(conn, query);
        }
        catch (Exception ex)
        {
            GraphQLHttpRequestException obj = ex?.InnerException as GraphQLHttpRequestException;
            if (obj != null && obj.StatusCode == HttpStatusCode.Unauthorized)
            {
                TableauConnection tableauConnection = OpenConnection(conn.ConnectionString);
                conn.Token = tableauConnection.Token;
                return SendGraphQLRequestBody(conn, query);
            }
            throw;
        }
    }

    public static JObject SendRESTGETRequest(string url, string path, string token)
    {
        HttpWebRequest request = GetRequest(url + "/" + path, "GET");
        request.Headers.Add("Authorization", "Bearer " + token);
        return GetResponse(request);
    }

    private static HttpWebRequest GetRequest(string url, string method)
    {
        HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(url);
        obj.Method = method;
        obj.ContentType = "application/xml";
        obj.Accept = "application/json";
        return obj;
    }

    private static JObject GetResponse(HttpWebRequest request)
    {
        HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
        if (httpWebResponse.StatusCode == HttpStatusCode.OK)
        {
            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<JObject>(streamReader.ReadToEnd());
            }
        }
        throw new Exception(httpWebResponse.StatusDescription);
    }

    public static TableauConnection OpenConnection(TableauConnectionString tableauConnectionString, bool withSite = true)
    {
        string text = (tableauConnectionString.IsToken ? ("<credentials personalAccessTokenName=\"" + tableauConnectionString.Token + "\" personalAccessTokenSecret=\"" + tableauConnectionString.Password + "\">") : ("<credentials name=\"" + tableauConnectionString.Username + "\" password=\"" + tableauConnectionString.Password + "\">"));
        string text2 = (withSite ? ("<site contentUrl=\"" + tableauConnectionString.SiteForConnection + "\"/>") : "<site/>");
        string value = "<tsRequest>\r\n\t" + text + "\r\n    " + text2 + "\r\n\t</credentials>\r\n</tsRequest>";
        HttpWebRequest request = GetRequest(tableauConnectionString.BaseUrl + "/auth/signin", "POST");
        using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(value);
        }
        JToken? jToken = GetResponse(request)["credentials"];
        string token = jToken!["token"].Value<string>();
        string site = jToken!["site"]!["id"].Value<string>();
        string apiVersion = SendRESTGETRequest(tableauConnectionString.BaseUrl, "serverinfo", token)["serverInfo"]!["restApiVersion"].Value<string>();
        return new TableauConnection(tableauConnectionString, site, token, apiVersion);
    }

    public static TableauConnection OpenConnection(string connectionString, bool withSite = true)
    {
        return OpenConnection(new TableauConnectionString(connectionString), withSite);
    }

    public static string GetNameWithHash(string id, string name)
    {
        using SHA256 sHA = SHA256.Create();
        byte[] array = sHA.ComputeHash(Encoding.UTF8.GetBytes(id));
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < 3; i++)
        {
            stringBuilder.Append(array[i].ToString("x2"));
        }
        return $"{name}_{stringBuilder}";
    }
}
