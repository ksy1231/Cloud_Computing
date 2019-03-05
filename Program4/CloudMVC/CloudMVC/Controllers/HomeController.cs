using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using CloudMVC.Models;

namespace CloudMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult ProcessForm(string ButtonName, string FirstName, string LastName)
        {
            switch (ButtonName)
            {
                case "Load Data":
                    LoadData();
                    break;
                case "Clear Data":
                    ClearData();
                    break;
                case "Query":
                    Query();
                    break;
            }

            return View("Index");
        }

        private void LoadData()
        {
            string url = "https://css490.blob.core.windows.net/lab4/input.txt";
            string result = null;
            WebResponse response = null;
            StreamReader reader = null;

            // 1. save txt file to Azure blob storage
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            
            string storageConnection = "DefaultEndpointsProtocol=https;AccountName=webstorageapi;AccountKey=BOcD+JlMzzHjh61u2Sit8XqCqU5T+HrLJdSPxOG+xFXQ7EUESZKXezmKDrMUbZK9BJ0H6jzBx50zmt5+JmeMug==;EndpointSuffix=core.windows.net";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("webstorage");
            
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference("input.txt");
            cloudBlockBlob.Properties.ContentType = "text/plain";
            cloudBlockBlob.UploadFromStream(stream);

            if (stream != null)
                stream.Close();
            if (response != null)
                response.Close();

            // 2. insert/update to Azure Table
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            response = request.GetResponse();
            stream = response.GetResponseStream();
            reader = new StreamReader(stream, Encoding.UTF8);
            result = reader.ReadToEnd();

            List<Customer> customers = new List<Customer>();
            string[] rows = result.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string row in rows)
            {
                string[] columns = row.Split(new char[] { ' ' }, 3);
                Customer customer = new Customer
                {
                    LastName = columns[0],
                    FirstName = columns[1],
                    Attributes = columns[2]
                };
                customer.AssignPartitionKey();
                customer.AssignRowKey();
                customers.Add(customer);
            }

            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable cloudTable = tableClient.GetTableReference("customers");
            cloudTable.CreateIfNotExists();

            foreach (Customer customer in customers)
            {
                Customer record = RetrieveRecord(cloudTable, customer.PartitionKey, customer.RowKey);
                if (record == null)
                {
                    // record doesn't exists, insert
                    TableOperation tableOperation = TableOperation.Insert(customer);
                    cloudTable.Execute(tableOperation);
                }
                else
                {
                    // record exists, update
                    TableOperation tableOperation = TableOperation.Replace(customer);
                    cloudTable.Execute(tableOperation);
                }
            }

            if (stream != null)
                stream.Close();
            if (reader != null)
                reader.Close();
            if (response != null)
                response.Close();
        }

        private Customer RetrieveRecord(CloudTable table, string partitionKey, string rowKey)
        {
            TableOperation tableOperation = TableOperation.Retrieve<Customer>(partitionKey, rowKey);
            TableResult tableResult = table.Execute(tableOperation);
            return tableResult.Result as Customer;
        }

        private void ClearData()
        {
            // 1. Delete txt file from Azure blog
            string storageConnection = "DefaultEndpointsProtocol=https;AccountName=webstorageapi;AccountKey=BOcD+JlMzzHjh61u2Sit8XqCqU5T+HrLJdSPxOG+xFXQ7EUESZKXezmKDrMUbZK9BJ0H6jzBx50zmt5+JmeMug==;EndpointSuffix=core.windows.net";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("webstorage");
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference("input.txt");
            cloudBlockBlob.DeleteIfExists();

            // 2. Delete the table from Azure table
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable cloudTable = tableClient.GetTableReference("customers");
            cloudTable.DeleteIfExists();
        }

        private void Query()
        {

        }
    }
}