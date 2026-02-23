# ClassCopier Documentation

The `ClassCopier` class provides methods for creating deep copies of objects using JSON serialization. It is part of the `GUtils.ClassCopier` namespace and supports .NET 8 and .NET 9.

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Methods](#methods)
  - [DeepCopy\<T\>](#deepcopyt)
  - [TryDeepCopy\<T\>](#trydeepcopyt)
  - [DeepCopyOrDefault\<T\>](#deepcopyordefaultt)
  - [DeepCopyMany\<T\>](#deepcopymanyt)
- [Supported Types](#supported-types)
- [Limitations](#limitations)
- [Best Practices](#best-practices)
- [Advanced Examples](#advanced-examples)

---

## Overview

`ClassCopier` creates true deep copies of objects, meaning all nested objects and collections are also copied. Changes to the copy will not affect the original object. The implementation uses Newtonsoft.Json for serialization with custom handling for:

- Circular references (ignored to prevent infinite loops)
- Delegate properties (excluded from serialization)
- Private fields and backing fields
- `IPAddress` objects
- Non-public constructors

---

## Installation

Add a reference to the `GUtils` project or NuGet package, then add the following using directive:

```csharp
using GUtils.ClassCopier;
```

---

## Methods

### DeepCopy\<T\>

Creates a deep copy of an object using JSON serialization.

#### Signature

```csharp
public static T DeepCopy<T>(T source)
```

#### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `source` | `T` | The source object to copy. Must be JSON-serializable. |

#### Returns

A deep copy of the source object.

#### Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | Thrown when `source` is null. |
| `InvalidOperationException` | Thrown when deserialization fails or results in null. |

#### Examples

**Basic Usage - Simple Object**

```csharp
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
}

var original = new Person { Id = 1, Name = "John Doe" };
var copy = ClassCopier.DeepCopy(original);

// Modify the copy - original is unaffected
copy.Name = "Jane Doe";

Console.WriteLine(original.Name); // Output: "John Doe"
Console.WriteLine(copy.Name);     // Output: "Jane Doe"
```

**Nested Objects**

```csharp
public class Order
{
    public int OrderId { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; }
}

public class Customer
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class OrderItem
{
    public string ProductName { get; set; }
    public decimal Price { get; set; }
}

var original = new Order
{
    OrderId = 1001,
    Customer = new Customer { Name = "Alice", Email = "alice@example.com" },
    Items = new List<OrderItem>
    {
        new() { ProductName = "Widget", Price = 29.99m },
        new() { ProductName = "Gadget", Price = 49.99m }
    }
};

var copy = ClassCopier.DeepCopy(original);

// All nested objects are also copied
copy.Customer.Name = "Bob";
copy.Items.Add(new OrderItem { ProductName = "Gizmo", Price = 19.99m });

Console.WriteLine(original.Customer.Name); // Output: "Alice"
Console.WriteLine(original.Items.Count);   // Output: 2
Console.WriteLine(copy.Customer.Name);     // Output: "Bob"
Console.WriteLine(copy.Items.Count);       // Output: 3
```

**Collections**

```csharp
// Lists
var originalList = new List<int> { 1, 2, 3, 4, 5 };
var copyList = ClassCopier.DeepCopy(originalList);

// Dictionaries
var originalDict = new Dictionary<string, int>
{
    ["one"] = 1,
    ["two"] = 2,
    ["three"] = 3
};
var copyDict = ClassCopier.DeepCopy(originalDict);

// Arrays
var originalArray = new[] { "a", "b", "c" };
var copyArray = ClassCopier.DeepCopy(originalArray);
```

**Records**

```csharp
public record PersonRecord(int Id, string Name, string Email);

var original = new PersonRecord(1, "John", "john@example.com");
var copy = ClassCopier.DeepCopy(original);

// Records are immutable, but you get a new instance
Console.WriteLine(ReferenceEquals(original, copy)); // Output: False
Console.WriteLine(original == copy);                 // Output: True (value equality)
```

---

### TryDeepCopy\<T\>

Attempts to create a deep copy of an object without throwing exceptions on failure.

#### Signature

```csharp
public static bool TryDeepCopy<T>(T source, [NotNullWhen(true)] out T? copy)
```

#### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `source` | `T` | The source object to copy. |
| `copy` | `out T?` | When this method returns, contains the deep copy if successful; otherwise, the default value. |

#### Returns

`true` if the copy was created successfully; otherwise, `false`.

#### Examples

**Safe Copying with Error Handling**

```csharp
var original = new Person { Id = 1, Name = "John" };

if (ClassCopier.TryDeepCopy(original, out var copy))
{
    Console.WriteLine($"Successfully copied: {copy.Name}");
}
else
{
    Console.WriteLine("Failed to create copy");
}
```

**Handling Null Sources**

```csharp
Person? nullPerson = null;

if (ClassCopier.TryDeepCopy(nullPerson, out var copy))
{
    // This block won't execute
    Console.WriteLine(copy.Name);
}
else
{
    Console.WriteLine("Source was null or copy failed");
    // Output: "Source was null or copy failed"
}
```

**Processing User Input Safely**

```csharp
public Person? ClonePersonSafely(Person? input)
{
    if (ClassCopier.TryDeepCopy(input, out var copy))
    {
        return copy;
    }
    
    _logger.LogWarning("Failed to clone person object");
    return null;
}
```

**Batch Processing with Fallback**

```csharp
public List<Order> ProcessOrders(IEnumerable<Order> orders)
{
    var processedOrders = new List<Order>();
    
    foreach (var order in orders)
    {
        if (ClassCopier.TryDeepCopy(order, out var copy))
        {
            copy.Status = "Processed";
            processedOrders.Add(copy);
        }
        else
        {
            _logger.LogError($"Failed to copy order {order.OrderId}");
        }
    }
    
    return processedOrders;
}
```

---

### DeepCopyOrDefault\<T\>

Creates a deep copy of an object, returning a default value if the copy fails.

#### Signature

```csharp
public static T DeepCopyOrDefault<T>(T source, T defaultValue)
```

#### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `source` | `T` | The source object to copy. |
| `defaultValue` | `T` | The default value to return if copying fails. |

#### Returns

A deep copy of the source object, or the default value if copying fails.

#### Examples

**Basic Usage with Default Value**

```csharp
var original = new Person { Id = 1, Name = "John" };
var defaultPerson = new Person { Id = 0, Name = "Unknown" };

var copy = ClassCopier.DeepCopyOrDefault(original, defaultPerson);
Console.WriteLine(copy.Name); // Output: "John"
```

**Handling Null with Default**

```csharp
Person? nullPerson = null;
var defaultPerson = new Person { Id = 0, Name = "Guest" };

var result = ClassCopier.DeepCopyOrDefault(nullPerson, defaultPerson);
Console.WriteLine(result.Name); // Output: "Guest"
```

**Configuration Cloning with Fallback**

```csharp
public class AppConfig
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }
    public bool EnableLogging { get; set; }
}

public AppConfig GetWorkingConfig(AppConfig? customConfig)
{
    var defaultConfig = new AppConfig
    {
        ConnectionString = "Server=localhost;Database=default",
        Timeout = 30,
        EnableLogging = true
    };
    
    return ClassCopier.DeepCopyOrDefault(customConfig, defaultConfig);
}
```

**Template-Based Object Creation**

```csharp
public class EmailTemplate
{
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<string> Recipients { get; set; } = new();
}

var welcomeTemplate = new EmailTemplate
{
    Subject = "Welcome!",
    Body = "Welcome to our service.",
    Recipients = new List<string>()
};

// Create a copy for customization, falling back to template if copy fails
var email = ClassCopier.DeepCopyOrDefault(welcomeTemplate, welcomeTemplate);
email.Recipients.Add("newuser@example.com");
```

---

### DeepCopyMany\<T\>

Creates deep copies of multiple objects in parallel for improved performance.

#### Signature

```csharp
public static IEnumerable<T> DeepCopyMany<T>(IEnumerable<T> sources, int maxDegreeOfParallelism = -1)
```

#### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `sources` | `IEnumerable<T>` | The source objects to copy. |
| `maxDegreeOfParallelism` | `int` | The maximum number of concurrent operations. Default is -1 (unlimited). |

#### Returns

An enumerable of deep copies.

#### Exceptions

| Exception | Condition |
|-----------|-----------|
| `ArgumentNullException` | Thrown when `sources` is null. |

#### Examples

**Basic Parallel Copying**

```csharp
var originalPeople = new List<Person>
{
    new() { Id = 1, Name = "Alice" },
    new() { Id = 2, Name = "Bob" },
    new() { Id = 3, Name = "Charlie" }
};

var copies = ClassCopier.DeepCopyMany(originalPeople).ToList();

// All items are copied independently
copies[0].Name = "Modified Alice";
Console.WriteLine(originalPeople[0].Name); // Output: "Alice"
```

**Controlling Parallelism**

```csharp
var largeDataset = Enumerable.Range(1, 10000)
    .Select(i => new DataRecord { Id = i, Value = $"Record {i}" })
    .ToList();

// Limit to 4 concurrent operations
var copies = ClassCopier.DeepCopyMany(largeDataset, maxDegreeOfParallelism: 4).ToList();
```

**Processing Large Datasets**

```csharp
public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public List<Order> CreateOrderSnapshots(IEnumerable<Order> activeOrders)
{
    // Create copies of all orders for archival
    var snapshots = ClassCopier.DeepCopyMany(activeOrders, maxDegreeOfParallelism: 8).ToList();
    
    // Add snapshot metadata to copies without affecting originals
    foreach (var snapshot in snapshots)
    {
        snapshot.OrderDate = DateTime.UtcNow; // Mark as snapshot time
    }
    
    return snapshots;
}
```

**Parallel Processing Pipeline**

```csharp
public async Task<List<ProcessedItem>> ProcessItemsAsync(List<RawItem> items)
{
    // Step 1: Create copies for processing (parallel)
    var workingCopies = ClassCopier.DeepCopyMany(items).ToList();
    
    // Step 2: Process copies without affecting originals
    var tasks = workingCopies.Select(async item =>
    {
        await ProcessItemAsync(item);
        return new ProcessedItem(item);
    });
    
    return (await Task.WhenAll(tasks)).ToList();
}
```

**Memory-Efficient Processing with Limited Parallelism**

```csharp
// For memory-constrained environments, limit parallelism
var copies = ClassCopier.DeepCopyMany(
    largeCollection, 
    maxDegreeOfParallelism: Environment.ProcessorCount / 2
).ToList();
```

---

## Supported Types

`ClassCopier` supports a wide variety of types:

| Category | Examples |
|----------|----------|
| **Primitive Types** | `int`, `long`, `double`, `bool`, `char`, etc. |
| **Common Types** | `string`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid`, `decimal` |
| **Modern Types** | `DateOnly`, `TimeOnly` (C# 10+) |
| **Collections** | `List<T>`, `Dictionary<K,V>`, `HashSet<T>`, `Queue<T>`, `Stack<T>`, arrays |
| **Special Collections** | `SortedDictionary<K,V>`, `SortedList<K,V>`, `LinkedList<T>`, `ReadOnlyCollection<T>` |
| **Records** | Record classes and record structs |
| **Custom Classes** | Classes with public/private properties and fields |
| **Nullable Types** | `int?`, `string?`, nullable objects |
| **Network Types** | `IPAddress` (with custom converter) |
| **Other** | `Uri`, `Version`, `BigInteger`, `Complex`, `KeyValuePair<K,V>`, tuples |

---

## Limitations

### Types That Cannot Be Copied

| Type | Reason |
|------|--------|
| **Delegates** | Excluded from serialization (set to null in copy) |
| **Events** | Event handlers are not serialized |
| **Circular References** | Handled gracefully but the loop is broken |
| **Unserializable Types** | Types that cannot be JSON serialized will fail |

### Behavior Notes

1. **Static Properties**: Not copied (they are shared, not instance-specific)
2. **Indexers**: Ignored during copying
3. **Computed Properties**: Re-computed based on copied backing data
4. **Thread Affinity**: Copies are thread-safe to create and use independently

### Example: Delegates Are Not Copied

```csharp
public class Calculator
{
    public int Value { get; set; }
    public Func<int, int>? Operation { get; set; }
}

var original = new Calculator
{
    Value = 10,
    Operation = x => x * 2
};

var copy = ClassCopier.DeepCopy(original);

Console.WriteLine(original.Operation?.Invoke(5)); // Output: 10
Console.WriteLine(copy.Operation?.Invoke(5));     // Output: (null - nothing, throws if not checked)
Console.WriteLine(copy.Operation is null);        // Output: True
```

---

## Best Practices

### 1. Use TryDeepCopy for Untrusted Input

```csharp
// Good: Safe handling of potentially problematic objects
if (ClassCopier.TryDeepCopy(userInput, out var safeCopy))
{
    ProcessData(safeCopy);
}

// Avoid: May throw on problematic input
var copy = ClassCopier.DeepCopy(userInput); // Could throw
```

### 2. Use DeepCopyMany for Bulk Operations

```csharp
// Good: Parallel processing for large collections
var copies = ClassCopier.DeepCopyMany(largeList).ToList();

// Less efficient: Sequential processing
var copies = largeList.Select(item => ClassCopier.DeepCopy(item)).ToList();
```

### 3. Limit Parallelism in Resource-Constrained Environments

```csharp
// Good: Control resource usage
var copies = ClassCopier.DeepCopyMany(items, maxDegreeOfParallelism: 4);

// May cause issues: Unlimited parallelism on large datasets
var copies = ClassCopier.DeepCopyMany(veryLargeList); // Uses all cores
```

### 4. Use DeepCopyOrDefault for Configuration Objects

```csharp
// Good: Always get a valid configuration
var config = ClassCopier.DeepCopyOrDefault(customConfig, DefaultConfig);

// Requires more handling
var config = ClassCopier.TryDeepCopy(customConfig, out var copy) ? copy : DefaultConfig;
```

---

## Advanced Examples

### Creating Undo/Redo Functionality

```csharp
public class UndoManager<T> where T : class
{
    private readonly Stack<T> _undoStack = new();
    private readonly Stack<T> _redoStack = new();
    private T _current;

    public UndoManager(T initial)
    {
        _current = ClassCopier.DeepCopy(initial);
    }

    public void SaveState()
    {
        _undoStack.Push(ClassCopier.DeepCopy(_current));
        _redoStack.Clear();
    }

    public T? Undo()
    {
        if (_undoStack.Count == 0) return null;
        
        _redoStack.Push(ClassCopier.DeepCopy(_current));
        _current = _undoStack.Pop();
        return ClassCopier.DeepCopy(_current);
    }

    public T? Redo()
    {
        if (_redoStack.Count == 0) return null;
        
        _undoStack.Push(ClassCopier.DeepCopy(_current));
        _current = _redoStack.Pop();
        return ClassCopier.DeepCopy(_current);
    }

    public T Current => ClassCopier.DeepCopy(_current);
}
```

### Snapshot Testing

```csharp
public class SnapshotManager
{
    private readonly Dictionary<string, object> _snapshots = new();

    public void TakeSnapshot<T>(string name, T obj)
    {
        if (ClassCopier.TryDeepCopy(obj, out var copy))
        {
            _snapshots[name] = copy;
        }
    }

    public T? GetSnapshot<T>(string name) where T : class
    {
        if (_snapshots.TryGetValue(name, out var snapshot) && snapshot is T typed)
        {
            return ClassCopier.DeepCopy(typed);
        }
        return null;
    }
}
```

### Thread-Safe State Management

```csharp
public class ThreadSafeState<T> where T : class
{
    private T _state;
    private readonly object _lock = new();

    public ThreadSafeState(T initialState)
    {
        _state = ClassCopier.DeepCopy(initialState);
    }

    public T GetState()
    {
        lock (_lock)
        {
            return ClassCopier.DeepCopy(_state);
        }
    }

    public void UpdateState(Action<T> modifier)
    {
        lock (_lock)
        {
            var copy = ClassCopier.DeepCopy(_state);
            modifier(copy);
            _state = copy;
        }
    }
}
```

### API Response Cloning for Caching

```csharp
public class CachedApiClient
{
    private readonly Dictionary<string, object> _cache = new();
    private readonly HttpClient _httpClient;

    public async Task<T?> GetAsync<T>(string endpoint) where T : class
    {
        if (_cache.TryGetValue(endpoint, out var cached) && cached is T typedCached)
        {
            // Return a copy so modifications don't affect cache
            return ClassCopier.DeepCopy(typedCached);
        }

        var response = await _httpClient.GetFromJsonAsync<T>(endpoint);
        
        if (response != null && ClassCopier.TryDeepCopy(response, out var copy))
        {
            _cache[endpoint] = copy;
        }

        return response;
    }
}
```

---

## Performance Considerations

1. **Caching**: The internal contract resolver caches reflection results for improved performance on repeated copies of the same type.

2. **Large Objects**: For very large objects or collections, consider using `DeepCopyMany` with controlled parallelism.

3. **Frequent Copies**: If copying the same type repeatedly, the second and subsequent copies will be faster due to caching.

4. **Memory**: Deep copying creates entirely new objects. For very large object graphs, consider memory implications.

---

## Thread Safety

- `DeepCopy`, `TryDeepCopy`, `DeepCopyOrDefault`, and `DeepCopyMany` are all thread-safe.
- The internal caching mechanism uses appropriate locking for .NET 8 (`object`) and .NET 9 (`Lock`).
- Copies can be safely used across threads without synchronization.

---

## See Also

- [Newtonsoft.Json Documentation](https://www.newtonsoft.com/json/help/html/Introduction.htm)
- [Deep Copy vs Shallow Copy](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone)
