using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudMVC.Models
{
    public class Customer : TableEntity
    {
        private string lastName;
        private string firstName;
        private string attributes;

        public void AssignRowKey()
        {
            this.RowKey = lastName + " " + firstName;
        }
        public void AssignPartitionKey()
        {
            this.PartitionKey = "Customer";
        }

        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
            }
        }

        public string Attributes
        {
            get
            {
                return attributes;
            }

            set
            {
                attributes = value;
            }
        }
    }
}