using System;
using System.Collections.Generic;
using FortnoxAPILibrary.Connectors;
using FortnoxAPILibrary.Entities;
using FortnoxAPILibrary.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FortnoxAPILibrary.GeneratedTests
{
    [TestClass]
    public class InvoicePaymentTests
    {
        [TestInitialize]
        public void Init()
        {
            //Set global credentials for SDK
            //--- Open 'TestCredentials.resx' to edit the values ---\\
            ConnectionCredentials.AccessToken = TestCredentials.Access_Token;
            ConnectionCredentials.ClientSecret = TestCredentials.Client_Secret;
        }

        //[Ignore("Require booked invoice")]
        [TestMethod]
        public void Test_InvoicePayment_CRUD()
        {
            #region Arrange
            var tmpCustomer = new CustomerConnector().Create(new Customer() { Name = "TmpCustomer", CountryCode = "SE", City = "Testopolis" });
            var tmpArticle = new ArticleConnector().Create(new Article() { Description = "TmpArticle", Type = ArticleType.STOCK, PurchasePrice = 10 });
            var invoiceConnector = new InvoiceConnector();
            var tmpInvoice = invoiceConnector.Create(new Invoice()
            {
                CustomerNumber = tmpCustomer.CustomerNumber,
                InvoiceDate = new DateTime(2020, 1, 20),
                DueDate = new DateTime(2020, 6, 20),
                InvoiceRows = new List<InvoiceRow>()
                {
                    new InvoiceRow(){ ArticleNumber = tmpArticle.ArticleNumber, DeliveredQuantity = 2, Price = 10},
                }
            });
            invoiceConnector.Bookkeep(tmpInvoice.DocumentNumber);
            #endregion Arrange

            var connector = new InvoicePaymentConnector();

            #region CREATE
            var newInvoicePayment = new InvoicePayment()
            {
                InvoiceNumber = tmpInvoice.DocumentNumber,
                Amount = 10.5,
                AmountCurrency = 10.5,
                PaymentDate = new DateTime(2020, 2,1)
            };

            var createdInvoicePayment = connector.Create(newInvoicePayment);
            MyAssert.HasNoError(connector);
            Assert.AreEqual("2020-02-01", createdInvoicePayment.PaymentDate?.ToString(APIConstants.DateFormat));

            #endregion CREATE

            #region UPDATE

            createdInvoicePayment.PaymentDate = new DateTime(2020, 3,1);

            var updatedInvoicePayment = connector.Update(createdInvoicePayment); 
            MyAssert.HasNoError(connector);
            Assert.AreEqual("2020-03-01", updatedInvoicePayment.PaymentDate?.ToString(APIConstants.DateFormat));

            #endregion UPDATE

            #region READ / GET

            var retrievedInvoicePayment = connector.Get(createdInvoicePayment.Number);
            MyAssert.HasNoError(connector);
            Assert.AreEqual("2020-03-01", retrievedInvoicePayment.PaymentDate?.ToString(APIConstants.DateFormat));

            #endregion READ / GET

            #region DELETE

            connector.Delete(createdInvoicePayment.Number);
            MyAssert.HasNoError(connector);

            retrievedInvoicePayment = connector.Get(createdInvoicePayment.Number);
            Assert.AreEqual(null, retrievedInvoicePayment, "Entity still exists after Delete!");

            #endregion DELETE

            #region Delete arranged resources
            new CustomerConnector().Delete(tmpCustomer.CustomerNumber);
            new ArticleConnector().Delete(tmpArticle.ArticleNumber);
            #endregion Delete arranged resources
        }
    }
}