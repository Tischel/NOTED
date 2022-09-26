using System;
using System.Collections.Generic;

namespace NOTED.Models
{
    public class Duty: IEquatable<Duty>
    {
        public uint ID;
        public string Name;
        public List<Note> Notes;

        public Duty(uint id, string name)
        {
            ID = id;
            Name = name;
            Notes = new List<Note>();
        }

        public bool Equals(Duty? other)
        {
            return ID == other?.ID;
        }
    }
}
