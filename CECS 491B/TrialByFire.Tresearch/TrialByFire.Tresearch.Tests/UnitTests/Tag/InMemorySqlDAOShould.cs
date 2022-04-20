﻿using Microsoft.Extensions.DependencyInjection;
using TrialByFire.Tresearch.DAL.Contracts;
using TrialByFire.Tresearch.DAL.Implementations;
using TrialByFire.Tresearch.Models.Contracts;
using Xunit;

namespace TrialByFire.Tresearch.Tests.UnitTests.Tag
{
    public class InMemorySqlDAOShould: TestBaseClass
    {
        public InMemorySqlDAOShould() : base(new string[] { })
        {
            TestServices.AddScoped<ISqlDAO, InMemorySqlDAO>();
            TestProvider = TestServices.BuildServiceProvider();
        }


        /// <summary>
        ///     Tests user creating tag in tag bank
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <param name="count">Number of nodes tagged</param>
        /// <param name="response">Enumerated Response based on case</param>
        /// <returns></returns>
        [Theory]
        [MemberData(nameof(CreateTagData))]
        public async Task CreateTagAsync(string tagName, int count, IMessageBank.Responses response)
        {
            //Arrange
            ISqlDAO sqlDAO = TestProvider.GetService<ISqlDAO>();
            IMessageBank messageBank = TestProvider.GetService<IMessageBank>();
            string expected = await messageBank.GetMessage(response);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            //Act
            string result = await sqlDAO.CreateTagAsync(tagName, count, cancellationTokenSource.Token);

            //Arrange
            Assert.Equal(expected, result);
        }

        /// <summary>
        ///     Tests user deleting tag from tag bank
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <param name="response">Cancellation Token</param>
        /// <returns></returns>
        [Theory]
        [MemberData(nameof(DeleteTagData))]
        public async Task DeleteTagAsync(string tagName,  IMessageBank.Responses response)
        {
            //Arrange
            ISqlDAO sqlDAO = TestProvider.GetService<ISqlDAO>();
            IMessageBank messageBank = TestProvider.GetService<IMessageBank>();
            string expected = await messageBank.GetMessage(response);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            //Act
            string result = await sqlDAO.DeleteTagAsync(tagName, cancellationTokenSource.Token);

            //Arrange
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetTagsAsync()
        {
            //Arrange
            string expected = "200: Server: Tag(s) retrieved.";
            ISqlDAO sqlDAO = TestProvider.GetService<ISqlDAO>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            //Act
            Tuple<List<ITag>, string> resultTags = await sqlDAO.GetTagsAsync(cancellationTokenSource.Token);
            string result = resultTags.Item2;

            //Assert
            Assert.Equal(expected, result);
        }
        [Theory]
        [MemberData(nameof(AddTagData))]
        public async Task AddTagToNodes(List<long> nodeIDs, string tagName, string expected)
        {
            //Arrange
            ISqlDAO sqlDAO = TestProvider.GetService<ISqlDAO>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            //Act
            string result = await sqlDAO.AddTagAsync(nodeIDs, tagName, cancellationTokenSource.Token);

            //Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> AddTagData()
        {
            //nodes already have tag
            var tagNameCase0 = "Tresearch SqlDAO Add Tag1";
            var nodeListCase0 = new List<long> { 2022030533, 2022030534, 2022030535 };
            var resultCase0 = "200: Server: Tag added to node(s).";

            //nodes do not contain these tags already
            var tagNameCase1 = "Tresearch SqlDAO Add Tag2";
            var nodeListCase1 = new List<long> { 2022030533, 2022030534, 2022030535 };
            var resultCase1 = "200: Server: Tag added to node(s).";

            // node doesn't already contain tag
            var tagNameCase2 = "Tresearch SqlDAO Add Tag3";
            var nodeListCase2 = new List<long> { 2022030533 };
            var resultCase2 = "200: Server: Tag added to node(s).";

            //Node already has tag
            var tagNameCase3 = "Tresearch SqlDAO Add Tag4";
            var nodeListCase3 = new List<long> { 2022030533 };
            var resultCase3 = "200: Server: Tag added to node(s).";

            //Tag doesn't exist
            var tagNameCase4 = "Tresearch SqlDAO Add Tag5";
            var nodeListCase4 = new List<long> { 2022030533 };
            var resultCase4 = "404: Database: Tag not found.";

            return new[]
            {
                new object[] { nodeListCase0, tagNameCase0, resultCase0 },
                new object[] { nodeListCase1, tagNameCase1, resultCase1 },
                new object[] { nodeListCase2, tagNameCase2, resultCase2 },
                new object[] { nodeListCase3, tagNameCase3, resultCase3 },
                new object[] { nodeListCase4, tagNameCase4, resultCase4 }
            };
        }

        [Theory]
        [MemberData(nameof(GetNodeTagData))]
        public async Task GetNodeTagsAsync(List<long> nodeIDs, string expected, List<string> expectedTags)
        {
            //Arrange
            ISqlDAO sqlDAO = TestProvider.GetService<ISqlDAO>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            //Act
            Tuple<List<string>, string> myResult = await sqlDAO.GetNodeTagsAsync(nodeIDs, cancellationTokenSource.Token);
            string result = myResult.Item2;
            List<string> resultTags = myResult.Item1;


            //Assert
            Assert.Equal(expected, result);
            Assert.Equal(expectedTags, resultTags);
        }

        public static IEnumerable<object[]> CreateTagData()
        {
            /**
             *  Case 0: Create tag.  Tag does not exist in database
             *      Tag Name: 
             */
            var tagCase0 = "Tresearch SqlDAO test tag1";
            var cntCase0 = 0;
            var expCase0 = IMessageBank.Responses.tagCreateSuccess;


            /**
             *  Case 1: Create tag.  Tag already exists in database
             *      Tag Name: Tresearch SqlDAO This Tag Exists Already
             */
            var tagCase1 = "Tresearch SqlDAO This Tag Exists Already";
            var cntCase1 = 0;
            var expCase1 = IMessageBank.Responses.tagDuplicate;

            /**
             *  Case 2: Create tag.  Tag name is null
             *      Tag Name: 
             */
            string tagCase2 = null;
            var cntCase2 = 0;
            var expCase2 = IMessageBank.Responses.tagNameInvalid;

            /**
            *  Case 3: Create tag.  Tag name is empty
            *      Tag Name: 
            */
            var tagCase3 = "";
            var cntCase3 = 0;
            var expCase3 = IMessageBank.Responses.tagNameInvalid;

            /**
            *  Case 4: Create tag.  Tag count is negative
            *      Tag Name: Tresearch SqlDAO Add
            */
            var tagCase4 = "Tresearch SqlDAO Add1";
            int? cntCase4 = -1;
            var expCase4 = IMessageBank.Responses.tagCountInvalid;

            /**
            *  Case 5: Create tag.  Tag count is not zero
            *      Tag Name: Tresearch SqlDAO Add
            */
            var tagCase5 = "Tresearch SqlDAO Add2";
            var cntCase5 = 20;
            var expCase5 = IMessageBank.Responses.tagCreateSuccess;

            return new[]
            {
                new object[] { tagCase0, cntCase0, expCase0 },
                new object[] { tagCase1, cntCase1, expCase1 },
                new object[] { tagCase2, cntCase2, expCase2 },
                new object[] { tagCase3, cntCase3, expCase3 },
                new object[] { tagCase4, cntCase4, expCase4 },
                new object[] { tagCase5, cntCase5, expCase5 }
            };
        }

        public static IEnumerable<object[]> DeleteTagData()
        {
            
            /**
             *  Case 0: Delete tag.  Tag does not exist in database
             *      Tag Name: Tresearch This Tag Doesnt exist
             */
            var tagCase0 = "Tresearch This Tag Doesnt exist";
            var expCase0 = IMessageBank.Responses.tagDeleteSuccess;


            /**
             *  Case 1: Delete tag.  Tag already exists in database
             *      Tag Name: Tresearch SqlDAO Delete Me Tag
             */
            var tagCase1 = "Tresearch SqlDAO Delete Me Tag";
            var expCase1 = IMessageBank.Responses.tagDeleteSuccess;

            /**
             *  Case 2: Delete tag.  Tag name is null
             *      Tag Name: 
             */
            string tagCase2 = null;
            var expCase2 = IMessageBank.Responses.tagNameInvalid;

            /**
            *  Case 3: Delete tag.  Tag name is empty
            *      Tag Name: 
            */
            var tagCase3 = "";
            var expCase3 = IMessageBank.Responses.tagNameInvalid;

            /**
            *  Case 4: Delete tag.  Nodes contain tag.
            *      Tag Name: Tresearch SqlDAO Delete Me Tag1
            */
            var tagCase4 = "Tresearch SqlDAO Delete Me Tag1";
            var expCase4 = IMessageBank.Responses.tagDeleteSuccess;

            return new[]
            {
                new object[] { tagCase0, expCase0 },
                new object[] { tagCase1, expCase1 },
                new object[] { tagCase2, expCase2 },
                new object[] { tagCase3, expCase3 },
                new object[] { tagCase4, expCase4 }
            };
        }
        public static IEnumerable<object[]> GetNodeTagData()
        {
            /**Nodes contain shared tags
             *      
             */
            var nodeListCase0 = new List<long> { 2022030539, 2022030540, 2022030541 };
            string expectedCase0 = "200: Server: Tag(s) retrieved.";
            var expectedTags0 = new List<string> { "Tresearch SqlDAO Get Tag1", "Tresearch SqlDAO Get Tag2" };

            /**Nodes contain no shared tags
             * 
             */
            var nodeListCase1 = new List<long> { 2022030539, 2022030540, 2022030541, 2022030542 };
            string expectedCase1 = "200: Server: Tag(s) retrieved.";
            var expectedTags1 = new List<string> { };

            //Node has tags
            var nodeListCase2 = new List<long> { 2022030539 };
            string expectedCase2 = "200: Server: Tag(s) retrieved.";
            var expectedTags2 = new List<string> { "Tresearch SqlDAO Get Tag1", "Tresearch SqlDAO Get Tag2", "Tresearch SqlDAO Get Tag3" };

            //Node contains no tags
            var nodeListCase3 = new List<long> { 2022030543 };
            string expectedCase3 = "200: Server: Tag(s) retrieved.";
            var expectedTags3 = new List<string> { };

            //No nodes passed in
            var nodeListCase4 = new List<long> { };
            string expectedCase4 = "200: Server: Tag(s) retrieved.";
            var expectedTags4 = new List<string> { };

            return new[]
            {
                new object[] { nodeListCase0, expectedCase0, expectedTags0 },
                new object[] { nodeListCase1, expectedCase1, expectedTags1},
                new object[] { nodeListCase2, expectedCase2, expectedTags2 },
                new object[] { nodeListCase3, expectedCase3, expectedTags3 },
                new object[] { nodeListCase4, expectedCase4, expectedTags4 }
            };
        }

        [Theory]
        [MemberData(nameof(RemoveTagData))]
        public async Task RemoveNodeTagAsync(List<long> nodeIDs, string tagName, string expected)
        {
            //Arrange
            ISqlDAO sqlDAO = TestProvider.GetService<ISqlDAO>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            //Act
            string result = await sqlDAO.RemoveTagAsync(nodeIDs, tagName, cancellationTokenSource.Token);

            //Arrange
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> RemoveTagData()
        {
            //nodes already have tag
            var tagNameCase0 = "Tresearch SqlDAO Delete Tag1";
            var nodeListCase0 = new List<long> { 2022030536, 2022030537, 2022030538 };
            var resultCase0 = "200: Server: Tag removed from node(s).";

            //nodes do not contain these tags already
            var tagNameCase1 = "Tresearch SqlDAO Delete Tag2";
            var nodeListCase1 = new List<long> { 2022030536, 2022030537, 2022030538 };
            var resultCase1 = "200: Server: Tag removed from node(s).";

            // node doesn't already contain tag
            var tagNameCase2 = "Tresearch SqlDAO Delete Tag3";
            var nodeListCase2 = new List<long> { 2022030536 };
            var resultCase2 = "200: Server: Tag removed from node(s).";

            //Node already has tag
            var tagNameCase3 = "Tresearch SqlDAO Delete Tag4";
            var nodeListCase3 = new List<long> { };
            var resultCase3 = "200: Server: Tag removed from node(s).";

            return new[]
            {
                new object[] { nodeListCase0, tagNameCase0, resultCase0 },
                new object[] { nodeListCase1, tagNameCase1, resultCase1 },
                new object[] { nodeListCase2, tagNameCase2, resultCase2 },
                new object[] { nodeListCase3, tagNameCase3, resultCase3 }
            };
        }
    }
}
