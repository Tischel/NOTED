using System.Collections.Generic;

namespace NOTED.Models
{
    public class Note
    {
        public string Title;
        public string Text;
        public JobsData Jobs;

        public Note()
        {
            Title = "";
            Text = "";
            Jobs = new JobsData();
        }

        public Note(string title)
        {
            Title = title;
            Text = "";
            Jobs = new JobsData();
        }

        public Note(string title, string text, JobsData jobs)
        {
            Title = title;
            Text = text;
            Jobs = jobs;
        }
    }
}
