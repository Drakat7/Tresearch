﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using TrialByFire.Tresearch.Managers.Contracts;
using TrialByFire.Tresearch.Managers.Implementations;
using TrialByFire.Tresearch.Models;
using TrialByFire.Tresearch.Models.Contracts;
using TrialByFire.Tresearch.Models.Implementations;
using TrialByFire.Tresearch.Services.Contracts;
using TrialByFire.Tresearch.Services.Implementations;
using TrialByFire.Tresearch.WebApi.Controllers.Contracts;
using TrialByFire.Tresearch.WebApi.Controllers.Implementations;
using Xunit;

namespace TrialByFire.Tresearch.Tests.IntegrationTests.Tag
{

    /// <summary>
    ///     Tests the Controller Layer of Tag Feature.
    /// </summary>
    public class TagControllerShould: TestBaseClass, IClassFixture<TagControllerDatabaseFixture>
    {
        TagControllerDatabaseFixture fixture;           // Access to the fixture instance (this is where data to database is initialized and  disposed)
        public TagControllerShould(TagControllerDatabaseFixture fixture) : base(new string[] { })
        {
            this.fixture = fixture;
            TestServices.AddScoped<ITagService, TagService>();
            TestServices.AddScoped<IAccountVerificationService, AccountVerificationService>();
            TestServices.AddScoped<ITagManager, TagManager>();
            TestServices.AddScoped<ILogManager, LogManager>();
            TestServices.AddScoped<ITagController, TagController>();
            TestProvider = TestServices.BuildServiceProvider();
        }

        [Theory]
        [MemberData(nameof(AddNodeTagData))]
        public async Task AddTagToNodeAsync(IRoleIdentity roleIdentity, List<long> nodeIDs, string tagName, string expected)
        {
            //Arrange
            IRolePrincipal rolePrincipal = new RolePrincipal(roleIdentity);
            if (!roleIdentity.Name.Equals("guest"))
                Thread.CurrentPrincipal = rolePrincipal;
            
            string[] exps = expected.Split(":");
            ObjectResult expectedResult = new ObjectResult(exps[2])
            { StatusCode = Convert.ToInt32(exps[0]) };

            ITagController tagController = TestProvider.GetService<ITagController>();

            //Act
            IActionResult aResult = await tagController.AddTagToNodesAsync(nodeIDs, tagName);
            var result = aResult as ObjectResult;

            //Arrange
            Assert.Equal(expectedResult.StatusCode, result.StatusCode);
            Assert.Equal(expectedResult.Value, result.Value);
        }

        public static IEnumerable<object[]> AddNodeTagData()
        {
            //User Authorized, Nodes already have tag
            IRoleIdentity roleIdentity0 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase0 = "Tresearch Controller Add Tag1";
            var nodeListCase0 = new List<long> { 9019303350, 9019303351, 9019303352 };
            var resultCase0 = "200: Server: Tag added to node(s).";

            //User Authorized, Nodes do not contain these tags
            IRoleIdentity roleIdentity1 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase1 = "Tresearch Controller Add Tag2";
            var nodeListCase1 = new List<long> { 9019303350, 9019303351, 9019303352 };
            var resultCase1 = "200: Server: Tag added to node(s).";

            //User Authorized, Node already has tag
            IRoleIdentity roleIdentity2 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase2 = "Tresearch Controller Add Tag3";
            var nodeListCase2 = new List<long> { 9019303350 };
            var resultCase2 = "200: Server: Tag added to node(s).";

            //User Authorized, Node does not have tag
            IRoleIdentity roleIdentity3 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase3 = "Tresearch Controller Add Tag4";
            var nodeListCase3 = new List<long> { 9019303350 };
            var resultCase3 = "200: Server: Tag added to node(s).";

            //User Authorized, No node is passed in
            IRoleIdentity roleIdentity4 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase4 = "Tresearch Controller Add Tag4";
            var nodeListCase4 = new List<long> { };
            var resultCase4 = "404: Database: The node was not found.";

            //User Authorized, Tag doesn't exist
            IRoleIdentity roleIdentity5 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase5 = "Tresearch Controller Add Tag5";
            var nodeListCase5 = new List<long> { 9019303350 };
            var resultCase5 = "404: Database: Tag not found.";

            //User Authorized, Some Nodes already have tag (9019303351 contains tag)
            IRoleIdentity roleIdentity6 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase6 = "Tresearch Controller Add Tag3";
            var nodeListCase6 = new List<long> { 9019303350, 9019303351, 9019303352 };
            var resultCase6 = "200: Server: Tag added to node(s).";

            //User not Auhorized For All Nodes
            IRoleIdentity roleIdentity7 = new RoleIdentity(false, "tagControllerIntegration2@tresearch.system", "user");
            var tagNameCase7 = "Tresearch Controller Add Tag3";
            var nodeListCase7 = new List<long> { 9019303350, 9019303351, 9019303352 };
            var resultCase7 = "403: Database: You are not authorized to perform this operation.";

            //User not Authorized for some Nodes (user owns 9019303353, 9019303354 but not 9019303350)
            IRoleIdentity roleIdentity8 = new RoleIdentity(false, "tagControllerIntegration2@tresearch.system", "user");
            var tagNameCase8 = "Tresearch Controller Add Tag3";
            var nodeListCase8 = new List<long> { 9019303350, 9019303353, 9019303354 };
            var resultCase8 = "403: Database: You are not authorized to perform this operation.";

            //User not Authorized for node
            IRoleIdentity roleIdentity9 = new RoleIdentity(false, "tagControllerIntegration2@tresearch.system", "user");
            var tagNameCase9 = "Tresearch Controller Add Tag3";
            var nodeListCase9 = new List<long> { 9019303350 };
            var resultCase9 = "403: Database: You are not authorized to perform this operation.";

            //User has not been authenticated
            IRoleIdentity roleIdentity10 = new RoleIdentity(false, "guest", "guest");
            var tagNameCase10 = "Tresearch Controller Add Tag3";
            var nodeListCase10 = new List<long> { 9019303350 };
            var resultCase10 = "401: Server: No active session found. Please login and try again.";

            //User is not enabled
            IRoleIdentity roleIdentity11 = new RoleIdentity(false, "tagControllerIntegrationNotEnabled@tresearch.system", "user");
            var tagNameCase11 = "Tresearch Manager Add Tag3";
            var nodeListCase11 = new List<long> { 9019303365 };
            var resultCase11 = "401: Database: Account disabled. Perform account recovery or contact system admin.";

            //User is not confirmed
            IRoleIdentity roleIdentity12 = new RoleIdentity(false, "tagControllerIntegrationNotConfirmed@tresearch.system", "user");
            var tagNameCase12 = "Tresearch Controller Add Tag3";
            var nodeListCase12 = new List<long> { 9019303366 };
            var resultCase12 = "401: Database: Please confirm your account before attempting to login.";

            //Account  does not exist
            IRoleIdentity roleIdentity13 = new RoleIdentity(false, "tagControllerNoAccount@tresearch.system", "user");
            var tagNameCase13 = "Tresearch Controller Add Tag3";
            var nodeListCase13 = new List<long> { };
            var resultCase13 = "500: Database: The Account was not found.";

            return new[]
            {
                new object[] { roleIdentity0, nodeListCase0, tagNameCase0, resultCase0 },
                new object[] { roleIdentity1, nodeListCase1, tagNameCase1, resultCase1 },
                new object[] { roleIdentity2, nodeListCase2, tagNameCase2, resultCase2 },
                new object[] { roleIdentity3, nodeListCase3, tagNameCase3, resultCase3 },
                new object[] { roleIdentity4, nodeListCase4, tagNameCase4, resultCase4 },
                new object[] { roleIdentity5, nodeListCase5, tagNameCase5, resultCase5 },
                new object[] { roleIdentity6, nodeListCase6, tagNameCase6, resultCase6 },
                new object[] { roleIdentity7, nodeListCase7, tagNameCase7, resultCase7 },
                new object[] { roleIdentity8, nodeListCase8, tagNameCase8, resultCase8 },
                new object[] { roleIdentity9, nodeListCase9, tagNameCase9, resultCase9 },
                new object[] { roleIdentity10, nodeListCase10, tagNameCase10, resultCase10 },
                new object[] { roleIdentity11, nodeListCase11, tagNameCase11, resultCase11 },
                new object[] { roleIdentity12, nodeListCase12, tagNameCase12, resultCase12 },
                new object[] { roleIdentity13, nodeListCase13, tagNameCase13, resultCase13 }
            };
        }

        [Theory]
        [MemberData(nameof(RemoveNodeTagData))]
        public async Task RemoveNodTagData(IRoleIdentity roleIdentity, List<long> nodeIDs, string tagName, string expected)
        {
            //Arrange
            IRolePrincipal rolePrincipal = new RolePrincipal(roleIdentity);
            if (!roleIdentity.Name.Equals("guest"))
                Thread.CurrentPrincipal = rolePrincipal;
            ITagController tagController = TestProvider.GetService<ITagController>();

            string[] exps = expected.Split(":");
            ObjectResult expectedResult = new ObjectResult(exps[2])
            { StatusCode = Convert.ToInt32(exps[0]) };

            //Act
            IActionResult aResult = await tagController.RemoveTagFromNodesAsync(nodeIDs, tagName);
            var result = aResult as ObjectResult;

            //Arrange
            Assert.Equal(expectedResult.StatusCode, result.StatusCode);
            Assert.Equal(expectedResult.Value, result.Value);
        }

        public static IEnumerable<object[]> RemoveNodeTagData()
        {
            //User Authorized, Nodes already have tag
            IRoleIdentity roleIdentity0 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase0 = "Tresearch Controller Delete Tag1";
            var nodeListCase0 = new List<long> { 9019303350, 9019303351, 9019303352 };
            var resultCase0 = "200: Server: Tag removed from node(s).";

            //User Authorized, Nodes do not contain these tags
            IRoleIdentity roleIdentity1 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase1 = "Tresearch Controller Delete Tag2";
            var nodeListCase1 = new List<long> { 9019303350, 9019303351, 9019303352 };
            var resultCase1 = "200: Server: Tag removed from node(s).";

            //User Authorized, Node already has tag
            IRoleIdentity roleIdentity2 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase2 = "Tresearch Controller Delete Tag3";
            var nodeListCase2 = new List<long> { 9019303350 };
            var resultCase2 = "200: Server: Tag removed from node(s).";

            //User Authorized, Node does not have tag
            IRoleIdentity roleIdentity3 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase3 = "Tresearch Controller Delete Tag4";
            var nodeListCase3 = new List<long> { 9019303350 };
            var resultCase3 = "200: Server: Tag removed from node(s).";

            //User Authorized, No node is passed in
            IRoleIdentity roleIdentity4 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase4 = "Tresearch Controller Delete Tag4";
            var nodeListCase4 = new List<long> { };
            var resultCase4 = "404: Database: The node was not found.";

            //User Authorized, Tag doesn't exist
            IRoleIdentity roleIdentity5 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase5 = "Tresearch Controller Delete Tag5";
            var nodeListCase5 = new List<long> { 9019303350 };
            var resultCase5 = "200: Server: Tag removed from node(s).";

            //User Authorized, Some Nodes already have tag (9019303351 contains tag)
            IRoleIdentity roleIdentity6 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            var tagNameCase6 = "Tresearch Controller Delete Tag3";
            var nodeListCase6 = new List<long> { 9019303350, 9019303351, 9019303352 };
            var resultCase6 = "200: Server: Tag removed from node(s).";

            //User not Auhorized For All Nodes
            IRoleIdentity roleIdentity7 = new RoleIdentity(false, "tagControllerIntegration2@tresearch.system", "user");
            var tagNameCase7 = "Tresearch Controller Delete Tag3";
            var nodeListCase7 = new List<long> { 9019303350, 9019303351, 9019303352 };
            var resultCase7 = "403: Database: You are not authorized to perform this operation.";

            //User not Authorized for some Nodes (user owns 9019303353, 9019303354 but not 9019303350)
            IRoleIdentity roleIdentity8 = new RoleIdentity(false, "tagControllerIntegration2@tresearch.system", "user");
            var tagNameCase8 = "Tresearch Controller Delete Tag3";
            var nodeListCase8 = new List<long> { 9019303350, 9019303353, 9019303354 };
            var resultCase8 = "403: Database: You are not authorized to perform this operation.";

            //User not Authorized for node
            IRoleIdentity roleIdentity9 = new RoleIdentity(false, "tagControllerIntegration2@tresearch.system", "user");
            var tagNameCase9 = "Tresearch Controller Delete Tag3";
            var nodeListCase9 = new List<long> { 9019303350 };
            var resultCase9 = "403: Database: You are not authorized to perform this operation.";

            //User has not been authenticated
            IRoleIdentity roleIdentity10 = new RoleIdentity(false, "guest", "guest");
            var tagNameCase10 = "Tresearch Controller Delete Tag3";
            var nodeListCase10 = new List<long> { 9019303350 };
            var resultCase10 = "401: Server: No active session found. Please login and try again.";

            //User is not enabled
            IRoleIdentity roleIdentity11 = new RoleIdentity(false, "tagControllerIntegrationNotEnabled@tresearch.system", "user");
            var tagNameCase11 = "Tresearch Controller Delete Tag3";
            var nodeListCase11 = new List<long> { 9019303365 };
            var resultCase11 = "401: Database: Account disabled. Perform account recovery or contact system admin.";

            //User is not confirmed
            IRoleIdentity roleIdentity12 = new RoleIdentity(false, "tagControllerIntegrationNotConfirmed@tresearch.system", "user");
            var tagNameCase12 = "Tresearch Controller Delete Tag3";
            var nodeListCase12 = new List<long> { 9019303366 };
            var resultCase12 = "401: Database: Please confirm your account before attempting to login.";

            //Account  does not exist
            IRoleIdentity roleIdentity13 = new RoleIdentity(false, "tagControllerNoAccount@tresearch.system", "user");
            var tagNameCase13 = "Tresearch Controller Delete Tag3";
            var nodeListCase13 = new List<long> { };
            var resultCase13 = "500: Database: The Account was not found.";

            return new[]
            {
                new object[] { roleIdentity0, nodeListCase0, tagNameCase0, resultCase0 },
                new object[] { roleIdentity1, nodeListCase1, tagNameCase1, resultCase1 },
                new object[] { roleIdentity2, nodeListCase2, tagNameCase2, resultCase2 },
                new object[] { roleIdentity3, nodeListCase3, tagNameCase3, resultCase3 },
                new object[] { roleIdentity4, nodeListCase4, tagNameCase4, resultCase4 },
                new object[] { roleIdentity5, nodeListCase5, tagNameCase5, resultCase5 },
                new object[] { roleIdentity6, nodeListCase6, tagNameCase6, resultCase6 },
                new object[] { roleIdentity7, nodeListCase7, tagNameCase7, resultCase7 },
                new object[] { roleIdentity8, nodeListCase8, tagNameCase8, resultCase8 },
                new object[] { roleIdentity9, nodeListCase9, tagNameCase9, resultCase9 },
                new object[] { roleIdentity10, nodeListCase10, tagNameCase10, resultCase10 },
                new object[] { roleIdentity11, nodeListCase11, tagNameCase11, resultCase11 },
                new object[] { roleIdentity12, nodeListCase12, tagNameCase12, resultCase12 },
                new object[] { roleIdentity13, nodeListCase13, tagNameCase13, resultCase13 }
            };
        }

        [Theory]
        [MemberData(nameof(GetNodeTagData))]
        public async Task GetNodeTagsAsync(IRoleIdentity roleIdentity, List<long> nodeIDs, List<string> expectedTags, string expected)
        {
            //Arrange
            IRolePrincipal rolePrincipal = new RolePrincipal(roleIdentity);
            if (!roleIdentity.Name.Equals("guest"))
                Thread.CurrentPrincipal = rolePrincipal;

            string[] exps = expected.Split(":");
            ObjectResult expectedResult = new ObjectResult(exps[2])
            { StatusCode = Convert.ToInt32(exps[0]) };

            ITagController tagController = TestProvider.GetService<ITagController>();

            //Act
            IActionResult results = await tagController.GetNodeTagsAsync(nodeIDs);
            var result = results as ObjectResult;

            //Arrange
            Assert.Equal(expectedResult.StatusCode, result.StatusCode);
        }

        public static IEnumerable<object[]> GetNodeTagData()
        {
            /*User Authorized, Nodes contain shared tags but also have other tags
             *          9019303356: Tresearch Controller Get Tag1, Tresearch Controller Get Tag2
             *          9019303357: Tresearch Controller Get Tag1, Tresearch Controller Get Tag2
             *          9019303358: Tresearch Controller Get Tag1, Tresearch Controller Get Tag2, Tresearch Controller Get Tag3
             */

            IRoleIdentity roleIdentity0 = new RoleIdentity(false, "tagControllerIntegration3@tresearch.system", "user");
            var tagListCase0 = new List<string> { "Tresearch Controller Get Tag1", "Tresearch Controller Get Tag2" };
            var nodeListCase0 = new List<long> { 9019303356, 9019303357, 9019303358 };
            var resultCase0 = "200: Server: Tag(s) retrieved.";

            /*User Authorized, Nodes do not contain shared tags
             *          9019303358: Tresearch Controller Get Tag1, Tresearch Controller Get Tag2, Tresearch Controller Get Tag3
             *          9019303359: Tresearch Controller Get Tag2
             *          9019303360: Tresearch Controller Get Tag1
             */
            IRoleIdentity roleIdentity1 = new RoleIdentity(false, "tagControllerIntegration3@tresearch.system", "user");
            var tagListCase1 = new List<string> { };
            var nodeListCase1 = new List<long> { 9019303358, 9019303359, 9019303360 };
            var resultCase1 = "200: Server: Tag(s) retrieved.";

            //User Authorized, Grab Single Node's Tags (9019303356: Tresearch Controller Get Tag1, Tresearch Controller Get Tag2)
            IRoleIdentity roleIdentity2 = new RoleIdentity(false, "tagControllerIntegration3@tresearch.system", "user");
            var tagListCase2 = new List<string> { "Tresearch Controller Get Tag1", "Tresearch Controller Get Tag2" };
            var nodeListCase2 = new List<long> { 9019303356 };
            var resultCase2 = "200: Server: Tag(s) retrieved.";

            //User Authorized, Node does not have tag
            IRoleIdentity roleIdentity3 = new RoleIdentity(false, "tagControllerIntegration3@tresearch.system", "user");
            var tagListCase3 = new List<string> { };
            var nodeListCase3 = new List<long> { 9019303361 };
            var resultCase3 = "200: Server: Tag(s) retrieved.";

            //User Authorized, No node is passed in
            IRoleIdentity roleIdentity4 = new RoleIdentity(false, "tagControllerIntegration3@tresearch.system", "user");
            var tagListCase4 = new List<string> { };
            var nodeListCase4 = new List<long> { };
            var resultCase4 = "404: Database: Node not found.";


            //User not Auhorized For All Nodes
            IRoleIdentity roleIdentity5 = new RoleIdentity(false, "tagControllerIntegration2@tresearch.system", "user");
            var tagListCase5 = new List<string> { };
            var nodeListCase5 = new List<long> { 9019303350, 9019303351, 9019303352 };
            var resultCase5 = "403: Database: You are not authorized to perform this operation.";

            //User not Authorized for some Nodes (user owns 9019303353, 9019303354 but not 9019303350)
            IRoleIdentity roleIdentity6 = new RoleIdentity(false, "tagControllerIntegration2@tresearch.system", "user");
            var tagListCase6 = new List<string> { };
            var nodeListCase6 = new List<long> { 9019303350, 9019303353, 9019303354 };
            var resultCase6 = "403: Database: You are not authorized to perform this operation.";

            //User not Authorized for node
            IRoleIdentity roleIdentity7 = new RoleIdentity(false, "tagControllerIntegration2@tresearch.system", "user");
            var tagListCase7 = new List<string> { };
            var nodeListCase7 = new List<long> { 9019303350 };
            var resultCase7 = "403: Database: You are not authorized to perform this operation.";

            //User has not been authenticated
            IRoleIdentity roleIdentity8 = new RoleIdentity(false, "guest", "guest");
            var tagListCase8 = new List<string> { };
            var nodeListCase8 = new List<long> { 9019303350 };
            var resultCase8 = "401: Server: No active session found. Please login and try again.";

            //User is not enabled
            IRoleIdentity roleIdentity9 = new RoleIdentity(false, "tagControllerIntegrationNotEnabled@tresearch.system", "user");
            var tagListCase9 = new List<string> { };
            var nodeListCase9 = new List<long> { 9019303365 };
            var resultCase9 = "401: Database: Account disabled. Perform account recovery or contact system admin.";

            //User is not confirmed
            IRoleIdentity roleIdentity10 = new RoleIdentity(false, "tagControllerIntegrationNotConfirmed@tresearch.system", "user");
            var tagListCase10 = new List<string> { };
            var nodeListCase10 = new List<long> { 9019303366 };
            var resultCase10 = "401: Database: Please confirm your account before attempting to login.";

            //Account  does not exist
            IRoleIdentity roleIdentity11 = new RoleIdentity(false, "tagControllerNoAccount@tresearch.system", "user");
            var tagListCase11 = new List<string> { };
            var nodeListCase11 = new List<long> { };
            var resultCase11 = "500: Database: The Account was not found.";

            return new[]
            {
                new object[] { roleIdentity0, nodeListCase0, tagListCase0, resultCase0 },
                new object[] { roleIdentity1, nodeListCase1, tagListCase1, resultCase1 },
                new object[] { roleIdentity2, nodeListCase2, tagListCase2, resultCase2 },
                new object[] { roleIdentity3, nodeListCase3, tagListCase3, resultCase3 },
                new object[] { roleIdentity4, nodeListCase4, tagListCase4, resultCase4 },
                new object[] { roleIdentity5, nodeListCase5, tagListCase5, resultCase5 },
                new object[] { roleIdentity6, nodeListCase6, tagListCase6, resultCase6 },
                new object[] { roleIdentity7, nodeListCase7, tagListCase7, resultCase7 },
                new object[] { roleIdentity8, nodeListCase8, tagListCase8, resultCase8 },
                new object[] { roleIdentity9, nodeListCase9, tagListCase9, resultCase9 },
                new object[] { roleIdentity10, nodeListCase10, tagListCase10, resultCase10 },
                new object[] { roleIdentity11, nodeListCase11, tagListCase11, resultCase11 }
            };
        }

        [Theory]
        [MemberData(nameof(CreateTagData))]
        public async Task CreateTag(IRoleIdentity roleIdentity, string tagName, string expected)
        {
            //Arrange
            IRolePrincipal rolePrincipal = new RolePrincipal(roleIdentity);
            if (!roleIdentity.Name.Equals("guest"))
                Thread.CurrentPrincipal = rolePrincipal;

            string[] exps = expected.Split(":");
            ObjectResult expectedResult = new ObjectResult(exps[2])
            { StatusCode = Convert.ToInt32(exps[0]) };

            ITagController tagController = TestProvider.GetService<ITagController>();

            //Act
            IActionResult results = await tagController.CreateTagAsync(tagName);
            var result = results as ObjectResult;

            //Arrange
            Assert.Equal(expectedResult.StatusCode, result.StatusCode);
            Assert.Equal(expectedResult.Value, expectedResult.Value);
        }

        public static IEnumerable<object[]> CreateTagData()
        {
            //User is Authenticated and Authorized as admin, tag doesn't exist
            IRoleIdentity roleIdentity0 = new RoleIdentity(false, "tagControllerIntegrationAdmin1@tresearch.system", "admin");
            string tagName0 = "Tresearch Controller Doesnt Exist";
            string resultCase0 = "200: Server: Tag created in tag bank.";

            //User is Authenticated and Authorized as admin, tag exists
            IRoleIdentity roleIdentity1 = new RoleIdentity(false, "tagControllerIntegrationAdmin1@tresearch.system", "admin");
            string tagName1 = "Tresearch Controller Tag Exist";
            string resultCase1 = "409: Database: The tag already exists.";

            //User is Authenticated but not authorized (user)
            IRoleIdentity roleIdentity2 = new RoleIdentity(false, "tagControllerIntegration1@gmail.com", "user");
            string tagName2 = "Tresearch Controller Doesnt Managaer";
            string resultCase2 = "403: Database: You are not authorized to perform this operation.";

            //User is not Authenticated
            IRoleIdentity roleIdentity3 = new RoleIdentity(false, "guest", "guest");
            string tagName3 = "Tresearch Controller Doesnt Matter";
            string resultCase3 = "401: Server: No active session found. Please login and try again.";


            //Account  does not exist
            IRoleIdentity roleIdentity4 = new RoleIdentity(false, "tagControllerNoAccount@tresearch.system", "admin");
            string tagName4 = "Tresearch Controller Doesnt Matter";
            var resultCase4 = "500: Database: The Account was not found.";

            return new[]
           {
                new object[] { roleIdentity0, tagName0, resultCase0 },
                new object[] { roleIdentity1, tagName1, resultCase1 },
                new object[] { roleIdentity2, tagName2, resultCase2 },
                new object[] { roleIdentity3, tagName3, resultCase3 },
                new object[] { roleIdentity4, tagName4, resultCase4 }
            };
        }

        [Theory]
        [MemberData(nameof(DeleteTagData))]
        public async Task DeleteTagAsync(IRoleIdentity roleIdentity, string tagName, string expected)
        {
            //Arrange
            IRolePrincipal rolePrincipal = new RolePrincipal(roleIdentity);
            if (!roleIdentity.Name.Equals("guest"))
                Thread.CurrentPrincipal = rolePrincipal;
            string[] exps = expected.Split(":");
            ObjectResult expectedResult = new ObjectResult(exps[2])
            { StatusCode = Convert.ToInt32(exps[0]) };

            ITagController tagController = TestProvider.GetService<ITagController>();

            //Act
            IActionResult results = await tagController.DeleteTagAsync(tagName);
            var result = results as ObjectResult;

            //Arrange
            Assert.Equal(expectedResult.StatusCode, result.StatusCode);
            Assert.Equal(expectedResult.Value, result.Value);
        }

        public static IEnumerable<object[]> DeleteTagData()
        {
            //User is Authenticated and Authorized as admin, tag doesn't exist
            IRoleIdentity roleIdentity0 = new RoleIdentity(false, "tagControllerIntegrationAdmin1@tresearch.system", "admin");
            string tagName0 = "Tresearch Controller REMOVE Doesnt Exist";
            string resultCase0 = "200: Server: Tag removed from tag bank.";

            //User is Authenticated and Authorized as admin, tag exists
            IRoleIdentity roleIdentity1 = new RoleIdentity(false, "tagControllerIntegrationAdmin1@tresearch.system", "admin");
            string tagName1 = "Tresearch Controller REMOVE Tag Exist";
            string resultCase1 = "200: Server: Tag removed from tag bank.";

            //User is Authenticated but not authorized (user)
            IRoleIdentity roleIdentity2 = new RoleIdentity(false, "tagControllerIntegration1@gmail.com", "user");
            string tagName2 = "Tresearch Controller Doesnt Matter";
            string resultCase2 = "403: Database: You are not authorized to perform this operation.";

            //User is not Authenticated
            IRoleIdentity roleIdentity3 = new RoleIdentity(false, "guest", "guest");
            string tagName3 = "Tresearch Controller Doesnt Matter";
            string resultCase3 = "401: Server: No active session found. Please login and try again.";


            //Account  does not exist
            IRoleIdentity roleIdentity4 = new RoleIdentity(false, "tagControllerNoAccount@tresearch.system", "admin");
            string tagName4 = "Tresearch Controller Doesnt Matter";
            var resultCase4 = "500: Database: The Account was not found.";

            //User is Authenticated and Authorized as admin, tag exists and other nodes have this tag
            IRoleIdentity roleIdentity5 = new RoleIdentity(false, "tagControllerIntegrationAdmin1@tresearch.system", "admin");
            string tagName5 = "Tresearch Controller REMOVE Exist and Tagged";
            string resultCase5 = "200: Server: Tag removed from tag bank.";

            return new[]
           {
                new object[] { roleIdentity0, tagName0, resultCase0 },
                new object[] { roleIdentity1, tagName1, resultCase1 },
                new object[] { roleIdentity2, tagName2, resultCase2 },
                new object[] { roleIdentity3, tagName3, resultCase3 },
                new object[] { roleIdentity4, tagName4, resultCase4 },
                new object[] { roleIdentity5, tagName5, resultCase5 }
            };
        }

        [Theory]
        [MemberData(nameof(GetTagData))]
        public async Task GetTagsAsync(IRoleIdentity roleIdentity, string expected)
        {
            //Arrange
            IRolePrincipal rolePrincipal = new RolePrincipal(roleIdentity);
            if (!roleIdentity.Name.Equals("guest"))
                Thread.CurrentPrincipal = rolePrincipal;

            string[] exps = expected.Split(":");
            ObjectResult expectedResult = new ObjectResult(exps[2])
            { StatusCode = Convert.ToInt32(exps[0]) };

            ITagController tagController = TestProvider.GetService<ITagController>();

            //Act
            IActionResult resultTags = await tagController.GetTagsAsync();
            var result = resultTags as ObjectResult;

            //Arrange
            Assert.Equal(expectedResult.StatusCode, result.StatusCode);
        }

        public static IEnumerable<object[]> GetTagData()
        {
            //User is Authenticated and Authorized 
            IRoleIdentity roleIdentity0 = new RoleIdentity(false, "tagControllerIntegration1@tresearch.system", "user");
            string resultCase0 = "200: Server: Tag(s) retrieved.";

            //User is Authenticated and Authorized and Admin
            IRoleIdentity roleIdentity1 = new RoleIdentity(false, "tagControllerIntegrationAdmin1@tresearch.system", "admin");
            string resultCase1 = "200: Server: Tag(s) retrieved.";

            //User has not been authenticated
            IRoleIdentity roleIdentity2 = new RoleIdentity(false, "guest", "guest");
            string resultCase2 = "401: Server: No active session found. Please login and try again.";

            //User is not enabled
            IRoleIdentity roleIdentity3 = new RoleIdentity(false, "tagControllerIntegrationNotEnabled@tresearch.system", "user");
            string resultCase3 = "401: Database: Account disabled. Perform account recovery or contact system admin.";

            //User is not confirmed
            IRoleIdentity roleIdentity4 = new RoleIdentity(false, "tagControllerIntegrationNotConfirmed@tresearch.system", "user");
            string resultCase4 = "401: Database: Please confirm your account before attempting to login.";

            //User is has unknown role
            IRoleIdentity roleIdentity5 = new RoleIdentity(false, "tagControllerIntegrationAdmin1@tresearch.system", "wrong");
            string resultCase5 = "400: Server: Unknown role used.";

            //Account not found
            IRoleIdentity roleIdentity6 = new RoleIdentity(false, "tagControllerNoAccount@tresearch.system", "user");
            var resultCase6 = "500: Database: The Account was not found.";

            return new[]
            {
                new object[] { roleIdentity0, resultCase0 },
                new object[] { roleIdentity1, resultCase1 },
                new object[] { roleIdentity2, resultCase2 },
                new object[] { roleIdentity3, resultCase3 },
                new object[] { roleIdentity4, resultCase4 },
                new object[] { roleIdentity5, resultCase5 },
                new object[] { roleIdentity6, resultCase6 }
            };
        }
    }

    /// <summary>
    ///     Tag Controller Database Fixture Initializes the database. Runs the startup script and upon completion of testing, disposes all data created
    ///     while performing tests
    /// </summary>
    public class TagControllerDatabaseFixture : TestBaseClass, IDisposable
    {
        public TagControllerDatabaseFixture() : base(new string[] { })
        {
            TestProvider = TestServices.BuildServiceProvider();
            IOptionsSnapshot<BuildSettingsOptions> options = TestProvider.GetService<IOptionsSnapshot<BuildSettingsOptions>>();
            BuildSettingsOptions optionsValue = options.Value;

            string script = File.ReadAllText("../../../IntegrationTests/Tag/SetupAndCleanup/ControllerIntegrationSetup.sql");

            IEnumerable<string> commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (var connection = new SqlConnection(optionsValue.SqlConnectionString))
            {
                connection.Open();
                foreach (string command in commands)
                    if (!string.IsNullOrWhiteSpace(command.Trim()))
                        using (var com = new SqlCommand(command, connection))
                            com.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            IOptionsSnapshot<BuildSettingsOptions> options = TestProvider.GetService<IOptionsSnapshot<BuildSettingsOptions>>();
            BuildSettingsOptions optionsValue = options.Value;

            string script = File.ReadAllText("../../../IntegrationTests/Tag/SetupAndCleanup/ControllerIntegrationCleanup.sql");

            IEnumerable<string> commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (var connection = new SqlConnection(optionsValue.SqlConnectionString))
            {
                connection.Open();
                foreach (string command in commands)
                    if (!string.IsNullOrWhiteSpace(command.Trim()))
                        using (var com = new SqlCommand(command, connection))
                            com.ExecuteNonQuery();
            }
        }
    }
}
