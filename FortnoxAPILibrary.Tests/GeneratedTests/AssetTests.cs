using System;
using System.Collections.Generic;
using FortnoxAPILibrary.Connectors;
using FortnoxAPILibrary.Entities;
using FortnoxAPILibrary.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FortnoxAPILibrary.GeneratedTests
{
    [TestClass]
    public class AssetTests
    {
        [TestInitialize]
        public void Init()
        {
            //Set global credentials for SDK
            //--- Open 'TestCredentials.resx' to edit the values ---\\
            ConnectionCredentials.AccessToken = TestCredentials.Access_Token;
            ConnectionCredentials.ClientSecret = TestCredentials.Client_Secret;
        }

        [Ignore("Irregular jsons")]
        [TestMethod]
        public void Test_Asset_CRUD()
        {
            #region Arrange
            var tmpCostCenter = new CostCenterConnector().Create(new CostCenter(){ Code = "TMP", Description = "TmpCostCenter"});
            #endregion Arrange

            var connector = new AssetConnector();

            #region CREATE
            var newAsset = new Asset()
            {
                Description = "TestAsset",
                AcquisitionStart = new DateTime(2011,1,1),
                AcquisitionValue = 123.45,
                Department = "Some Department",
                Notes = "Some notes",
                Group = "Some Group",
                Room = "Some room",
                Placement = "Right here",
                CostCenter = tmpCostCenter.Code,
                TypeId = "1250" //Datorer
            };

            var createdAsset = connector.Create(newAsset);
            MyAssert.HasNoError(connector);
            Assert.AreEqual("TestAsset", createdAsset.Description); //returns entity named "Assets" instead of "asset"

            #endregion CREATE

            #region UPDATE

            createdAsset.Description = "UpdatedTestAsset";

            var updatedAsset = connector.Update(createdAsset); 
            MyAssert.HasNoError(connector);
            Assert.AreEqual("UpdatedTestAsset", updatedAsset.Description);

            #endregion UPDATE

            #region READ / GET

            var retrievedAsset = connector.Get(createdAsset.Id);
            MyAssert.HasNoError(connector);
            Assert.AreEqual("UpdatedTestAsset", retrievedAsset.Description);

            #endregion READ / GET

            #region DELETE

            connector.Delete(createdAsset.Id);
            MyAssert.HasNoError(connector);

            retrievedAsset = connector.Get(createdAsset.Id);
            Assert.AreEqual(null, retrievedAsset, "Entity still exists after Delete!");

            #endregion DELETE

            #region Delete arranged resources
            //Add code to delete temporary resources
            #endregion Delete arranged resources
        }
    }
}