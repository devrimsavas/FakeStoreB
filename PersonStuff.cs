
public class Person
{
    public int Id { get; set; } = 0;
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Location { get; set; } = "";
    public string PhotoUrl { get; set; } = "";

    public Person()
    {

    }

};


public static class StaffManager
{
    private static List<Person> staff = new List<Person>();

    public static bool IsFull => staff.Count > 0;

    public static List<Person> GetStaff()
    {
        return staff;
    }


    //this initializes table 
    public static void AddStaff(List<Person> newStaff)
    {
        if (staff.Count > 0) return; // If already populated, do nothing
        staff.AddRange(newStaff);
    }

    //allows add new person 
    public static void AddNewStaff(Person person)
    {
        var existedPerson = staff.FirstOrDefault(p => p.Id == person.Id);

        if (existedPerson != null)
        {
            throw new Exception("Person already exits");
        }
        staff.Add(person);

    }

    public static List<Person> FindByName(string firstName)
    {

        var existedNames = staff.FindAll(p => p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));
        return existedNames;
    }

    public static List<Person> FindByPrefix(string prefix)
    {
        var matchedNames = staff.FindAll(p =>
            p.FirstName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || p.LastName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

        return matchedNames;

    }

    //delete a person by id 
    public static void DeletePersonById(int id)
    {
        var existedPerson = staff.FirstOrDefault(p => p.Id == id);
        if (existedPerson == null)
        {
            throw new Exception("No person with this Id");
        }

        staff.Remove(existedPerson);

    }


    //update existed person 

    public static void UpdatePersonById(int id, Person updatedPerson)
    {

        // Find the person by ID
        var existingPerson = staff.FirstOrDefault(p => p.Id == id);

        // If person not found, throw an exception
        if (existingPerson == null)
        {
            throw new Exception($"Person with ID {id} not found");
        }

        // Update the person's properties
        existingPerson.FirstName = updatedPerson.FirstName ?? existingPerson.FirstName;
        existingPerson.LastName = updatedPerson.LastName ?? existingPerson.LastName;
        existingPerson.Email = updatedPerson.Email ?? existingPerson.Email;
        existingPerson.Phone = updatedPerson.Phone ?? existingPerson.Phone;
        existingPerson.Location = updatedPerson.Location ?? existingPerson.Location;
        existingPerson.PhotoUrl = updatedPerson.PhotoUrl ?? existingPerson.PhotoUrl;


    }



}