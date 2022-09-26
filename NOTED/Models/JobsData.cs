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
                if (_text.Length <= 32) { return _text; }

                return _text.Substring(0, 28) + " ...";
            }
        }

        public JobsData()
        {
            Map = new Dictionary<JobRoles, List<bool>>();

            JobRoles[] roles = (JobRoles[])Enum.GetValues(typeof(JobRoles));

            foreach (JobRoles role in roles)
            {
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

            CalculateText();
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

        private void CalculateText()
        {
            List<string> jobs = new List<string>();
            JobRoles[] roles = (JobRoles[])Enum.GetValues(typeof(JobRoles));

            foreach (JobRoles role in roles)
            {
                List<string> jobsInRole = new List<string>();
                int count = JobsHelper.JobsByRole[role].Count;

                for (int i = 0; i < count; i++)
                {
                    if (Map[role][i]) 
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
            } else
            {
                _text = string.Join(", ", jobs);
            }
        }
    }
}
