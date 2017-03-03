namespace CommonLib.Dashboard.Model
{
    public class Person
    {
        public bool IsAccountVerified { get; private set; }
        public bool IsAdministrator { get; private set; }

        public Person(string uniqueid, string role) {
            IsAccountVerified = true;
            if (role == "admin") IsAdministrator = true;
        }
    }
}
