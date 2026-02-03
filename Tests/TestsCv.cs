using CvBuilderBack.Models;
using Newtonsoft.Json;

namespace Tests;

public class TestsCv
{
    private Cv cv;
    
    [SetUp]
    public void Setup()
    {
        cv = new Cv
        {
            Name = "Name",
            Profession = "Profession",
            AboutMe = "AboutMe",
            Contacts = JsonConvert.SerializeObject(new List<Contact>{new() {Type = 0, Value = "test@example.com"}}),
            Links = JsonConvert.SerializeObject(new List<Link>{new() {Name = "test", Url = "https://example.com"}}),
            Works = JsonConvert.SerializeObject(new List<Work>{new() {Title = "test", From = DateTime.Now, To = DateTime.Now, Description = "test"}}),
            Educations = JsonConvert.SerializeObject(new List<Education>{new() {Title = "test", Date = DateTime.Now}}),
            Projects = JsonConvert.SerializeObject(new List<Project>{new() {Title = "test", Date = DateTime.Now, Description = "test"}}),
            Languages = JsonConvert.SerializeObject(new List<Language>{new() {Name = "test"}}),
            Skills = JsonConvert.SerializeObject(new List<Skill>{new() {Name = "test"}}),
            Hobbies = JsonConvert.SerializeObject(new List<Hobby>{new() {Name = "test"}}),
            CustomHtml = "<h1>test</h1>",
            CustomCss = "h1 {color: red;}"
        };
    }

    [TestCase("Name")]
    [TestCase("Another Name")]
    public void SetName(string name)
    {
        // Arrange (Préparation)
        var isNameEqual = string.Equals(cv.Name, name);
        Assert.That(Cv.SetName(cv, name), Is.EqualTo(!isNameEqual));
    }
    
    [Test]
    public void Duplicate()
    {
        // Act (Action)
        var duplicateCv = Cv.Duplicate(cv);

        // Assert (Null)
        Assert.That(duplicateCv, Is.Not.Null);
        Assert.That(duplicateCv, Is.Not.EqualTo(cv));
        
        // Assert (Fields)
        using (Assert.EnterMultipleScope())
        {
            Assert.That(duplicateCv.Name, Contains.Substring(cv.Name));
            Assert.That(duplicateCv.Profession, Is.EqualTo(cv.Profession));
            Assert.That(duplicateCv.AboutMe, Is.EqualTo(cv.AboutMe));
            Assert.That(duplicateCv.Contacts, Is.EqualTo(cv.Contacts));
            Assert.That(duplicateCv.Links, Is.EqualTo(cv.Links));
            Assert.That(duplicateCv.Works, Is.EqualTo(cv.Works));
            Assert.That(duplicateCv.Educations, Is.EqualTo(cv.Educations));
            Assert.That(duplicateCv.Projects, Is.EqualTo(cv.Projects));
            Assert.That(duplicateCv.Languages, Is.EqualTo(cv.Languages));
            Assert.That(duplicateCv.Skills, Is.EqualTo(cv.Skills));
            Assert.That(duplicateCv.Hobbies, Is.EqualTo(cv.Hobbies));
            Assert.That(duplicateCv.CustomHtml, Is.EqualTo(cv.CustomHtml));
            Assert.That(duplicateCv.CustomCss, Is.EqualTo(cv.CustomCss));
        }
    }
}