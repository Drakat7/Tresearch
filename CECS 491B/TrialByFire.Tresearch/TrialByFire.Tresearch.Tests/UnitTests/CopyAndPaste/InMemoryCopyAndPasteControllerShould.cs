﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrialByFire.Tresearch.DAL.Contracts;
using TrialByFire.Tresearch.DAL.Implementations;
using TrialByFire.Tresearch.Managers.Contracts;
using TrialByFire.Tresearch.Managers.Implementations;
using TrialByFire.Tresearch.Models.Contracts;
using TrialByFire.Tresearch.Models.Implementations;
using TrialByFire.Tresearch.Services.Contracts;
using TrialByFire.Tresearch.Services.Implementations;
using TrialByFire.Tresearch.WebApi.Controllers.Contracts;
using TrialByFire.Tresearch.WebApi.Controllers.Implementations;
using Xunit;

namespace TrialByFire.Tresearch.Tests.UnitTests.CopyAndPaste
{
    public class InMemoryCopyAndPasteControllerShould : TestBaseClass
    {
        public InMemoryCopyAndPasteControllerShould() : base(new string[] { })
        {
            TestServices.AddScoped<ISqlDAO, InMemorySqlDAO>();
            TestServices.AddScoped<ICopyAndPasteService, CopyAndPasteService>();
            TestServices.AddScoped<ICopyAndPasteManager, CopyAndPasteManager>();
            TestServices.AddScoped<ICopyAndPasteController, CopyAndPasteController>();
            TestProvider = TestServices.BuildServiceProvider();
        }

        
        
        
        

        [Theory]

        [MemberData(nameof(CopyNodeData))]
        

        

        public async Task CopyNodeAsync(IRoleIdentity roleIdentity, List<long> nodesCopy, IMessageBank.Responses response)
        {
            // Arrange
            ICopyAndPasteController copyAndPasteController = TestProvider.GetService<ICopyAndPasteController>();
            IMessageBank messageBank = TestProvider.GetService<IMessageBank>();
            IRolePrincipal rolePrincipal = new RolePrincipal(roleIdentity);
            Thread.CurrentPrincipal = rolePrincipal;

            string expected = await messageBank.GetMessage(response);
            string[] exps = expected.Split(":");
            ObjectResult expectedResult = new ObjectResult(exps[2]) { StatusCode = Convert.ToInt32(exps[0]) };

            // Act
            IActionResult resultCopy = await copyAndPasteController.CopyNodeAsync(nodesCopy).ConfigureAwait(false);
            var result = resultCopy as ObjectResult;

            // Assert
            Assert.NotNull(resultCopy);
            //Assert.Equal(expectedResult.StatusCode, result.StatusCode);
            Assert.Equal(expectedResult.Value, result.Value);
        }


        /*

        [Theory]
        [MemberData(nameof(PasteNodeData))]


        */





        
        
        public static IEnumerable<object[]> CopyNodeData()
        {


            Node node1 = new Node("0B1CC9CFB7380E8E7A80726D12CB997C936D95B514E7F921187119FD80996BBACA103C08EFCC39553EFF5DFC368D4D8D197C9080C7015AE4DA2E87884E7DE9A6", 1, 1, "Cooking", "This is a test node.", new DateTime(2022, 4, 17), true, false);
            Node node2 = new Node("0B1CC9CFB7380E8E7A80726D12CB997C936D95B514E7F921187119FD80996BBACA103C08EFCC39553EFF5DFC368D4D8D197C9080C7015AE4DA2E87884E7DE9A6", 2, 1, "Cooking Pasta", "This is a test node.", new DateTime(2022, 4, 18), true, false);
            Node node3 = new Node("0B1CC9CFB7380E8E7A80726D12CB997C936D95B514E7F921187119FD80996BBACA103C08EFCC39553EFF5DFC368D4D8D197C9080C7015AE4DA2E87884E7DE9A6", 3, 1, "Cooking Rice", "This is a test node.", new DateTime(2022, 4, 19), true, false);
            Node node4 = new Node("D8FC97AC79D370FC43BE4528C72B02AD7B560DC707956B77D5892504754E6C2484C07BF28243FF3CD1A2EA6F778BBBF924B384A34975D6A7D590A40CEE455A32", 4, 1, "Cooking", "This is a test node.", new DateTime(2022, 4, 17), true, false);
            Node node5 = new Node("D8FC97AC79D370FC43BE4528C72B02AD7B560DC707956B77D5892504754E6C2484C07BF28243FF3CD1A2EA6F778BBBF924B384A34975D6A7D590A40CEE455A32", 5, 1, "Cooking Pasta", "This is a test node.", new DateTime(2022, 4, 18), true, false);
            Node node6 = new Node("D8FC97AC79D370FC43BE4528C72B02AD7B560DC707956B77D5892504754E6C2484C07BF28243FF3CD1A2EA6F778BBBF924B384A34975D6A7D590A40CEE455A32", 6, 1, "Cooking Rice", "This is a test node.", new DateTime(2022, 4, 19), true, false);
            Node node7 = new Node("AE57D4CD0E7DC14F7C8C7EEF4DC8C8B833567A71021C1D123328D9B85C3825D8B72376D162C7F03C78D3CE048104A6BB0047979544F4852679D937048258558D", 7, 1, "Cooking", "This is a test node.", new DateTime(2022, 4, 19), true, false);
            Node node8 = new Node("E5D6801551E6079FCAF2B10403FA86F9B9EC40B0D7A70256EDA0A9988ABAB4CC250681D5054D18E224DCF0CADB730BCF6E07546F2B775A0E31D64C3DC41BC159", 8, 1, "Cooking", "This is a test node.", new DateTime(2022, 4, 19), true, false);
            Node node9 = new Node("AE57D4CD0E7DC14F7C8C7EEF4DC8C8B833567A71021C1D123328D9B85C3825D8B72376D162C7F03C78D3CE048104A6BB0047979544F4852679D937048258558D", 9, 1, "Cooking Pasta", "This is a test node.", new DateTime(2022, 4, 19), true, false);







            // User not authenticated and nodeID list is empty
            IRoleIdentity roleIdentity0 = new RoleIdentity(true, "guest", "guest", "");
            var nodesToCopyList0 = new List<long>();
            var resultCase0 = IMessageBank.Responses.notAuthenticated;


            // User authenticated but nodeID list is empty
            IRoleIdentity roleIdentity1 = new RoleIdentity(true, "grizzly@gmail.com", "user", "87ec69f0ab41c3dcb31e01dcf9942d756501b421887524a1e691dff69a698cf1d46c26b68f73dddb29a7d2729eddf43580bab9a5002d2289c0c7bf4d5db7c7ae");
            var nodesToCopyList1 = new List<long>();
            var resultCase1 = IMessageBank.Responses.copyNodeEmptyError;


            // User authenticated and nodelist not empty, but amount of nodeID is not equal to amount of nodes getting back
            IRoleIdentity roleIdentity2 = new RoleIdentity(true, "grizzly@gmail.com", "user", "87ec69f0ab41c3dcb31e01dcf9942d756501b421887524a1e691dff69a698cf1d46c26b68f73dddb29a7d2729eddf43580bab9a5002d2289c0c7bf4d5db7c7ae");
            var nodesToCopyList2 = new List<long> { 1, 2, 3 };
            var resultCase2 = IMessageBank.Responses.copyNodeMistmatchError;


            // User authenticated and nodeID list is populated, and was successful in grabbing nodes from database
            IRoleIdentity roleIdentity3 = new RoleIdentity(true, "grizzly@gmail.com", "user", "87ec69f0ab41c3dcb31e01dcf9942d756501b421887524a1e691dff69a698cf1d46c26b68f73dddb29a7d2729eddf43580bab9a5002d2289c0c7bf4d5db7c7ae");
            var nodesToCopyList3 = new List<long> { 1, 2, 3 };
            var resultCase3 = IMessageBank.Responses.copyNodeSuccess;


            // User not authenticated but nodeID list is NOT empty




            // User authenticated and duplicated nodeID in list, meaning they copied the same node twice
            IRoleIdentity roleIdentity4 = new RoleIdentity(true, "user1", "user", "09bdb27005ebc8c2f3894957ece9703d2d2c7b848d5175da7181af2841e35be54708d3faf6b16e7ee29eef8bb71e2debebc619401a118849435368da610c20f5");

            // User authenticated and nodeIDs 
            IRoleIdentity roleIdentity5 = new RoleIdentity(true, "user1", "user", "09bdb27005ebc8c2f3894957ece9703d2d2c7b848d5175da7181af2841e35be54708d3faf6b16e7ee29eef8bb71e2debebc619401a118849435368da610c20f5");

            // User authenticated
            IRoleIdentity roleIdentity6 = new RoleIdentity(true, "user1", "user", "09bdb27005ebc8c2f3894957ece9703d2d2c7b848d5175da7181af2841e35be54708d3faf6b16e7ee29eef8bb71e2debebc619401a118849435368da610c20f5");

            // User authenticated
            IRoleIdentity roleIdentity7 = new RoleIdentity(true, "user1", "user", "09bdb27005ebc8c2f3894957ece9703d2d2c7b848d5175da7181af2841e35be54708d3faf6b16e7ee29eef8bb71e2debebc619401a118849435368da610c20f5");





            var nodeList1 = new List<INode>() { node1, node2, node3, node4, node5, node6, node7, node8, node9};






            return new[]
            {
                //new object[] { roleIdentity0, nodesToCopyList0, resultCase0},
                //new object[] { roleIdentity1, nodesToCopyList1, resultCase1},
                new object[] { roleIdentity2, nodesToCopyList2, resultCase2},
                new object[] { roleIdentity3, nodesToCopyList3, resultCase3},
            };
        }
        





        /*

        public static IEnumerable<object[]> PasteNodeData()
        {

        }
        
        */

        


    }
}
