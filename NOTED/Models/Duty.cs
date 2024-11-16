using Dalamud.Game.ClientState.Objects.SubKinds;
using NOTED.Helpers;
using System;
using System.Collections.Generic;

namespace NOTED.Models
{
    public class Duty
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

        public Note? GetActiveNote()
        {
            IPlayerCharacter? player = Plugin.ClientState.LocalPlayer;
            if (player == null) { return null; }

            uint jobId = player.ClassJob.RowId;
            Note? firstNote = null;

            foreach (Note note in Notes)
            {
                if (!note.Enabled) { continue; }

                if (note.Jobs.IsEmpty && firstNote == null)
                {
                    firstNote = note;
                    continue;
                }

                JobRoles role = JobsHelper.RoleForJob(jobId);
                int index = JobsHelper.JobsByRole[role].IndexOf(jobId);

                if (note.Jobs.IsEnabled(role, index))
                {
                    return note;
                }
            }

            return firstNote;
        }

    }
}