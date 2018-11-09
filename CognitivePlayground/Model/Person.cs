using System;
using System.Collections.Generic;

namespace Hodor.Model
{
    public struct Person
    {
        public string Name { get; set; }
        public Guid PersonId { get; set; }
        public string UserData { get; set; }
        public IList<Guid> PersistedFaceIds { get; set; }
    }    
}