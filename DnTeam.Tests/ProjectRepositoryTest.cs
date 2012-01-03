using System.Linq;
using DnTeamData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DnTeamData.Models;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeam.Tests
{
    /// <summary>
    ///This is a test class for ProjectRepositoryTest and is intended
    ///to contain all ProjectRepositoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class ProjectRepositoryTest
    {

        private const string CollectionName = "Projects_Test";
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Project> Coll = Db.GetCollection<Project>(CollectionName);

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            ProjectRepository.SetTestCollection(CollectionName);
            Coll.Drop();

            Coll.EnsureIndex(new IndexKeysBuilder().Ascending("ProductId").Ascending("Name").Ascending("IsDeleted"), IndexOptions.SetUnique(true));
            Coll.EnsureIndex(new IndexKeysBuilder().Ascending("IsDeleted"), IndexOptions.SetUnique(false));
            Coll.EnsureIndex(new IndexKeysBuilder().Ascending("_id").Ascending("Milestones.Name"), IndexOptions.SetUnique(true));
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            Coll.Drop();
        }

        #endregion

        /// <summary>
        ///A test for GetAllProjects
        ///</summary>
        [TestMethod]
        public void GetAllProjectsTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            ProjectRepository.Insert("name2", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            var expected = new List<string> { "name1", "name2" };

            var actual = ProjectRepository.GetAllProjects().Select(o => o.Name).ToList();
            
            Assert.IsTrue(expected.SequenceEqual(actual));
            
        }

        /// <summary>
        ///A test for InsertMilestone
        ///</summary>
        [TestMethod]
        public void InsertMilestoneTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            string name = string.Empty;
            string targetDate = DateTime.Now.AddDays(-10).ToShortDateString();
            string actualDate = DateTime.Now.AddDays(10).ToShortDateString();
            const int index = 1;
            const bool addName = false;

            //MilestoneEditStatus.ErrorNameIsEmpty----------------------------//
            MilestoneEditStatus expected = MilestoneEditStatus.ErrorNoName;
            MilestoneEditStatus actual = ProjectRepository.InsertMilestone(projectId, name, targetDate, actualDate, index, addName);
           
            Assert.AreEqual(expected, actual);


            //MilestoneEditStatus.ErrorTargetDateFormat--------//
            expected = MilestoneEditStatus.ErrorTargetDateFormat;
            name = "milestone test 1";
            targetDate = "foo date";
            actual = ProjectRepository.InsertMilestone(projectId, name, targetDate, actualDate, index, addName);

            Assert.AreEqual(expected, actual);


            //MilestoneEditStatus.ErrorActualDateFormat--------//
            expected = MilestoneEditStatus.ErrorActualDateFormat;
            targetDate = DateTime.Now.Date.ToShortDateString();
            actualDate = "foo date";
            actual = ProjectRepository.InsertMilestone(projectId, name, targetDate, actualDate, index, addName);

            Assert.AreEqual(expected, actual);


            //MilestoneEditStatus.Ok--------//
            expected = MilestoneEditStatus.Ok;
            actualDate = string.Empty;
            actual = ProjectRepository.InsertMilestone(projectId, name, targetDate, actualDate, index, addName);

            Assert.AreEqual(expected, actual);
            var a = ProjectRepository.GetProject(projectId).Milestones.SingleOrDefault(o => o.Name == name);
            Assert.IsTrue(a != null);
            Assert.AreEqual(targetDate, ((DateTime)a.TargetDate).ToShortDateString());
            Assert.IsTrue(a.ActualDate == null);
            Assert.AreEqual(index, a.Index);

            //MilestoneEditStatus.ErrorDuplicateName--------//
            expected = MilestoneEditStatus.ErrorDuplicateName;
            actual = ProjectRepository.InsertMilestone(projectId, name, targetDate, actualDate, index, addName);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod]
        public void DeleteTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            ProjectRepository.Insert("name2", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            ProjectRepository.Insert("name3", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            const int expectedDeleted = 2;
            IEnumerable<string> values = ProjectRepository.GetAllProjects().Select(o => o.Id.ToString()).Take(expectedDeleted);
            
            ProjectRepository.Delete(values);

            var left = ProjectRepository.GetAllProjects().Count();
            Assert.AreEqual(1, left);

            var deleted = ProjectRepository.GetAllProjects(isDeleted:true).Count();
            Assert.AreEqual(expectedDeleted, deleted);
        }

        /// <summary>
        ///A test for DeleteAssignments
        ///</summary>
        [TestMethod]
        public void DeleteAssignmentsTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            ProjectRepository.InsertAssignment(projectId, "role1", string.Empty, "note 1", DateTime.Now.ToString(), DateTime.Now.ToString(), 10);
            ProjectRepository.InsertAssignment(projectId, "role1", string.Empty, "note 1", DateTime.Now.ToString(), DateTime.Now.ToString(), 10);
            ProjectRepository.InsertAssignment(projectId, "role1", string.Empty, "note 1", DateTime.Now.ToString(), DateTime.Now.ToString(), 10);
            var assignments = ProjectRepository.GetAssignments(projectId).Select(o => o.Id.ToString()).ToList();
            const int toBeDeleted = 2;
            int expected = assignments.Count() - toBeDeleted;
            var values = assignments.Take(toBeDeleted).ToList();
            
            ProjectRepository.DeleteAssignments(projectId, values);

            
            int actual = ProjectRepository.GetAssignments(projectId).Count();
            Assert.AreEqual(expected, actual);
            assignments = ProjectRepository.GetAssignments(projectId).Select(o => o.Id.ToString()).ToList();
            Assert.IsTrue(assignments.Where(values.Contains).Count() == 0);
        }

        /// <summary>
        ///A test for DeleteMilestones
        ///</summary>
        [TestMethod]
        public void DeleteMilestonesTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            ProjectRepository.InsertMilestone(projectId, "name1", string.Empty, string.Empty, addName: false);
            ProjectRepository.InsertMilestone(projectId, "name2", string.Empty, string.Empty, addName: false);
            ProjectRepository.InsertMilestone(projectId, "name3", string.Empty, string.Empty, addName: false);
            var milestones = ProjectRepository.GetMilestones(projectId).Select(o => o.Id.ToString()).ToList();
            const int toBeDeleted = 2;
            int expected = milestones.Count() - toBeDeleted;
            var values = milestones.Take(toBeDeleted).ToList();

            ProjectRepository.DeleteMilestones(projectId, values);

            int actual = ProjectRepository.GetMilestones(projectId).Count();
            Assert.AreEqual(expected, actual);
            milestones = ProjectRepository.GetMilestones(projectId).Select(o => o.Id.ToString()).ToList();
            Assert.IsTrue(milestones.Where(values.Contains).Count() == 0);
        }

        /// <summary>
        ///A test for FinishMilestones
        ///</summary>
        [TestMethod]
        public void FinishMilestonesTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            ProjectRepository.InsertMilestone(projectId, "test_name 1", string.Empty, DateTime.Now.AddDays(-1).ToString(), addName: false);
            ProjectRepository.InsertMilestone(projectId, "test_name 2", string.Empty, string.Empty, addName: false);
            ProjectRepository.InsertMilestone(projectId, "test_name 3", string.Empty, string.Empty, addName: false);
            ProjectRepository.InsertMilestone(projectId, "test_name 4", string.Empty, string.Empty, addName: false);
            var milestones = ProjectRepository.GetMilestones(projectId).Where(o => o.Name == "test_name 1" || o.Name == "test_name 2" || o.Name == "test_name 3")
                .Select(o => o.Id.ToString()).ToList();

            ProjectRepository.FinishMilestones(projectId, milestones);

            var actual = ProjectRepository.GetMilestones(projectId).Where(o=>o.ActualDate == DateTime.Now.Date).Select(o=>o.Name).ToList();
            Assert.AreEqual(2, actual.Count());
            Assert.IsTrue(actual.Contains("test_name 2") && actual.Contains("test_name 3"));
        }

        /// <summary>
        ///A test for GetAssignments
        ///</summary>
        [TestMethod]
        public void GetAssignmentsTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            ProjectRepository.InsertAssignment(projectId, "test_role1", string.Empty, "note 1", DateTime.Now.ToString(), DateTime.Now.ToString(), 10);
            ProjectRepository.InsertAssignment(projectId, "test_role1", string.Empty, "note 1", DateTime.Now.ToString(), DateTime.Now.ToString(), 10);
            ProjectRepository.InsertAssignment(projectId, "test_role1", string.Empty, "note 1", DateTime.Now.ToString(), DateTime.Now.ToString(), 10);

            var actual = ProjectRepository.GetAssignments(projectId).Where(o=>o.Role.StartsWith("test_role"));
            
            Assert.AreEqual(3, actual.Count());
        }

        /// <summary>
        ///A test for GetMilestones
        ///</summary>
        [TestMethod]
        public void GetMilestonesTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            ProjectRepository.InsertMilestone(projectId, "test_name1", string.Empty, string.Empty, addName: false);
            ProjectRepository.InsertMilestone(projectId, "test_name2", string.Empty, string.Empty, addName: false);
            ProjectRepository.InsertMilestone(projectId, "test_name3", string.Empty, string.Empty, addName: false);

            var actual = ProjectRepository.GetMilestones(projectId).Where(o => o.Name.StartsWith("test_name"));

            Assert.AreEqual(3, actual.Count());
        }

        /// <summary>
        ///A test for GetPersonProjectSpecialties
        ///</summary>
        [TestMethod]
        public void GetPersonProjectSpecialtiesTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            var minDate = DateTime.Now.AddDays(-10).ToShortDateString();
            var maxDate = DateTime.Now.AddDays(10).ToShortDateString();
            var personId = ObjectId.GenerateNewId().ToString();
            ProjectRepository.InsertAssignment(projectId, "test_role1", personId, "note 1", minDate, DateTime.Now.ToString(), 10);
            ProjectRepository.InsertAssignment(projectId, "test_role2", personId, "note 1", DateTime.Now.ToString(), DateTime.Now.ToString(), 10);
            ProjectRepository.InsertAssignment(projectId, "test_role3", personId, "note 1", maxDate,maxDate, 10);
            ProjectRepository.InsertAssignment(projectId, "test_role4", string.Empty, "note 1", maxDate, DateTime.Now.ToString(), 10);
            ProjectRepository.Insert("name2", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);

            var actual = ProjectRepository.GetPersonProjectSpecialties(personId).ToList();
            
            Assert.IsTrue(actual.Count() == 1);
            Assert.IsTrue(actual[0].FirstUsed == DateTime.Parse(minDate));
            Assert.IsTrue(actual[0].LastUsed == DateTime.Parse(maxDate));
            Assert.IsTrue(actual[0].Name == "name1");
            Assert.IsTrue(actual[0].ProjectId == projectId);
            Assert.IsTrue(actual[0].Roles == "test_role1, test_role2, test_role3");
        }

        /// <summary>
        ///A test for GetProject
        ///</summary>
        [TestMethod]
        public void GetProjectTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            Project expected = ProjectRepository.GetAllProjects().First();
            string projectId = expected.Id.ToString();

            Project actual = ProjectRepository.GetProject(projectId);
            
            Assert.AreEqual(expected.Id, actual.Id);
        }

        /// <summary>
        ///A test for Insert
        ///</summary>
        [TestMethod]
        public void InsertTest()
        {
            string name = "project1";
            const string priority = "priority1";
            DateTime createdDate = DateTime.Now;
            const string status = "status1";
            const string noise = "noise1";
            string product = ObjectId.GenerateNewId().ToString();
            const string projectType = "type1";
            string programManager = string.Empty;
            string technicalLead = string.Empty;

            //ProjectEditStatus.Ok--------------------------//
            ProjectEditStatus expected = ProjectEditStatus.Ok;
            
            ProjectEditStatus actual = ProjectRepository.Insert(name, priority, createdDate, status, noise, product, projectType, programManager, technicalLead);
            
            Assert.AreEqual(expected, actual);

            //ProjectEditStatus.ErrorDuplicateItem--------//
            expected = ProjectEditStatus.ErrorDuplicateItem;

            actual = ProjectRepository.Insert(name, priority, createdDate, status, noise, product, projectType, programManager, technicalLead);
            
            Assert.AreEqual(expected, actual);


            //ProjectEditStatus.ErrorDuplicateItem--------//
            expected = ProjectEditStatus.ErrorNoName;
            name = string.Empty;

            actual = ProjectRepository.Insert(name, priority, createdDate, status, noise, product, projectType, programManager, technicalLead);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for InsertAssignment
        ///</summary>
        [TestMethod]
        public void InsertAssignmentTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            const string role = "test_role1";
            var person = ObjectId.GenerateNewId();
            const string note = "note1";
            string startDate = DateTime.Now.AddDays(-10).ToShortDateString();
            string endDate = DateTime.Now.AddDays(10).ToShortDateString(); 
            const int commitment = 0;
            
            const AssignmentEditStatus expected = AssignmentEditStatus.Ok;

            AssignmentEditStatus actual = ProjectRepository.InsertAssignment(projectId, role, person.ToString(), note, startDate, endDate, commitment);
            
            Assert.AreEqual(expected, actual);
            var a = ProjectRepository.GetAssignments(projectId).Where(o => o.Role == "test_role1").ToList();
            Assert.AreEqual(1, a.Count());
            Assert.AreEqual(person, a[0].PersonId);
            Assert.AreEqual(note, a[0].Note);
            Assert.AreEqual(startDate, a[0].StartDate.ToShortDateString());
            Assert.AreEqual(endDate, a[0].EndDate.ToShortDateString());
            Assert.AreEqual(commitment, a[0].Commitment);
        }

        /// <summary>
        ///A test for UpdateAssignment
        ///</summary>
        [TestMethod]
        public void UpdateAssignmentTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            string role = "test_role1";
            var person = ObjectId.GenerateNewId();
            string note = "note1";
            string startDate = DateTime.Now.ToShortDateString();
            string endDate = DateTime.Now.ToShortDateString();
            int commitment = 0;
            ProjectRepository.InsertAssignment(projectId, role, person.ToString(), note, startDate, endDate, commitment);
            string assignmentId = ProjectRepository.GetAssignments(projectId).SingleOrDefault(o => o.Role == "test_role1").Id.ToString();

            
            role = "test_role2";
            person = ObjectId.GenerateNewId();
            note = "note2";
            startDate = DateTime.Now.AddDays(-10).ToShortDateString();
            endDate = DateTime.Now.AddDays(10).ToShortDateString();
            commitment = 10;
            const AssignmentEditStatus expected = AssignmentEditStatus.Ok;

            AssignmentEditStatus actual = ProjectRepository.UpdateAssignment(projectId, assignmentId, role, person.ToString(), note, startDate, endDate, commitment);
            
            Assert.AreEqual(expected, actual);
            var a = ProjectRepository.GetAssignments(projectId).Where(o => o.Role == "test_role2").ToList();
            Assert.AreEqual(1, a.Count());
            Assert.AreEqual(person, a[0].PersonId);
            Assert.AreEqual(note, a[0].Note);
            Assert.AreEqual(startDate, a[0].StartDate.ToShortDateString());
            Assert.AreEqual(endDate, a[0].EndDate.ToShortDateString());
            Assert.AreEqual(commitment, a[0].Commitment);
        }

        /// <summary>
        ///A test for UpdateMilestone
        ///</summary>
        [TestMethod]
        public void UpdateMilestoneTest()
        {
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", ObjectId.GenerateNewId().ToString(), "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().First().Id.ToString();
            const string name = "test_mistone1";
            string targetDate = DateTime.Now.AddDays(-10).ToShortDateString();
            string actualDate = DateTime.Now.AddDays(10).ToShortDateString();
            ProjectRepository.InsertMilestone(projectId, name, targetDate, actualDate, addName: false);
            string id = ProjectRepository.GetMilestones(projectId).Where(o => o.Name == name).First().Id.ToString();

            
            //MilestoneEditStatus.ErrorTargetDateFormat--------//
            MilestoneEditStatus expected = MilestoneEditStatus.ErrorTargetDateFormat;
            targetDate = "foo date";
            MilestoneEditStatus actual = ProjectRepository.UpdateMilestone(projectId, id, targetDate, actualDate);

            Assert.AreEqual(expected, actual);


            //MilestoneEditStatus.ErrorActualDateFormat--------//
            expected = MilestoneEditStatus.ErrorActualDateFormat;
            targetDate = DateTime.Now.AddDays(-10).ToShortDateString();
            actualDate = "foo date";
            actual = ProjectRepository.UpdateMilestone(projectId, id, targetDate, actualDate);

            Assert.AreEqual(expected, actual);

            //MilestoneEditStatus.Ok--------//
            expected = MilestoneEditStatus.Ok;
            actualDate = DateTime.Now.AddDays(10).ToShortDateString();
            actual = ProjectRepository.UpdateMilestone(projectId, id, targetDate, actualDate);

            Assert.AreEqual(expected, actual);
            var m = ProjectRepository.GetMilestones(projectId).Where(o => o.Name == name).First();
            Assert.AreEqual(DateTime.Parse(targetDate), m.TargetDate);
            Assert.AreEqual(DateTime.Parse(actualDate), m.ActualDate);
        }

        /// <summary>
        ///A test for UpdateProjectProperty
        ///</summary>
        [TestMethod]
        public void UpdateProjectPropertyTest()
        {
            string productId = ObjectId.GenerateNewId().ToString();
            ProjectRepository.Insert("name1", "priority1", DateTime.Now, "status1", "noise1", productId, "projecttype1", string.Empty, string.Empty);
            ProjectRepository.Insert("name2", "priority1", DateTime.Now, "status1", "noise1", productId, "projecttype1", string.Empty, string.Empty);
            string projectId = ProjectRepository.GetAllProjects().Where(o=>o.Name == "name1").First().Id.ToString();

            ProjectEditStatus expected = ProjectEditStatus.ErrorDuplicateItem;
            string name = "Name";
            string value = "name2";

            ProjectEditStatus actual = ProjectRepository.UpdateProjectProperty(projectId, name, value);

            Assert.AreEqual(expected, actual);


            expected = ProjectEditStatus.Ok;
            name = "Name";
            value = "name new";

            actual = ProjectRepository.UpdateProjectProperty(projectId, name, value);

            Assert.AreEqual(expected, actual);


            name = "ProductId";
            value = ObjectId.GenerateNewId().ToString();
            expected = ProjectEditStatus.Ok;
            
            actual = ProjectRepository.UpdateProjectProperty(projectId, name, value);

            Assert.AreEqual(expected, actual);
        }
    }
}
