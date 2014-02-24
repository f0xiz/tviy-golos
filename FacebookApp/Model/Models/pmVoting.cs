namespace Model
{
    public class pmVoting
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsLock { get; set; }

        public bool IsPersonal { get; set; }

        public int VoutersCount { get; set; }
    }
}
