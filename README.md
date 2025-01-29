# Buggy Code for Students to Debug

This project contains an ASP.NET Core application with **4 intentional bugs** for students to find and fix. Your task is to identify and correct these errors.

## Identified Bugs

### 1. Typo in API URL

**Bug:**

```csharp
var response = await client.GetFromJsonAsync<JsonElement>("https://randomuser.me/ap/?results={personRequest.PersonNumber}");
```

**Fix:**

```csharp
var response = await client.GetFromJsonAsync<JsonElement>("https://randomuser.me/api/?results={personRequest.PersonNumber}");
```

### 2. Incorrect Index Usage

**Bug:**

```csharp
Id = index,
```

**Fix:**

```csharp
Id = index + 1,
```

### 3. Calling a Non-Existing Method

**Bug:**

```csharp
StaffManager.AddPerson(newPerson);
```

**Fix:**

```csharp
StaffManager.AddNewStaff(newPerson);
```

### 4. Incorrect Method Call

**Bug:**

```csharp
StaffManager.RemovePersonById(id);
```

**Fix:**

```csharp
StaffManager.DeletePersonById(id);
```

## Task

1. Identify these bugs in the code.
2. Correct them to make the application work properly.
3. Test the functionality to ensure all features work correctly.

Good luck debugging! 🚀
