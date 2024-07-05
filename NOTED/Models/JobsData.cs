using NOTED.Helpers;
using System;
using System.Collections.Generic;

namespace NOTED.Models
{
    public class JobsData
    {
        public Dictionary<JobRoles, List<bool>> Map;

        private string _text = "All";
        public string Text {
            get
            {
                if (_text.Length <= 24) { return _text; }

                return _text.Substring(0, 20) + " ...";
            }
        }

        private bool _isEmpty = true;
        public bool IsEmpty => _isEmpty;

        public JobsData()
        {
            Map = new Dictionary<JobRoles, List<bool>>();

            JobRoles[] roles = (JobRoles[])Enum.GetValues(typeof(JobRoles));

            foreach (JobRoles role in roles)
            {
                if (role == JobRoles.Unknown) { continue; }

                int count = JobsHelper.JobsByRole[role].Count;
                List<bool> list = new List<bool>(count);

                for (int i = 0; i < count; i++)
                {
                    list.Add(false);
                }

                Map.Add(role, list);
            }
        }

        public bool GetRoleEnabled(JobRoles role)
        {
            foreach (bool value in Map[role])
            {
                if (!value)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetRoleEnabled(JobRoles role, bool value)
        {
            for (int i = 0; i < Map[role].Count; i++)
            {
                Map[role][i] = value;
            }

            Update();
        }

        public void SetJobEnabled(JobRoles role, int index, bool value)
        {
            if (Map[role].Count > index)
            {
                Map[role][index] = value;
                Update();
            }
        }

        public bool IsEnabled(JobRoles role, int index)
        {
            if (Map.TryGetValue(role, out List<bool>? list) && list != null)
            {
                if (index >= list.Count)
                {
                    return false;
                }

                return list[index];
            }

            return false;
        }

        public void Update()
        {
            List<string> jobs = new List<string>();
            JobRoles[] roles = (JobRoles[])Enum.GetValues(typeof(JobRoles));

            foreach (JobRoles role in roles)
            {
                if (role == JobRoles.Unknown) { continue; }

                List<string> jobsInRole = new List<string>();
                int count = JobsHelper.JobsByRole[role].Count;

                for (int i = 0; i < count; i++)
                {
                    if (i >= Map[role].Count)
                    {
                        Map[role].Add(false);
                    }
                    else if (Map[role][i]) 
                    {
                        uint key = JobsHelper.JobsByRole[role][i];
                        string name = JobsHelper.JobNames[key];
                        jobsInRole.Add(name);
                    }
                }

                if (jobsInRole.Count == count)
                {
                    jobs.Add(JobsHelper.RoleNames[role]);
                }
                else if (jobsInRole.Count > 0)
                {
                    jobs.AddRange(jobsInRole);
                }
            }

            if (jobs.Count == 0)
            {
                _text = "All";
                _isEmpty = true;
            }
            else
            {
                _text = string.Join(", ", jobs);
                _isEmpty = false;
            }
        }
    }
}
