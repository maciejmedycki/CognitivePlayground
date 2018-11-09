using azure = Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Hodor.Model.ExtensionMethods.Azure
{
    public static class PersonExtension
    {
        public static Person CreatePerson(this azure.Person person)
        {
            return new Person
            {
                Name = person.Name,
                PersonId = person.PersonId,
                UserData = person.UserData,
                PersistedFaceIds = person.PersistedFaceIds,
            };
        }
    }
}