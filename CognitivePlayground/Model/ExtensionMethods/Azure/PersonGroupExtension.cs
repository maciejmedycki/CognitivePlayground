using azure = Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CognitivePlayground.Model.ExtensionMethods.Azure
{
    public static class PersonGroupExtension
    {
        public static PersonGroup CreatePersonGroup(this azure.PersonGroup personGroup)
        {
            return new PersonGroup
            {
                Name = personGroup.Name,
                PersonGroupId = personGroup.PersonGroupId,
                UserData = personGroup.UserData,
            };
        }

        public static void UpdatePersonGroup(this azure.PersonGroup sourcePersonGroup, ref PersonGroup targetPersonGroup)
        {
            targetPersonGroup.Name = sourcePersonGroup.Name;
            targetPersonGroup.PersonGroupId = sourcePersonGroup.PersonGroupId;
            targetPersonGroup.UserData = sourcePersonGroup.UserData;
        }
    }
}